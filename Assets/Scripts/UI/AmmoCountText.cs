using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCountText : MonoBehaviour {

    public Text inMagCount, totalCount;

    [HideInInspector] public StateManager state;

    private void FixedUpdate()
    {
        inMagCount.text = state.currentWeapon.currentRoundsInMag.ToString();
        totalCount.text = state.inventoryHandler.GetTotalAmmo(state.currentWeapon.ammoType).ToString();
    }
}
