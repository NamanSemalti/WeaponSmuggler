using UnityEngine;

public class LockedLootContainer : LootContainer
{
    [Header("Lock Settings")]
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private string requiredKeyID; // Matches an inventory item ID
    [SerializeField] private bool requiresSkill = false;
    [SerializeField, Range(0, 10)] private int requiredLockpickLevel = 2;
    [SerializeField] private bool allowForcedUnlock = false;
    [SerializeField] private float forcedUnlockChance = 0.3f;
    [SerializeField] private AudioClip lockedSound;
    [SerializeField] private AudioClip unlockSuccessSound;
    [SerializeField] private AudioClip unlockFailSound;

    private bool _isUnlocked = false;

    protected override void OnInteract()
    {
        if (_isOpened) return; // already opened
        if (_isUnlocked)
        {
            base.OnInteract(); // Call LootContainer OnInteract
            return;
        }

        // Check Key
        if (requiresKey && !HasRequiredKey())
        {
            PlayLockedSound();
            UIManager.Instance.ShowMessage($"🔒 Requires Key: {requiredKeyID}");
            return;
        }

        // Check Skill
        if (requiresSkill && !HasRequiredSkill())
        {
            PlayLockedSound();
            UIManager.Instance.ShowMessage($"🔒 Requires Lockpick Lv.{requiredLockpickLevel}");
            return;
        }

        // Force Unlock Attempt
        if (allowForcedUnlock)
        {
            TryForcedUnlock();
            return;
        }

        // If all conditions pass
        UnlockContainer();
    }

    private bool HasRequiredKey()
    {
        // Placeholder: integrate with your InventoryManager later
        // Example: return InventoryManager.Instance.HasItem(requiredKeyID);
        Debug.Log($"Checking for key: {requiredKeyID}");
        return false;
    }

    private bool HasRequiredSkill()
    {
        // Placeholder: integrate with PlayerStats/SkillManager
        // Example: return PlayerStats.Instance.LockpickLevel >= requiredLockpickLevel;
        int playerSkillLevel = 1; // test placeholder
        return playerSkillLevel >= requiredLockpickLevel;
    }

    private void TryForcedUnlock()
    {
        float roll = Random.value;
        if (roll <= forcedUnlockChance)
        {
            UIManager.Instance.ShowMessage("🔓 Forced unlock successful!");
            PlaySound(unlockSuccessSound);
            UnlockContainer();
        }
        else
        {
            // InteractionUI.ShowMessage("❌ Lockpick failed! You made noise...");
            PlaySound(unlockFailSound);

            // Optional: increase heat or alert nearby AI
            // HeatManager.Instance.AddHeat(5);
        }
    }

    private void UnlockContainer()
    {
        _isUnlocked = true;
        PlaySound(unlockSuccessSound);
        UIManager.Instance.ShowMessage("✅ Container Unlocked!");
        base.OnInteract(); // Opens normally
    }

    private void PlayLockedSound()
    {
        PlaySound(lockedSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip)
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
