using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Game.EventManagement;

namespace Game.RTS
{
    public class DamageableComponent : MonoBehaviour
    {
        public float MaxHealth => maxHealth;
        public ArmorType ArmorType => armorType;
        public float HitBox => hitBox;

        [ReadOnly] public  EntityController referenceEntity;

        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private  ArmorType armorType;
        [SerializeField] private  float hitBox = 1f;

        [ReadOnly]
        public float currentHealth;

        private bool isDead;

        public void Start()
        {
            currentHealth = maxHealth;
            isDead = false;
        }

        public void TakeDamage(float amount)
        {
            // Implement damage calculation based on armor type and hitbox
            float effectiveDamage = CalculateEffectiveDamage(amount);
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

            EventParam diedParam = new EventParam();
            diedParam.Set(EventParam.Keys.GameObject, gameObject);
            diedParam.Set("entityController", referenceEntity);
            diedParam.Set("finalHealth", currentHealth);
            EventManager.TriggerEvent(GameEvent.ENTITY_DIED, diedParam);
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
