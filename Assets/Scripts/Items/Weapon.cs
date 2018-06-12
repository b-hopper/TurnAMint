using UnityEngine;

namespace TurnAMint.Items
{
    public abstract class Weapon : ItemBase
    {
        public WeaponStats stats;
        public Transform leftHandIKTarget, rightHandIKTarget;
        public float leftHandWeight = 1, rightHandWeight = 1;

        public Vector3 alignedWithHandLocalPos, alignedWithHandLocalRot;

        public Vector3 holsteredPosition;
        public Vector3 holsteredEulerAngles;

        public WeaponClass weaponClass;
        public AmmoType ammoType;

        public GameObject ammoPrefab;

        public int currentRoundsInMag;

        [HideInInspector]
        public Transform currentHolster;

        internal int currentFireMode;

        public abstract void Fire();

        public abstract void StopFiring();

        public abstract void RemotePlayerFire();

        internal override void Start()
        {
            currentFireMode = GetStartingFireMode();
            base.Start();
        }

        public virtual void EquipWeapon(Player.StateManager newState)
        {
            state = newState;
        }

        public virtual void UnequipWeapon(Player.StateManager oldState)
        {
            state = null;
        }

        int GetStartingFireMode()
        {
            int i = 0, j = 1;

            if ((int)stats.fireMode == 0)
            {
                Debug.Log("Weapon " + itemName + " does not have any fire modes capable of firing. Check fire mode flags.", this);
                return 0;
            }
            if ((j & (int)stats.fireMode) == 1)
            {
                return 1;
            }
            while (i == 0 && j < (int)stats.fireMode)
            {
                i = j << 1 & (int)stats.fireMode;
                j = j << 1;
            }
            return i;
        }

        public void ChangeFireMode()
        {
            currentFireMode = currentFireMode << 1;

            int i = 0;

            if ((int)stats.fireMode == 0 || (int)stats.fireMode == 1)
            {
                Debug.Log("Weapon " + itemName + " does not have any fire modes capable of firing. Check fire mode flags.", this);
            }
            else
            {
                while ((currentFireMode & (int)stats.fireMode) == 0 && i < 2)
                {
                    currentFireMode = currentFireMode << 1;
                    if (currentFireMode > (int)stats.fireMode)
                    {
                        currentFireMode = 1;
                        i++;
                    }
                }
            }
        }

        [ContextMenu("Assign current transform values to hand position")]
        void AssignTransformToHandPos()
        {
            alignedWithHandLocalPos = transform.localPosition;
            alignedWithHandLocalRot = transform.localEulerAngles;
        }

        [ContextMenu("Assign current transform values to holstered position")]
        void AssignTransformToHolsteredPos()
        {
            holsteredPosition = transform.localPosition;
            holsteredEulerAngles = transform.localEulerAngles;
        }

        public bool Reload()
        {
            if (currentRoundsInMag < stats.roundsPerMag && state.inventoryHandler.CheckIfAmmoAvailable(ammoType))
            {
                state.handleAnim.StartReload();
                state.cBehavior.RequestRemoteReload();

                state.handleAnim.OnReloadFinished += AddRoundsToCurrentMag;

                return true;
            }

            return false;
        }

        void AddRoundsToCurrentMag()
        {
            currentRoundsInMag += state.inventoryHandler.RequestAmmo(ammoType, stats.roundsPerMag - currentRoundsInMag);
            state.handleAnim.OnReloadFinished -= AddRoundsToCurrentMag;
        }
    }

    public enum WeaponClass
    {
        LongGun = 0,
        ShortGun = 1,
        Pistol = 2
    }
}