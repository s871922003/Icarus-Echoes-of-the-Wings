using MoreMountains.TopDownEngine;
using UnityEngine;
using MoreMountains.Tools;


public class DamageAndMarkOnTouch : DamageOnTouch
{
    protected override void OnCollideWithDamageable(Health health)
    {

        _collidingHealth = health;

        if (health.CanTakeDamageThisFrame())
        {
            // if what we're colliding with is a TopDownController, we apply a knockback force
            _colliderTopDownController = health.gameObject.MMGetComponentNoAlloc<TopDownController>();
            if (_colliderTopDownController == null)
            {
                _colliderTopDownController = health.gameObject.GetComponentInParent<TopDownController>();
            }

            HitDamageableFeedback?.PlayFeedbacks(this.transform.position);
            HitDamageableEvent?.Invoke(_colliderHealth);

            // we apply the damage to the thing we've collided with
            float randomDamage =
                UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));

            ApplyKnockback(randomDamage, TypedDamages);

            DetermineDamageDirection();

            if (RepeatDamageOverTime)
            {
                _colliderHealth.DamageOverTime(randomDamage, gameObject, InvincibilityDuration,
                    InvincibilityDuration, _damageDirection, TypedDamages, AmountOfRepeats, DurationBetweenRepeats,
                    DamageOverTimeInterruptible, RepeatedDamageType);
            }
            else
            {
                _colliderHealth.Damage(randomDamage, gameObject, InvincibilityDuration, InvincibilityDuration,
                    _damageDirection, TypedDamages);
            }

            MarkTargetPosition(health);
        }

        // we apply self damage
        if (DamageTakenEveryTime + DamageTakenDamageable > 0 && !_colliderHealth.PreventTakeSelfDamage)
        {
            SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
        }        
    }

    private void MarkTargetPosition(Health health)
    {
        //// Ensure CIWhiteboard exists
        //if (CIWhiteboard.Instance == null)
        //{
        //    Debug.LogWarning("CIWhiteboard Instance is missing in the scene.");
        //    return;
        //}

        //// Ensure the target is within TargetLayerMask
        //int targetLayer = health.gameObject.layer;
        //if (((1 << targetLayer) & TargetLayerMask) != 0)
        //{
        //    // Mark the enemy's position
        //    CIWhiteboard.Instance.AddMarkedTarget(health.transform.position);
        //}
    }
}
