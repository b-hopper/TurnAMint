﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnAMint.Management
{
    public class Respawner : MonoBehaviour
    {

        public void Despawn(GameObject go, float inSeconds)
        {
            go.SetActive(false);

            if (inSeconds > 0)
            {
                GameManager.Instance.Timer.Add(() =>
                {
                    go.SetActive(true);
                }, inSeconds);
            }
        }

        public void SetRandomSeed(int seed)
        {
            Random.InitState(seed);
        }
    }
}