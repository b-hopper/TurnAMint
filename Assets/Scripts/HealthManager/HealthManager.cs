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

    StateManager state;

    public UnityEvent OnDamaged;
    public UnityEvent OnHealthDepleted;

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
        OnDamaged.Invoke();

        if (currentHealth <= 0)
        {
            OnHealthDepleted.Invoke();
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
}
