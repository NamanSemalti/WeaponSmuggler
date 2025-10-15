using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string interactPrompt = "Interact";
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private bool canHighlight = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private UnityEvent onFocus;
    [SerializeField] private UnityEvent onLoseFocus;

    private bool _isFocused;
    private Transform _playerTransform;

    public string InteractPrompt => interactPrompt;
    public bool IsFocused => _isFocused;

    protected virtual void Awake()
    {
        // Optional: Set up defaults
    }

    public void SetFocus(Transform player)
    {
        _isFocused = true;
        _playerTransform = player;
        onFocus?.Invoke();
        OnFocus();
    }

    public void ClearFocus()
    {
        _isFocused = false;
        _playerTransform = null;
        onLoseFocus?.Invoke();
        OnLoseFocus();
    }

    public bool CanInteract()
    {
        if (!_isFocused || _playerTransform == null) return false;
        return Vector3.Distance(transform.position, _playerTransform.position) <= interactDistance;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        onInteract?.Invoke();
        OnInteract();
    }

    protected virtual void OnFocus() { }
    protected virtual void OnLoseFocus() { }
    protected abstract void OnInteract(); // Must be implemented in subclasses

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
