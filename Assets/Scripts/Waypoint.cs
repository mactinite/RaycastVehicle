using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {
    public JobManager jobManager;

    void OnTriggerEnter(Collider col)
    {
        this.gameObject.SetActive(false);
        jobManager.GiveNewJob();
        
    }

}
