using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;
using upstage.Content.Projectiles;

namespace upstage.Content.Buffs
{
    public class NaturalSpikesP : ModPlayer
    {
        public bool spiking = false;
        
  

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!spiking || Player.whoAmI != Main.myPlayer) return; // only the owner spawns projectiles

            int count = 6;
            int spikeDamage = 15;
            

            for (int i = 0; i < count; i++)
            {
                // Evenly spaced ring with a bit of randomness
                float angle = (MathHelper.Pi / count * i + Main.rand.NextFloat(-0.2f, 0.2f))+MathHelper.Pi;
                Vector2 pos = Player.Center + angle.ToRotationVector2() * Main.rand.NextFloat(30f, 60f);

                Projectile.NewProjectile(Player.GetSource_Misc("Natural spikes"), pos, (pos - Player.Center)/15,
                    ModContent.ProjectileType<NaturalSpike>(),
                    spikeDamage, 2f, Player.whoAmI, ai0: angle);
            }
        }


        public override void ResetEffects()
        {
            spiking = false;
        }
    }
	public class NaturalSpikes : ModBuff, IBannerBuff
	{
	    public int BannerColor => 0;

		public override void Update(Player player, ref int buffIndex) {
            NaturalSpikesP sp = player.GetModPlayer<NaturalSpikesP>();
            sp.spiking = true;
		}
	}
}