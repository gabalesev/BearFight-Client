using UnityEngine;
using System.Collections;

public class ClickMove : MonoBehaviour, IClickable {

    public GameObject player;	
	
	public void OnClick (RaycastHit hit) {
        var navigator = player.GetComponent<Navigator>();

        Debug.Log("x: " + hit.point.x + " y: " + hit.point.z);

        navigator.NavigateTo(hit.point);

        Network.Move(player.transform.position, hit.point);
    }
}
