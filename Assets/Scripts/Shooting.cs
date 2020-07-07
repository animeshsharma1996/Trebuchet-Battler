	using System.Collections;
	//using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;


		public class Shooting : MonoBehaviour
		{
			public Rigidbody m_cannon_ball;             // Prefab of the cannon ball.
			public Transform m_FireTransform;           // A child of the cart where the cannon balls are spawned.
			public Transform m_Turret;                  // The child of the cart which rotates when firing starts.
			public Slider m_AimSlider;                  // A child of the cart that displays the current launch force.
			public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
			public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
			public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
            public GameObject m_ToggleShot;             // Game Object referencing the Shoot/Reload Toggle Text
	        public float m_MinLaunchForce = 10f;        // The force given to the cannon ball if the fire button is not held.
			public float m_MaxLaunchForce = 40f;        // The force given to the cannon ball if the fire button is held for the max charge time.
			public float m_MaxChargeTime = 2f;        // How long the cannon ball can charge for before it is fired at max force.
	        
			  


			private string m_FireButton;                // The input axis that is used for launching cannon balls.
			private float m_CurrentLaunchForce;         // The force that will be given to the cannon ball when the fire button is released.
			private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
			private bool m_Fired;                       // Whether or not the cannon ball has been launched with this button press.
			private bool m_released = false;
			private float m_TimeCount;
			private bool m_rotating = false;
	        private bool m_rotatingback = false;
			/*private float m_turn = 0;
			private float m_turnSpeed = 10f;
	        private bool m_maxPos = false;
	        private bool m_startPos = true;*/

			private void OnEnable()
			{
				// When the cart is turned on, reset the launch force and the UI
				m_CurrentLaunchForce = m_MinLaunchForce;
				m_AimSlider.value = m_MinLaunchForce;
	}


			private void Start()
			{
				// The fire axis is based on the player number.
				m_FireButton = "Fire1";

				// The rate that the launch force charges up is the range of possible forces by the max charge time.
				m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

			}


			void Update()
			{
		if((Mathf.RoundToInt(m_Turret.transform.rotation.eulerAngles.x) == 270f))
        {
			m_ToggleShot.GetComponent<TextMesh>().text = "Shoot";
		}
				// The slider should have a default value of the minimum launch force.
				//m_AimSlider.value = m_MinLaunchForce;
		
			// If the max force has been exceeded and the cannon ball hasn't yet been launched...
			if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
			{
				// ... use the max force and launch the cannon ball.
				m_CurrentLaunchForce = m_MaxLaunchForce;
				Fire();
			}
			// Otherwise, if the fire button has just started being pressed...
			else if (Input.GetButtonDown(m_FireButton) && (Mathf.RoundToInt(m_Turret.transform.rotation.eulerAngles.x) == 270f))
			{
				// ... reset the fired flag and reset the launch force.
				m_Fired = false;
				m_CurrentLaunchForce = m_MinLaunchForce;
				// Change the clip to the charging clip and start it playing.
				m_ShootingAudio.clip = m_ChargingClip;
				m_ShootingAudio.Play();
			}
			// Otherwise, if the fire button is being held and the cannon ball hasn't been launched yet...
			else if (Input.GetButton(m_FireButton) && !m_Fired)
			{
				// Increment the launch force and update the slider.
				m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
				m_AimSlider.value = m_CurrentLaunchForce;
			}
			// Otherwise, if the fire button is released and the cannon ball hasn't been launched yet...
			else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
			{
				// ... launch the cannon ball.
				Fire();
			}
		}

		

	     private void FixedUpdate()
    {
		if (Input.GetButtonUp(m_FireButton) && (Mathf.RoundToInt(m_Turret.transform.rotation.eulerAngles.x) == 270f))
		{
			Quaternion ToRotation = Quaternion.Euler(-150f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
			m_rotating = true;
			
			m_TimeCount = 0;
			if (m_rotating)
			{
				m_TimeCount += Time.deltaTime * 50f;
				m_Turret.transform.rotation = Quaternion.Lerp(m_Turret.transform.rotation, ToRotation, m_TimeCount);

				m_released = true;
				if (m_TimeCount > 1)
				{
					m_rotating = false;
					
				}
				
				
			}
		}
		if(Input.GetKey(KeyCode.Space) || Input.GetKey("r"))
		{ 
			Quaternion BackRotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
			m_rotatingback = true;
			m_TimeCount = 0;
			if (m_rotatingback)
			{
				m_TimeCount += Time.deltaTime * 10f;
				m_Turret.transform.rotation = Quaternion.Lerp(m_Turret.transform.rotation, BackRotation, m_TimeCount);

				if (m_TimeCount > 1)
				{
					m_rotatingback = false;
				}
			}
		}
		
	}

		private void Fire()
			{

		if (m_released)
		{
			// Create an instance of the cannon ball and store a reference to it's rigidbody.
			Rigidbody cannon_ballInstance =
						Instantiate(m_cannon_ball, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
			
				// Set the cannon ball's velocity to the launch force in the fire position's forward direction.
				cannon_ballInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
			// Set the fired flag so only Fire is only called once.
			m_Fired = true;
			// Change the clip to the firing clip and play it.
			m_ShootingAudio.clip = m_FireClip;
					m_ShootingAudio.Play();
			m_ToggleShot.GetComponent<TextMesh>().text = "Reload";
			// Reset the launch force.  This is a precaution in case of missing button events.
			m_CurrentLaunchForce = m_MinLaunchForce;
					m_released = false;
			m_AimSlider.value = m_MinLaunchForce;

		}
			}

		
		}
	

