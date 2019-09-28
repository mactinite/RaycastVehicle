using UnityEngine;
using System.Collections;

public class CameraDolly : MonoBehaviour {
    public Transform target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 targetPos = new Vector3(target.position.x  - 75, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5);
	}
}
