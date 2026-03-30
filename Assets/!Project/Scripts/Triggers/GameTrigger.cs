using UnityEngine;
using UnityEngine.Events;

namespace RPGProject
{
    /// <summary>
    /// Базовый класс для игровых триггеров.
    /// </summary>
    public abstract class GameTrigger : GameEntrypoint
    {
        [Header("Настройки триггера")]
        [SerializeField] protected bool _triggerOnce = true;
        [SerializeField] protected UnityEvent _onTriggerEnter;
        [SerializeField] protected UnityEvent _onTriggerExit;

        protected bool _hasTriggered;

        protected override void OnInitialize() => _hasTriggered = false;

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_hasTriggered && _triggerOnce) return;

            if (CanTrigger(other))
            {
                _hasTriggered = true;
                OnTriggerEntered(other);
                _onTriggerEnter?.Invoke();
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (CanTrigger(other))
            {
                OnTriggerExited(other);
                _onTriggerExit?.Invoke();
            }
        }

        protected abstract bool CanTrigger(Collider other);
        protected abstract void OnTriggerEntered(Collider other);
        protected virtual void OnTriggerExited(Collider other) { }
    }

    /// <summary>
    /// Триггер, активируемый игроком.
    /// </summary>
    public class PlayerTrigger : GameTrigger
    {
        protected override bool CanTrigger(Collider other) => other.CompareTag("Player");

        protected override void OnTriggerEntered(Collider other)
        {
            Debug.Log($"[PlayerTrigger] Игрок вошёл в триггер: {gameObject.name}");
        }
    }

    /// <summary>
    /// Триггер урона для игрока.
    /// </summary>
    public class DamageTrigger : GameTrigger
    {
        [Header("Настройки урона")]
        [SerializeField] private float _damage = 10f;

        protected override bool CanTrigger(Collider other) => other.CompareTag("Player");

        protected override void OnTriggerEntered(Collider other)
        {
            var player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            if (player != null)
            {
                player.TakeDamage(_damage);
                Debug.Log($"[DamageTrigger] Игрок получил {_damage} урона");
            }
        }
    }

    /// <summary>
    /// Триггер победы.
    /// </summary>
    public class VictoryTrigger : GameTrigger
    {
        protected override bool CanTrigger(Collider other) => other.CompareTag("Player");

        protected override void OnTriggerEntered(Collider other)
        {
            var gameStateManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameStateManager>();
            gameStateManager?.Victory();
            Debug.Log("[VictoryTrigger] Победа!");
        }
    }

    /// <summary>
    /// Триггер смерти врага.
    /// </summary>
    public class EnemyDeathTrigger : GameTrigger
    {
        [SerializeField] private BaseEnemy _enemy;

        protected override bool CanTrigger(Collider other) => true;

        protected override void OnTriggerEntered(Collider other)
        {
            if (_enemy != null && !_enemy.IsAlive)
            {
                _onTriggerEnter?.Invoke();
            }
        }
    }
}
