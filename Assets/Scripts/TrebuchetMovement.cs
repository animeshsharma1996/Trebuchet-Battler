using UnityEngine;
using UnityEngine.UI;

public class TrebuchetMovement : MonoBehaviour
    {
 
        public float m_Speed = 12f;                 // How fast the cart moves forward and back.
        public float m_TurnSpeed = 180f;            // How fast the cart turns in degrees per second.
        public bool  m_Fix = true;
        public GameObject m_ToggleFix;              // Game Object referencing the Fix/Free Toggle Text
        private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
        private string m_TurnAxisName;              // The name of the input axis for turning.
        private Rigidbody m_Rigidbody;              // Reference used to move the cart.
        private float m_MovementInputValue;         // The current value of the movement input.
        private float m_TurnInputValue;             // The current value of the turn input.
        private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the cart
        private Quaternion m_turnRotation;
    
        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody> ();
        }


        private void OnEnable()
        {
            // When the cart is turned on, make sure it's not kinematic.
            m_Rigidbody.isKinematic = false;

            // Also reset the input values.
            m_MovementInputValue = 0f;
            m_TurnInputValue = 0f;

            // We grab all the Particle systems child of that cart to be able to Stop/Play them on Deactivate/Activate
            // It is needed because we move the cart when spawning it, and if the Particle System is playing while we do that
            // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
            m_particleSystems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Play();
            }
        }


        private void OnDisable()
        {
            // When the cart is turned off, set it to kinematic so it stops moving.
            m_Rigidbody.isKinematic = true;

            // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
            for(int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Stop();
            }
        }


        private void Start()
        {
            // The axes names are based on player number.
            m_MovementAxisName = "Vertical";
            m_TurnAxisName = "Horizontal";
        }

        private void Update()
        {
            if (Input.GetKeyUp("l"))
            {
                Invoke("ToggleFix", 1.0f);

            }

            if (!m_Fix)
            {
            m_ToggleFix.GetComponent<TextMesh>().text = "Free";
            // Store the value of both input axes.
               m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
               m_TurnInputValue = Input.GetAxis(m_TurnAxisName);
            }
            else
        {
            m_ToggleFix.GetComponent<TextMesh>().text = "Fixed";
        }

            


        }
        private void ToggleFix()
        {
            m_Fix = !m_Fix;
        }

        private void FixedUpdate ()
        {
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        Turn();
        Move();
            
        }


        private void Move()
        {
            // Create a vector in the direction the cart is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = -transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position.
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }


    private void Turn()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float m_turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        m_turnRotation = Quaternion.Euler(0f, m_turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        m_Rigidbody.MoveRotation((m_Rigidbody.rotation)*m_turnRotation);

    }
    }
