using ExampleMod.Content.DamageClasses;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace upstage.Common.Players
{
    /// <summary>
    /// Implemented by every aura buff so the item that grants the aura can read what the aura will
    /// actually cost before granting it.
    /// <para/>
    /// Without this the pendants gated on a hardcoded morale total that had nothing to do with the
    /// cap they were about to impose, which let a player start an aura that capped away every point
    /// of morale they had.
    /// </summary>
    public interface IAuraBuff
    {
        /// <summary>
        /// How much of <see cref="Morale.MoraleTrueMax"/> this aura caps away while it is active.
        /// </summary>
        int MoraleCap { get; }
    }

    public class AuraP : ModPlayer
    {
        private Random rnd = new Random();
        public bool Aura;
        public int AuraId;

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
                        NPC.HitInfo Hit = target.CalculateHitInfo(NearbyPlayer() ? AuraDamage / 5 : AuraDamage, 0);
                        Hit.DamageType = ModContent.GetInstance<MoraleDamageClass>();
                        Player.StrikeNPCDirect(target, Hit);
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