using Microsoft.Xna.Framework;
using System;
using upstage.Common.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static upstage.upstage;
using System.Security.Cryptography.Pkcs;
using Terraria.DataStructures;

namespace upstage.Common.Players
{

    public class DamageMods : ModPlayer
    {
        public float ParryAmount;

        public int ParryGain;

        public int ParryTimer;

        public bool Parrying;

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