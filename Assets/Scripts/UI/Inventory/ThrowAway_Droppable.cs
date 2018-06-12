using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnAMint.Items;

namespace TurnAMint.UI.Inventory
{
    public class ThrowAway_Droppable : InvUI_Droppable
    {

        public override void OnItemDroppedIntoThis(ItemBase occupyingItem)
        {
            invUI.InventoryHandler.DropItem(occupyingItem);
            Debug.Log("Dropped: " + occupyingItem);
        }

        internal override void OnItemHeldByCursor()
        {
            if (col != null)
                col.enabled = true;
        }

        internal override void OnItemDroppedByCursor()
        {
            if (col != null)
                col.enabled = false;
        }

        internal override void OnMouseEnter()
        {
            invUI.currentDroppableUI = this;
            img.color = highlightColor;
        }

        internal override void OnMouseExit()
        {
            invUI.currentDroppableUI = null;
            img.color = Color.clear;
        }
    }
}