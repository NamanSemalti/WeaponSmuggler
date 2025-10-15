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
            _input.interact = false; // Reset input to avoid spamming
        }
    }

    private void DetectInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayerMask))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                Debug.Log("hit " + hit.transform.name);
                if (_currentTarget != interactable)
                {
                    ClearFocus();
                    _currentTarget = interactable;
                    _currentTarget.SetFocus(transform);

                    // Notify UI system (decoupled)
                    InteractionEvents.TriggerFocus(interactable);
                }
            }
        }
        else
        {
            ClearFocus();
        }
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
