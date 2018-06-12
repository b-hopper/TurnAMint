using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TurnAMint.Player.Inventory;
using TurnAMint.Items;

namespace TurnAMint.UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {

        public GameObject nearbyItems_UI, itemsInInventory_UI, longGunSlot_UI, shortGunSlot_UI, pistolSlot_UI;

        Dictionary<string, InventoryItemUI> InventoryItems = new Dictionary<string, InventoryItemUI>();
        Dictionary<Weapon, InventoryItemUI> EquippedWeapons = new Dictionary<Weapon, InventoryItemUI>();
        List<ItemBase> ItemsOnGround = new List<ItemBase>();
        List<ItemBase> ItemsInInventory = new List<ItemBase>();

        [SerializeField]
        List<InventoryItemUI> ItemSlotsInInventory = new List<InventoryItemUI>();

        [SerializeField]
        List<InventoryItemUI> ItemSlotsOnGround = new List<InventoryItemUI>();

        [SerializeField]
        List<AmmoCountUI> AmmoCounters = new List<AmmoCountUI>();

        ItemBase currentlyHoldingObject;

        public delegate void ItemInteractionEvent();
        public ItemInteractionEvent OnItemPickedUpByCursor;
        public ItemInteractionEvent OnItemDroppedByCursor;

        [HideInInspector] public InvUI_Droppable currentDroppableUI;

        int firstOpenSlotIndex;

        HandleInventory m_inventoryHandler;
        internal HandleInventory InventoryHandler
        {
            get
            {
                if (m_inventoryHandler == null)
                {
                    m_inventoryHandler = Management.GameManager.Instance.LocalPlayerReference.inventoryHandler;
                    m_inventoryHandler.OnInventoryChanged += ChangeInventoryUI;
                    m_inventoryHandler.OnAmmoChanged += ChangeAmmoUI;
                }
                return m_inventoryHandler;
            }
            set
            {
                if (m_inventoryHandler != null)
                {
                    m_inventoryHandler.OnInventoryChanged -= ChangeInventoryUI;
                    m_inventoryHandler.OnAmmoChanged -= ChangeAmmoUI;
                }
                m_inventoryHandler = value;
                m_inventoryHandler.OnInventoryChanged += ChangeInventoryUI;
                m_inventoryHandler.OnAmmoChanged += ChangeAmmoUI;
            }
        }

        public void SetCurrentlyHoldingObject(ItemBase item)
        {
            if (OnItemPickedUpByCursor != null && currentlyHoldingObject == null)
            {
                OnItemPickedUpByCursor();
            }
            currentlyHoldingObject = item;
        }

        public bool ToggleInventory()
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
            return gameObject.activeInHierarchy;
        }

        internal void ChangeInventoryUI(bool isAdding, ItemBase newItem)
        {
            switch (newItem.itemType)
            {
                case ItemType.Ammo:
                    ChangeAmmoUI(isAdding, (Ammunition)newItem);
                    break;  //TODO
                            /*case ItemType.Armor:
                                break;  //TODO
                            case ItemType.Cosmetic:
                                break;  //TODO
                            case ItemType.Throwable:
                                break;  //TODO
                            case ItemType.Weapon:
                                break;  //TODO*/
                default:
                    DefaultChangeItemUI(isAdding, newItem);
                    break;  //TODO
            }

        }

        void DefaultChangeItemUI(bool isAdding, ItemBase newItem)
        {
            InventoryItemUI itm;
            if (isAdding)
            {
                if (!InventoryItems.TryGetValue(newItem.name, out itm))
                {
                    itm = ItemSlotsInInventory[firstOpenSlotIndex];
                    InventoryItems.Add(newItem.name, itm);
                    firstOpenSlotIndex++;
                }

                itm.AddItemToSlot(newItem);
            }
            else
            {
                Debug.Log(newItem.name);
                if (InventoryItems.TryGetValue(newItem.name, out itm))
                {
                    itm.ClearItemFromSlot();
                    ItemsInInventory.Remove(newItem);
                    ItemsOnGround.Add(newItem);
                    firstOpenSlotIndex--;
                }
            }
        }

        #region ChangeAmmoUI

        void ChangeAmmoUI(bool isAdding, Ammunition ammo)
        {
            ChangeAmmoUI(ammo.ammoType);
            if (isAdding) InventoryHandler.DestroyItem(ammo.gameObject);
        }

        void ChangeAmmoUI(AmmoType type)
        {
            AmmoCounters[(int)type].SetAmmo(InventoryHandler.GetTotalAmmo(type));
        }
        #endregion

        internal void TriggerItemMove(ItemBase occupyingItem)
        {
            if (currentDroppableUI != null)
            {
                currentDroppableUI.OnItemDroppedIntoThis(occupyingItem);
            }
            if (OnItemDroppedByCursor != null)
            {
                OnItemDroppedByCursor();
            }
            currentlyHoldingObject = null;
        }
    }

}