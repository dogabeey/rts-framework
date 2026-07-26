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
        public float TargetScanRange => range;
        public float AttackCooldown => attackCooldown;
        public AttackStrategy AttackStrategy => attackStrategy;

        [ReadOnly] public  EntityController referenceEntity;
        
        [SerializeField] private Transform facingTransform;
        [SerializeField] private ParticleSystem weaponMuzzleFlash;
        [SerializeField] private Transform weaponMuzzlePosition;
        [SerializeField] private float damage;
        [SerializeField] private DamageType damageType;
        [SerializeField, Tooltip("Maximum distance at which AI can acquire a target; attack distance is defined by the attack strategy.")]
        private float range;
        [SerializeField] private float attackCooldown;
        [SerializeField, SerializeReference] private AttackStrategy attackStrategy;

        private float nextAttackTime;

        private void Awake()
        {
            EnsureAttackStrategy();
        }

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
            attackStrategy.Attack(target);

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

            EnsureAttackStrategy();
            return attackStrategy.CanAttack(target);
        }

        internal void ApplyDamage(DamageableComponent target)
        {
            if (target != null && !target.IsDead)
            {
                target.TakeDamage(damageType, damage);
            }
        }

        private void EnsureAttackStrategy()
        {
            // Preserve existing prefabs that were created before strategies were required.
            if (attackStrategy == null)
            {
                attackStrategy = new MeleeAttackStrategy();
            }

            attackStrategy.Initialize(this);
        }
    }
}
