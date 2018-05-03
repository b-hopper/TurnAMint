using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public struct Attack
{
    public int damage;
    public Vector3 origin;
    public Vector3 hitLocation;
    //public Weapon originWeapon;
    public Attack(int newDamage, Vector3 newOrigin, Vector3 newHitLocation)//, Weapon newOriginWeapon)
    {
        damage = newDamage;
        origin = newOrigin;
        hitLocation = newHitLocation;
        //originWeapon = newOriginWeapon;
    }
}
