using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class HealthManager : NetworkBehaviour, IAttackReceiver {

	[SerializeField]
	int maxHealth = 100;
	[SerializeField]
	int currentHealth;
	[SerializeField]
	bool destroyOnDepleted = false;

    CharacterHitEffects hitEffects;

    StateManager state;

    public delegate void HealthChangedEvent(Attack attack);

    public HealthChangedEvent OnDamaged;
    public HealthChangedEvent OnHealthDepleted;

    public bool isAlive
    {
        get
        {
            return (currentHealth > 0);
        }
    }


	public float MaxHealth {
		get {
			return maxHealth;
		}
	}

	public float CurrentHealth {
		get {
			return currentHealth;
		}
	}

    private void Awake()
    {
        hitEffects = GetComponent<CharacterHitEffects>();
        state = GetComponent<StateManager>();
    }

    void OnEnable () {
		Reset();
	}

    [ClientRpc]
    public void RpcReceiveAttack(Attack attack)
    {
        currentHealth -= attack.damage;
        print(attack.damage);
        
        if (OnDamaged != null)
        {
            OnDamaged(attack);
        }

        if (hitEffects != null)
        {
            hitEffects.PlayEffect("normalHit", attack.hitLocation, attack.origin - attack.hitLocation);
        }

        if (currentHealth <= 0)
        {
            if (OnHealthDepleted != null)
            {
                OnHealthDepleted(attack);
            }
            if (destroyOnDepleted)
            {
                Destroy(gameObject);
            }
        }
    }

    [Command]
    public void CmdReceiveAttack(Attack attack)
    {
        RpcReceiveAttack(attack);
    }


    public void Reset () {
		currentHealth = maxHealth;
        if (isServer)
        {
            state.RpcPlayerRespawn();
        }
        else
        {
            state.CmdPlayerRespawn();
        }
    }

    #region Test Functions

    [ContextMenu("Test Die")]
    void TestDie()
    {
        CmdReceiveAttack(new Attack(maxHealth, Vector3.zero, Vector3.zero));
    }

    [ContextMenu("Test Damage 30")]
    void TestDamage30()
    {
        CmdReceiveAttack(new Attack(30, Vector3.zero, Vector3.up * 1.5f));
    }

    #endregion
}
