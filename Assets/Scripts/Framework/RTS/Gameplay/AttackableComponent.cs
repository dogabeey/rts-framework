using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using Game.EventManagement;

namespace Game.RTS
{
    public class AttackableComponent : MonoBehaviour
    {
        public float Damage => damage;
        public DamageType DamageType => damageType;
        public float Range => range;
        public float AttackCooldown => attackCooldown;
        public AttackStrategy AttackStrategy => attackStrategy;

        [ReadOnly] public  EntityController referenceEntity;
        
        [SerializeField] private Transform facingTransform;
        [SerializeField] private ParticleSystem weaponMuzzleFlash;
        [SerializeField] private Transform weaponMuzzlePosition;
        [SerializeField] private float damage;
        [SerializeField] private DamageType damageType;
        [SerializeField] private float range;
        [SerializeField] private float attackCooldown;
        [SerializeField, SerializeReference] private AttackStrategy attackStrategy;

        private float nextAttackTime;

        /// <summary>Attempts one complete attack, including range, cooldown and damage checks.</summary>
        public bool TryAttack(DamageableComponent target)
        {
            FaceTarget(target);
            if (!CanAttack(target) || Time.time < nextAttackTime)
            {
                return false;
            }

            nextAttackTime = Time.time + Mathf.Max(0f, attackCooldown);
            PlayRangedMuzzleFlash();
            target.TakeDamage(damageType, damage);
            attackStrategy?.Attack(target);

            EventParam attackParam = new EventParam();
            attackParam.Set(EventParam.Keys.GameObject, gameObject);
            attackParam.Set("entityController", referenceEntity);
            attackParam.Set("target", target);
            attackParam.Set("targetGameObject", target != null ? target.gameObject : null);
            attackParam.Set("damage", damage);
            attackParam.Set("damageType", damageType);
            EventManager.TriggerEvent(GameEvent.ENTITY_ATTACKED, attackParam);
            return true;
        }

        /// <summary>Rotates the assigned visual independently of navigation to face the current target.</summary>
        public void FaceTarget(DamageableComponent target)
        {
            if (facingTransform == null || target == null)
            {
                return;
            }

            var direction = target.transform.position - facingTransform.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f)
            {
                facingTransform.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }
        }

        private void PlayRangedMuzzleFlash()
        {
            if (!(attackStrategy is RangedAttackStrategy) || weaponMuzzleFlash == null)
            {
                return;
            }

            if (weaponMuzzlePosition != null)
            {
                weaponMuzzleFlash.transform.SetPositionAndRotation(
                    weaponMuzzlePosition.position,
                    weaponMuzzlePosition.rotation);
            }

            weaponMuzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            weaponMuzzleFlash.Play(true);
        }

        // Kept for callers that used the original API. New gameplay code should use TryAttack.
        public void Attack(DamageableComponent target)
        {
            TryAttack(target);
        }

        public bool CanAttack(DamageableComponent target)
        {
            if (target == null || target.IsDead || target == GetComponent<DamageableComponent>())
            {
                return false;
            }

            var rangeWithHitBox = Mathf.Max(0f, range) + Mathf.Max(0f, target.HitBox);
            var offset = target.transform.position - transform.position;
            offset.y = 0f;
            if (offset.sqrMagnitude > rangeWithHitBox * rangeWithHitBox)
            {
                return false;
            }

            return attackStrategy == null || attackStrategy.CanAttack(target);
        }
    }
}
