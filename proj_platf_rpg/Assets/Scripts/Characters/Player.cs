using UnityEngine;

public class Player : PlayableCharacter
{
  [Header("Player")]
  public float runningSpeedMultiplier = 2.0f;

  [SerializeField]
  protected GameOverCondition m_conditionDeath;


  public override void NotifyDamageTaken()
  {
    GameMaster.gm.UpdateGUI();

    base.NotifyDamageTaken();
    m_conditionDeath.CheckConditions();
  }

  protected override void Start()
  {
    base.Start();

    if(m_conditionDeath == null)
    {
      Debug.LogWarning("Death condition for player is not set!", this);
    }

    GameMaster.gm.UpdateGUI();

    // NOTICE default behaviour for player
    // we are assuming there is only one player in game
    equipment = GameMaster.gm.playerEquipment;
  }

  protected override void Move()
  {
    float vy = m_rigidbody.velocity.y;
    float vx = m_vspeed * m_controller.moveDirection;

    if (m_controller.isRunningKeyClicked)
      vx *= runningSpeedMultiplier;

    m_rigidbody.velocity = new Vector2(vx, vy);
  }
}
