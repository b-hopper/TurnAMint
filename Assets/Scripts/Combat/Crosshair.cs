using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons {
    public class Crosshair : MonoBehaviour {
        [SerializeField] Texture2D image;
        [SerializeField] int size;
        [SerializeField] float maxAngle, minAngle;

        float lookHeight;

        public void LookHeight(float val)
        {
            /*lookHeight += val;
            if (lookHeight > maxAngle || lookHeight < minAngle)
            {
                lookHeight -= val;
            }*/
        }

        private void OnGUI()
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            screenPos.y = Screen.height - screenPos.y;
            GUI.DrawTexture(new Rect(screenPos.x, screenPos.y - lookHeight, size, size), image);
        }
    }
}