using UnityEngine;
using UnityEngine.InputSystem;
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

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Drive(m_gas, m_brake);
    }

    private void Drive(float acceleration, float brake)
    {
        m_forwardTorque = acceleration * DriveTorque;
        brake *= BrakeTorque;
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
        }
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
        }
        if (!button.isPressed)
        {
            m_brake = 0;
        }
    }
    public void OnSteering(InputValue value)
    {
        m_steering = value.Get<Vector2>();
    }
}

