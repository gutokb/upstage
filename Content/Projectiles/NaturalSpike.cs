using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class NaturalSpike : ModProjectile
{
    
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.JungleSpike;

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.JungleSpike); // copies size, AI style, gravity, etc.
        AIType = ProjectileID.JungleSpike;                  // use the vanilla movement AI

        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Generic;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Poisoned, 300);
    }
}