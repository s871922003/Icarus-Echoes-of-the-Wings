using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MoreMountains.TopDownEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace MoreMountains.TopDownEngine
{
    [TaskCategory("Custom/Character")]
    [TaskDescription("Triggers the CharacterDash2D ability toward a target or mouse position.")]
    public class PilotCharacterDash : Action
    {
        [Tooltip("The target Transform to dash toward (ignored if UseMousePosition is true)")]
        public SharedTransform TargetTransform;

        [Tooltip("If true, the dash will target the mouse position instead of the transform")]
        public SharedBool UseMousePosition;

        protected Character _character;
        protected CharacterDash2D _characterDash2D;

        public override void OnStart()
        {
            _character = GetComponent<Character>();
            _characterDash2D = _character?.FindAbility<CharacterDash2D>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_character == null || _characterDash2D == null)
            {
                Debug.LogWarning("[PilotCharacterDash] Missing Character or CharacterDash2D ability.");
                return TaskStatus.Failure;
            }

            if (!_characterDash2D.Cooldown.Ready())
            {
                return TaskStatus.Failure;
            }

            Vector3 dashDirection = Vector3.zero;

            if (UseMousePosition.Value)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = _character.transform.position.z;
                dashDirection = (mousePosition - _character.transform.position).normalized;
            }
            else if (TargetTransform.Value != null)
            {
                dashDirection = (TargetTransform.Value.position - _character.transform.position).normalized;
            }
            else
            {
                Debug.LogWarning("[PilotCharacterDash] No valid target provided.");
                return TaskStatus.Failure;
            }

            // 設定 Dash2D 的方向與模式
            _characterDash2D.DashMode = CharacterDash2D.DashModes.Script;
            _characterDash2D.DashDirection = dashDirection;

            // 觸發 Dash
            _characterDash2D.DashStart();

            return TaskStatus.Success;
        }
    }
}
