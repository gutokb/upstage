using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace upstage.Common.ModUtils
{
    /// <summary>
    /// Multiplayer-safe helpers for the support/healing class.
    /// <para/>
    /// Every client owns its own <see cref="Player.statLife"/>: message 16 (PlayerLifeMana) is
    /// discarded by the client it describes, so writing another player's life locally - or on the
    /// server - gets overwritten the moment the real owner reports its life back. That is why a
    /// heal looks like it lands and then snaps away.
    /// <para/>
    /// Healing therefore travels as <see cref="MessageID.SpiritHeal"/>, the message vanilla uses
    /// for Spirit of the Hero. The server relays it to every other client, so the target applies
    /// the heal to the copy that actually counts and no one disagrees afterwards.
    /// </summary>
    public static class PlayerUtils
    {
        /// <summary>
        /// Heals <paramref name="target"/> and syncs it to every machine.
        /// <para/>
        /// Only the machine that decided the heal should happen may call this - see
        /// <see cref="IsLocalAuthority"/>. Projectile AI, <c>HoldItem</c> and <c>UseItem</c> all run
        /// for every player on every client, so calling this unguarded applies the heal once per
        /// machine in the world.
        /// </summary>
        /// <returns>Life actually restored; 0 when the target was already full or invalid.</returns>
        public static int HealPlayer(Player target, int amount, bool showEffect = true)
        {
            if (target == null || !target.active || target.dead || amount <= 0)
                return 0;

            // Never send overheal. SpiritHeal clamps on arrival anyway, but clamping here keeps the
            // combat text honest about how much life was really restored.
            amount = Math.Min(amount, target.statLifeMax2 - target.statLife);

            // SpiritHeal carries the amount as a short.
            amount = Math.Min(amount, short.MaxValue);

            if (amount <= 0)
                return 0;

            target.statLife += amount;

            // The relay calls HealEffect on the other clients, so don't broadcast it again here.
            if (showEffect)
                target.HealEffect(amount, broadcast: false);

            // From a client this goes to the server, which applies it and forwards it to everyone
            // else. From the server it goes straight out to all clients. Either way the heal is
            // applied exactly once per machine.
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SpiritHeal, number: target.whoAmI, number2: amount);

            return amount;
        }

        /// <inheritdoc cref="HealPlayer(Player, int, bool)"/>
        public static int HealPlayer(int targetIndex, int amount, bool showEffect = true)
        {
            if (targetIndex < 0 || targetIndex >= Main.maxPlayers)
                return 0;

            return HealPlayer(Main.player[targetIndex], amount, showEffect);
        }

        /// <summary>
        /// True when this machine is the one entitled to act for <paramref name="playerIndex"/>.
        /// Guards gameplay that runs everywhere (projectile AI, item hooks) down to a single
        /// machine, and keeps the server out of it - <see cref="Main.myPlayer"/> is 0 on a
        /// dedicated server, which would otherwise make it impersonate player 0.
        /// </summary>
        public static bool IsLocalAuthority(int playerIndex)
            => Main.netMode != NetmodeID.Server && Main.myPlayer == playerIndex;

        /// <summary>
        /// Whether <paramref name="healer"/> is allowed to support <paramref name="target"/>:
        /// active, alive, and not on an opposing PvP team.
        /// </summary>
        public static bool CanHeal(Player healer, Player target, bool includeSelf = false)
        {
            if (target == null || !target.active || target.dead)
                return false;

            if (target.whoAmI == healer.whoAmI)
                return includeSelf;

            return !healer.InOpposingTeam(target);
        }

        /// <summary>
        /// The valid heal target nearest to <paramref name="center"/> within
        /// <paramref name="radius"/> pixels, or null if there is none.
        /// </summary>
        public static Player FindClosestHealTarget(Player healer, Vector2 center, float radius, bool includeSelf = false)
        {
            Player closest = null;
            float closestDistance = radius;

            foreach (Player other in Main.ActivePlayers)
            {
                if (!CanHeal(healer, other, includeSelf))
                    continue;

                float distance = Vector2.Distance(other.Center, center);
                if (distance < closestDistance)
                {
                    closest = other;
                    closestDistance = distance;
                }
            }

            return closest;
        }
    }
}
