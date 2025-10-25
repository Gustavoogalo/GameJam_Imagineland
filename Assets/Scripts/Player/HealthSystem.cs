using System;
using UnityEngine;

namespace Player
{
    public class HealthSystem : MonoBehaviour, HSI_Damageable
    {
        [SerializeField] private int maxHealth = 10;
        [SerializeField] private int health = 10;
        private bool isInvincible = false;
        private bool isDead = false;
        private bool isInterruptible = true;
        private bool isBlocking = false;

        public event Action<HealthSystem> OnDeath;

        // public event Action<HSE_DamageResponse> OnDamageResponseReceived;
        public event Action<HSS_DamageInfo> OnDamageReceived;
        public event Action<int, int> OnHealthChanged;


        public bool IsDead => isDead;

        public int CurrentHealth => health;

        public int MaxHealth => maxHealth;


        public int Heal(int amount)
        {
            if (isDead)
            {
                return health;
            }

            health = Mathf.Clamp(health + amount, 0, maxHealth);
            OnHealthChanged?.Invoke(health, maxHealth);
            return health;
        }

        public bool TakeDamage(HSS_DamageInfo damageInfo)
        {
            if (!CanBeDamaged(damageInfo))
            {
                return false;
            }

            health -= damageInfo.Amount;
            OnHealthChanged?.Invoke(health, maxHealth);
            OnDamageReceived?.Invoke(damageInfo);

            if (health <= 0)
            {
                health = 0;
                Die();
            }
            // else
            // {
            //     if (damageInfo.ShouldForceInterrupt) { OnDamageResponseReceived?.Invoke(damageInfo.DamageResponse); }
            // }

            return true;
        }

        private void Die()
        {
            if (IsDead)
            {
                return;
            }

            isDead = true;
            Debug.Log($"{gameObject.name} morreu");

            OnDeath?.Invoke(this);
        }

        public bool CanBeDamaged(HSS_DamageInfo damageInfo)
        {
            if (isDead)
            {
                return false;
            }

            if (isInvincible && !damageInfo.ShouldDamageInvincible)
            {
                return false;
            }

            if (isBlocking && damageInfo.CanBeBlocked)
            {
                return false;
            }

            return true;
        }

        public void Revive()
        {
            isDead = false;
        }

        public void SetInvincible(bool state) => isInvincible = state;
        public void SetBlocking(bool state) => isBlocking = state;
    }
}