using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    
    public GameObject nearbyItems_UI, itemsInInventory_UI, longGunSlot_UI, shortGunSlot_UI, pistolSlot_UI;

    Dictionary<string, InventoryItemUI> InventoryItems = new Dictionary<string, InventoryItemUI>();
    Dictionary<Weapon, InventoryItemUI> EquippedWeapons = new Dictionary<Weapon, InventoryItemUI>();
    List<ItemBase> ItemsOnGround = new List<ItemBase>();
    List<ItemBase> ItemsInInventory = new List<ItemBase>();

    [SerializeField]
    List<InventoryItemUI> ItemSlotsInInventory = new List<InventoryItemUI>();

    [SerializeField]
    List<InventoryItemUI> ItemSlotsOnGround = new List<InventoryItemUI>();

    ItemBase currentlyHoldingObject;

    public delegate void ItemInteractionEvent();
    public ItemInteractionEvent OnItemPickedUpByCursor;
    public ItemInteractionEvent OnItemDroppedByCursor;

    [HideInInspector] public InventoryDroppableUI currentDroppableUI;

    int firstOpenSlotIndex;

    HandleInventory m_inventoryHandler;
    internal HandleInventory InventoryHandler
    {
        get
        {
            if (m_inventoryHandler == null)
            {
                m_inventoryHandler = GameManager.Instance.LocalPlayerReference.inventoryHandler;
                m_inventoryHandler.OnInventoryChanged += MakeInventoryChange;
            }
            return m_inventoryHandler;
        }
        set
        {
            if (m_inventoryHandler != null)
            {
                m_inventoryHandler.OnInventoryChanged -= MakeInventoryChange;
            }
            m_inventoryHandler = value;
            m_inventoryHandler.OnInventoryChanged += MakeInventoryChange;
        }
    }

    public void SetCurrentlyHoldingObject(ItemBase item)
    {
        if (OnItemPickedUpByCursor != null && currentlyHoldingObject == null)
        {
            OnItemPickedUpByCursor();
        }
        currentlyHoldingObject = item;
        Debug.Log("Item assigned: " + item);
    }

    public bool ToggleInventory()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        return gameObject.activeInHierarchy;
    }

    internal void MakeInventoryChange(bool isAdding, ItemBase newItem)
    {
        InventoryItemUI itm;        // Will need more conditions, this is just basic all-in-one inventory
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
                Debug.Log("Found");
                itm.ClearItemFromSlot();
                ItemsInInventory.Remove(newItem);
                ItemsOnGround.Add(newItem);
                firstOpenSlotIndex--;
            }
        }
    }

    internal void TriggerItemMove(ItemBase occupyingItem)
    {
        if (currentDroppableUI != null)
        {
            currentDroppableUI.TryDropItem(occupyingItem);
        }
        if (OnItemDroppedByCursor != null)
        {
            OnItemDroppedByCursor();
        }
    }


}

