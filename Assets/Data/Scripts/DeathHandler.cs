using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathHandler : MonoBehaviour
{
    private PlayerStats playerStats;
    private bool isDead = false;

    public float restartDelay = 5f; 

    private GameObject deathTextObject;
    private GUIStyle bigFontStyle;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();

        bigFontStyle = new GUIStyle();
        bigFontStyle.fontSize = Screen.width / 10; 
        bigFontStyle.fontStyle = FontStyle.Bold;
        bigFontStyle.alignment = TextAnchor.MiddleCenter;
        bigFontStyle.normal.textColor = Color.red;
    }

    void Update()
    {
        if (!isDead && playerStats != null && playerStats.currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("LOL YOU DIED!");

        DisableAllControls();

        StartCoroutine(RestartAfterDelay());
    }

    void DisableAllControls()
    {
        PlayerMover mover = GetComponent<PlayerMover>();
        if (mover != null)
            mover.enabled = false;

        ResourceCollector collector = GetComponent<ResourceCollector>();
        if (collector != null)
            collector.enabled = false;

        if (playerStats != null)
            playerStats.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);

        Debug.Log("Restarting game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnGUI()
    {
        if (isDead)
        {
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "LOL YOU DIED", bigFontStyle);
        }
    }
}