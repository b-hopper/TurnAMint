using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

    public int weaponIndex;
    public List<WeaponReferenceBase> Weapons = new List<WeaponReferenceBase>();
    WeaponReferenceBase currentWeapon;
    IKHandler ikHandler;
    HandleShooting handleShooting;
    //StateManager states;
    CharacterAudioManager audioManager;

    private void Start()
    {
        // states = GetComponent<StateManager>();
        ikHandler = GetComponent<IKHandler>();
        handleShooting = GetComponent<HandleShooting>();
        audioManager = GetComponent<CharacterAudioManager>();

        CloseAllWeapons();
        SwitchWeapon(weaponIndex);
    }

    private void Update()
    {
        // test switch
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (weaponIndex < Weapons.Count - 1)
            {
                weaponIndex++;
            }
            else
            {
                weaponIndex = 0;
            }
            SwitchWeapon(weaponIndex);
        }
    }

    private void SwitchWeapon(int desiredIndex)
    {
        if (currentWeapon != null)
        {
            currentWeapon.weaponModel.SetActive(false);
            currentWeapon.ikHolder.SetActive(false);
        }

        WeaponReferenceBase newWeapon = Weapons[desiredIndex];

        ikHandler.rightHandIkTarget = newWeapon.rightHandTarget;
        ikHandler.leftHandIkTarget = newWeapon.leftHandTarget;

        if (newWeapon.lookTarget)
        {
            ikHandler.overrideLookTarget = newWeapon.lookTarget;
        }
        else
        {
            ikHandler.overrideLookTarget = null;
        }

        if (newWeapon.modelAnimator)
        {
            handleShooting.modelAnim = newWeapon.modelAnimator;
        }
        else
        {
            handleShooting.modelAnim = null;
        }

        ikHandler.LHIK_dis_notAiming = newWeapon.dis_LHIK_notAiming;

        handleShooting.fireRate = newWeapon.weaponStats.fireRate;
        handleShooting.weaponAnim = newWeapon.ikHolder.GetComponent<Animator>();
        handleShooting.bulletSpawnPoint = newWeapon.bulletSpawnLocation;
        handleShooting.curBullets = newWeapon.weaponStats.curBullets;
        handleShooting.magazineBullets = newWeapon.weaponStats.maxBullets;
        handleShooting.caseSpawn = newWeapon.casingSpawnLocation;
        handleShooting.muzzleFlash = newWeapon.muzzleFlash;
        handleShooting.objPool = newWeapon.objPool;

        audioManager.gunSounds.clip = newWeapon.weaponStats.shotSound;

        weaponIndex = desiredIndex;
        newWeapon.weaponModel.SetActive(true);
        newWeapon.ikHolder.SetActive(true);
        currentWeapon = newWeapon;
    }

    private void CloseAllWeapons()
    {
        for (int i = 0; i < Weapons.Count; i++)
        {
            Weapons[i].weaponModel.SetActive(false);
            Weapons[i].ikHolder.SetActive(false);
        }
    }
}

[System.Serializable]
public class WeaponReferenceBase
{
    public string weaponID;
    public GameObject weaponModel;
    public Animator modelAnimator;
    public GameObject ikHolder;
    public Transform rightHandTarget;
    public Transform leftHandTarget;
    public Transform lookTarget;
    public GameObject[] muzzleFlash;
    public Transform bulletSpawnLocation;
    public Transform casingSpawnLocation;
    public WeaponStats weaponStats;

    public ObjectPool objPool;

    public bool dis_LHIK_notAiming;
}

[System.Serializable]
public class WeaponStats
{
    public int curBullets;
    public int maxBullets;
    public float fireRate;
    public AudioClip shotSound;
}