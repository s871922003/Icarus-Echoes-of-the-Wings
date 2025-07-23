using UnityEngine;

[ExecuteInEditMode]
public class SpawnPointGenerator : MonoBehaviour
{
    public Camera referenceCamera;
    public float margin = 2f;
    public Transform spawnRoot;

    [ContextMenu("Generate Spawn Points")]
    public void GeneratePoints()
    {
        if (referenceCamera == null)
        {
            referenceCamera = Camera.main;
        }

        if (spawnRoot == null)
        {
            Debug.LogWarning("請指定 spawnRoot。");
            return;
        }

        // 清除舊的
        for (int i = spawnRoot.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(spawnRoot.GetChild(i).gameObject);
        }

        Vector3 camCenter = referenceCamera.transform.position;
        float camHeight = 2f * referenceCamera.orthographicSize;
        float camWidth = camHeight * referenceCamera.aspect;

        Vector2[] directions = new Vector2[]
        {
            new Vector2(-1,  1), new Vector2(0,  1), new Vector2(1,  1),
            new Vector2( 1,  0), new Vector2(1, -1), new Vector2(0, -1),
            new Vector2(-1, -1), new Vector2(-1, 0),
        };

        string[] names = { "NW", "N", "NE", "E", "SE", "S", "SW", "W" };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2 dir = directions[i].normalized;
            Vector3 offset = new Vector3(dir.x * (camWidth / 2f + margin), dir.y * (camHeight / 2f + margin), 0);

            GameObject point = new GameObject($"SpawnPoint_{names[i]}");
            point.transform.SetParent(spawnRoot);
            point.transform.position = camCenter + offset;
        }

        Debug.Log("生成完成。");
    }
}
