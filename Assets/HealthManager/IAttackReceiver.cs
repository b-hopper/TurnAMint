using UnityEngine.EventSystems;

namespace HealthManagement
{
    public interface IAttackReceiver : IEventSystemHandler
    {
        void ReceiveAttack(Weapons.Attack attack);
    }
}
