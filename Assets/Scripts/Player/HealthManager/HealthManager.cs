using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using TurnAMint.Management;

namespace TurnAMint.Player.Health
{
    public class HealthManager : NetworkBehaviour, IAttackReceiver
    {

        #region Declaring variables
        [SerializeField]
        int maxHealth = 100;
        [SerializeField]
        int maxDownedHealth = 200;
        [SerializeField, SyncVar(hook = "HealthChanged")]
        int currentHealth;
        [SerializeField]
        int currentDownedHealth;

        public PlayerStatus currentAliveStatus
        {
            get
            {
                if (currentHealth > 0)
                {
                    return PlayerStatus.Alive;
                }
                if (currentHealth <= 0 && currentDownedHealth > 0)
                {
                    return PlayerStatus.Downed;
                }
                else
                {
                    return PlayerStatus.Dead;
                }
            }
        }

        CharacterHitEffects hitEffects;

        StateManager state;

        public delegate void AttackReceievedEvent(Attack attack);
        public delegate void HealthChangedEvent(int damage);
        public delegate void PlayerStatusChangedEvent();

        public AttackReceievedEvent OnAttackReceived;
        public HealthChangedEvent OnHealthRemoved;
        public HealthChangedEvent OnHealthRestored;
        public PlayerStatusChangedEvent OnKnockedDown;
        public PlayerStatusChangedEvent OnKilled;
        public PlayerStatusChangedEvent OnReset;

        public HealthChangedEvent OnEnvironmentalDamageTaken;
        public float downedHealthSkimAmount;

        private void Awake()
        {
            hitEffects = GetComponent<CharacterHitEffects>();
            state = GetComponent<StateManager>();
        }

        private void Start()
        {
            currentHealth = maxHealth;
            currentDownedHealth = maxDownedHealth;
        }

        public int MaxHealth
        {
            get
            {
                return maxHealth;
            }
        }

        public int CurrentHealth
        {
            get
            {
                return currentHealth;
            }
        }

        public int MaxDownedHealth
        {
            get
            {
                return maxDownedHealth;
            }
        }

        public int CurrentDownedHealth
        {
            get
            {
                return currentDownedHealth;
            }
        }
        #endregion

        #region Taking Damage
        public void ReceiveAttack(Attack attack)
        {
            if (OnAttackReceived != null)
            {
                OnAttackReceived(attack);
            }

            if (hitEffects != null)
            {
                hitEffects.PlayEffect(attack.hitLocation, (attack.origin - attack.hitLocation).normalized);
            }

            TakeDamage(attack);
        }

        public void TakeEnvironmentalDamage(int amount)
        {
            print("Environmental damage: " + amount);
            if (OnEnvironmentalDamageTaken != null)
            {
                OnEnvironmentalDamageTaken(amount);
            }
        }

        #endregion

        #region Healing
        public void HealInstant(int amount)
        {
            AddHealth(amount);
        }

        public void HealOverTime(int amount, int ticks)
        {
            GameManager.Instance.Timer.Add(() =>
            {
                AddHealth(amount);
            }, 1f, ticks);
        }

        void AddHealth(int amount)
        {
            int tmp = amount;

            if (OnHealthRestored != null)
            {
                OnHealthRestored(amount);
            }

            if (currentDownedHealth < maxDownedHealth)
            {
                int skimHealing = (int)(tmp * downedHealthSkimAmount);

                currentDownedHealth += skimHealing;

                if (currentDownedHealth > maxDownedHealth)
                    currentDownedHealth = maxDownedHealth;

                tmp -= skimHealing;
            }

            currentHealth += tmp;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }

        public void Reset()
        {
            currentHealth = maxHealth;
            currentDownedHealth = maxDownedHealth;
            state.PlayerRespawn();
            if (OnReset != null)
            {
                OnReset();
            }
        }
        #endregion

        #region Test Functions

        [ContextMenu("Test Die")]
        void TestDie()
        {
            ReceiveAttack(new Attack(maxHealth + maxDownedHealth, Vector3.zero, Vector3.up * 1.5f));//, null));
        }

        [ContextMenu("Test Damage 30")]
        void TestDamage30()
        {
            ReceiveAttack(new Attack(30, Vector3.zero, Vector3.up * 1.5f));//, null));
        }

        [ContextMenu("Test Heal 40")]
        void TestHealInstant40()
        {
            HealInstant(40);
        }

        [ContextMenu("Test Heal 5 ticks 10 health")]
        void TestHealOverTime()
        {
            HealOverTime(10, 5);
        }

        #endregion

        #region Network

        public void TakeDamage(Attack newAttack)
        {
            if (!isLocalPlayer) return;
            Debug.Log("Client-side Damage Taken: " + newAttack.damage);
            int tmp = newAttack.damage;

            if (OnHealthRemoved != null)
            {
                OnHealthRemoved(newAttack.damage);
            }

            if (currentHealth > 0)
            {
                currentHealth -= tmp;

                if (currentHealth <= 0)
                {
                    if (OnKnockedDown != null)
                    {
                        OnKnockedDown();
                    }
                    tmp = Mathf.Abs(currentHealth);
                }
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                currentDownedHealth -= tmp;
            }

            if (currentDownedHealth <= 0)
            {
                if (OnKilled != null)
                {
                    OnKilled();
                }
            }
        }

        void HealthChanged(int newHealth)
        {
            //TO-DO
        }

        #endregion
    }

    public enum PlayerStatus
    {
        Alive = 0,
        Downed = 1,
        Dead = 2
    }

    public struct Attack
    {
        public int damage;
        public Vector3 origin;
        public Vector3 hitLocation;
        //public Weapon originWeapon;
        public Attack(int newDamage, Vector3 newOrigin, Vector3 newHitLocation)//, Weapon newOriginWeapon)
        {
            damage = newDamage;
            origin = newOrigin;
            hitLocation = newHitLocation;
            //originWeapon = newOriginWeapon;
        }
    }
}