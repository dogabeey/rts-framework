using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Game.EventManagement;

namespace Game.RTS
{
    public class DamageableComponent : MonoBehaviour
    {
        private static readonly HashSet<DamageableComponent> activeDamageables = new HashSet<DamageableComponent>();
        public static IReadOnlyCollection<DamageableComponent> All => activeDamageables;
        public float MaxHealth => maxHealth;
        public ArmorType ArmorType => armorType;
        public float HitBox => hitBox;
        public bool IsDead => isDead;

        [ReadOnly] public  EntityController referenceEntity;

        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private  ArmorType armorType;
        [SerializeField] private  float hitBox = 1f;

        [ReadOnly]
        public float currentHealth;

        private bool isDead;

        private void Awake()
        {
            currentHealth = maxHealth;
            isDead = false;
        }

        private void OnEnable() => activeDamageables.Add(this);
        private void OnDisable() => activeDamageables.Remove(this);

        public void TakeDamage(float amount)
        {
            TakeDamage(null, amount);
        }

        public void TakeDamage(DamageType damageType, float amount)
        {
            if (isDead || amount <= 0f)
            {
                return;
            }

            // Implement damage calculation based on armor type and hitbox
            float effectiveDamage = CalculateEffectiveDamage(damageType, amount);
            // Apply the effective damage to the entity's health
            ApplyDamage(effectiveDamage);
        }

        private void ApplyDamage(float effectiveDamage)
        {
            float previousHealth = currentHealth;
            currentHealth -= effectiveDamage;
            currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0

            EventParam damageParam = new EventParam();
            damageParam.Set(EventParam.Keys.GameObject, gameObject);
            damageParam.Set("entityController", referenceEntity);
            damageParam.Set("damage", effectiveDamage);
            damageParam.Set("previousHealth", previousHealth);
            damageParam.Set("currentHealth", currentHealth);
            EventManager.TriggerEvent(GameEvent.ENTITY_DAMAGED, damageParam);

            if (currentHealth == 0)
            {
                // Handle entity death logic here
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            if (isDead)
            {
                return;
            }

            isDead = true;
            referenceEntity?.SetMissionState(EntityMissionType.Sleep, true);

            EventParam diedParam = new EventParam();
            diedParam.Set(EventParam.Keys.GameObject, gameObject);
            diedParam.Set("entityController", referenceEntity);
            diedParam.Set("finalHealth", currentHealth);
            EventManager.TriggerEvent(GameEvent.ENTITY_DIED, diedParam);
        }

        private float CalculateEffectiveDamage(DamageType damageType, float amount)
        {
            if (damageType != null && damageType.DamageModifiers != null
                && damageType.DamageModifiers.TryGetValue(armorType, out var modifier))
            {
                return Mathf.Max(0f, amount * modifier);
            }

            return amount;
        }
    }
}
