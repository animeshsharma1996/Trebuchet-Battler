using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyShooting : MonoBehaviour
{
	public Rigidbody m_cannon_ball;             // Prefab of the cannon ball.
	public Transform m_FireTransform;           // A child of the cart where the cannon balls are spawned.
	public Transform m_Turret;                  // The child of the cart which rotates when firing starts.
	public Transform m_Target;                  // The tranform of the target at which Enemy shoots
	public Transform m_Enemy;
	public Slider m_AimSlider;                  // A child of the cart that displays the current launch force.
	public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
	public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
	public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
	//public float m_MinLaunchForce = 10f;        // The force given to the cannon ball if the fire button is not held.
	//public float m_MaxLaunchForce = 40f;        // The force given to the cannon ball if the fire button is held for the max charge time.
	public float m_MaxChargeTime = 0.75f;        // How long the cannon ball can charge for before it is fired at max force.
	public bool m_isGameOver = false;


	private string m_FireButton;                // The input axis that is used for launching cannon balls.
	
	private float m_CurrentLaunchForce;         // The force that will be given to the cannon ball when the fire button is released.
	private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
	private bool m_Fired;                       // Whether or not the cannon ball has been launched with this button press.
	private bool m_released = false;
	private float m_TimeCount;
	private float m_TimeCountBack;
	private bool m_rotating = false;
	private bool m_rotatingback = false;

	/*private float m_turn = 0;
	private float m_turnSpeed = 10f;*/

	private float m_targetDistance = 0f;
	private float m_launchSpeed = 0f;
	private bool m_maxPos = false;
	private bool m_restPos = true;
	private float m_shotInterval = 4.0f;
	private float m_reloadInterval = 1.0f;
	private float m_shotTime = 0f;
	private float m_reloadTime = 0f;

	private void OnEnable()
	{
		// When the cart is turned on, reset the launch force and the UI
		//m_CurrentLaunchForce = m_MinLaunchForce;
		//m_AimSlider.value = m_MinLaunchForce;

		m_Target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		m_maxPos = false;
		m_restPos = true;
		m_Fired = false;
	}


	private void Start()
	{ 


	}


	void Update()
	{
	
		if(!(GameObject.FindGameObjectWithTag("Player").activeSelf))
        {
			m_isGameOver = true;
        }
		m_Enemy.LookAt(m_Target);

		m_targetDistance = Vector3.Distance(m_Turret.position, m_Target.position);

		float s = m_targetDistance;
		m_launchSpeed = (Mathf.Sqrt((8.5f) * s / Mathf.Sin(Mathf.PI/3)));

		if (m_maxPos && (!m_isGameOver))
		{
			Fire();
		}
	}



	private void FixedUpdate()
	{
		
		if ((m_restPos || m_rotating) && ((Time.time - m_shotTime) > m_shotInterval))
		{
			Quaternion ToRotation = Quaternion.Euler(-150f,180f + transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
			m_rotating = true;
			m_TimeCount = 0;
			if (m_rotating)
			{
				m_TimeCount += Time.deltaTime * 50f;
				m_Turret.transform.rotation = Quaternion.Lerp(m_Turret.transform.rotation, ToRotation, m_TimeCount);

				m_released = true;
				if (Mathf.RoundToInt(m_Turret.transform.rotation.eulerAngles.x) == 330f)
				{
					m_rotating = false;
					m_maxPos = true;
					m_restPos = false;
					
				}


			}
		}
		if ((m_maxPos || m_rotatingback) && (m_Fired) && ((Time.time - m_reloadTime) > m_reloadInterval))
		{
			Quaternion BackRotation = Quaternion.Euler(-90f, 180f + transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
			m_rotatingback = true;
			m_TimeCountBack = 0;
			if (m_rotatingback)
			{
				m_TimeCountBack += Time.deltaTime * 5f;
				m_Turret.transform.rotation = Quaternion.Lerp(m_Turret.transform.rotation, BackRotation, m_TimeCountBack);

				if (Mathf.RoundToInt(m_Turret.transform.rotation.eulerAngles.x) <= 270f)
				{
					m_rotatingback = false;
					m_maxPos = false;
					m_restPos = true;
					m_Fired = false;

				}
			}
		}
		

	}

	private void Fire()
	{

		// Set the fired flag so only Fire is only called once.
		//m_Fired = true;
		if (m_released)
		{
			m_shotTime = Time.time;
			m_reloadTime = Time.time;
			// Create an instance of the cannon ball and store a reference to it's rigidbody.
			Rigidbody cannon_ballInstance = Instantiate(m_cannon_ball, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

			// Set the cannon ball's velocity to the launch force in the fire position's forward direction.
			cannon_ballInstance.velocity = (m_launchSpeed) * m_FireTransform.forward;
			

			// Change the clip to the firing clip and play it.
			m_ShootingAudio.clip = m_FireClip;
			m_ShootingAudio.Play();
			m_Fired = true;
			m_maxPos = true;
			m_restPos = false;

			m_released = false;

		}
	}

}
