using UnityEngine;

[CreateAssetMenu(menuName = "Level/LevelData")]
public class LevelData : ScriptableObject
{
    public string LevelName;
    public Sprite LevelIcon;
    public string SceneName;
    public string LevelDescription;
}
