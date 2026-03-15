using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public float healthRegenDelay = 10f;
    public int healthRegenRate = 1;

    [Header("Stamina Settings")]
    public int maxStamina = 100;
    public int currentStamina;
    public float staminaRegenDelay = 5f;
    public int staminaRegenRate = 20;
    public int staminaDrainPerSecond = 15;
    public int staminaCostPerAction = 10;
    public int healthCostNoStamina = 5;

    [Header("Movement Settings")]
    public float baseSpeed = 300f;
    public float sprintSpeed = 1000f;
    public float currentSpeed;

    [Header("UI References")]
    public Slider healthSlider;
    public Slider staminaSlider;
    public Text healthText;
    public Text staminaText;

    private bool isRegeneratingHealth = false;
    private bool isRegeneratingStamina = false;
    public bool isSprinting { get; private set; } 
    private Coroutine healthRegenCoroutine;
    private Coroutine staminaRegenCoroutine;
    private Coroutine sprintDrainCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentSpeed = baseSpeed;

        UpdateUI();
    }

    void Update()
    {
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);

        if (wantsToSprint && currentStamina > 0 && !isSprinting)
        {
            StartSprinting();
        }
        else if ((!wantsToSprint || currentStamina <= 0) && isSprinting)
        {
            StopSprinting();
        }

        UpdateUI();
    }

    void StartSprinting()
    {
        isSprinting = true;
        currentSpeed = sprintSpeed;

        if (sprintDrainCoroutine != null)
            StopCoroutine(sprintDrainCoroutine);
        sprintDrainCoroutine = StartCoroutine(DrainStaminaWhileSprinting());
    }

    void StopSprinting()
    {
        isSprinting = false;
        currentSpeed = baseSpeed;

        if (sprintDrainCoroutine != null)
        {
            StopCoroutine(sprintDrainCoroutine);
            sprintDrainCoroutine = null;
        }

        StartStaminaRegen();
    }

    IEnumerator DrainStaminaWhileSprinting()
    {
        while (isSprinting && currentStamina > 0)
        {
            yield return new WaitForSeconds(1f); 
            UseStamina(staminaDrainPerSecond);
        }

        if (currentStamina <= 0)
        {
            StopSprinting();
        }
    }

    public bool UseStamina(int amount)
    {
        if (staminaRegenCoroutine != null)
            StopCoroutine(staminaRegenCoroutine);

        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            Debug.Log($"Потрачено {amount} стамины. Осталось: {currentStamina}");

            if (currentStamina <= 0 && isSprinting)
            {
                StopSprinting();
            }

            StartStaminaRegen();
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log($"Не хватает стамины! Нужно {amount}, есть {currentStamina}");
            StartStaminaRegen();
            UpdateUI();
            return false;
        }
    }

    public void UseHealth(int amount)
    {
        if (healthRegenCoroutine != null)
            StopCoroutine(healthRegenCoroutine);

        currentHealth = Mathf.Max(0, currentHealth - amount);
        Debug.Log($"Потрачено {amount} здоровья. Осталось: {currentHealth}");

        StartHealthRegen();
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void StartHealthRegen()
    {
        if (healthRegenCoroutine != null)
            StopCoroutine(healthRegenCoroutine);

        healthRegenCoroutine = StartCoroutine(RegenerateHealth());
    }

    void StartStaminaRegen()
    {
        if (staminaRegenCoroutine != null)
            StopCoroutine(staminaRegenCoroutine);

        staminaRegenCoroutine = StartCoroutine(RegenerateStamina());
    }

    IEnumerator RegenerateHealth()
    {
        Debug.Log($"Регенерация здоровья начнется через {healthRegenDelay} сек");
        yield return new WaitForSeconds(healthRegenDelay);

        while (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + healthRegenRate);
            Debug.Log($"Здоровье восстановлено: {currentHealth}/{maxHealth}");
            UpdateUI();
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator RegenerateStamina()
    {
        Debug.Log($"Регенерация стамины начнется через {staminaRegenDelay} сек");
        yield return new WaitForSeconds(staminaRegenDelay);

        while (currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate);
            Debug.Log($"Стаamina восстановлена: {currentStamina}/{maxStamina}");
            UpdateUI();
            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;

            Image fillImage = staminaSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = isSprinting ? Color.cyan : Color.green;
            }
        }

        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";

        if (staminaText != null)
            staminaText.text = $"{currentStamina}/{maxStamina}";
    }

    void Die()
    {
        Debug.Log("Персонаж погиб!");
    }
}