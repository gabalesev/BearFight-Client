using UnityEngine;
using System.Collections.Generic;
using SocketIO;
using System;

public class Network : MonoBehaviour {

    static SocketIOComponent socket;
    
    public GameObject myPlayer;

    public Spawner spawner;

	void Start () {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open", OnConnected);
        socket.On("register", OnRegister);
        socket.On("spawn", OnSpawned);
        socket.On("move", OnMove);
        socket.On("follow", OnFollow);
        socket.On("attack", OnAttack);
        socket.On("disconnected", OnDisconnect);
        socket.On("requestPosition", OnRequestPosition);
        socket.On("updatePosition", OnUpdatePosition);

        //spawner = new Spawner();
	}

    public static void Follow(string id)
    {
        Debug.Log("sending follow player id " + Network.PlayerIdToJson(id));
        socket.Emit("follow", Network.PlayerIdToJson(id));
    }

    public static void Attack(string targetId)
    {
        Debug.Log("attacking player " + Network.PlayerIdToJson(targetId));
        socket.Emit("attack", Network.PlayerIdToJson(targetId));
    }

    public static void Move(Vector3 current, Vector3 destination)
    {
        Debug.Log("sending position to node " + Network.VectorToJson(destination));

        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("c", Network.VectorToJson(current));
        j.AddField("d", Network.VectorToJson(destination));
        //j.AddField("z", vector.z);

        socket.Emit("move", j);
    }

    private void OnAttack(SocketIOEvent e)
    {
        Debug.Log("recieved attack " + e.data);

        var targetPlayer = spawner.FindPlayer(e.data["targetId"].str);

        targetPlayer.GetComponent<Hitable>().OnHit();

        var attackingPlayer = spawner.FindPlayer(e.data["id"].str);
        
        attackingPlayer.GetComponent<Animator>().SetTrigger("Attack");
    }

    private void OnRegister(SocketIOEvent e)
    {
        Debug.Log("Succesfully registered, with id " + e.data);
        spawner.AddPlayer(e.data["id"].str, myPlayer);
    }

    private void OnFollow(SocketIOEvent e)
    {
        Debug.Log("Follow request " + e.data);

        var player = spawner.FindPlayer(e.data["id"].str);

        var targetTransform = spawner.FindPlayer(e.data["targetId"].str).transform;

        var targeter = player.GetComponent<Targeter>();

        targeter.target = targetTransform;

    }    

    private void OnUpdatePosition(SocketIOEvent e)
    {
        var player = spawner.FindPlayer(e.data["id"].str);

        var position = GetVectorFromJson(e);

        player.transform.position = position;
    }

    private void OnRequestPosition(SocketIOEvent e)
    {
        Debug.Log("server is requesting position");

        socket.Emit("updatePosition", VectorToJson(myPlayer.transform.position));
    }

    private void OnDisconnect(SocketIOEvent e)
    {
        Debug.Log("player disconnected: " + e.data);

        var id = e.data["id"].str;        

        spawner.Remove(id);
    }

    private void OnSpawned(SocketIOEvent e)
    {
        try
        {
            var player = spawner.SpawnPlayer(e.data["id"].str);
            
            if (e.data["x"])
            {              

                var movePosition = GetVectorFromJson(e);

                var navigatePos = player.GetComponent<Navigator>();

                navigatePos.NavigateTo(movePosition);
            }            
        }
        catch(Exception ex)
        {
            Debug.Log("Exception in OnSpawned():" + ex.Message);
            Debug.Log("Stacktrace:" + ex.StackTrace);
            Debug.Log("Innerexception:" + ex.InnerException);
        }
    }

    private void OnMove(SocketIOEvent e)
    {
        try
        {
            var player = spawner.FindPlayer(e.data["id"].str);
            
            Vector3 position = GetVectorFromJson(e);

            Debug.Log("Position: x = " + position.x + " y = " + position.z);

            Debug.Log("Position " + player.name + ": " + position);

            var navigatePos = player.GetComponent<Navigator>();

            navigatePos.NavigateTo(position);
        }
        catch (Exception ex)
        {
            Debug.Log("Exception in OnMove():" + ex.Message);
            Debug.Log("Stacktrace:" + ex.StackTrace);
            Debug.Log("Innerexception:" + ex.InnerException);
        }
    }    

    private void OnConnected(SocketIOEvent e)
    {
        Debug.Log("connected");
    }

    public Vector3 GetVectorFromJson(SocketIOEvent e)
    {
        //JSONObject j = new JSONObject(JSONObject.Type.OBJECT);

        Vector3 pos = new Vector3(e.data["x"].f, e.data["z"].f, e.data["y"].f);

        //Debug.Log("GetVectorFromJSON: x = " + e.data["x"] + " y = " + e.data["z"]);

        //Debug.Log("Vector3: x = " + pos.x + " y = " + pos.z);

        //Debug.Log("Vector3:" + pos);

        return pos;
    }

    public static JSONObject VectorToJson(Vector3 vector)
    {
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("x", vector.x);
        j.AddField("y", vector.z);
        j.AddField("z", vector.y);

        return j;        
    }

    public static JSONObject PlayerIdToJson(string id)
    {
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("targetId", id);

        return j;
    }
}
