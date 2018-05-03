using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager {
    
    public GameObject cnmGO;

    ItemSpawnPoint[] itemSpawnPoints;
    PlayerSpawnPoint[] playerSpawnPoints;

    public ItemSpawnTable itemSpawnTable;

    int frameCounter = 0;

    float spawnedItemValue = 0;

    List<ItemBase> itemList = new List<ItemBase>();

    Dictionary<GameObject, ClientBehavior> cBehaviors = new Dictionary<GameObject, ClientBehavior>();
    List<NetworkConnection> clientConnections = new List<NetworkConnection>();

    private void Awake()
    {
        itemSpawnTable.RegisterAllItems();
    }

    public override void OnServerConnect(NetworkConnection Conn)
    {        
        base.OnServerConnect(Conn);

        Debug.Log("Player connected!: " + Conn.connectionId);
        
        clientConnections.Add(Conn);               
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        
        StartCoroutine(ServerStarted());
    }

    IEnumerator ServerStarted()
    {
        Debug.LogWarning("Server starting: " + Time.realtimeSinceStartup);
        yield return new WaitForSeconds(3);                                 // Needs a delay, to make sure isServer returns True
        itemSpawnPoints = FindObjectsOfType<ItemSpawnPoint>();
        SpawnItems();
        Debug.Log("Server started: " + Time.realtimeSinceStartup);
    }

    void SpawnItems()
    {
        Debug.Log("Attempting to spawn items. Spawn points found: " + itemSpawnPoints.Length);
        foreach(ItemSpawnPoint i in itemSpawnPoints)
        {
            ItemType type = ItemType.Weapon;
            
            spawnedItemValue += i.SpawnItem(itemSpawnTable.GetRandomObject(type));
        }
        Debug.Log("Total spawned item value: " + spawnedItemValue);
    }
    
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        Debug.Log("Player joined!");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        Debug.Log("Player left!");

        clientConnections.Remove(conn);
    }

    public void GetConnections()
    {
        Debug.Log("Current connections:");
        foreach(ClientBehavior a in cBehaviors.Values)
        {
            Debug.Log("   * " + a.playerName);
        }
    }
    
    public void RegisterPlayer(ClientBehavior newPlayer)
    {
        cBehaviors.Add(newPlayer.gameObject, newPlayer);
        Debug.Log("Added player " + newPlayer.name + "! Total players: " + cBehaviors.Count);
    }
    
    internal ClientBehavior GetClientFromGO(GameObject receivingPlayer)
    {
        ClientBehavior ret;
        if (cBehaviors.TryGetValue(receivingPlayer, out ret))
        {
            return ret;
        }
        return null;
    }
}
