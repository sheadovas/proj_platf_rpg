using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
  public static GameMaster gm = null;
  public SpecialEffects specialEffects;
  public Equipment playerEquipment;
  public Player player;
  public QuestBoard questBoard;

  [Header("Overlay")]
  public Text playerHp;
  public string levelDetails = "";
  public GameObject equipmentMenu;

  [SerializeField]
  public SkillTreeVisual skillTreeOverlay;

  [Header("Ingame Menu")]
  [SerializeField]
  GameObject m_gameMenuParent;
  [SerializeField]
  Text m_title;
  [SerializeField]
  Text m_gameStatus;
  [SerializeField]
  Text m_missionDetails;
  [SerializeField]
  Button m_continue;

  [Header("Other")]
  public ConditionNoEnemies condNoEnemies;

  private string m_currentScene;
  private bool m_isGameOver = false;

  void Awake()
  {
    if (gm == null)
      gm = this;
  }

  private void Start()
  {
    if (specialEffects == null)
      Debug.LogWarning("Special Effects not found", this);

    m_currentScene = SceneManager.GetActiveScene().name;

    m_title.text = m_currentScene;
    m_gameStatus.text = "";
    m_missionDetails.text = levelDetails;

    if(condNoEnemies != null)
    {
      condNoEnemies.AddActionOnSuccess(() => NotifySuccess(condNoEnemies));
    }

    equipmentMenu.SetActive(false);
#if !UNITY_EDITOR
    ShowMenu();
#endif
  }

  public void NotifySuccess(GameOverCondition cond)
  {
    NotifyGameOver(true, cond.GetProgressInfo());
  }

  public void NotifyFailure(GameOverCondition cond)
  {
    NotifyGameOver(false, cond.GetProgressInfo());
  }

  public void NotifyGameOver(bool success, string message)
  {
    if (!success)
      m_continue.interactable = false;

    string title = (success ? "Congratz!" : "EPIC FAIL!");
    m_isGameOver = true;

    m_gameStatus.text = title;
    m_missionDetails.text = message;

    Invoke("ShowMenu", 0.5f);
  }

  public void NotifyEnemyDeath()
  {
    if (condNoEnemies != null)
      condNoEnemies.CheckConditions();
  }

  public void UpdateGUI()
  {
    playerHp.text = string.Format("{0} HP", player.stats.hp);
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      m_gameStatus.text = "Pause";
      ShowMenu();
    }

    if (Input.GetKeyDown(KeyCode.I))
    {
      // toggle inventory window
      equipmentMenu.SetActive(!equipmentMenu.activeSelf);
    }

    if(Input.GetKeyDown(KeyCode.K))
    {
      // toggle skill overlay
      skillTreeOverlay.gameObject.SetActive(!skillTreeOverlay.gameObject.activeSelf);
    }
  }

  
  /* UI */
  public void ShowMenu()
  {
    Time.timeScale = 0;
    if(m_isGameOver && SceneManager.GetActiveScene().buildIndex + 1 == SceneManager.sceneCountInBuildSettings)
    {
      m_continue.interactable = false;
    }

    m_gameMenuParent.SetActive(true);
  }

  public void HideMenu()
  {
    Time.timeScale = 1.0f;
    m_gameMenuParent.SetActive(false);
  }

  public void OnClickedContinue()
  {
    if(!m_isGameOver)
    {
      HideMenu();
    }
    else
    {
      int current = SceneManager.GetActiveScene().buildIndex;
      SceneManager.LoadScene(current + 1);
    }
  }

  public void OnClickedRestart()
  {
    Time.timeScale = 1.0f;
    SceneManager.LoadScene(m_currentScene);
  }

  public void OnClickedExit()
  {
    Application.Quit();
  }
}
