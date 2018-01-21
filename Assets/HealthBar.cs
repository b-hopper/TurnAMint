using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public RectTransform healthBar;

    public HealthManager hm;

    float startSize;

    private void Start()
    {
        startSize = healthBar.rect.width;
    }

    private void FixedUpdate()
    {
        if (hm != null)
        {
            healthBar.sizeDelta = new Vector2(hm.CurrentHealth * (startSize / hm.MaxHealth), healthBar.sizeDelta.y);
        }
    }
}
