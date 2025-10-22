using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayerMask;
    [SerializeField] private Camera playerCamera;

    private Interactable _currentTarget;
    private StarterAssetsInputs _input;  // From Starter Assets
    private bool _isInspecting = false;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        _input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (_isInspecting) return; // Prevent raycasting during inspection

        DetectInteractable();
        if (_currentTarget != null && _input.interact) // "E" key mapped in StarterAssetsInputs
        {
            _currentTarget.Interact();
            // ClearFocus();
            _input.interact = false; // Reset input to avoid spamming
        }
    }

    private void DetectInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayerMask))
        {
            // Try to find the root interactable (handles child colliders)
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();

            if (interactable != null)
            {
                // ✅ Only trigger if we're looking at a *new* interactable
                if (_currentTarget == null || _currentTarget.GetInstanceID() != interactable.GetInstanceID())
                {
                    ClearFocus(); // remove focus from previous
                    _currentTarget = interactable;
                    _currentTarget.SetFocus(transform);
                    Debug.Log("Hitting " + interactable.name);
                    // 🔥 Trigger only once when focus changes
                    InteractionEvents.TriggerFocus(_currentTarget);
                }
                return; // still looking at an interactable, no need to clear
            }
        }

        // ✅ If no interactable detected, clear focus once
        ClearFocus();
    }


    private void ClearFocus()
    {
        if (_currentTarget != null)
        {
            _currentTarget.ClearFocus();
            InteractionEvents.TriggerLoseFocus(_currentTarget);
            _currentTarget = null;
        }
    }

    public void SetInspecting(bool isInspecting)
    {
        _isInspecting = isInspecting;
    }
}
