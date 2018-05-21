using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SocketIO;

public class Spawner : MonoBehaviour {

    public GameObject myPlayer;
    public GameObject playerPrefab;
    public SocketIOComponent socket;

    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    public GameObject SpawnPlayer(string id)
    {
        var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;

        player.GetComponent<ClickFollow>().myPlayer = myPlayer;
        
        player.GetComponent<NetworkEntity>().id = id;

        AddPlayer(id, player);

        Debug.Log("count: " + players.Count);

        return player;
    }

    public void AddPlayer(string id, GameObject player)
    {
        players.Add(id, player);
    }

    public GameObject FindPlayer(string id)
    {
        return players[id];
    }

    internal void Remove(string id)
    {
        var player = players[id];
        Destroy(player);
        players.Remove(id);
    }
}
