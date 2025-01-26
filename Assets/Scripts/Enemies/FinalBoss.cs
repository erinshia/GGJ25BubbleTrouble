public class FinalBoss : Enemy
{
    protected override void HandleDeath()
    {
        GlobalEventHandler.Instance.TriggerWin();
    }
}
