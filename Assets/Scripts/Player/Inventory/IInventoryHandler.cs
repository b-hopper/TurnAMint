using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnAMint.Items;

namespace TurnAMint.Player.Inventory
{
    public interface IInventoryHandler
    {
        void PickUp(ItemBase newItem);
        Transform DropItem(ItemBase item);      // Returns drop location
    }
}