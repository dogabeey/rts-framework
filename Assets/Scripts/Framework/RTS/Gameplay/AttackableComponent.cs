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
        
        [SerializeField] private Renderer weaponRenderer;
        [SerializeField] private float damage;
        [SerializeField] private DamageType damageType;
        [SerializeField] private float range;
        [SerializeField] private float attackCooldown;
        [SerializeField, SerializeReference] private AttackStrategy attackStrategy;

        private float nextAttackTime;

        /// <summary>Attempts one complete attack, including range, cooldown and damage checks.</summary>
        public bool TryAttack(DamageableComponent target)
        {
            if (!CanAttack(target) || Time.time < nextAttackTime)
            {
                return false;
            }

            nextAttackTime = Time.time + Mathf.Max(0f, attackCooldown);
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
