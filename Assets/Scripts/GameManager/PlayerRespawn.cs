using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnAMint.Management
{
    public class PlayerRespawn : MonoBehaviour
    {
        [SerializeField] PlayerSpawnPoint[] spawnPoints;

        public void RespawnAtSpawnPoint(Player.StateManager playerState, float delay)
        {
            GameManager.Instance.Timer.Add(() =>
            {
                playerState.playerHealth.Reset();
                if (spawnPoints.Length > 0)
                {
                    int spawnIndex = Random.Range(0, spawnPoints.Length);
                    transform.position = spawnPoints[spawnIndex].transform.position;
                    transform.rotation = spawnPoints[spawnIndex].transform.rotation;
                }
            }, delay);
        }
    }
}