using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnAMint.Player.Inventory
{
    public class WeaponHolster : MonoBehaviour
    {
        public Transform[] holsters;
        bool[] isFull;

        private void Awake()
        {
            isFull = new bool[holsters.Length];
        }

        int GetEmptyHolster()
        {
            for (int i = 0; i < isFull.Length; i++)
            {
                if (isFull[i])
                {
                    continue;
                }
                isFull[i] = true;
                return i;
            }
            return -1;
        }

        void SetEmptyHolster(int index)
        {
            if (index != -1)
                isFull[index] = false;
        }

        public Transform RequestEmptyHolster()
        {
            Transform ret = null;
            int i = GetEmptyHolster();
            if (i != -1)
            {
                ret = holsters[i];
            }
            return ret;
        }

        public void RemoveFromHolster(Transform holster)
        {
            if (holster == null)
                return;
            for (int i = 0; i < holsters.Length; i++)
            {
                if (holster == holsters[i])
                {
                    SetEmptyHolster(i);
                }
            }
        }
    }
}