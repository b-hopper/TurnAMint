using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class AssaultRifle : Weapon
    {

        public override void Fire(Attack attack)
        {
            
        }

        public override void AttemptFire()
        {
            base.AttemptFire();
            if (canFire)
            {

            }
        }

        private void Update()
        {
            if (GameManager.Instance.InputController.Reload)
            {
                Reload();
            }
        }
    }
}