using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TurnAMint.Items;

namespace TurnAMint.Management
{
    public class ItemSpawnPoint : NetworkBehaviour
    {

        public SpawnType spawnType;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero + (Vector3.up * 0.5f), Vector3.one);
        }

        public float SpawnItem(GameObject itemToSpawn)
        {
            if (!isServer) return 0;

            float val = 0;

            GameObject itm = Instantiate(itemToSpawn, transform.position, transform.rotation);

            if (itm != null)
            {
                Debug.Log("Spawned item " + itemToSpawn.name + " at position " + transform.position);
                NetworkServer.Spawn(itm);

                val += itm.GetComponent<ItemBase>().value;

                Weapon wpn;

                if ((wpn = itm.GetComponent<Weapon>()) != null)
                {
                    val += SpawnAmmoWithWeapon(wpn);
                }
            }
            else
            {
                Debug.LogWarning("Item spawn failed!");
            }

            return val;
        }

        float SpawnAmmoWithWeapon(Weapon wpn)
        {
            float val = 0;
            int amount = Random.Range(1, 4);

            GameObject itm;

            for (int i = 0; i < amount; i++)
            {
                itm = Instantiate(wpn.ammoPrefab, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0), transform.rotation);
                val += itm.GetComponent<ItemBase>().value;
                NetworkServer.Spawn(itm);
            }

            return val;
        }
    }

    public enum SpawnType { LONG_GUN, SHORT_GUN, PISTOL, AMMO, OTHER }
}