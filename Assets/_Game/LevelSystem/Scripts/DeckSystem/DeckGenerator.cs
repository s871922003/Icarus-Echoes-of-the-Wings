using UnityEngine;

public class DeckGenerator : MonoBehaviour
{
    [Tooltip("Deck 模組會以這個 Transform 為原點生成")]
    public Transform DeckAnchor;

    public void GenerateDeck()
    {
        var config = DeckConfigurationManager.Instance.CurrentConfiguration;

        foreach (var module in config.Modules)
        {
            if (module.Prefab == null) continue;

            var instance = Instantiate(module.Prefab, DeckAnchor);
            instance.transform.localPosition = new Vector3(module.GridPosition.x, module.GridPosition.y, 0);
        }
    }

    private void Start()
    {
        GenerateDeck();
    }
}
