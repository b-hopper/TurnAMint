using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandleInventory : NetworkBehaviour, IInventoryHandler {

    StateManager state;

    PlayerInventory inventory;

    public Transform InventoryParent;

    HandleAnimations animHandler;

    public WeaponClass equippedWeaponClass;
    int currentIndex = 0;

    public WeaponHolster holster;
    public WeaponHolster pistolHolster;

    public delegate void InventoryChangedEvent(bool isAdding, ItemBase item);
    public InventoryChangedEvent OnInventoryChanged;

    private void Awake()
    {
        state = GetComponent<StateManager>();
        inventory = GetComponent<PlayerInventory>();
        animHandler = GetComponent<HandleAnimations>();
    }

    public void PickUp(ItemBase newItem)
    {
        if (newItem == null) return;
        int i = inventory.AddItemToInventory(newItem);

        if (i != -1)
        {
            PlaceItem(i, newItem);
            newItem.EnableColliders(false);
            state.PrepareInteract(false);
            if (OnInventoryChanged != null)
            {
                OnInventoryChanged(true, newItem);
            }
        }
    }

    void PlaceItem(int locationIndex, ItemBase item)
    {        
        switch (locationIndex)
        {
            case 3:             // Armor
                HandleArmorSwitch(item);
                break;
            case 4:             // Cosmetic
                HandleCosmeticSwitch(item);
                break;
            case 5:             // Non-visible
                item.transform.parent = InventoryParent;
                item.transform.localPosition = Vector3.zero;
                item.transform.localEulerAngles = Vector3.zero;
                item.transform.gameObject.SetActive(false);
                break;
            default:
                HandleWeaponPickup(item);
                break;
        }
    }

    private void HandleWeaponPickup(ItemBase item)
    {
        Weapon wpn = item.weapon;

        Weapon prevWeapon = inventory.GetWeapon(wpn.weaponClass);
        
        if (state.currentWeapon == prevWeapon)
        {
            state.ForceEquipWeapon(wpn);
            equippedWeaponClass = wpn.weaponClass;
            animHandler.ForceChangeWeapon(wpn);
        }
        else
        {
            ForceHolsterWeapon(wpn);
        }
        DropItem(prevWeapon);
        inventory.AssignWeapon(wpn);
    }

    private void HandleCosmeticSwitch(ItemBase item)
    {
        throw new NotImplementedException();
    }

    private void HandleArmorSwitch(ItemBase item)
    {
        throw new NotImplementedException();
    }

    public void HolsterWeapon(Weapon wpn)
    {
        ForceHolsterWeapon(wpn);
        state.currentWeapon = null;
        state.currentWeaponType = 0;
    }

    public void ForceHolsterWeapon(Weapon wpn)      // Instantly sets holstered location
    {
        if (wpn == null) return;
        WeaponHolster finalHolster = wpn.weaponClass == WeaponClass.Pistol ? pistolHolster : holster;

        wpn.currentHolster = finalHolster.RequestEmptyHolster();
        if (wpn.currentHolster != null)
        {
            wpn.transform.parent = wpn.currentHolster;
            wpn.transform.localPosition = wpn.holsteredPosition;
            wpn.transform.localEulerAngles = wpn.holsteredEulerAngles;
        }
        else
        {
            wpn.gameObject.SetActive(false);
        }
        wpn.UnequipWeapon(state);
    }

    public void UnholsterWeapon(Weapon wpn)
    {
        if (wpn != null)
        {
            WeaponHolster finalHolster = wpn.weaponClass == WeaponClass.Pistol ? pistolHolster : holster;

            finalHolster.RemoveFromHolster(wpn.currentHolster);
            wpn.currentHolster = null;
        }
    }

    public Transform DropItem(ItemBase item)        // Returns drop location
    {
        if (item == null)
        {
            return null;
        }
        if (item.itemType == ItemType.Weapon)
        {
            if (item.weapon == state.currentWeapon)
            {
                if (isLocalPlayer) UIManager.instance.EnableAmmoCountText(false);
                state.SetCurrentWeapon(null);
                animHandler.ForceUnequipWeapon();
            }
            UnholsterWeapon(item.weapon);
            item.weapon.UnequipWeapon(state);
        }

        inventory.RemoveItem(item);

        item.transform.parent = null;
        item.transform.position = transform.position + (transform.forward * 0.7f) + (transform.up * 0.3f);
        item.transform.eulerAngles = Vector3.zero;
        if (OnInventoryChanged != null)
        {
            OnInventoryChanged(false, item);
        }
        item.OnDrop();

        state.cBehavior.RemoteDrop(item.gameObject, item.transform.position);

        return item.transform;
    }

    public void HandleWeaponSwitchWithIndex(int index)
    {
        if (inventory.HasAnyWeapons())
        {
            Weapon wpn = inventory.GetWeapon((WeaponClass)index);
            if (wpn == null)
            {
                return;
            }
            if (state.currentWeapon == wpn)
            {
                return;
            }            
            animHandler.ChangeWeapon(wpn, state.currentWeapon != null);
            state.isHolstering = true;
            equippedWeaponClass = wpn.weaponClass;
        }
    }

    public void HandleWeaponSwitchWithDirection(int direction)
    {
        if (inventory.HasAnyWeapons())
        {
            if (state.currentWeapon == null)
            {
                animHandler.ChangeWeapon(inventory.GetWeapon(equippedWeaponClass), false);
                state.isHolstering = true;
            }
            else
            {
                int index = IncrementIndex((int)equippedWeaponClass, direction);

                Weapon wpn = inventory.GetWeapon((WeaponClass)index);
                if (wpn == null)
                    return;
                if (state.currentWeapon == wpn)
                    return;
                animHandler.ChangeWeapon(wpn, true);
                state.isHolstering = true;
                equippedWeaponClass = wpn.weaponClass;
            }
        }
    }

    int IncrementIndex(int index, int direction)                //*****CAUSING OVERFLOW*****//
    {   /*    
                                                                      
        index = (index + direction) % 3;                              

        if (inventory.GetWeapon((WeaponClass)index) == null)
        {
            index = IncrementIndex(index, direction);
        }*/
        return index;
    }

    public void HandleWeaponUnequip()
    {
        if (isLocalPlayer) GameManager.Instance.UIManager.EnableAmmoCountText(false);
        if (state.currentWeapon != null)
        {
            animHandler.ChangeWeapon(null, true);
        }
    }

    public int GetTotalAmmo(AmmoType ammoType)
    {
        return inventory.GetAmmoCount(ammoType);        
    }

    public int RequestAmmo(AmmoType ammoType, int amount)
    {
        int ret = inventory.RequestAmmo(ammoType, amount);

        if (OnInventoryChanged != null)
        {
            //OnInventoryChanged(false, inventory)
        }

        return ret;
    }

    public bool CheckIfAmmoAvailable(AmmoType ammoType)
    {
        return inventory.CheckIfAmmoAvailable(ammoType);
    }

    public float GetRemainingInventoryWeight()
    {
        return inventory.GetRemainingWeight;
    }
}
