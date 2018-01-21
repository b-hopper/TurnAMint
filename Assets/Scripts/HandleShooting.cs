using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class HandleShooting : NetworkBehaviour {

    StateManager states;
    [HideInInspector] public Animator weaponAnim;
    [HideInInspector] public Animator modelAnim;
    [HideInInspector] public float fireRate;
    float timer;
    [HideInInspector] public Transform bulletSpawnPoint;
    [HideInInspector] public GameObject smokeParticle;            // IMPACT SYSTEM HERE
    [HideInInspector] public GameObject[] muzzleFlash;

    [HideInInspector] public Transform caseSpawn;

    [HideInInspector] public ObjectPool objPool;

    [HideInInspector] public ImpactProfile impactProfile;

    WeaponManager weaponManager;

    public int curBullets = 30;
    public int magazineBullets = 0;

    private void Start()
    {
        objPool = GetComponent<ObjectPool>();
        
        states = GetComponent<StateManager>();
        weaponManager = GetComponent<WeaponManager>();

    }

    bool shoot;
    bool dontShoot;

    bool emptyGun;

    private void Update()
    {
        shoot = states.shoot;

        if (modelAnim != null)
        {
            modelAnim.SetBool("Shoot", false);

            if (curBullets > 0)
            {
                modelAnim.SetBool("Empty", false);
            }
            else
            {
                modelAnim.SetBool("Empty", true);
            }
        }

        if (shoot)
        {
            if (timer <= 0)
            {
                if (modelAnim != null)
                {
                    modelAnim.SetBool("Shoot", false);
                }
                weaponAnim.SetBool("Shoot", false);

                if (curBullets > 0)
                {
                    emptyGun = false;
                    
                    if (modelAnim != null)
                    {
                        modelAnim.SetBool("Shoot", true);
                    }

                    weaponAnim.SetBool("Shoot", true);


                    states.actualShooting = true;

                    Vector3 direction = states.lookHitPosition - bulletSpawnPoint.position;

                    if (isLocalPlayer)
                    {
                        CmdDoOnShootActions();
                        CmdRaycastShoot(bulletSpawnPoint.position, direction);
                    }

                    curBullets -= 1;
                }
                else
                {
                    if (emptyGun)
                    {
                        states.handleAnim.StartReload();
                        curBullets = magazineBullets;
                    }
                    else
                    {
                        states.audioManager.PlayEffect("empty_gun");
                        emptyGun = true;
                    }
                }
                timer = fireRate;
            }
            else
            {
                states.actualShooting = false;

                weaponAnim.SetBool("Shoot", false);
                timer -= Time.deltaTime;
            }            

        }
        else
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 0;
            }

            states.actualShooting = false;

            if (weaponAnim != null)
            {
                weaponAnim.SetBool("Shoot", false);
            }
        }
    }

    [Command]
    private void CmdDoOnShootActions()
    {
        states.audioManager.PlayGunSound();

        if (muzzleFlash.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, muzzleFlash.Length - 1);
            muzzleFlash[index].SetActive(true);
            GameManager.Instance.Timer.Add(() =>
            {
                muzzleFlash[index].SetActive(false);
            }, 0.1f);
        }

        if (objPool != null && caseSpawn != null)
        {
            GameObject go = objPool.GetNewObj();
            go.transform.position = caseSpawn.position;
            go.transform.rotation = caseSpawn.rotation;

            Rigidbody rig = go.GetComponent<Rigidbody>();
            rig.AddForce(transform.right.normalized * 2 + Vector3.up * 1.3f, ForceMode.Impulse);
            rig.AddRelativeTorque(go.transform.right * 1.5f, ForceMode.Impulse);
        }
    }

    [Command]
    private void CmdRaycastShoot(Vector3 origin, Vector3 direction)
    {
        states.audioManager.PlayGunSound();
        RaycastHit hit;

        print("Origin: " + origin + ", Direction: " + direction);

        Debug.DrawRay(origin, direction * 100, Color.red, 15);

        if (Physics.Raycast(origin, direction, out hit, 100, states.layerMask))
        {
            if (impactProfile != null)
            {
                ImpactInfo impact = impactProfile.GetImpactInfo(hit);
                GameObject cloneImpact = Instantiate(impact.GetRandomPrefab(), hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;   // Need to decide where Object Pool goes, here
                cloneImpact.transform.parent = hit.transform;
            }
            print("Hit: " + hit.collider.gameObject);

            if (isServer)
            {
                ExecuteEvents.Execute<IAttackReceiver>(hit.collider.gameObject, null, ((handler, eventData) => handler.RpcReceiveAttack(weaponManager.ReturnCurrentWeapon().weaponStats.attack)));
            }
            else
            {
                ExecuteEvents.Execute<IAttackReceiver>(hit.collider.gameObject, null, ((handler, eventData) => handler.CmdReceiveAttack(weaponManager.ReturnCurrentWeapon().weaponStats.attack)));
            }
        }
    }
}
