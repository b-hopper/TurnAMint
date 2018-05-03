using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryDroppableUI : MonoBehaviour {

    InventoryUI invUI;
    Image img;
    Collider col;

    public Color highlightColor;

    public DroppableUITypes droppableType;

    private void Awake()
    {
        img = GetComponent<Image>();
        col = GetComponent<Collider>();
        invUI = GetComponentInParent<InventoryUI>();
    }

    private void Start()
    {
        invUI.OnItemPickedUpByCursor += OnItemHeldByCursor;
        invUI.OnItemDroppedByCursor += OnItemDroppedByCursor;
    }

    void OnItemHeldByCursor()
    {
        if (col != null)
            col.enabled = true;
    }

    void OnItemDroppedByCursor()
    {
        if (col != null)
            col.enabled = false;
    }

    private void OnMouseEnter()
    {
        invUI.currentDroppableUI = this;
        img.color = highlightColor;
        Debug.Log("Enter");
    }

    private void OnMouseExit()
    {
        invUI.currentDroppableUI = null;
        img.color = Color.clear;
    }

    internal void TryDropItem(ItemBase occupyingItem)
    {
        switch (droppableType)
        {
            case DroppableUITypes.DROP_TO_WORLD:
                DropToWorld(occupyingItem);
                break;
            case DroppableUITypes.EQUIP_WEAPON:
                Debug.LogWarning("TODO", this);
                break;
            case DroppableUITypes.EQUIP_WEAPON_MOD:
                Debug.LogWarning("TODO", this);
                break;
            case DroppableUITypes.EQUIP_WEARABLE:
                Debug.LogWarning("TODO", this);
                break;
            case DroppableUITypes.MOVE_TO_INVENTORY:
                Debug.LogWarning("TODO", this);
                break;
        }
    }

    void DropToWorld(ItemBase item)
    {
        invUI.InventoryHandler.DropItem(item);
        Debug.Log("Dropped: " + item);
    }
}

public enum DroppableUITypes
{
    DROP_TO_WORLD,
    MOVE_TO_INVENTORY,
    EQUIP_WEAPON,
    EQUIP_WEAPON_MOD,
    EQUIP_WEARABLE
}