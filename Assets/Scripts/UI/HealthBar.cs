using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnAMint.Player.Health;

namespace TurnAMint.UI
{
    public class HealthBar : MonoBehaviour
    {

        public RectTransform healthBar, downedHealthBar;

        public HealthManager hm;

        float startSize, downedStartSize;

        private void Start()
        {
            startSize = healthBar.rect.width;
            downedStartSize = downedHealthBar.rect.width;
        }

        private void FixedUpdate()
        {
            if (hm != null)
            {
                healthBar.gameObject.SetActive(hm.currentAliveStatus == PlayerStatus.Alive);
                healthBar.sizeDelta = new Vector2(hm.CurrentHealth * (startSize / hm.MaxHealth), healthBar.sizeDelta.y);
                downedHealthBar.gameObject.SetActive(hm.currentAliveStatus != PlayerStatus.Dead);
                downedHealthBar.sizeDelta = new Vector2(hm.CurrentDownedHealth * (downedStartSize / hm.MaxDownedHealth), downedHealthBar.sizeDelta.y);
            }
        }
    }
}