using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
    public class WeaponStats
    {
        public int damage;
        public float fireRate;
        public int roundsPerMag;
        public int shotsPerBurst;

        [BitMask(typeof(FireMode))]
        public FireMode fireMode;

        public AudioClip[] shotSound;
        public MuzzleFlashEffects[] muzzleParticleEffects;
        public ParticleSystem casingParticleEffects;

        public enum FireMode
        {
            Semiauto = (1 << 0),
            Burst = (1 << 1),
            Automatic = (1 << 2)
        }
    }

    [System.Serializable]
    public class MuzzleFlashEffects
    {
        public ParticleSystem[] particleEffects;
    }