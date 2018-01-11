using UnityEngine;
using UnityEngine.Events;

namespace HealthManagement
{
    public class HealthManager : MonoBehaviour, IAttackReceiver
    {

        [SerializeField] int maxHealth = 100;
        [SerializeField] int currentHealth;
        [SerializeField] bool destroyOnDepleted = false;
        [SerializeField] float respawnTime;

        public UnityEvent OnDamaged;
        public UnityEvent OnHealthDepleted;

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

        void OnEnable()
        {
            Reset();
        }

        public void ReceiveAttack(Weapons.Attack attack)
        {
            currentHealth -= attack.damage;
            OnDamaged.Invoke();


            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnHealthDepleted.Invoke();
                OnDeath();
                if (destroyOnDepleted)
                {
                    Destroy(gameObject);
                }
            }

            print("Health remaining: " + currentHealth);
        }

        void OnDeath()
        {
            GameManager.Instance.Respawner.Despawn(gameObject, respawnTime);
        }

        void Reset()
        {
            currentHealth = maxHealth;
        }
    }
}