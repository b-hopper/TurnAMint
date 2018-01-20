using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
<<<<<<< HEAD
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class HandleShooting : NetworkBehaviour {
=======

public class HandleShooting : MonoBehaviour {
>>>>>>> refs/remotes/origin/master
=======

public class HandleShooting : MonoBehaviour {
>>>>>>> refs/remotes/origin/master

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

<<<<<<< HEAD
<<<<<<< HEAD
    [HideInInspector] public ImpactProfile impactProfile;

=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
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
                    states.audioManager.PlayGunSound();

                    if (modelAnim != null)
                    {
                        modelAnim.SetBool("Shoot", true);
                    }

                    weaponAnim.SetBool("Shoot", true);

<<<<<<< HEAD
<<<<<<< HEAD
=======
                    Debug.Log(objPool);
>>>>>>> refs/remotes/origin/master
=======
                    Debug.Log(objPool);
>>>>>>> refs/remotes/origin/master
                    if (objPool != null && caseSpawn != null)
                    {
                        GameObject go = objPool.GetNewObj();
                        go.transform.position = caseSpawn.position;
                        go.transform.rotation = caseSpawn.rotation;

                        Rigidbody rig = go.GetComponent<Rigidbody>();
                        rig.AddForce(transform.right.normalized * 2 + Vector3.up * 1.3f, ForceMode.Impulse);
                        rig.AddRelativeTorque(go.transform.right * 1.5f, ForceMode.Impulse);
                    }

                    states.actualShooting = true;

                    if (muzzleFlash.Length > 0)
                    {
                        int index = UnityEngine.Random.Range(0, muzzleFlash.Length - 1);
                        muzzleFlash[index].SetActive(true);
                        GameManager.Instance.Timer.Add(() =>
                        {
                            muzzleFlash[index].SetActive(false);
                        }, 0.1f);
                    }

<<<<<<< HEAD
<<<<<<< HEAD
                    Vector3 direction = states.lookHitPosition - bulletSpawnPoint.position;

                    CmdRaycastShoot(bulletSpawnPoint.position, direction);
=======
                    RaycastShoot();
>>>>>>> refs/remotes/origin/master
=======
                    RaycastShoot();
>>>>>>> refs/remotes/origin/master

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

<<<<<<< HEAD
<<<<<<< HEAD
    [Command]
    private void CmdRaycastShoot(Vector3 origin, Vector3 direction)
    {
        print("shoot test: " + this);
        RaycastHit hit;

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
=======
=======
>>>>>>> refs/remotes/origin/master
    private void RaycastShoot()
    {
        Vector3 direction = states.lookHitPosition - bulletSpawnPoint.position;
        RaycastHit hit;

        Debug.DrawRay(bulletSpawnPoint.position, bulletSpawnPoint.forward * 100, Color.red, 15);

        if (Physics.Raycast(bulletSpawnPoint.position, direction, out hit, 100, states.layerMask))
        {
            if (smokeParticle != null)
            {
                GameObject go = Instantiate(smokeParticle, hit.point, Quaternion.identity) as GameObject;
                go.transform.LookAt(bulletSpawnPoint.position);
            }

            /*if (hit.transform.GetComponent<ShootingRangeTarget>())
            {
                hit.transform.GetComponent<ShootingRangeTarget>().HitTarget();                      // HEALTHMANAGER HERE
            }*/
<<<<<<< HEAD
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
        }
    }
}
