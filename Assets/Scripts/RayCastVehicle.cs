using UnityEngine;
using UnityEditor;
using System.Collections;

public class RayCastVehicle : MonoBehaviour
{


    public bool canControl;
    public float DriveForce = 50;
    public float TurnThreshold;
    private float TurnAmount = 0;
    public float TurnForce = 25;
    [Range(0, 1)]
    public float Friction = 0.5f;
    [Range(0, 100)]
    public float SideFriction = 0.5f;
    public float wheelRadius = 2;

    [Header("Suspension")]
    public float suspensionDistance = 0.3f;
    public float suspensionStiffness = 5;
    public Transform centerOfMass;
    public RayCastWheel[] Wheels = new RayCastWheel[4];


    public Transform AccelerationPosition;
    public GameObject collisionMesh;
    private Vector3[] rayPositions = new Vector3[4];

    private new Rigidbody rigidbody;

    public bool isGrounded = false;

    Vector3 averageImpactNormal = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centerOfMass.localPosition;


    }
    Bounds b;
    // Update is called once per frame
    void FixedUpdate()
    {

        CalculateRayPositions();
        Suspension();
        DriveForces();
    }

    void CalculateRayPositions()
    {
        var vertices = new Vector3[8];
        var thisMatrix = collisionMesh.transform.localToWorldMatrix;

        var extents = collisionMesh.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
    }

    private Vector3 newPos, prevPos, fwd, movement;
    void DriveForces()
    {
        newPos = transform.position;
        movement = newPos - prevPos;
        if (isGrounded)
        {
            averageImpactNormal.Normalize();
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up);
            Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
            Vector3 VehicleForward = Vector3.ProjectOnPlane(centerOfMass.forward, Vector3.up);

            //Apply Basic Friction
            float sideVelocity = localVelocity.x;
            rigidbody.AddForceAtPosition(-projectedVelocity.normalized * (DriveForce / 2 * Friction), AccelerationPosition.position, ForceMode.Acceleration);
            rigidbody.AddForce((transform.right) * (-sideVelocity * SideFriction), ForceMode.Acceleration);

            if (canControl)
            {
                if (Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Epsilon)
                {

                    rigidbody.AddForceAtPosition(VehicleForward * (DriveForce) * Input.GetAxis("Vertical"), AccelerationPosition.position, ForceMode.Acceleration);
                }

                TurnAmount = rigidbody.velocity.magnitude;
                TurnAmount = Mathf.Clamp(TurnAmount, 0, TurnThreshold);
                if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon && TurnAmount > 0)
                { // Only allow turning while moving.
                    if (localVelocity.z < 0)
                    {
                        rigidbody.AddRelativeTorque(transform.up * (-Input.GetAxis("Horizontal")) * TurnForce * (TurnAmount / TurnThreshold), ForceMode.Acceleration);
                    }
                    else if (localVelocity.z > 0)
                    {
                        rigidbody.AddRelativeTorque(transform.up * Input.GetAxis("Horizontal") * TurnForce * (TurnAmount / TurnThreshold), ForceMode.Acceleration);
                    }
                }
            }

        }
    }
    void LateUpdate()
    {
        prevPos = transform.position;
        fwd = transform.forward;
    }

    void Suspension()
    {
        int wheelsTouching = 0;

        foreach (RayCastWheel wheel in Wheels)
        {
            Ray ray = new Ray(wheel.transform.position, -transform.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, suspensionDistance))
            {
                Vector3 currentVelocity = rigidbody.GetPointVelocity(wheel.transform.position);
                Vector3 sideForce = Vector3.Project(wheel.transform.right, hit.normal);

                float dot = Vector3.Dot(currentVelocity, sideForce);

                sideForce = Vector3.Scale(sideForce, currentVelocity * -dot);

                Vector3 suspensionForce = Vector3.Project(currentVelocity, transform.up);

                float compressionRatio = (1- (hit.distance / suspensionDistance));
                wheel.CompressionRatio = compressionRatio;

                Vector3 newSuspensionForce = transform.up * compressionRatio * suspensionStiffness;
                Vector3 deltaForce = newSuspensionForce - suspensionForce;

                rigidbody.AddForceAtPosition(deltaForce, wheel.transform.position, ForceMode.Acceleration);
                Debug.DrawRay(wheel.transform.position, -transform.up * suspensionDistance, Color.green, Time.deltaTime);

                //Place the wheel visual at a position at a value of the suspension distance (compression ratio) offset by the wheel radius
                wheel.WheelMeshPosition = new Vector3(0, 0 - ((hit.distance - wheelRadius) * (1 - compressionRatio)), 0);


                averageImpactNormal += hit.normal;
                wheelsTouching++;
            }
            else
            {
                //also not touching 
                Debug.DrawRay(transform.InverseTransformPoint(wheel.transform.position), -transform.up * suspensionDistance, Color.red, Time.deltaTime);
                //Place the wheel visual at a position at the suspension distance offset by the wheel radius
                wheel.WheelMeshPosition = new Vector3(0, 0 - (suspensionDistance - wheelRadius), 0);

            }


        }

        if (wheelsTouching > 0)
        {
            isGrounded = true;
            //rigidbody.drag = 2.5f;
        }
        else
        {
            isGrounded = false;
            //rigidbody.drag = 0.25f;
        }
        averageImpactNormal = averageImpactNormal / 4;
    }

    void OnDrawGizmos()
    {
        CalculateRayPositions();

        foreach (RayCastWheel wheel in Wheels)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(wheel.transform.position, -transform.up * suspensionDistance);
            Gizmos.DrawWireSphere(wheel.WheelMesh.position, wheelRadius);
            Handles.color = Color.white;
            Handles.Label(wheel.transform.position, "CR: " + wheel.CompressionRatio);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerOfMass.position, 0.25f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(AccelerationPosition.position, 0.25f);
    }


    public void SetCanControl(bool b)
    {
        canControl = b;
    }
}
