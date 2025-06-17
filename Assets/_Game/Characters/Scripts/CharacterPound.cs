using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Pound 2D")]
    public class CharacterPound : CharacterAbility
    {
        [Header("Pound Settings")]
        public float PoundSpeed = 30f;
        public float MaxPoundDistance = 8f;
        public AnimationCurve PoundCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        protected bool _pounding;
        protected float _poundTimer;
        protected float _poundDuration;
        protected Vector3 _poundOrigin;
        protected Vector3 _poundDestination;
        protected Vector3 _poundAnimDirection;

        protected CharacterOrientation2D _orientation2D;
        protected CharacterOrientation2D.FacingModes _originalFacingMode;

        public bool IsPoundInProgress => _pounding;

        /// <summary>
        /// Starts a Pound toward a target position.
        /// </summary>
        public virtual void TriggerPound(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.z = 0f;

            if (direction.sqrMagnitude < 0.01f)
                return;

            float distance = Mathf.Min(direction.magnitude, MaxPoundDistance);

            _poundOrigin = transform.position;
            _poundDestination = transform.position + direction.normalized * distance;
            _poundAnimDirection = direction.normalized;

            _poundDuration = distance / PoundSpeed;
            _poundTimer = 0f;
            _pounding = true;

            _controller.FreeMovement = false;
            _movement.ChangeState(CharacterStates.MovementStates.Dashing);

            _orientation2D = _character.FindAbility<CharacterOrientation2D>();
            if (_orientation2D != null)
            {
                _originalFacingMode = _orientation2D.FacingMode;
                _orientation2D.FacingMode = CharacterOrientation2D.FacingModes.None;
                _orientation2D.Face((_poundAnimDirection.x >= 0f) ? Character.FacingDirections.East : Character.FacingDirections.West);
            }
        }

        /// <summary>
        /// Stops the Pound and resets orientation.
        /// </summary>
        protected virtual void StopPound()
        {
            _pounding = false;
            _controller.FreeMovement = true;
            _movement.ChangeState(CharacterStates.MovementStates.Idle);

            _controller.SetMovement(Vector3.zero);
            _controller.CurrentDirection = Vector3.zero;

            if (_orientation2D != null)
            {
                _orientation2D.FacingMode = _originalFacingMode;
                var finalFacing = (_poundAnimDirection.x >= 0f) ? Character.FacingDirections.East : Character.FacingDirections.West;
                _orientation2D.Face(finalFacing);
            }
        }

        public void ForceStopPound()
        {
            if (_pounding)
            {
                StopPound();
            }
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (!_pounding)
                return;

            if (_poundTimer < _poundDuration)
            {
                float progress = _poundTimer / _poundDuration;
                Vector3 newPosition = Vector3.Lerp(_poundOrigin, _poundDestination, PoundCurve.Evaluate(progress));
                _poundTimer += Time.deltaTime;
                _controller.MovePosition(newPosition);
            }
            else
            {
                StopPound();
            }
        }

        public virtual void ClearPoundTarget()
        {
            _pounding = false;
            _poundOrigin = transform.position;
            _poundDestination = transform.position;
            _poundTimer = 0f;

            _controller.FreeMovement = true;
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _controller.SetMovement(Vector3.zero);
        }
    }
}
