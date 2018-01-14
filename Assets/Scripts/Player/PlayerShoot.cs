using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

    [SerializeField] float weaponSwitchTime;
    Weapons.Weapon[] weapons;
    [HideInInspector]
    public Weapons.Weapon activeWeapon;

    Transform weaponHolster;

    public event System.Action<Weapons.Weapon> OnWeaponSwitch;

    int currentWeaponIndex;
    bool canFire;

    private void Awake()
    {
        canFire = true;
        weaponHolster = transform.Find("Weapons");
        weapons = weaponHolster.GetComponentsInChildren<Weapons.Weapon>();
               
        if (weapons.Length > 0)
        {
            Equip(0);
        }
    }

    void DeactivateWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].transform.SetParent(weaponHolster);
            weapons[i].gameObject.SetActive(false);
        }
    }

    void SwitchWeapon(int direction)
    {
        canFire = false;
        currentWeaponIndex += direction;

        if (currentWeaponIndex > weapons.Length - 1)
        {
            currentWeaponIndex = 0;
        }
        else if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weapons.Length - 1;
        }
        GameManager.Instance.Timer.Add(() =>
        {
            Equip(currentWeaponIndex);
        }, weaponSwitchTime);        
    }

    void Equip(int index)
    {
        canFire = true;
        DeactivateWeapons();
        weapons[index].gameObject.SetActive(true);
        activeWeapon = weapons[index];
        activeWeapon.Equip();
        if (OnWeaponSwitch != null)
        {
            OnWeaponSwitch(activeWeapon);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.InputController.MouseWheelDown)
        {
            SwitchWeapon(-1);
        }
        else if (GameManager.Instance.InputController.MouseWheelUp)
        {
            SwitchWeapon(1);
        }
        
        if (GameManager.Instance.LocalPlayer.PlayerState.MoveState == PlayerState.EMoveState.SPRINTING)
        {
            return;
        }

        if (GameManager.Instance.InputController.Fire1 && canFire)
        {
            activeWeapon.AttemptFire();
        }


    }
}
