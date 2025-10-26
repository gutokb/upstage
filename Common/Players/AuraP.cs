using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace upstage.Common.Players
{
    public class AuraP : ModPlayer
    {
        private Random rnd = new Random();
        public bool Aura;

        public float PlayerMinDistance = 1000f;
        public float AuraSize;
        private int AuraDamage , AuraTimer, AuraTimerMax = 60;
        private HashSet<int> NearbyEnemies = new HashSet<int>();

        public void SetAura(int Damage, float Size, int TimerMax = 60)
        {
            AuraDamage = Damage;
            AuraSize = Size;
            AuraTimerMax = TimerMax;
            Aura = true;
        }

        private bool NearbyPlayer()
        {
            foreach (Player other in Main.player)
            {
                if(other!= Player && Vector2.Distance(Player.Center, other.Center) < PlayerMinDistance)
                {
                    return true;
                }
            }
            return false;
        }
        private void EnemiesNearby()
        {
            NearbyEnemies.Clear();
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.lifeMax > 5) // only real enemies
                {
                    float distance = Vector2.Distance(Player.Center, npc.Center);

                    if (distance <= AuraSize)
                    {
                        NearbyEnemies.Add(npc.whoAmI);
                    }
                }
            }
        
        }
        public override void PostUpdateMiscEffects()
        {
            if (AuraTimer == 0 && Aura)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    EnemiesNearby();
                    if (NearbyEnemies.Count > 0)
                    {
                        int npcWhoAmI = NearbyEnemies.ElementAt(rnd.Next(0, NearbyEnemies.Count));
                        NPC target = Main.npc[npcWhoAmI];
                        NPC.HitInfo MaimHit = target.CalculateHitInfo(NearbyPlayer() ? AuraDamage / 5 : AuraDamage, 0);
                        Player.StrikeNPCDirect(target, MaimHit);
                    }
                    AuraTimer = AuraTimerMax;
                }
            }
        }

        public override void PreUpdate()
        {
            if (AuraTimer > 0)
            {
                AuraTimer--;
            }
            base.PreUpdate();
        }




        public override void Initialize()
        {
            Aura = false;
            AuraTimer = AuraTimerMax;
        }

        public override void ResetEffects()
        {
            Aura = false;
            base.ResetEffects();
        }
    }
}