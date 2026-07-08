using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    public class DamageableComponent : MonoBehaviour
    {
        public float MaxHealth { get;}
        public ArmorType ArmorType { get; }
        public float HitBox { get; }


        [ReadOnly]
        public float currentHealth;

        public void TakeDamage(float amount)
        {
            // Implement damage calculation based on armor type and hitbox
            float effectiveDamage = CalculateEffectiveDamage(amount);
            // Apply the effective damage to the entity's health
            ApplyDamage(effectiveDamage);
        }

        private void ApplyDamage(float effectiveDamage)
        {
            currentHealth -= effectiveDamage;
            if (currentHealth <= 0)
            {
                // Handle entity death logic here
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            throw new NotImplementedException("Death handling logic is not implemented yet.");
        }

        private float CalculateEffectiveDamage(float amount)
        {
            // Implement damage calculation logic based on armor type and hitbox
            // For example, you can reduce the damage based on armor type and hitbox size
            float damageReduction = GetDamageReduction();
            return amount * (1 - damageReduction);
        }

        private float GetDamageReduction()
        {
            // Implement logic to determine damage reduction based on armor type and hitbox
            // For example, you can return a percentage reduction based on the armor type
            throw new NotImplementedException("Damage reduction calculation based on armor type and hitbox is not implemented yet.");
        }
    }
}
