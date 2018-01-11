using UnityEngine;
using UnityEngine.Events;
using HealthManagement;

public class AttackReceiver : MonoBehaviour, IAttackReceiver {
	public UnityEvent OnAttackReceived;

	public void ReceiveAttack (Weapons.Attack attack)
	{
		OnAttackReceived.Invoke();
	}
}
