using UnityEngine;
using UnityEngine.InputSystem;

public class InspectableItem : Interactable
{
    [Header("Inspect Settings")]
    [SerializeField] private float inspectDistance = 2f;
    [SerializeField] private float rotationSpeed = 80f;
    [SerializeField] private float zoomInFOV = 40f;
    [SerializeField] private float transitionSpeed = 5f;
    [SerializeField] private Transform inspectAnchor; // empty object in front of camera
    [SerializeField] private AudioClip inspectStartSound;
    [SerializeField] private AudioClip inspectEndSound;

    private Camera _mainCamera;
    private bool _isInspecting = false;
    private bool _canRotate = false;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private float _defaultFOV;

    private PlayerInteractor _playerInteractor;

    protected override void Awake()
    {
        base.Awake();
        _mainCamera = Camera.main;
    }

    protected override void OnInteract()
    {
        if (_isInspecting)
        {
            EndInspect();
        }
        else
        {
            StartInspect();
        }
    }

    private void StartInspect()
    {
        if (_mainCamera == null) return;
        if (_isInspecting) return;

        _isInspecting = true;
        _playerInteractor = FindObjectOfType<PlayerInteractor>();
        if (_playerInteractor != null)
            _playerInteractor.SetInspecting(true);

        // Store transform info
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _defaultFOV = _mainCamera.fieldOfView;

        // Optional sound
        if (inspectStartSound)
            AudioSource.PlayClipAtPoint(inspectStartSound, transform.position);

        // Move object to inspect anchor smoothly
        StartCoroutine(MoveToInspectPosition());

        // Notify systems
        InteractionEvents.TriggerInspect(this);
    }

    private System.Collections.IEnumerator MoveToInspectPosition()
    {
        float t = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(startPos, inspectAnchor.position, t);
            transform.rotation = Quaternion.Slerp(startRot, inspectAnchor.rotation, t);
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, zoomInFOV, t);
            yield return null;
        }

        _canRotate = true;
    }

    private void Update()
    {
        if (!_isInspecting || !_canRotate) return;

        // Rotate object using mouse
        float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Rotate(_mainCamera.transform.up, -rotX, Space.World);
        transform.Rotate(_mainCamera.transform.right, rotY, Space.World);

        // Exit inspection
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            EndInspect();
        }
    }

    private void EndInspect()
    {
        if (!_isInspecting) return;

        _isInspecting = false;
        _canRotate = false;

        if (_playerInteractor != null)
            _playerInteractor.SetInspecting(false);

        // Optional sound
        if (inspectEndSound)
            AudioSource.PlayClipAtPoint(inspectEndSound, transform.position);

        // Restore position + FOV
        StartCoroutine(ReturnToOriginalPosition());
    }

    private System.Collections.IEnumerator ReturnToOriginalPosition()
    {
        float t = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float startFOV = _mainCamera.fieldOfView;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(startPos, _originalPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, _originalRotation, t);
            _mainCamera.fieldOfView = Mathf.Lerp(startFOV, _defaultFOV, t);
            yield return null;
        }
    }
}
