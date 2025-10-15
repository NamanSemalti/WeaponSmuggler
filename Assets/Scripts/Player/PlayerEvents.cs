using System;
using UnityEngine;

public static class PlayerEvents
{
    public static event Action OnPlayerIdle;
    public static event Action OnPlayerWalk;
    public static event Action OnPlayerRun;
    public static event Action OnPlayerJump;

    public static void TriggerIdle() => OnPlayerIdle?.Invoke();
    public static void TriggerWalk() => OnPlayerWalk?.Invoke();
    public static void TriggerRun() => OnPlayerRun?.Invoke();
    public static void TriggerJump() => OnPlayerJump?.Invoke();
}

