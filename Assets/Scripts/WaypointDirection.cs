using UnityEngine;
using System.Collections;

public class WaypointDirection : MonoBehaviour {

    public Transform PointAt;
    public Transform Player;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.rotation = Quaternion.LookRotation(PointAt.position - Player.position, transform.up);

	}
}
