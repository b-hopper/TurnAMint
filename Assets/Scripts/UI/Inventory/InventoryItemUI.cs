using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TurnAMint.Items;

namespace TurnAMint.UI.Inventory
{
    public class InventoryItemUI : InvUI_Droppable
    {
        public Sprite smallSprite, largeSprite, emptySprite;
        public GameObject detailGO;

        RectTransform rTransform;

        Vector3 originalPosition;
        public Vector3 offset;

        bool cursorIsHolding;

        Camera mainCam;

        Color emptyColor, fullColor;

        internal ItemBase occupyingItem;

        bool isOccupied;

        int amount;

        Image largeImage;

        internal override void Awake()
        {
            base.Awake();
            rTransform = GetComponent<RectTransform>();
            emptyColor = img.color;
            fullColor = new Color(1, 1, 1, 0.396f);
            originalPosition = rTransform.localPosition;
            mainCam = Management.GameManager.Instance.CamRefs.mainCam;
        }

        private void Update()
        {
            if (!cursorIsHolding) return;

            rTransform.position = mainCam.ScreenToWorldPoint(Input.mousePosition - offset);
            if (Input.GetMouseButtonUp(0))
            {
                cursorIsHolding = false;
                rTransform.localPosition = originalPosition;
                invUI.TriggerItemMove(occupyingItem);
            }

        }

        internal override void OnItemHeldByCursor()
        {
            if (col == null) return;

            col.enabled = false;
        }

        internal override void OnItemDroppedByCursor()
        {
            if (col == null) return;

            col.enabled = true;
        }

        public void AddItemToSlot(ItemBase item)
        {
            amount += item.amount;
            SetImage(item.UI.SmallSprite, item.UI.LargeSprite);

            isOccupied = true;
            occupyingItem = item;

            if (amount > 1) txt.text = amount.ToString();
            else txt.text = "";

            col.enabled = true;
        }

        public void ClearItemFromSlot()
        {
            if (isOccupied)
            {
                ClearImage();
                amount = 0;
                txt.text = "";
                isOccupied = false;
                occupyingItem = null;
                col.enabled = false;
            }
        }

        public void RemoveSomeAmount(int amountRemoved)
        {
            amount -= amountRemoved;

            if (amount > 1) txt.text = amount.ToString();
            else txt.text = "";
        }

        void SetImage(Sprite newSmall, Sprite newLarge)
        {
            if (newSmall != null)
            {
                Debug.Log(img);
                img.sprite = newSmall;
                img.color = fullColor;
            }
        }
        public void ClearImage()
        {
            smallSprite = null;
            img.sprite = null;
            largeSprite = null;
            img.color = emptyColor;
        }

        internal override void OnMouseEnter()
        {
            img.color = isOccupied ? highlightColor : emptyColor;
        }

        internal override void OnMouseExit()
        {
            img.color = isOccupied ? fullColor : emptyColor;
        }

        internal override void OnMouseDown()
        {
            offset = Input.mousePosition - mainCam.WorldToScreenPoint(rTransform.position);
            cursorIsHolding = true;
            invUI.SetCurrentlyHoldingObject(occupyingItem);
        }
    }
}