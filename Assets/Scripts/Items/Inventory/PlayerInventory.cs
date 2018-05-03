using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IInventoryHandler))]
public class PlayerInventory : MonoBehaviour {

    public Inventory inventory;

    IInventoryHandler invHandler;

    public Weapon LongGun;
    public Weapon ShortGun;
    public Weapon Pistol;

    int currentWeaponIndex;

    public float totalAllowedWeight;
    float currentWeight;
    public float GetRemainingWeight
    {
        get
        {
            return totalAllowedWeight - currentWeight;
        }
    }

    private void Awake()
    {
        inventory = new Inventory();
        invHandler = GetComponent<IInventoryHandler>();
    }

    public int AddItemToInventory(ItemBase item)
    {
        int ret = -1;

        if (item.itemType == ItemType.Weapon)
        {
            Weapon wpn = item.weapon;
            ret = (int)wpn.weaponClass;

            inventory.ItemsInInventory.Add(wpn);            
        }
        else if (item.itemType == ItemType.Cosmetic)
        {
            // Handle cosmetic
        }
        else if (currentWeight + item.totalWeight <= totalAllowedWeight)
        {
            if (item.stackable)
            {
                if (item.itemType == ItemType.Ammo)
                {
                    Ammunition tmp = item.ammo;
                    inventory.ammoCounts.AddAmmoToInventory(tmp.ammoType, tmp.amount);
                }
                bool exists = false;
                for (int i = 0; i < inventory.ItemsInInventory.Count; i++)
                {
                    if (inventory.ItemsInInventory[i].name == item.name)
                    {
                        inventory.ItemsInInventory[i].amount += item.amount;
                        exists = true;
                    }
                }
                if (!exists)
                {
                    inventory.ItemsInInventory.Add(item);
                }
                ret = (int)VisibilityType.NonVisible;
            }
            else
            {
                inventory.ItemsInInventory.Add(item);
                ret = (int)VisibilityType.NonVisible;       // OTHER VISIBILITY TYPES HERE
            }
        }

        return ret;
    }

    public ItemBase RequestItemFromInventory(ItemBase item)
    {
        ItemBase tmp = null;

        for (int i = 0; i < inventory.ItemsInInventory.Count; i++)
        {
            if (item.name == inventory.ItemsInInventory[i].name)
            {
                tmp = inventory.ItemsInInventory[i];
                break;
            }
        }
        return tmp;
    }

    public int RequestAmmo(AmmoType type, int amount)
    {
        int ret = inventory.ammoCounts.GetAmmoFromInventory(type, amount);
        return ret;
    }

    public bool CheckIfAmmoAvailable(AmmoType type)
    {
        bool ret = inventory.ammoCounts.IsAmmoAvailable(type);
        return ret;
    }

    public int GetAmmoCount(AmmoType type)
    {
        int ret = inventory.ammoCounts.GetAmmoCount(type);
        return ret;
    }

    public Weapon GetWeapon(WeaponClass weaponClass)
    {
        switch (weaponClass)
        {
            case WeaponClass.LongGun:
                return LongGun;
            case WeaponClass.ShortGun:
                return ShortGun;
            case WeaponClass.Pistol:
                return Pistol;
        }
        return null;
    }

    public Inventory GetFullInventory()
    {
        return inventory;
    }

    public bool ContainsItem(ItemBase item)
    {
        bool ret = false;
        if (inventory.ItemsInInventory.Contains(item))
        {
            ret = true;
        }
        return ret;
    }

    public void RemoveItem(ItemBase item)
    {
        if (item != null && inventory.ItemsInInventory.Contains(item))
            inventory.ItemsInInventory.Remove(item);
        if(item.itemType == ItemType.Weapon)
        {
            switch (item.weapon.weaponClass)
            {
                case WeaponClass.LongGun:
                    LongGun = null;
                    break;
                case WeaponClass.ShortGun:
                    ShortGun = null;
                    break;
                case WeaponClass.Pistol:
                    Pistol = null;
                    break;
            }
        }
    }

    public void AssignWeapon(Weapon wpn)
    {
        switch (wpn.weaponClass)
        {
            case WeaponClass.LongGun:
                LongGun = wpn;
                break;
            case WeaponClass.ShortGun:
                ShortGun = wpn;
                break;
            case WeaponClass.Pistol:
                Pistol = wpn;
                break;
        }
    }

    public bool HasAnyWeapons() { return (LongGun != null || ShortGun != null || Pistol != null); }

    [ContextMenu("Print Inventory")]
    public void TEST_PrintInventory()
    {
        string tmp = "";
        for (int i = 0; i < inventory.ItemsInInventory.Count; i++)
        {
            tmp += inventory.ItemsInInventory[i].itemName + "\n";
        }        
    }
}

[System.Serializable]
public class Inventory
{
    public List<ItemBase> ItemsInInventory;
    public AmmoCounts ammoCounts;

    public Inventory()
    {
        ItemsInInventory = new List<ItemBase>();
        ammoCounts = new AmmoCounts();
    }
}

[System.Serializable]
public class AmmoCounts
{
    Dictionary<AmmoType, int> ammoCounts;

    public AmmoCounts()
    {
        ammoCounts = new Dictionary<AmmoType, int>();
    }

    public int AddAmmoToInventory(AmmoType type, int amount)
    {
        int i = 0;
        if (ammoCounts.TryGetValue(type, out i))
        {
            ammoCounts[type] = i + amount;
        }
        else
        {
            ammoCounts.Add(type, amount);
        }

        return ammoCounts[type];
    }

    public int GetAmmoFromInventory(AmmoType type, int desiredAmount)
    {
        int ret = 0;
        if (ammoCounts.TryGetValue(type, out ret))
        {
            if (ret >= desiredAmount)
            {
                ammoCounts[type] -= desiredAmount;
                ret = desiredAmount;
            }
            else
            {
                ret = ammoCounts[type];
                ammoCounts[type] = 0;
            }
        }
        return ret;
    }

    public bool IsAmmoAvailable(AmmoType type)
    {
        int i;
        ammoCounts.TryGetValue(type, out i);
        return i > 0;
    }

    public int GetAmmoCount(AmmoType type)
    {
        int ret = 0;
        ammoCounts.TryGetValue(type, out ret);

        return ret;
    }
}

enum VisibilityType
{
    Armor = 3,
    Cosmetic = 4,
    NonVisible = 5
}