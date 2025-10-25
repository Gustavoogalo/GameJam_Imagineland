using UnityEngine;

namespace Player
{
    public enum HSE_DamageType
    {
        //tipo do dano causado
        None,
        Melee,
        StrongKnockback,
        Projectile,
        Explosion,
        Environment
    }

    public enum HSE_DamageResponse
    {
        None,
        HitReaction,
        Stagger,
        Stun,
        KnockBack,
        StrongKnockback
    }

    public struct HSS_DamageInfo
    {
        public int Amount { get; set; }
        public HSE_DamageType DamageType { get; set; }
        public HSE_DamageResponse DamageResponse { get; set; }
        public float KnockbackForce { get; set; } //Forca e Direcao do Knoback

        public bool ShouldDamageInvincible { get; set; } //invencibilidade enquanto da o rolamento por exemplo
        public bool CanBeBlocked { get; set; }
        public bool CanBeParried { get; set; }
        public bool ShouldForceInterrupt { get; set; }

        public HSS_DamageInfo(int amount, HSE_DamageType type = HSE_DamageType.Melee)
        {
            Amount = amount;
            DamageType = type;
            DamageResponse = HSE_DamageResponse.HitReaction;
            KnockbackForce = 0; //valor padrao
            ShouldDamageInvincible = true;
            CanBeBlocked = true;
            CanBeParried = true;
            ShouldForceInterrupt = true;
        }
    }

    public interface HSI_Damageable
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }
        int Heal(int amount);
        bool TakeDamage(HSS_DamageInfo damageInfo);
    }
}