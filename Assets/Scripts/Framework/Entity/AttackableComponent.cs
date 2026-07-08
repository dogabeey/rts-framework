using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Game.Entity
{
    public class AttackableComponent : MonoBehaviour
    {
        float Damage { get; }
        DamageType AttackType { get; }
        float Range { get; }
        float AttackCooldown { get; }
        AttackStrategy AttackStrategy { get; }

        public void Attack(DamageableComponent target)
        {
            AttackStrategy.Attack(target);
        }

        private bool CanAttack(DamageableComponent target)
        {
            return AttackStrategy.CanAttack(target);
        }
    }
}
