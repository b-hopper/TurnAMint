using UnityEngine;
using UnityEngine.Events;

namespace HealthManagement
{
    [RequireComponent(typeof(Collider))]
    public class HealthManager : MonoBehaviour, IAttackReceiver
    {

        [SerializeField] int maxHealth = 100;
        [SerializeField] int currentHealth;
        [SerializeField] bool destroyOnDepleted = false;
        [SerializeField, Tooltip("If this value is <= 0, will not respawn.")] float respawnTime;

        public UnityEvent OnDamaged;
        public UnityEvent OnHealthDepleted;
        public UnityEvent OnReset;

        public float MaxHealth
        {
            get
            {
                return maxHealth;
            }
        }

        public float CurrentHealth
        {
            get
            {
                return currentHealth;
            }
        }

        public bool IsAlive
        {
            get
            {
                return currentHealth > 0;
            }
        }

        void OnEnable()
        {
            Reset();
        }

        public void ReceiveAttack(Weapons.Attack attack)
        {
            bool shouldFireOnDepleted = true;
            if (!IsAlive)
            {
                shouldFireOnDepleted = false;
            }
            currentHealth -= attack.damage;
            OnDamaged.Invoke();
            
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                if (shouldFireOnDepleted)
                {
                    OnHealthDepleted.Invoke();
                    if (destroyOnDepleted)
                    {
                        GameManager.Instance.Respawner.Despawn(gameObject, respawnTime);
                    }
                }
            }
        }


        [ContextMenu("Test Die")]
        void TestDie()
        {
            ReceiveAttack(new Weapons.Attack(maxHealth, Vector3.zero, Vector3.zero));
        }

        public void Reset()
        {
            OnReset.Invoke();
            currentHealth = maxHealth;
        }
    }
}