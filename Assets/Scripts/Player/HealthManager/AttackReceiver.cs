using UnityEngine;
using UnityEngine.Events;

namespace TurnAMint.Player.Health
{
    public class AttackReceiver : MonoBehaviour, IAttackReceiver
    {
        public UnityEvent OnAttackReceived;

        public void ReceiveAttack(Attack attack)
        {
            OnAttackReceived.Invoke();
        }
    }
}