// MissionUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    public static MissionUI Instance { get; private set; }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Transform objectivesParent;
    [SerializeField] private GameObject objectivePrefab;

    private void Awake()
    {
        Instance = this;
        if (panelRoot) panelRoot.SetActive(false);
    }

    public void ShowMission(MissionDataSO mission)
    {
        if (mission == null) { Hide(); return; }

        if (panelRoot) panelRoot.SetActive(true);
        if (titleText) titleText.text = mission.missionName;
        if (descriptionText) descriptionText.text = mission.missionDescription;

        // clear old
        foreach (Transform t in objectivesParent) Destroy(t.gameObject);

        // populate
        foreach (var obj in mission.objectives)
        {
            var go = Instantiate(objectivePrefab, objectivesParent);
            var t = go.GetComponentInChildren<TMP_Text>();
            t.text = $"{obj.objectiveName}: {obj.currentAmount}/{obj.requiredAmount}";
        }
    }

    public void UpdateMissionUI(MissionDataSO mission)
    {
        if (mission == null) { Hide(); return; }
        ShowMission(mission); // re-render simply; cheap for MVP
    }

    public void Hide()
    {
        if (panelRoot) panelRoot.SetActive(false);
    }
}
