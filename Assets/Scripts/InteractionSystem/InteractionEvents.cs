using System;
using UnityEngine;

public static class InteractionEvents
{
    public static Action<Interactable> OnFocus;
    public static Action<Interactable> OnLoseFocus;
    public static Action<Interactable> OnPickup;
    public static Action<Interactable> OnInspect;
    public static event Action<Interactable> OnUse; // 👈 NEW

    public static void TriggerFocus(Interactable obj) => OnFocus?.Invoke(obj);
    public static void TriggerLoseFocus(Interactable obj) => OnLoseFocus?.Invoke(obj);
    public static void TriggerPickup(Interactable obj) => OnPickup?.Invoke(obj);
    public static void TriggerInspect(Interactable obj) => OnInspect?.Invoke(obj);
    public static void TriggerUse(Interactable obj) => OnUse?.Invoke(obj); // 👈 NEW
}
