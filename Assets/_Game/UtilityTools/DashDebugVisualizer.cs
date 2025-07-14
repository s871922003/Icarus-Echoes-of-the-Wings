using UnityEngine;

public class DashDebugVisualizer : MonoBehaviour
{
    public CompanionAIContext AIContext;
    public float DashDistance = 6f;
    public Color DashColor = new Color(1f, 0.2f, 0f, 0.4f);

    private void OnDrawGizmos()
    {
        if (AIContext == null || AIContext.OwnerCharacter == null) return;

        Gizmos.color = DashColor;
        Gizmos.DrawWireSphere(AIContext.OwnerCharacter.transform.position, DashDistance);
    }
}
