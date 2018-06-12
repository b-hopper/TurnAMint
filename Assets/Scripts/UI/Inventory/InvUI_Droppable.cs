using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TurnAMint.Items;

namespace TurnAMint.UI.Inventory
{
    [RequireComponent(typeof(Collider))]
    public class InvUI_Droppable : MonoBehaviour
    {

        internal InventoryUI invUI;
        public Image img;
        public Text txt;
        internal Collider col;

        public Color highlightColor;

        internal virtual void Awake()
        {
            if (img == null) img = GetComponent<Image>();
            col = GetComponent<Collider>();
            if (txt == null) txt = GetComponent<Text>();
            invUI = GetComponentInParent<InventoryUI>();
        }

        internal virtual void Start()
        {
            invUI.OnItemPickedUpByCursor += OnItemHeldByCursor;
            invUI.OnItemDroppedByCursor += OnItemDroppedByCursor;
        }

        public virtual void OnItemDroppedIntoThis(ItemBase occupyingItem) { }
        internal virtual void OnMouseEnter() { }
        internal virtual void OnMouseExit() { }
        internal virtual void OnMouseDown() { }
        internal virtual void OnItemHeldByCursor() { }
        internal virtual void OnItemDroppedByCursor() { }
    }
}