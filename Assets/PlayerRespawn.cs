using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour {

    [SerializeField] SpawnPoint[] spawnPoints;
    [SerializeField] float spawnDelay;

    HealthManagement.HealthManager health;

    private void Start()
    {
        health = GetComponent<HealthManagement.HealthManager>();
    }

    public void RespawnAtSpawnPoint()
    {
        GameManager.Instance.Timer.Add(() =>
        {
            health.Reset();
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            transform.position = spawnPoints[spawnIndex].transform.position;
            transform.rotation = spawnPoints[spawnIndex].transform.rotation;
        }, spawnDelay);
    }
}
