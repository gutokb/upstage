using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Content.Debuffs;

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
            Projectile.ai[0] = 0;

        }

        public override void AI()
        {

            if (Projectile.ai[0] == 1)
            {
                Projectile.Kill();
            }
            Player owner = Main.player[Projectile.owner];

            foreach (Player other in Main.player)
            {
                if (!other.dead && other.whoAmI != owner.whoAmI)
                {
                    if (Projectile.Hitbox.Intersects(other.Hitbox) && Projectile.ai[0] == 0)
                    {
                        other.Heal(healAmount);
                        owner.AddBuff(ModContent.BuffType<HealingDebuff>(), 3600);
                        Projectile.ai[0] = 5;
                    }
                }
            }
            if(Projectile.ai[0] > 1)
            {
                Projectile.ai[0]--;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

    }
}