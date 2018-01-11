using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    [SerializeField]
    float speed;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        ExecuteEvents.Execute<HealthManagement.IAttackReceiver>(other.gameObject, null, ((handler, eventData) => handler.ReceiveAttack(new Weapons.Attack(damage, Vector3.zero, Vector3.zero))));
    }
}
