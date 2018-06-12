using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TurnAMint.UI.Inventory
{
    public class AmmoCountUI_Switcher : InvUI_Droppable
    {

        internal int currentAmount;
        int index, length;
        [SerializeField] int[] amountChoices;

        internal override void Start()
        {
            base.Start();
            length = amountChoices.Length;
            currentAmount = amountChoices[0];
            txt.text = currentAmount.ToString();
        }

        internal override void OnMouseDown()
        {
            index = (index + 1) % length;
            currentAmount = amountChoices[index];
            txt.text = currentAmount.ToString();
        }

    }
}