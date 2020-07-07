using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrebuchetHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;               // The amount of health each cart starts with.
    public Slider m_Slider;                             // The slider to represent how much health the cart currently has.
    public Image m_FillImage;                           // The image component of the slider.
    public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
    public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the cart dies.
    
    
    private GameObject Camera;
    private AudioSource m_ExplosionAudio;               // The audio source to play when the cart explodes.
    private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the cart is destroyed.
    private float m_CurrentHealth;                      // How much health the cart currently has.
    private bool m_Dead;                                // Has the cart been reduced beyond zero health yet?


    private void Awake()
    {
        // Instantiate the explosion prefab and get a reference to the particle system on it.
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

        // Get a reference to the audio source on the instantiated prefab.
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // Disable the prefab so it can be activated when it's required.
        m_ExplosionParticles.gameObject.SetActive(false);

        // Get the Main Camera Game Object
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
    }


    private void OnEnable()
    {
        // When the cart is enabled, reset the cart's health and whether or not it's dead.
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        // Update the health slider's value and color.
        SetHealthUI();
    }


    public void TakeDamage(float amount)
    {
        // Reduce current health by the amount of damage done.
        m_CurrentHealth -= amount;

        // Change the UI elements appropriately.
        SetHealthUI();

        // If the current health is at or below zero and it has not yet been registered, call OnDeath.
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        m_Slider.value = m_CurrentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Set the flag so that this function is only called once.
        m_Dead = true;

        // Move the instantiated explosion prefab to the cart's position and turn it on.
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        // Play the particle system of the cart exploding.
        m_ExplosionParticles.Play();

        // Play the cart explosion sound effect.
        m_ExplosionAudio.Play();
        
        if(gameObject.tag == "Player")
        {
            Camera.transform.parent = null;
            Camera.transform.Find("GameOverTrigger").gameObject.SetActive(true);
            
        }
        // Turn the cart off.
        gameObject.SetActive(false);
    }
}
