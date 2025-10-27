using UnityEngine;

public class VehicleInteractable : Interactable
{
    [SerializeField] private Transform doorTransform;
    private bool isDoorOpened;
    protected override void OnInteract()
    {
        HandleDoorRotation();
    }

    private void HandleDoorRotation()
    {
        if (!isDoorOpened)
        {
            Vector3 currentRotation = doorTransform.eulerAngles;
            currentRotation.x = -90f;
            doorTransform.eulerAngles = currentRotation;
            isDoorOpened = true;
            OnFocus();
        }
        else
        {
            Vector3 currentRotation = doorTransform.eulerAngles;
            currentRotation.x = 0f;
            doorTransform.eulerAngles = currentRotation;
            isDoorOpened = false;
        }
    }
    protected override void OnFocus()
    {
        base.OnFocus();
        if (isDoorOpened)
        {
            SetDisplayUI(true);
        }
        else SetDisplayUI(false);
    }
    protected override void OnLoseFocus()
    {
        // SetDisplayUI(false);
    }
}
