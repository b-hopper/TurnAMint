﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleStartup : MonoBehaviour {
    
	void Awake () {

        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)

        {
            gameObject.AddComponent<ServerConsole>();

            FindObjectOfType<NetManager>().StartServer();

            Application.targetFrameRate = 60;
        }
    }	
}