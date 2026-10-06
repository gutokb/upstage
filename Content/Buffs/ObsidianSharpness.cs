using System.Runtime.Serialization;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Buffs
{
    public class ObsidianSharpnessP : ModPlayer
    {
        public bool ObsidianSharpnessing;

        public override void ResetEffects()
        {
            ObsidianSharpnessing = false;
            base.ResetEffects();
        }
    }
    public class ObsidianSharpness : ModBuff, IBannerBuff
    {
        public int BannerColor => 0;


        public override void Update(Player player, ref int buffIndex)
        {
            ObsidianSharpnessP ObsidianSharpnessplayer = player.GetModPlayer<ObsidianSharpnessP>();
            ObsidianSharpnessplayer.ObsidianSharpnessing = true;
        }
    }


    public class ObsidianSharpnessingGlobal : GlobalProjectile
    {
        // True only for projectiles a player's weapon actually fired
        private static bool IsPlayerShot(Projectile p) =>
            p.friendly && !p.hostile && p.owner >= 0 && p.owner < Main.maxPlayers
            && !p.minion && !p.sentry && p.aiStyle != ProjAIStyleID.Explosive; // tweak as needed

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!IsPlayerShot(projectile)) return;
            if (projectile.owner != Main.myPlayer) return; // only the owner applies it, so it isn't doubled in multiplayer

            if (Main.player[projectile.owner].GetModPlayer<ObsidianSharpnessP>().ObsidianSharpnessing)
            {
                if(projectile.penetrate > -1)
                    projectile.penetrate += 1;

                    if (!projectile.usesLocalNPCImmunity && !projectile.usesIDStaticNPCImmunity)
                    {
                        projectile.usesLocalNPCImmunity = true;
                        projectile.localNPCHitCooldown = -1; // -1 = each enemy can be hit only once by this projectile
                    }
            }
        }

  

        public override void PostAI(Projectile projectile)
        {
            if (!IsPlayerShot(projectile)) return;

            if (Main.player[projectile.owner].GetModPlayer<ObsidianSharpnessP>().ObsidianSharpnessing)
            {
                // Example: trailing dust on every projectile
                if (Main.rand.NextBool(3))
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Lava);
            }
        }
    }

}
