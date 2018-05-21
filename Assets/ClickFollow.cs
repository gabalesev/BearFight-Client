using UnityEngine;
using System.Collections;
using System;

public class ClickFollow : MonoBehaviour, IClickable {

    public GameObject myPlayer;
    public NetworkEntity networkentity;

    Targeter myPlayerTargeter;

    void Start()
    {
        networkentity = GetComponent<NetworkEntity>();       
        if(myPlayer != null) 
            myPlayerTargeter = myPlayer.GetComponent<Targeter>();
    }

	public void OnClick(RaycastHit hit)
    {        
        Debug.Log("following " +hit.collider.gameObject.name);        
        try
        {
            Network.Follow(networkentity.id);
            myPlayerTargeter.target = transform;
        }
        catch (Exception ex)
        {
            Debug.Log("Exception in OnMove():" + ex.Message);
            Debug.Log("Stacktrace:" + ex.StackTrace);
            Debug.Log("Innerexception:" + ex.InnerException);
        }
    }
}
