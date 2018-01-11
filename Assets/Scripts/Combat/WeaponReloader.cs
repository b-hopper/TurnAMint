using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class WeaponReloader : MonoBehaviour
    {

        [SerializeField] int maxAmmo;
        [SerializeField] float reloadTime;
        [SerializeField] int magSize;

        int ammo;
        public int shotsFiredInMag;
        bool isReloading;

        private void Awake()
        {
            ammo = maxAmmo;
        }

        public int RoundsRemainingInMagazine
        {
            get
            {
                return magSize - shotsFiredInMag;
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
            print("Reload started");
            isReloading = true;
            GameManager.Instance.Timer.Add(DoReload, reloadTime);
        }

        private void DoReload()
        {
            print("Reload finished");

            isReloading = false;
            ammo -= shotsFiredInMag;
            shotsFiredInMag = 0;

            if (ammo < 0)
            {
                shotsFiredInMag += -ammo;
                ammo = 0;
            }
        }

        public void RemoveAmmoFromMagazine(int amount)
        {
            shotsFiredInMag += amount;
        }
    }
}