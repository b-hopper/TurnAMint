using UnityEngine;
using UnityEngine.Events;

public class AttackReceiver : MonoBehaviour, IAttackReceiver {
	public UnityEvent OnAttackReceived;
    
    public void ReceiveAttack(Attack attack)
    {
        OnAttackReceived.Invoke();
    }
}
