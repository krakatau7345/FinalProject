using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 20;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public Transform slotsParent;

    [Header("Items")]
    public Item stoneItem;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private bool isInventoryOpen = false;

    void Start()
    {
        // Создаем слоты
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot());

            if (slotsParent != null && slotPrefab != null)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotsParent);
                slotObj.name = $"Slot_{i}";
            }
        }

        // Проверяем настройки текста во всех слотах
        CheckTextSettings();

        inventoryPanel.SetActive(false);
    }

    void CheckTextSettings()
    {
        if (slotsParent == null) return;

        for (int i = 0; i < slotsParent.childCount; i++)
        {
            Transform slot = slotsParent.GetChild(i);
            Text amountText = slot.Find("Amount")?.GetComponent<Text>();

            if (amountText != null)
            {
                // Принудительно настраиваем текст
                amountText.fontSize = 18;
                amountText.color = Color.white;
                amountText.alignment = TextAnchor.MiddleCenter;
                amountText.text = "";

                // Проверяем RectTransform
                RectTransform rect = amountText.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(-5, 5); // Правый верхний угол
                rect.sizeDelta = new Vector2(25, 25);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);
        }
    }

    public bool AddItem(Item item, int amount)
    {
        // Сначала ищем существующий стак
        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty() && slot.item == item && slot.amount < item.maxStack)
            {
                int spaceInStack = item.maxStack - slot.amount;
                int canAdd = Mathf.Min(spaceInStack, amount);

                slot.amount += canAdd;
                amount -= canAdd;

                UpdateInventoryUI();

                if (amount <= 0)
                {
                    Debug.Log($"Теперь в стеке: {slot.amount} {item.itemName}");
                    return true;
                }
            }
        }

        // Ищем пустой слот
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.AddItem(item, amount);
                UpdateInventoryUI();
                Debug.Log($"Положил {amount} {item.itemName} в новый слот");
                return true;
            }
        }

        Debug.Log("Инвентарь полон!");
        return false;
    }

    void UpdateInventoryUI()
    {
        if (slotsParent == null) return;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < slotsParent.childCount)
            {
                Transform slotTransform = slotsParent.GetChild(i);

                // Находим иконку и текст
                Image iconImage = slotTransform.Find("Icon")?.GetComponent<Image>();
                Text amountText = slotTransform.Find("Amount")?.GetComponent<Text>();

                if (!slots[i].IsEmpty())
                {
                    // Показываем иконку
                    if (iconImage != null)
                    {
                        iconImage.sprite = slots[i].item.icon;
                        iconImage.enabled = true;
                    }

                    // Показываем количество
                    if (amountText != null)
                    {
                        amountText.text = slots[i].amount.ToString();
                        amountText.enabled = true;

                        // Делаем текст жирным для наглядности
                        amountText.fontStyle = FontStyle.Bold;

                        // Если предметов больше 1 - показываем, если 1 - тоже показываем
                        // Если 0 - не показываем (но у нас не бывает 0)
                    }
                }
                else
                {
                    // Пустой слот
                    if (iconImage != null)
                    {
                        iconImage.sprite = null;
                        iconImage.enabled = false;
                    }

                    if (amountText != null)
                    {
                        amountText.text = "";
                        amountText.enabled = false;
                    }
                }
            }
        }
    }

    // Для отладки - показать содержимое инвентаря
    public void PrintInventory()
    {
        Debug.Log("=== Содержимое инвентаря ===");
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty())
            {
                Debug.Log($"Слот {i}: {slots[i].amount} x {slots[i].item.itemName}");
            }
        }
    }
}