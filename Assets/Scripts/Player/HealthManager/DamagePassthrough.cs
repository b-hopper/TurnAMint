using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnAMint.Player.Health
{
    public class DamagePassthrough : MonoBehaviour, IAttackReceiver
    {
        HealthManager parentHM;

        [SerializeField]
        float damageMultiplier = 1;

        private void Start()
        {
            parentHM = GetComponentInParent<HealthManager>();
        }

        public void ReceiveAttack(Attack attack)
        {
            attack.damage = (int)(attack.damage * damageMultiplier);
            parentHM.ReceiveAttack(attack);
        }
    }
}