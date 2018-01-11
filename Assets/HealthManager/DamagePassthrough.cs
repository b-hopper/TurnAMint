using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HealthManagement;

public class DamagePassthrough : MonoBehaviour, IAttackReceiver {
    HealthManager parentHM;

    [SerializeField]
    float damageMultiplier = 1;

    private void Awake()
    {
        parentHM = GetComponentInParent<HealthManager>();
    }    

    public void ReceiveAttack(Weapons.Attack attack)
    {
        Weapons.Attack tmp = new Weapons.Attack((int)(attack.damage * damageMultiplier), attack.origin, attack.hitPoint);
        parentHM.ReceiveAttack(tmp);
    }

}
