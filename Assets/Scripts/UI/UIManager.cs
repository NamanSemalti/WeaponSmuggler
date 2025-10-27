using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TMP_Text messageText; // Assign in Inspector
    [SerializeField] private CanvasGroup messageGroup;
    [SerializeField] private GameObject interactionMessageTextObject;
    [SerializeField] private TMP_Text interactionMessageText;
    [SerializeField] private CanvasGroup interactionGroup;
    [Header("Settings")]
    [SerializeField] private float messageDisplayTime = 2f;
    [SerializeField] private float fadeSpeed = 5f;

    private Coroutine messageRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (messageGroup)
            messageGroup.alpha = 0f;
        HideInteractionMessage();
    }
    void OnEnable()
    {
        InteractionEvents.OnFocus += ShowInteractionMessage;
        InteractionEvents.OnLoseFocus += HideInteractionMessage;
    }
    void OnDisable()
    {
        InteractionEvents.OnFocus -= ShowInteractionMessage;
        InteractionEvents.OnLoseFocus -= HideInteractionMessage;
    }
    public void ShowMessage(string message)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(DisplayMessageRoutine(message));
    }
    public void ShowInteractionMessage(Interactable interactable)
    {
        interactionMessageText.text = interactable.InteractPrompt;
        // interactionMessageTextObject.SetActive(true);
        StartCoroutine(FadeCanvasGroup(interactionGroup, interactionGroup.alpha, 1f, 0.15f));
    }
    public void ShowInteractionMessage(string interactionText)
    {
        interactionMessageText.text = interactionText;
        StartCoroutine(FadeCanvasGroup(interactionGroup, interactionGroup.alpha, 1f, 0.15f));
    }
    public void HideInteractionMessage(Interactable interactable = null)
    {
        StartCoroutine(FadeCanvasGroup(interactionGroup, interactionGroup.alpha, 0f, 0.2f));
    }
    private IEnumerator DisplayMessageRoutine(string message)
    {
        if (messageText)
            messageText.text = message;

        // Fade in
        yield return StartCoroutine(FadeCanvasGroup(messageGroup, 0f, 1f, 0.25f));

        yield return new WaitForSeconds(messageDisplayTime);

        // Fade out
        yield return StartCoroutine(FadeCanvasGroup(messageGroup, 1f, 0f, 0.5f));

        if (messageText)
            messageText.text = "";
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end, float duration)
    {
        if (group == null)
            yield break;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime * fadeSpeed;
            group.alpha = Mathf.Lerp(start, end, time / duration);
            yield return null;
        }

        group.alpha = end;
    }
}
