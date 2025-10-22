using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class UIDragIcon : MonoBehaviour
{
    public static UIDragIcon Instance { get; private set; }

    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text quantityText;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Coroutine fadeRoutine;

    public ItemDataSO CurrentItem { get; private set; }
    public int CurrentQuantity { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (iconImage) iconImage.enabled = false;
        if (quantityText) quantityText.text = "";
        HideImmediate();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
            rectTransform.position = Input.mousePosition;
    }

    /// <summary>Shows the icon visually (also sets CurrentQuantity). Use SetDraggedItem beforehand to set both item+quantity.</summary>
    public void Show(Sprite sprite, int quantity)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        CurrentQuantity = quantity;
        if (iconImage)
        {
            iconImage.sprite = sprite;
            iconImage.enabled = true;
        }

        if (quantityText)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
            quantityText.gameObject.SetActive(quantity > 1);
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
    }

    /// <summary>Set the logical item+quantity that will be used by drop handlers.</summary>
    public void SetDraggedItem(ItemDataSO item, int quantity)
    {
        CurrentItem = item;
        CurrentQuantity = quantity;
    }

    /// <summary>Immediate hide (no fade)</summary>
    public void HideImmediate()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        gameObject.SetActive(false);
        CurrentItem = null;
        CurrentQuantity = 0;
    }

    /// <summary>Fade & hide after delay; safe to call multiple times.</summary>
    public void HideAfterDelay(float delay = 0.05f)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutRoutine(delay));
    }

    private IEnumerator FadeOutRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        float t = 0f;
        float duration = 0.12f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        HideImmediate();
    }

    private void OnDisable()
    {
        // cleanup
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        CurrentItem = null;
        CurrentQuantity = 0;
    }
}
