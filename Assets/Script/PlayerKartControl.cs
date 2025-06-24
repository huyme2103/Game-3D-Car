using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerKartControl : MonoBehaviour
{
    //Input variables
    private float m_gas;
    private float m_brake;
    private Vector2 m_steering;
    // Drive physics
    private Rigidbody m_Rigidbody;
    [SerializeField] private WheelCollider[] WheelColliders;
    [SerializeField] private GameObject[] Wheels;
    [SerializeField] private float DriveTorque = 100;
    [SerializeField] private float BrakeTorque = 500;
    private float m_forwardTorque;

    // giup xe khong bị lat
    public float DownForce = 100;
    // set vong quay toi da cua banh xe
    public float SteerAngle = 30;
    // fix the kart flipping over for powerful brakes
    public bool BrakeAssist = false;
    // check ground 2 banh sau
    private bool m_grounded;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Drive(m_gas, m_brake,m_steering);
        AddDownForce();
    }

    private void Drive(float acceleration, float brake, Vector2 steer)
    {
        m_forwardTorque = acceleration * DriveTorque;
        brake *= BrakeTorque;
        steer.x = steer.x * SteerAngle;
        //wheel meshes move with wheel colliders
        for(int i =0; i < Wheels.Length; i++)
        {
            Vector3 wheelPosition;
            Quaternion wheelRotation;

            WheelColliders[i].GetWorldPose(out wheelPosition, out wheelRotation);
            Wheels[i].transform.position = wheelPosition;
            Wheels[i].transform.rotation = wheelRotation;

        }

        for (int i = 0; i < WheelColliders.Length; i++)
        {
            WheelColliders[i].motorTorque = m_forwardTorque;
            WheelColliders[i].brakeTorque = brake;
            if (i < 2) // xoay 2 banh trc 
            {
                WheelColliders[i].steerAngle = steer.x;
            }
        }
    }

    private void AddDownForce()
    {
        if (m_grounded)
        {
            WheelColliders[0].attachedRigidbody.AddForce(-transform.up * DownForce * WheelColliders[0].attachedRigidbody.linearVelocity.magnitude);
        }
        for (int i = 2; i < WheelColliders.Length; ++i)
        {
            WheelHit wheelhit;
            WheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
            {
                m_grounded = false;
                StartCoroutine(SetConstraints());
            }
            else
            {
                m_grounded = true;

            }
        }
    }
    IEnumerator SetConstraints()
    {
        yield return new WaitForSeconds(0.1f);
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        yield return new WaitForSeconds(0.3f);
        m_Rigidbody.constraints = RigidbodyConstraints.None;
    }


    public void OnAccelerate(InputValue button)
    {
        if (button.isPressed)
        {
            m_gas = 1;
           
        }
        if (!button.isPressed)
        {
            m_gas = 0;
        }
    }

    public void OnBrake(InputValue button)
    {
        if (button.isPressed)
        {
            m_brake = 1;
            if (BrakeAssist)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
        }
        if (!button.isPressed)
        {
            m_brake = 0;
            if (BrakeAssist)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.None;
            }
        }
    }
    public void OnSteering(InputValue value)
    {
        m_steering = value.Get<Vector2>();
    }
}

