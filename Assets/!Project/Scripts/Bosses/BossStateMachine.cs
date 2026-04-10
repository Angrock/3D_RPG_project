namespace RPGProject
{
    public abstract class BossState
    {
        protected BossController boss;

        public virtual void Enter(BossController boss)
        {
            this.boss = boss;
        }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }

        public virtual void OnSeePlayer() { }
        public virtual void OnLosePlayer() { }
        public virtual void OnTakeDamage(float damage) { }
        public virtual void OnDeath() { }
    }

    public class BossStateMachine
    {
        public BossState CurrentState;
        private BossController boss;

        public BossStateMachine(BossController boss)
        {
            this.boss = boss;
        }

        public void ChangeState(BossState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter(boss);
        }

        public void Update() => CurrentState?.Update();
        public void FixedUpdate() => CurrentState?.FixedUpdate();

        public void OnSeePlayer() => CurrentState?.OnSeePlayer();
        public void OnLosePlayer() => CurrentState?.OnLosePlayer();
        public void OnTakeDamage(float damage) => CurrentState?.OnTakeDamage(damage);
    }
}