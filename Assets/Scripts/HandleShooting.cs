using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleShooting : MonoBehaviour {

    StateManager states;
    public Animator weaponAnim;
    public float fireRate;
    float timer;
    public Transform bulletSpawnPoint;
    public GameObject smokeParticle;            // IMPACT SYSTEM HERE
    public ParticleSystem[] muzzle;
    
    public Transform caseSpawn;

    ObjectPool objPool;

    public int curBullets = 30;

    private void Start()
    {
        objPool = GetComponent<ObjectPool>();
        
        states = GetComponent<StateManager>();

    }

    bool shoot;
    bool dontShoot;

    bool emptyGun;

    private void Update()
    {
        shoot = states.shoot;

        if (shoot)
        {
            if (timer <= 0)
            {
                weaponAnim.SetBool("Shoot", true);

                if (curBullets > 0)
                {
                    emptyGun = false;
                    states.audioManager.PlayGunSound();

                    GameObject go = objPool.GetNewObj();
                    go.transform.position = caseSpawn.position;
                    go.transform.rotation = caseSpawn.rotation;

                    Rigidbody rig = go.GetComponent<Rigidbody>();
                    rig.AddForce(transform.right.normalized * 2 + Vector3.up * 1.3f, ForceMode.Impulse);
                    rig.AddRelativeTorque(go.transform.right * 1.5f, ForceMode.Impulse);

                    if (muzzle.Length > 0)
                    {
                        for (int i = 0; i < muzzle.Length; i++)
                        {
                            muzzle[i].Emit(1);
                        }
                    }

                    RaycastShoot();

                    curBullets -= 1;
                }
                else
                {
                    if (emptyGun)
                    {
                        states.handleAnim.StartReload();
                        curBullets = 30;
                    }
                    else
                    {
                        states.audioManager.PlayEffect("empty_gun");
                        emptyGun = true;
                    }
                }
                timer = fireRate;
            }

            timer -= Time.deltaTime;

        }
        else
        {
            timer = -1;
            weaponAnim.SetBool("Shoot", false);
        }
    }

    private void RaycastShoot()
    {
        Vector3 direction = states.lookHitPosition - bulletSpawnPoint.position;
        RaycastHit hit;

        //Debug.DrawRay(bulletSpawnPoint.position, bulletSpawnPoint.forward * 100, Color.red, Mathf.Infinity);

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
        }
    }
}
