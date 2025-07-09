using MoreMountains.TopDownEngine;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "CommandAction/DashBodySlam")]
public class DashCommandAction : CommandAction
{
    [Header("�Ĩ�]�w")]
    public float DashDistance = 6f;
    public float DashSpeed = 15f;
    //public AnimationCurve DashCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("�N�o����")]
    [SerializeField] private float ReturnThreshold = 0.1f;

    private bool _canExecute = true;
    private CharacterMovement _characterMovement;

    public override bool CanExecute()
    {
        if (!_canExecute) return false;

        if (AIContext?.OwnerCharacter == null)
        {
            Debug.LogWarning("[DashCommandAction] �ʤ� OwnerCharacter");
            return false;
        }

        float distance = Vector3.Distance(AIContext.transform.position, AIContext.OwnerCharacter.transform.position);
        return distance < ReturnThreshold;
    }

    public override void Execute()
    {
        AIContext.CanDoDefaultBehavior = false;
        _canExecute = false;

        Vector3 origin = AIContext.transform.position;
        Vector3 targetPos = GetMouseWorldPosition();
        Vector3 direction = (targetPos - origin).normalized;

        DamageOnTouch damageOnTouch = AIContext.GetComponentInChildren<DamageOnTouch>(true);
        if (damageOnTouch != null)
        {
            damageOnTouch.gameObject.SetActive(true);
        }

        _characterMovement = AIContext.GetComponent<Character>()?.FindAbility<CharacterMovement>();
        if (_characterMovement == null)
        {
            Debug.LogWarning("[DashCommandAction] �L�k��� CharacterMovement");
            return;
        }

        AIContext.CommandExecutor.StartCoroutine(DashRoutine(direction, damageOnTouch));
    }

    private IEnumerator DashRoutine(Vector3 direction, DamageOnTouch damageOnTouch)
    {
        float distanceTraveled = 0f;
        float maxDuration = DashDistance / DashSpeed * 2f; // �O�@�ɶ�
        float timer = 0f;

        _characterMovement.ScriptDrivenInput = true;
        _characterMovement.SetMovement(Vector2.zero);
        _characterMovement.MovementSpeed = 0f;

        Vector3 previousPosition = AIContext.transform.position;

        while (distanceTraveled < DashDistance && timer < maxDuration)
        {
            _characterMovement.MovementSpeed = DashSpeed;
            _characterMovement.SetMovement(direction);

            yield return null;

            Vector3 currentPosition = AIContext.transform.position;
            distanceTraveled += Vector3.Distance(currentPosition, previousPosition);
            previousPosition = currentPosition;
            timer += Time.deltaTime;
        }

        if (damageOnTouch != null)
        {
            damageOnTouch.gameObject.SetActive(false);
        }

        _characterMovement.SetMovement(Vector2.zero);
        _characterMovement.ScriptDrivenInput = false;
        _characterMovement.ResetSpeed();

        AIContext.CanDoDefaultBehavior = true;
        ResetExecution();
    }


    public override void UpdateCooldown(float deltaTime) { }

    public void ResetExecution()
    {
        _canExecute = true;
        Debug.Log("[DashCommandAction] �N�o���m�A���\�A������");
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPosition = InputManager.Instance.MousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = AIContext.transform.position.z;
        return worldPosition;
    }
}
