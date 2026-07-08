using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Game.Entity
{
    public class AttackableComponent : MonoBehaviour
    {
        public float Damage => damage;
        public DamageType DamageType => damageType;
        public float Range => range;
        public float AttackCooldown => attackCooldown;
        public AttackStrategy AttackStrategy => attackStrategy;

        [SerializeField] private float damage;
        [SerializeField] private DamageType damageType;
        [SerializeField] private float range;
        [SerializeField] private float attackCooldown;
        [SerializeField] private AttackStrategy attackStrategy;

        public void Attack(DamageableComponent target)
        {
            attackStrategy.Attack(target);
        }

        private bool CanAttack(DamageableComponent target)
        {
            return attackStrategy.CanAttack(target);
        }
    }
}
