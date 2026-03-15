using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class IntroCutscene : MonoBehaviour
{
    [Header("Текст заставки")]
    [TextArea(3, 5)]
    public string[] introText = new string[]
    {
        "Королевство Востока...",
        "Великий город, откуда ты родом.",
        "Но сегодня ты оказался далеко от дома...",
        "",
        "Таинственный портал перенёс тебя в",
        "ПУРПУРНЫЙ УДЕЛ",
        "- маленькую деревню на краю света.",
        "",
        "Портал истощён.",
        "Чтобы вернуться домой,",
        "тебе нужно найти особые камни,сокровище этой долины,",
        "способные запитать его вновь.",
        "",
        "Старейшина не говорит, где скала с камнями",
        "где-то в этих землях...",
        "Но никто точно не помнит, где именно.",
        "",
        "Расспрашивать жителей деревни бесполезно,",
        "они не укажут тебе путь.",
        "",
        "Но только одно направление верное и тебе предстоит узнать какое,собери камни ...",
        "",
        "Управление:",
        "WASD - движение",
        "R - добыча ресурсов",
        "I - инвентарь",
        "",
        "Найди камни и ожидай нужного дня,чтобы вернуться обратно домой!"
    };

    [Header("Настройки")]
    public float textSpeed = 0.05f;
    public float delayBetweenLines = 0.5f;

    private Text displayText;
    private GameObject skipButton;
    private bool isSkipped = false;
    private Camera mainCamera;

    private GameObject canvasObject;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Камера не найдена!");
            return;
        }

        CreateIntroUI();
        StartCoroutine(PlayIntro());
    }

    void CreateIntroUI()
    {
        canvasObject = new GameObject("IntroCanvas");
        canvasObject.transform.SetParent(mainCamera.transform);
        canvasObject.transform.localPosition = Vector3.zero;
        canvasObject.transform.localRotation = Quaternion.identity;
        canvasObject.transform.localScale = Vector3.one;

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; 
        canvas.sortingOrder = 15; 

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObject.AddComponent<GraphicRaycaster>();

        
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasObject.transform);

        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = Color.black;

        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        GameObject textGO = new GameObject("IntroText");
        textGO.transform.SetParent(canvasObject.transform);

        displayText = textGO.AddComponent<Text>();
        displayText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        displayText.fontSize = 40;
        displayText.color = Color.white;
        displayText.alignment = TextAnchor.MiddleCenter;
        displayText.horizontalOverflow = HorizontalWrapMode.Wrap;
        displayText.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.1f);
        textRect.anchorMax = new Vector2(0.9f, 0.9f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        GameObject skipGO = new GameObject("SkipText");
        skipGO.transform.SetParent(canvasObject.transform);

        Text skipText = skipGO.AddComponent<Text>();
        skipText.text = "ПРОПУСТИТЬ [ПРОБЕЛ]";
        skipText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        skipText.fontSize = 24;
        skipText.color = Color.gray;
        skipText.alignment = TextAnchor.LowerCenter;

        RectTransform skipRect = skipGO.GetComponent<RectTransform>();
        skipRect.anchorMin = new Vector2(0, 0);
        skipRect.anchorMax = new Vector2(1, 0);
        skipRect.pivot = new Vector2(0.5f, 0);
        skipRect.anchoredPosition = new Vector2(0, 50);
        skipRect.sizeDelta = new Vector2(0, 50);

        skipButton = skipGO;

        canvasObject.SetActive(true);

        Debug.Log($"IntroCanvas создан. Родитель: {canvasObject.transform.parent.name}, Позиция: {canvasObject.transform.position}");
    }

    IEnumerator PlayIntro()
    {
        DisablePlayerControls(true);

        yield return new WaitForSeconds(0.1f);

        if (canvasObject != null)
        {
            Debug.Log($"Canvas активен: {canvasObject.activeSelf}");
            Debug.Log($"Текст назначен: {displayText != null}");
            if (displayText != null)
                Debug.Log($"Текст: '{displayText.text}'");
        }

        foreach (string line in introText)
        {
            if (isSkipped) break;

            if (displayText != null)
            {
                displayText.text = "";

                foreach (char c in line)
                {
                    if (isSkipped) break;

                    displayText.text += c;
                    yield return new WaitForSeconds(textSpeed);
                }

                yield return new WaitForSeconds(delayBetweenLines);
            }
        }

        if (displayText != null && !isSkipped)
        {
            displayText.text = "Нажми ПРОБЕЛ чтобы начать игру...";
        }

        while (!Input.GetKeyDown(KeyCode.Space) && !isSkipped)
        {
            yield return null;
        }

        StartGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSkipped = true;
            StartGame();
        }
    }

    void StartGame()
    {
        if (canvasObject != null)
        {
            Destroy(canvasObject);
        }

        DisablePlayerControls(false);
    }

    void DisablePlayerControls(bool disable)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerMover mover = player.GetComponent<PlayerMover>();
            if (mover != null) mover.enabled = !disable;

            ResourceCollector collector = player.GetComponent<ResourceCollector>();
            if (collector != null) collector.enabled = !disable;

            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null) stats.enabled = !disable;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = !disable;
        }
    }
}