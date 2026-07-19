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
        
        [SerializeField] private float damage;
        [SerializeField] private DamageType damageType;
        [SerializeField] private float range;
        [SerializeField] private float attackCooldown;
        [SerializeField, SerializeReference] private AttackStrategy attackStrategy;

        public void Attack(DamageableComponent target)
        {
            attackStrategy.Attack(target);

            EventParam attackParam = new EventParam();
            attackParam.Set(EventParam.Keys.GameObject, gameObject);
            attackParam.Set("entityController", referenceEntity);
            attackParam.Set("target", target);
            attackParam.Set("targetGameObject", target != null ? target.gameObject : null);
            attackParam.Set("damage", damage);
            attackParam.Set("damageType", damageType);
            EventManager.TriggerEvent(GameEvent.ENTITY_ATTACKED, attackParam);
        }

        private bool CanAttack(DamageableComponent target)
        {
            return attackStrategy.CanAttack(target);
        }
    }
}
