using UnityEngine;

public class DestinationDisplay : MonoBehaviour
{
    [SerializeField] private IcarusLevelManager levelManager;
    [SerializeField] private SpriteRenderer displayRenderer;

    private void Start()
    {
        UpdateDisplay();
        levelManager.OnLevelChanged.AddListener(UpdateDisplay);
    }

    private void UpdateDisplay()
    {
        displayRenderer.sprite = levelManager.CurrentLevel.LevelIcon;
    }
}
