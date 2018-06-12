using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TurnAMint.Items;

namespace TurnAMint.Management
{
    [System.Serializable]
    public class ItemSpawnTable
    {

        public GameObject[] LongGuns;
        public GameObject[] ShortGuns;
        public GameObject[] Pistols;
        public GameObject[] Ammo;
        public GameObject[] Medical;
        public GameObject[] Other;

        public GameObject GetRandomObject(ItemType itm)
        {
            GameObject ret = null;
            switch (itm)
            {
                case ItemType.Weapon:
                    ret = LongGuns[Random.Range(0, LongGuns.Length)];
                    break;
                case ItemType.Ammo:
                    ret = Ammo[Random.Range(0, Ammo.Length)];
                    break;
            }

            return ret;
        }

        public void RegisterAllItems()
        {
            foreach (GameObject g in LongGuns)
            {
                ClientScene.RegisterPrefab(g);
            }
            foreach (GameObject g in ShortGuns)
            {
                ClientScene.RegisterPrefab(g);
            }
            foreach (GameObject g in Pistols)
            {
                ClientScene.RegisterPrefab(g);
            }
            foreach (GameObject g in Ammo)
            {
                ClientScene.RegisterPrefab(g);
            }
            foreach (GameObject g in Medical)
            {
                ClientScene.RegisterPrefab(g);
            }
            foreach (GameObject g in Other)
            {
                ClientScene.RegisterPrefab(g);
            }
        }
    }
}