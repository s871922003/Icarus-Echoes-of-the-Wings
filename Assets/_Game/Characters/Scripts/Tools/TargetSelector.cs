using UnityEngine;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;


public class TargetSelector : MonoBehaviour
{
    [Header("Target Selection Settings")]
    [Tooltip("The layers considered valid targets.")]
    public LayerMask TargetLayerMask;

    [Tooltip("Radius to search for targets.")]
    public float SearchRadius = 5f;

    [Header("Debug Visualization")]
    [Tooltip("Show search radius gizmo in Scene view.")]
    public bool ShowGizmo = true;

    [Tooltip("Color of the search radius gizmo.")]
    public Color GizmoColor = new Color(1f, 1f, 0f, 0.2f);

    [Tooltip("Color of valid targets found.")]
    public Color TargetMarkerColor = Color.red;

    [HideInInspector]
    public List<Transform> CurrentTargetList = new List<Transform>();

    /// <summary>
    /// Searches for valid targets and selects the closest one.
    /// </summary>
    public Transform SearchForClosestTarget()
    {
        CurrentTargetList.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, SearchRadius, TargetLayerMask);

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.transform == this.transform) continue;

            if (!hit.transform.TryGetComponent<Health>(out var health)) continue;
            if (health.CurrentHealth <= 0) continue;

            CurrentTargetList.Add(hit.transform);

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = hit.transform;
            }
        }

        return closestTarget;
    }

    public Transform SearchAroundOwnerForTarget(Transform ownerTransform)
    {
        CurrentTargetList.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(ownerTransform.position, SearchRadius, TargetLayerMask);

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (!hit.transform.TryGetComponent<Health>(out var health)) continue;
            if (health.CurrentHealth <= 0) continue;

            CurrentTargetList.Add(hit.transform);

            float distance = Vector2.Distance(ownerTransform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = hit.transform;
            }
        }

        return closestTarget;
    }


    private void OnDrawGizmosSelected()
    {
        if (!ShowGizmo) return;

        Gizmos.color = GizmoColor;
        Gizmos.DrawWireSphere(transform.position, SearchRadius);

#if UNITY_EDITOR
        if (CurrentTargetList != null)
        {
            Gizmos.color = TargetMarkerColor;
            foreach (var target in CurrentTargetList)
            {
                if (target != null)
                {
                    Gizmos.DrawSphere(target.position, 0.15f);
                }
            }
        }
#endif
    }
}
