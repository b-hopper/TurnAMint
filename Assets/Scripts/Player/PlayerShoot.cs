using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

    [SerializeField]
    Weapons.Weapon assaultRifle;

    private void Update()
    {
        if (GameManager.Instance.InputController.Fire1)
        {
            assaultRifle.AttemptFire();
        }
    }
}
