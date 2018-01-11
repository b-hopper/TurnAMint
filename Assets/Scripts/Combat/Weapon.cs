using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public struct Attack
    {
        public int damage;
        public Vector3 origin;
        public Vector3 hitPoint;

        public Attack(int newDamage, Vector3 newOrigin, Vector3 newHitPoint)
        {
            damage = newDamage;
            origin = newOrigin;
            hitPoint = newHitPoint;
        }
    }

    public class Weapon : MonoBehaviour
    {

        [SerializeField] float rateOfFire;
        [SerializeField] int damage;
        [SerializeField] Projectile projectile;
        [SerializeField] Transform hand;
        [SerializeField] AudioController audioReload;
        [SerializeField] AudioController audioFire;

        [HideInInspector]
        public WeaponReloader reloader;

        float nextFireAllowed;
        Transform muzzle;

        [HideInInspector]
        public bool canFire;

        public void Equip()
        {
            transform.SetParent(hand);      // TEMPORARY
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        
        private void Awake()
        {
            muzzle = transform.Find("Model/Muzzle");
            reloader = GetComponent<WeaponReloader>();

        }

        public void Reload()
        {
            if (reloader == null)
            {
                return;
            }
            audioReload.Play();
            reloader.Reload();
        }

        public virtual void AttemptFire()
        {
            canFire = false;
            if (Time.time < nextFireAllowed)
            {
                return;
            }

            if (reloader != null)
            {
                if (reloader.IsReloading || reloader.RoundsRemainingInMagazine == 0)
                {
                    return;
                }

                reloader.RemoveAmmoFromMagazine(1);
            }

            Instantiate(projectile, muzzle.position, muzzle.rotation);
            Attack attack = new Attack(damage, muzzle.transform.position, Vector3.zero);

            Fire(attack);

            nextFireAllowed = Time.time + rateOfFire;

            canFire = true;
        }

        public virtual void Fire(Attack attack)
        {
            audioFire.Play();
        }
    }
}