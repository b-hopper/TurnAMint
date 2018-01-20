﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrosshairManager : MonoBehaviour {

    public int index;
    public Crosshair activeCrosshair;
    public Crosshair[] crosshairs;

    public Text pickupItemsText;

    public static CrosshairManager instance;
    public static CrosshairManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < crosshairs.Length; i++)
        {
            crosshairs[i].gameObject.SetActive(false);
        }

        crosshairs[index].gameObject.SetActive(true);
        activeCrosshair = crosshairs[index];
    }

    public void DefineCrosshairByIndex(int findIndex)
    {
        activeCrosshair = crosshairs[findIndex];
    }

    public void DefineCrosshairByName(string name)
    {
        for (int i = 0; i < crosshairs.Length; i++)
        {
            if(string.Equals(crosshairs[i].name,name))
            {
                activeCrosshair = crosshairs[i];
                break;
            }
        }
    }

}
