using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class WeaponReloader : MonoBehaviour
    {
        /*
         *      Following a tutorial for a lot of the basic movement/3rd person stuff.
         *      I'm planning on redoing this whole thing, I don't like how he handled it.
         *      Will fix later, though. 
         */


        [SerializeField] int maxAmmo;
        [SerializeField] float reloadTime;
        [SerializeField] int magSize;
        [SerializeField] Container inventory;
        [SerializeField] EWeaponType weaponType;

        int ammo;
        public int shotsFiredInMag;
        bool isReloading;
        System.Guid ContainerItemId;

        public event System.Action OnAmmoChanged;

        private void Awake()
        {
            inventory.OnContainerReady += () =>
            {
                ContainerItemId = inventory.Add(weaponType.ToString(), maxAmmo);
            };

        }

        public int RoundsRemainingInMagazine
        {
            get
            {
                return magSize - shotsFiredInMag;
            }
        }

        public int RoundsRemainingInInventory
        {
            get
            {
                return inventory.GetAmountRemaining(ContainerItemId);
            }
        }

        public bool IsReloading
        {
            get
            {
                return isReloading;
            }
        }        

        public void Reload()
        {
            if (isReloading)
            {
                return;
            }
            isReloading = true;
                        
            GameManager.Instance.Timer.Add(() => { DoReload(inventory.RemoveFromContainer(ContainerItemId, magSize - RoundsRemainingInMagazine)); }, reloadTime);
        }

        private void DoReload(int amount)
        {
            isReloading = false;
            shotsFiredInMag -= amount;
            HandleOnAmmoChanged();
        }

        public void RemoveAmmoFromMagazine(int amount)
        {
            shotsFiredInMag += amount;
            HandleOnAmmoChanged();
        }

        public void HandleOnAmmoChanged()
        {
            if (OnAmmoChanged != null)
            {
                OnAmmoChanged();
            }
        }
    }
}