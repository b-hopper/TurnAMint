using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

    public int maxWeapons = 2;
    public List<WeaponReferenceBase> AvailableWeapons = new List<WeaponReferenceBase>();

    public int weaponIndex;
    public List<WeaponReferenceBase> Weapons = new List<WeaponReferenceBase>();
    WeaponReferenceBase currentWeapon;
    IKHandler ikHandler;
    HandleShooting handleShooting;
    //StateManager states;
    CharacterAudioManager audioManager;

    private void Start()
    {
        //states = GetComponent<StateManager>();
        ikHandler = GetComponent<IKHandler>();
        handleShooting = GetComponent<HandleShooting>();
        audioManager = GetComponent<CharacterAudioManager>();

        if (Weapons.Count > 0)
        {
            AvailableWeapons.Add(Weapons[0]);
        }
        weaponIndex = 0;

        CloseAllWeapons();
        SwitchWeapon(weaponIndex);
    }

    private void Update()
    {
        // test switch
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (weaponIndex < AvailableWeapons.Count - 1)
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
        if (desiredIndex > AvailableWeapons.Count - 1)
        {
            desiredIndex = 0;
            weaponIndex = 0;
        }

        if (currentWeapon != null)
        {
            currentWeapon.weaponModel.SetActive(false);
            currentWeapon.ikHolder.SetActive(false);
        }

        WeaponReferenceBase newWeapon = AvailableWeapons[desiredIndex];
        SetNewWeapon(newWeapon, desiredIndex);
    }

    public WeaponReferenceBase ReturnWeaponWithID(string weaponID)
    {
        WeaponReferenceBase retVal = null;

        for (int i = 0; i < Weapons.Count; i++)
        {
            if (string.Equals(Weapons[i].weaponID, weaponID))
            {
                retVal = Weapons[i];
                break;
            }
        }
        return retVal;
    }

    void SetNewWeapon(WeaponReferenceBase newWeapon, int desiredIndex)
    {
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

    public WeaponReferenceBase ReturnCurrentWeapon()
    {
        if (currentWeapon != null)
        {
            Debug.Log(currentWeapon);
            return currentWeapon;
        }
        else
        {
            return null;
        }
    }

    public void SwitchWeaponWithTargetWeapon(WeaponReferenceBase targetWeapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.weaponModel.SetActive(false);
            currentWeapon.ikHolder.SetActive(false);
        }

        WeaponReferenceBase newWeapon = targetWeapon;
        SetNewWeapon(newWeapon, weaponIndex);
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

    public GameObject pickablePrefab;
}

[System.Serializable]
public class WeaponStats
{
    public int curBullets;
    public int maxBullets;
    public float fireRate;
    public AudioClip shotSound;
    public int damage;

    public Attack attack
    {
        get
        {
            return new Attack(damage, Vector3.zero, Vector3.zero);
        }
    }
    //etc...
}

public struct Attack
{
    public int damage;
    public Vector3 origin;
    public Vector3 hitLocation;
    public Attack(int newDamage, Vector3 newOrigin, Vector3 newHitLocation)
    {
        damage = newDamage;
        origin = newOrigin;
        hitLocation = newHitLocation;
    }
}