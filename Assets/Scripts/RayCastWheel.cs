using UnityEngine;
using System.Collections;

public class RayCastWheel : MonoBehaviour {

    public enum WheelPosition
    {
        Left,Right
    };
    public bool turningWheel = false;
    public float turnAmount = 30;
    //Data Object for caching some values

    public Vector3 rayPosition;
    public Vector3 InitWheelPos;
    public float CompressionRatio;
    [HideInInspector]
    public Vector3 SurfaceImpactPoint;
    [HideInInspector]
    public Vector3 ImpactNormal;
    public Transform WheelMesh;
    [HideInInspector]
    public Vector3 WheelMeshPosition;

    void Start()
    {

    }

    void Update()
    {
        
        WheelMesh.localPosition = Vector3.Lerp(WheelMesh.localPosition, WheelMeshPosition, Time.deltaTime * 50);
        if (turningWheel)
        {
            float degreesToTurn = turnAmount * Input.GetAxis("Horizontal");
            Vector3 rot = transform.rotation.eulerAngles;
            rot = new Vector3(rot.x, rot.y + degreesToTurn, rot.z);
            WheelMesh.rotation = Quaternion.Slerp(WheelMesh.rotation, Quaternion.Euler(rot), Time.deltaTime * 20);
        }
    }


}
