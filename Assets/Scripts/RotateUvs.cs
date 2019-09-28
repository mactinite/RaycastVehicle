using UnityEngine;
using System.Collections;

public class RotateUvs : MonoBehaviour {
    public float speed = 1;
    float offset;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        offset += Time.deltaTime * speed;
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
	}
}
