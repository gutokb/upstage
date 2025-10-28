using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Buffs
{
    public class DefileP : ModPlayer
    {
        public bool defiling;
        private int Hits = 0;
        private int HitsMax = 5, hitTimer = 20;
        private NPC NearbyEnemy;

        private void EnemyNearby(NPC target)
        {
            float minDistance = 1000f;
            NearbyEnemy = null;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && npc != target) // only real enemies
                {
                    float distance = Vector2.Distance(target.Center, npc.Center);

                    if (distance <= 150f && distance < minDistance)
                    {
                        NearbyEnemy = npc;
                    }
                }
            }
        
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (defiling && hitTimer==0)
            {
                Hits++;
                hitTimer = 20;
                if (Hits >= HitsMax)
                {
                    Hits = 0;
                    EnemyNearby(target);
                    if (NearbyEnemy != null)
                    {
                        NPC.HitInfo DefileHit = NearbyEnemy.CalculateHitInfo(damageDone > 50 ? 50 :damageDone, 0);
                        Player.StrikeNPCDirect(NearbyEnemy, DefileHit);
                    }
                    Hits = 0;
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void Initialize()
        {
            defiling = false;
            Hits = 0;
        }

        public override void ResetEffects()
        {
            defiling = false;
            base.ResetEffects();
        }

        public override void PostUpdateMiscEffects()
        {
            if(hitTimer > 0)
            {
                hitTimer--;
            }
        }
    }
    public class Defile : ModBuff
    {

        public int BColor = 0;
        public override void Update(Player player, ref int buffIndex)
        {
            Morale mother = player.GetModPlayer<Morale>();
            mother.Buffs[BColor].Add(Type);
            DefileP Defileplayer = player.GetModPlayer<DefileP>();
            Defileplayer.defiling = true;
        }
    }


}