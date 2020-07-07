using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public LayerMask m_cartMask;                        // Used to filter what the explosion affects, this should be set to "Players".
    public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.

    private AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.

    public AudioClip m_ExplosionClip;                // Reference to the audio clip that will play on explosion.

    public float m_MaxDamage = 30f;                    // The amount of damage done if the explosion is centred on a cart.
    public float m_DamageEnemy = 80f;                    // The amount of damage done to Enemy Trebuchet if the explosion is centred on the cart.
   // public float m_ExplosionForce = 100f;              // The amount of force added to a cart at the centre of the explosion.
   // public float m_MaxLifeTime = 15f;                    // The time in seconds before the cannon is removed.
    public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion, carts can be and are still affected.


    private void Start()
    {
        // If it isn't destroyed by then, destroy the cannon after it's lifetime.
        //Destroy(gameObject, m_MaxLifeTime);
        m_ExplosionAudio = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>(); 
    }


    /*private void OnCollisionEnter(Collision other)
    {
        // Unparent the particles from the cannon.
        //m_ExplosionParticles.transform.parent = null;
        if (other.gameObject.tag == "Ground")
        {


            // Play the particle system.
            m_ExplosionParticles.transform.gameObject.SetActive(true);
            m_ExplosionParticles.Play();
            m_ExplosionParticles.transform.parent = null;

            // Play the explosion sound effect.
            m_ExplosionAudio.clip = m_ExplosionClip;
            m_ExplosionAudio.Play();
           // Debug.Log("Die");

            // Once the particles have finished, destroy the gameobject they are on.
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            Destroy(m_ExplosionParticles.gameObject, mainModule.duration);

            // Destroy the cannon.
           Destroy(gameObject);
        }

      
    }*/
    private void OnTriggerEnter(Collider other)
    {
        // Collect all the colliders in a sphere from the cannon's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_cartMask);

        // Go through all the colliders...
        for (int i = 0; i < colliders.Length; i++)
        {
            // ... and find their rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            // If they don't have a rigidbody, go on to the next collider.
            if (!targetRigidbody)
                continue;

            // Add an explosion force.
            //targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Find the TrebutchetHealth script associated with the rigidbody.
            TrebuchetHealth targetHealth = targetRigidbody.GetComponent<TrebuchetHealth>();

            // If there is no cartHealth script attached to the gameobject, go on to the next collider.
            if (!targetHealth)
                continue;

            if(targetRigidbody.gameObject.tag == "Enemy")
            {
                m_MaxDamage = 80f;
                m_ExplosionRadius = 7f;
            }
            else
            {
                m_MaxDamage = 30f;
                m_ExplosionRadius = 5f;
            }
            // Calculate the amount of damage the target should take based on it's distance from the cannon.
            float damage = CalculateDamage(targetRigidbody.position);
            // Deal this damage to the cart.
            targetHealth.TakeDamage(damage);
        }


        // Play the explosion sound effect.
        m_ExplosionAudio.clip = m_ExplosionClip;
        m_ExplosionAudio.Play();
       

        // Play the particle system.
        m_ExplosionParticles.transform.gameObject.SetActive(true);
        m_ExplosionParticles.Play();
        m_ExplosionParticles.transform.parent = null;

        

        // Once the particles have finished, destroy the gameobject they are on.
        ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
        Destroy(m_ExplosionParticles.gameObject, mainModule.duration);

        // Destroy the cannon.
        Destroy(gameObject);
    }

        private float CalculateDamage(Vector3 targetPosition)
    {
        // Create a vector from the shell to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that the minimum damage is always 0.
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}
