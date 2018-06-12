using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TurnAMint.Items;

namespace TurnAMint.UI.Inventory
{
    public class AmmoCountUI : InvUI_Droppable
    {

        AmmoCountUI_Switcher switcher;

        int amount;

        public AmmoType ammoType;

        internal override void OnMouseDown()
        {
            base.OnMouseDown();
        }

        private void OnEnable()
        {
            if (txt == null) { txt = GetComponent<Text>(); }
            UpdateText();
        }

        private void OnDisable()
        {

        }

        internal void SetAmmo(int newAmount)
        {
            amount = newAmount;
            UpdateText();
        }

        void UpdateText()
        {
            txt.text = ammoType.ToString() + ": " + amount.ToString();
        }
    }
}