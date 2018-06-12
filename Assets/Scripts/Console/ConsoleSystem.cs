using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnAMint.Networking;

public class ConsoleSystem : MonoBehaviour
{
    static NetManager m_netM;
    static NetManager netM {
        get
        {
            if (m_netM == null)
            {
                m_netM = FindObjectOfType<NetManager>();

            }
            return m_netM;
        }
    }


    internal static void Run(string obj, bool v)
    {
        switch (obj)
        {
            case "connections":
                netM.GetConnections();
                break;
        }
    }
}