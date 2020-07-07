using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject m_enemyTrebutchet;
    public GameObject m_gameOverScreen;
    public Transform m_player;
    public float m_spawnRadius = 70f;
    public float m_restartDelay = 5f;
    public bool m_isGameOver = false;

    private float m_spawnTime = 0f;
    private float m_spawnInterval = 30f;
    private float m_restartTimer;
    private Vector3 m_spawnPosition;
    private float m_targetDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        m_spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        GameOver();

        bool m_enemySpawned = false;
        while (((Time.time -  m_spawnTime) > m_spawnInterval) && (!m_enemySpawned) && (!m_isGameOver))
        {
            
            m_spawnPosition = new Vector3(Random.Range(-m_spawnRadius, m_spawnRadius),0.5f, Random.Range(-m_spawnRadius, m_spawnRadius));
            m_targetDistance = Vector3.Distance(m_spawnPosition, m_player.position);

            if (m_targetDistance < 30f)
            {
                continue;
            }
            else
            {
                Instantiate(m_enemyTrebutchet, m_spawnPosition, Quaternion.identity);
                m_enemySpawned = true;
                m_spawnTime = Time.time;
            }
           
        }

        
    }
    void GameOver()
    {
        if(m_gameOverScreen.activeSelf)
        {
            m_isGameOver = true;
            // .. increment a timer to count up to restarting.
            m_restartTimer += Time.deltaTime;

            // .. if it reaches the restart delay...
            if (m_restartTimer >= m_restartDelay)
            {
                if (Input.GetKeyUp("r"))
                {
                    // .. then reload the currently loaded level.
                    Application.LoadLevel(Application.loadedLevel);
                }
            }
        }
    }
    
}

