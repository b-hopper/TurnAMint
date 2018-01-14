using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    [SerializeField]
    float range, speed;
    [SerializeField]
    float timeToLive;
    [SerializeField]
    int damage;
    

    private void Start()
    {
        Destroy(gameObject, timeToLive);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            ExecuteEvents.Execute<HealthManagement.IAttackReceiver>(hit.transform.gameObject, null, ((handler, eventData) => handler.ReceiveAttack(new Weapons.Attack(damage, Vector3.zero, Vector3.zero))));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
