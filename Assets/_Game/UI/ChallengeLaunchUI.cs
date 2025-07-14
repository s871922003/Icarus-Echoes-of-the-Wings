using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChallengeLaunchUI : MonoBehaviour
{
    [SerializeField] private IcarusLevelManager levelManager;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Button launchButton;

    private void Start()
    {
        UpdateLabel();
        levelManager.OnLevelChanged.AddListener(UpdateLabel);
        launchButton.onClick.AddListener(() => levelManager.StartChallenge());
    }

    private void UpdateLabel()
    {
        label.text = $"{levelManager.CurrentLevel.LevelName}";
    }

    private void Update()
    {
        gameObject.SetActive(!levelManager.ChallengeStarted);
    }
}
