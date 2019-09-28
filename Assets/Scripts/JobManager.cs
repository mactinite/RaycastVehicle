using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JobManager : MonoBehaviour {

    public List<TransportJob> Jobs = new List<TransportJob>();
    public List<Transform> Waypoints = new List<Transform>();

    public WaypointDirection Pointer;

    public Transform currentJobDestination;
    public string currentJobDialogue;
    public string currentJobEndingDialogue;


	void Start () {
        GiveNewJob();
	}

    public void GiveNewJob()
    {
        currentJobDestination = Waypoints[Random.Range(0, Waypoints.Count)];
        currentJobDestination.gameObject.SetActive(true);
        Pointer.PointAt = currentJobDestination;
    }

}
