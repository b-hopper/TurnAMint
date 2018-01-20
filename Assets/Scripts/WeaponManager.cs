using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

<<<<<<< HEAD
<<<<<<< HEAD
    public int maxWeapons = 2;
    public List<WeaponReferenceBase> AvailableWeapons = new List<WeaponReferenceBase>();

=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
    public int weaponIndex;
    public List<WeaponReferenceBase> Weapons = new List<WeaponReferenceBase>();
    WeaponReferenceBase currentWeapon;
    IKHandler ikHandler;
    HandleShooting handleShooting;
    //StateManager states;
    CharacterAudioManager audioManager;

    private void Start()
    {
<<<<<<< HEAD
<<<<<<< HEAD
        //states = GetComponent<StateManager>();
=======
        // states = GetComponent<StateManager>();
>>>>>>> refs/remotes/origin/master
=======
        // states = GetComponent<StateManager>();
>>>>>>> refs/remotes/origin/master
        ikHandler = GetComponent<IKHandler>();
        handleShooting = GetComponent<HandleShooting>();
        audioManager = GetComponent<CharacterAudioManager>();

<<<<<<< HEAD
<<<<<<< HEAD
        if (Weapons.Count > 0)
        {
            AvailableWeapons.Add(Weapons[0]);
        }
        weaponIndex = 0;

=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
        CloseAllWeapons();
        SwitchWeapon(weaponIndex);
    }

    private void Update()
    {
        // test switch
        if (Input.GetKeyUp(KeyCode.Q))
        {
<<<<<<< HEAD
<<<<<<< HEAD
            if (weaponIndex < AvailableWeapons.Count - 1)
=======
            if (weaponIndex < Weapons.Count - 1)
>>>>>>> refs/remotes/origin/master
=======
            if (weaponIndex < Weapons.Count - 1)
>>>>>>> refs/remotes/origin/master
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
<<<<<<< HEAD
<<<<<<< HEAD
        if (desiredIndex > AvailableWeapons.Count - 1)
        {
            desiredIndex = 0;
            weaponIndex = 0;
        }

=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
        if (currentWeapon != null)
        {
            currentWeapon.weaponModel.SetActive(false);
            currentWeapon.ikHolder.SetActive(false);
        }

<<<<<<< HEAD
<<<<<<< HEAD
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
=======
        WeaponReferenceBase newWeapon = Weapons[desiredIndex];

>>>>>>> refs/remotes/origin/master
=======
        WeaponReferenceBase newWeapon = Weapons[desiredIndex];

>>>>>>> refs/remotes/origin/master
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
<<<<<<< HEAD
<<<<<<< HEAD
        
=======

>>>>>>> refs/remotes/origin/master
=======

>>>>>>> refs/remotes/origin/master
        audioManager.gunSounds.clip = newWeapon.weaponStats.shotSound;

        weaponIndex = desiredIndex;
        newWeapon.weaponModel.SetActive(true);
        newWeapon.ikHolder.SetActive(true);
        currentWeapon = newWeapon;
    }

<<<<<<< HEAD
<<<<<<< HEAD
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

=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
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
<<<<<<< HEAD
<<<<<<< HEAD

    public GameObject pickablePrefab;
=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
}

[System.Serializable]
public class WeaponStats
{
    public int curBullets;
    public int maxBullets;
    public float fireRate;
    public AudioClip shotSound;
<<<<<<< HEAD
<<<<<<< HEAD
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
=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
}