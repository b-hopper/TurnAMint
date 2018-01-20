using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{

    [SerializeField] SpawnPoint[] spawnPoints;
    [SerializeField] float spawnDelay;

    HealthManager health;

    private void Start()
    {
        health = GetComponent<HealthManager>();
        spawnPoints = FindObjectsOfType<SpawnPoint>();
    }

    public void RespawnAtSpawnPoint(StateManager playerState)
    {
        GameManager.Instance.Timer.Add(() =>
        {
            playerState.playerHealth.Reset();
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            transform.position = spawnPoints[spawnIndex].transform.position;
            transform.rotation = spawnPoints[spawnIndex].transform.rotation;
        }, spawnDelay);
    }
}