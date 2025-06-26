using MoreMountains.TopDownEngine;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "CommandAction/DashBodySlam")]
public class DashCommandAction : CommandAction
{
    [Header("衝刺設定")]
    public float DashDuration = 0.2f;
    public float DashDistance = 6f;
    public AnimationCurve DashCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("冷卻條件")]
    [SerializeField] private float ReturnThreshold = 0.1f;

    private bool _canExecute = true;

    public override bool CanExecute()
    {
        if (!_canExecute) return false;

        if (AIContext?.OwnerCharacter == null)
        {
            Debug.LogWarning("[DashCommandAction] 缺少 OwnerCharacter");
            return false;
        }

        float distance = Vector3.Distance(AIContext.transform.position, AIContext.OwnerCharacter.transform.position);
        return distance < ReturnThreshold;
    }

    public override void Execute()
    {
        _canExecute = false;

        Vector3 origin = AIContext.transform.position;
        Vector3 targetPos = GetMouseWorldPosition();
        Vector3 direction = (targetPos - origin).normalized;
        Vector3 dashDestination = origin + direction * DashDistance;

        DamageOnTouch damageOnTouch = AIContext.GetComponentInChildren<DamageOnTouch>(true);
        if (damageOnTouch != null)
        {
            damageOnTouch.gameObject.SetActive(true);
        }

        AIContext.CommandExecutor.StartCoroutine(DashRoutine(origin, dashDestination, damageOnTouch));
    }

    private IEnumerator DashRoutine(Vector3 origin, Vector3 destination, DamageOnTouch damageOnTouch)
    {
        float timer = 0f;
        Transform characterTransform = AIContext.transform;

        while (timer < DashDuration)
        {
            float normalizedTime = timer / DashDuration;
            float ratio = DashCurve.Evaluate(normalizedTime);
            Vector3 newPos = Vector3.Lerp(origin, destination, ratio);
            characterTransform.position = newPos;

            timer += Time.deltaTime;
            yield return null;
        }

        if (damageOnTouch != null)
        {
            damageOnTouch.gameObject.SetActive(false);
        }

        ResetExecution();
    }

    public override void UpdateCooldown(float deltaTime) { }

    public void ResetExecution()
    {
        _canExecute = true;
        Debug.Log("[DashCommandAction] 冷卻重置，允許再次執行");
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPosition = InputManager.Instance.MousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = AIContext.transform.position.z;
        return worldPosition;
    }
}
