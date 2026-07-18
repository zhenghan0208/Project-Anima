using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Heart UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private bool isDead;

    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        currentHealth = maxHealth;

        UpdateHealthUI();
    }

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        if (isDead)
            return;

        playerController.ApplyKnockback(hitPoint);

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    public void Kill()
    {
        if (isDead)
            return;

        currentHealth = 0;

        UpdateHealthUI();

        Die();
    }

    void Die()
    {
        isDead = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}