using UnityEngine;

public class UsableObject : Interactable
{
    [Header("Usable Object Settings")]
    [SerializeField] private bool isToggleable = true;
    [SerializeField] private bool startActive = false;
    [SerializeField] private AudioClip useSound;
    [SerializeField] private Animator animator;
    [SerializeField] private string animatorParameter = "IsActive"; // Optional animator bool

    private bool _isActive = false;

    protected override void Awake()
    {
        base.Awake();
        _isActive = startActive;

        if (animator && !string.IsNullOrEmpty(animatorParameter))
        {
            animator.SetBool(animatorParameter, _isActive);
        }
    }

    protected override void OnInteract()
    {
        // Toggle or single-use activation
        if (isToggleable)
            _isActive = !_isActive;
        else
            _isActive = true;

        // Play sound
        if (useSound)
            AudioSource.PlayClipAtPoint(useSound, transform.position);

        // Update Animator if assigned
        if (animator && !string.IsNullOrEmpty(animatorParameter))
        {
            animator.SetBool(animatorParameter, _isActive);
        }

        // Trigger event system (for VFX/UI/sound integration)
        InteractionEvents.TriggerUse(this);

        // Optional: handle logic manually here
        HandleUseAction();
    }

    private void HandleUseAction()
    {
        // Example: you can override or extend this per prefab
        Debug.Log($"{gameObject.name} was used. Active: {_isActive}");

        // Add custom behavior here:
        // - Open a door (via animation)
        // - Turn on a generator (enable lights)
        // - Activate a crafting table
        // etc.
    }

    protected override void OnFocus()
    {
        // Highlight visuals, outline, etc.
        // Example: HighlightManager.Instance.HighlightObject(gameObject);
    }

    protected override void OnLoseFocus()
    {
        // Disable highlight
        // Example: HighlightManager.Instance.RemoveHighlight(gameObject);
    }
}
