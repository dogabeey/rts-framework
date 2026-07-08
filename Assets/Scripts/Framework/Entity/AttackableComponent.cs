using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Game.Entity
{
    public class AttackableComponent : MonoBehaviour
    {
        public float damage;
        public DamageType damageType;
        public float range;
        public float attackCooldown;
        public AttackStrategy attackStrategy;

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
