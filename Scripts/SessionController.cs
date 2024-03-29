using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using MimicSpace;

public class SessionController : MonoBehaviour
{
    //[SerializeField] int points = 0;
    [SerializeField] int lives = 3;
    [SerializeField] int health = 30;
    //[SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI livesText;

    [SerializeField] TextMeshProUGUI healthText;

    GameObject mimicBody;
    // Start is called before the first frame update

    public static SessionController instance = null; // Singleton pattern for the session controller
    
    private void Awake() // Singleton pattern for the session controller
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        livesText.text = lives.ToString();
        healthText.text = health.ToString();
    }

    //On hit, reduce health and reduce scale of player object
    public void ReduceHealth(int damage)
    {
        health -= damage;
        healthText.text = health.ToString();

        // Get the mimicbody
        mimicBody = GameObject.Find("Sphere");

        // Get the mimicbody current scale
        Vector3 currentScale = mimicBody.transform.localScale;

        // Decrease the scale by a certain factor
        float scaleReductionFactor = 0.5f; // Change this to whatever factor you want
        Vector3 newScale = currentScale * (1 - scaleReductionFactor);

        // Set the mimics new scale
        mimicBody.transform.localScale = newScale;
        if (health < 10)
        {
            RemoveLife(); // Remove a life
        }
    }

    public void AddHealth(int healthToAdd)
    {
        health += healthToAdd;
        healthText.text = health.ToString();

        // Get the mimicbody
        mimicBody = GameObject.Find("Sphere");

        // Get the mimics's current scale
        Vector3 currentScale = mimicBody.transform.localScale;

        // Increase the scale by a certain factor
        float scaleIncreaseFactor = 0.1f; // Change this to whatever factor you want
        Vector3 newScale = currentScale * (1 + scaleIncreaseFactor);

        // Set the mimics new scale
        mimicBody.transform.localScale = newScale;
    }

    public void RemoveLife()
    {
        lives--;
        //Debug.Log("Lives: " + lives);
        livesText.text = lives.ToString();

        // Reload the scene if the player loses a life
        if(lives >= 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            health = 30; // Reset the player's health
            healthText.text = health.ToString();

            // Get the mimicbody
            mimicBody = GameObject.Find("Sphere");

            mimicBody.transform.localScale = new Vector3(1, 1, 1); // Reset the player's scale
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the player has no more lives, reset the game
        if(lives == 0)
        {
            Debug.Log("Game Over");
            ResetGameSession();
        }

        // If the player presses the "R" key, reload the scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            // Reset the player's health and scale
            health = 50;
            healthText.text = health.ToString();

            // Get the mimicbody
            mimicBody = GameObject.Find("Sphere");
            
            mimicBody.transform.localScale = new Vector3(1, 1, 1);

            // Reset the player's lives
            lives = 3;
            livesText.text = lives.ToString();
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            // Kill the player
            RemoveLife();
        }
    }

    public void ResetGameSession()
    {
        SceneManager.LoadSceneAsync(0);
        Destroy(gameObject);

        // Re initialize the ui
        //scoreText.text = points.ToString();
        livesText.text = lives.ToString();
        healthText.text = health.ToString();

        Cursor.visible = true; // Make the cursor visible
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor if it was locked
    }

    
}
