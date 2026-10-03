using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Content.Debuffs;
using upstage.Common.ModUtils;

namespace upstage.Content.Projectiles
{
    public class CupidArrow : ModProjectile
    {
        
        public const int healAmount = 50;



        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(18); 
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.JestersArrow;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ownerHitCheck = true; 
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // AI runs on every client and on the server. Only the owner may decide that the arrow
            // connected, otherwise each machine heals the target once over.
            if (!PlayerUtils.IsLocalAuthority(Projectile.owner))
                return;

            Player owner = Main.player[Projectile.owner];

            foreach (Player other in Main.ActivePlayers)
            {
                if (!PlayerUtils.CanHeal(owner, other))
                    continue;

                if (!Projectile.Hitbox.Intersects(other.Hitbox))
                    continue;

                PlayerUtils.HealPlayer(other, healAmount);
                owner.AddBuff(ModContent.BuffType<HealingDebuff>(), 3600);

                // Kill() sends MessageID.KillProjectile, so the arrow disappears everywhere.
                Projectile.Kill();
                return;
            }
        }

    }
}