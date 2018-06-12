using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TurnAMint.Player.Health;

namespace TurnAMint.Items
{
    public class WeaponGun : Weapon
    {
        #region Declaring Variables, and Awake()
        public Transform muzzle;
        public LayerMask shotMask;

        AudioSource audioSource;


        int shotsSinceReset, bulletIndex;

        public float shotForce;

        public GameObject bulletObj;

        float nextShotTime;

        bool isFiring;

        internal override void Awake()
        {
            audioSource = GetComponentInChildren<AudioSource>();
            base.Awake();
        }
        #endregion

        #region Update
        private void FixedUpdate()
        {
            if (isFiring && CanFire())
            {
                FireActual();
            }
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(muzzle.position, Vector3.one * 0.1f);
            Gizmos.color = new Vector4(1, 0.5f, 0, 1);
            Gizmos.DrawRay(muzzle.position, muzzle.forward * 20);
        }
        #endregion

        private bool CanFire()
        {
            bool val = false;
            if (currentRoundsInMag <= 0 || state.handleAnim.isReloading())
            {
                return val;
            }
            if (Time.time > nextShotTime)
            {
                switch (currentFireMode)
                {
                    case (1 << 2):
                        val = true;
                        break;
                    case (1 << 1):
                        if (shotsSinceReset < stats.shotsPerBurst)
                        {
                            val = true;
                            shotsSinceReset++;
                        }
                        else
                        {
                            isFiring = false;
                        }
                        break;
                    case (1 << 0):
                        if (shotsSinceReset == 0)
                        {
                            val = true;
                            shotsSinceReset++;
                        }
                        else
                        {
                            isFiring = false;
                        }
                        break;
                }
            }
            return val;
        }

        public override void Fire()
        {
            enabled = true;
            isFiring = true;
        }

        public override void StopFiring()
        {
            shotsSinceReset = 0;
            isFiring = false;
            enabled = false;
        }

        private void FireActual()
        {
            state.cBehavior.RequestRemoteShoot();

            DoEffectiveFire();
            DoShotEffects();

            currentRoundsInMag--;

            nextShotTime = Time.time + stats.fireRate;
        }

        public override void RemotePlayerFire()
        {
            DoShotEffects();
        }

        public void DoEffectiveFire()
        {
            ////// This is separated out because the raycast shooting is temporary. I need two commands sent to the server, one for shots fired and one for shots hit
            ////// The shots hit one will probably have to move to the projectile scripts, so I'm keeping them separate

            RaycastHit hit;

            Debug.DrawRay(muzzle.position, (state.lookHitPosition - muzzle.position).normalized * 100, Color.red, 10);

            if (Physics.Raycast(muzzle.position, (state.lookHitPosition - muzzle.position).normalized, out hit, 100, shotMask))
            {
                Attack newAttack = new Attack(stats.damage, muzzle.position, hit.point);
                ExecuteEvents.Execute<IAttackReceiver>(hit.collider.gameObject, null, ((handler, eventData) => handler.ReceiveAttack(newAttack)));

                state.cBehavior.RequestPlayerHit(newAttack, hit.transform.root.gameObject);
            }

            /*
            GameObject tmp = GameManager.Instance.ObjectPool.GetNewObj(bulletPoolIndex, bulletIndex);
            Rigidbody rb = GameManager.Instance.ObjectPool.GetRigidBody(tmp);
            tmp.transform.position = muzzle.position;
            tmp.SetActive(false);
            tmp.SetActive(true);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            tmp.transform.parent = null;
            rb.AddForce((playerState.lookHitPosition - muzzle.position).normalized * shotForce, ForceMode.Impulse);

            bulletIndex = (bulletIndex + 1) % bulletObjectCount;
            */

        }

        public void DoShotEffects()
        {
            if (stats.muzzleParticleEffects.Length > 0)
            {
                int i = UnityEngine.Random.Range(0, stats.muzzleParticleEffects.Length);
                for (int j = 0; j < stats.muzzleParticleEffects[i].particleEffects.Length; j++)
                {
                    stats.muzzleParticleEffects[i].particleEffects[j].Play();
                }
            }

            if (stats.casingParticleEffects != null)
            {
                stats.casingParticleEffects.Play();
            }

            if (stats.shotSound.Length > 0)
            {
                audioSource.clip = stats.shotSound[UnityEngine.Random.Range(0, stats.shotSound.Length)];
                audioSource.Play();
            }
        }
    }

}

public class BitMaskAttribute : PropertyAttribute       // I don't like that this is here, but it doesn't seem to work elsewhere
{                                                       // Soft TODO: Figure out why I can't move this. Not urgent
    public Type propType;
    public BitMaskAttribute(Type aType)
    {
        propType = aType;
    }
}