using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace upstage.Common.Players
{
    public class MaimP : ModPlayer
    {
        public bool Maiming;
        private int Hits = 0;
        private int HitsMax = 5;
        private int MaimDamage = 20;

        public override void PostItemCheck()
        {
            Item holding = Player.HeldItem;
            if (!holding.IsAir)
            {
                HitsMax = 90/Player.HeldItem.useTime;
            }
            
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Maiming)
            {
                Hits++;
                if (Hits >= HitsMax)
                {
                    Hits = 0;
                    NPC.HitInfo MaimHit = target.CalculateHitInfo(MaimDamage, 0);
                    Player.StrikeNPCDirect(target, MaimHit);
                    Hits = 0;
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void Initialize()
        {
            Maiming = false;
            Hits = 0;
        }

        public override void ResetEffects()
        {
            Maiming = false;
            base.ResetEffects();
        }
    }
}