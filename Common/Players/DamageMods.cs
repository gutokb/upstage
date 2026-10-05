using Microsoft.Xna.Framework;
using System;
using upstage.Common.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static upstage.upstage;
using System.Security.Cryptography.Pkcs;
using Terraria.DataStructures;
using Terraria.GameInput;

namespace upstage.Common.Players
{

    public class DamageMods : ModPlayer
    {
        public float ParryAmount;

        public int ParryGain;

        public int ParryTimer;

        public bool Parrying;

        /// <summary>Ticks until the next parry is allowed.</summary>
        public int ParryCooldown;

        /// <summary>The orb in the orb slot this frame, or null. Set by OrbItem.UpdateAccessory.</summary>
        public OrbItem EquippedOrb;

        private bool parryRequested;

        public override void ResetEffects()
        {
            EquippedOrb = null;
        }

        // Only runs on the player's own client, so the parry and its morale cost stay local.
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (OrbSlotSystem.ParryKeybind?.JustPressed == true)
                parryRequested = true;
        }

        // Handled here rather than in ProcessTriggers: EquippedOrb is only set once equipment has
        // updated, so this is the point in the frame where it's known to be current.
        public override void PostUpdateEquips()
        {
            if (!parryRequested)
                return;

            parryRequested = false;
            EquippedOrb?.TryParry(Player);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Parrying && Player == Main.LocalPlayer && ParryAmount != 1f)
            {
                Morale MoralePlayer = Player.GetModPlayer<Morale>();
                modifiers.FinalDamage *= 1f - ParryAmount;
                modifiers.Knockback *= 0f;
                MoralePlayer.GainMorale(ParryGain);
            }

        }

        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (Parrying && Player == Main.LocalPlayer && ParryAmount == 1f)
            {
                Morale MoralePlayer = Player.GetModPlayer<Morale>();
                MoralePlayer.GainMorale(ParryGain);
                Player.SetImmuneTimeForAllTypes(20);
                Parrying = false;
                ParryTimer = 0;
                return true;
            }
            return false;
            
        }

       
        public override void PreUpdate()
        {
            if (ParryCooldown > 0)
            {
                ParryCooldown--;
            }

            if (ParryTimer > 0)
            {
                ParryTimer--;
            }
            else
            {
                Parrying = false;
            }
        }
        
        

    }
}