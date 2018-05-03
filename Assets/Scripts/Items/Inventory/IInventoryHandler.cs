using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryHandler {
    void PickUp(ItemBase newItem);
    Transform DropItem(ItemBase item);      // Returns drop location
}
