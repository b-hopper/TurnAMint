using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons {
    public class Crosshair : MonoBehaviour {
        [SerializeField] Texture2D image;
        [SerializeField] int size;
        
        private void OnGUI()
        {
            if (GameManager.Instance.LocalPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.AIMING ||
                GameManager.Instance.LocalPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.AIMEDFIRING)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                screenPos.y = Screen.height - screenPos.y;
                GUI.DrawTexture(new Rect(screenPos.x - (size * 0.5f), screenPos.y - (size * 0.5f), size, size), image);
            }
        }
    }
}