using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    private Inventory inventory;
    private PlayerStats playerStats;
    private bool isPlayerNearby = false;

    void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();
        playerStats = FindFirstObjectByType<PlayerStats>();

        if (inventory == null)
            Debug.LogError("Inventory не найден!");
        if (playerStats == null)
            Debug.LogError("PlayerStats не найден!");
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.R))
        {
            CollectResource();
        }
    }

    void CollectResource()
    {
        if (playerStats == null || inventory == null) return;

        bool staminaUsed = playerStats.UseStamina(playerStats.staminaCostPerAction);

        if (staminaUsed)
        {
            DoCollectResource();
            Debug.Log($"Добыча: потрачено {playerStats.staminaCostPerAction} стамины");
        }
        else
        {
            playerStats.UseHealth(playerStats.healthCostNoStamina);
            DoCollectResource();
            Debug.Log($"Добыча: нет стамины! Потрачено {playerStats.healthCostNoStamina} здоровья");
        }
    }

    void DoCollectResource()
    {
        ResourceItem resourceItem = GetComponent<ResourceItem>();

        if (resourceItem != null && resourceItem.item != null)
        {
            int randomAmount = Random.Range(1, 3);
            inventory.AddItem(resourceItem.item, randomAmount);
            Debug.Log($"Добыто {randomAmount} {resourceItem.item.itemName}!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Игрок рядом с ресурсом");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Игрок отошел от ресурса");
        }
    }
}