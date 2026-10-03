using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;
using upstage.Content.Debuffs;
using upstage.Common.ModUtils;

namespace upstage.Content.Items.Tools.Healing
{
    public class DarkRitual : ModItem
    {

        float healRadius = 50f;
        int healAmount = 15;
        int moraleCost = 15;
        int holdTimer = 0;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item4;
            Item.channel = true;
            Item.consumable = false;
        }

        public override void HoldItem(Player player)
        {
            // HoldItem runs for every player holding the item on every client, but the channel
            // target comes from Main.MouseWorld/Main.mouseLeft, which only describe the local
            // player. Running this for anyone else aims the ritual at the wrong place and heals
            // the target once per machine.
            if (!PlayerUtils.IsLocalAuthority(player.whoAmI))
                return;

            if (!player.channel || !Main.mouseLeft)
            {
                holdTimer = 0;
                return;
            }

            if (holdTimer < 61)
            {
                holdTimer++;

                for (int i = 0; i < 50; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Dust d = Dust.NewDustPerfect(Main.MouseWorld + (speed * healRadius * ((60f - holdTimer) / 60f)), DustID.PurpleCrystalShard, Vector2.Zero, Scale: 1f);
                    d.noGravity = true;
                }
            }

            if (holdTimer != 60)
                return;

            Player closest = PlayerUtils.FindClosestHealTarget(player, Main.MouseWorld, healRadius);
            if (closest == null)
                return;

            PlayerUtils.HealPlayer(closest, healAmount);

            Morale moralePlayer = player.GetModPlayer<Morale>();
            moralePlayer.UseMorale(moraleCost);
            player.AddBuff(ModContent.BuffType<HealingDebuff>(), 3600);

            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust d = Dust.NewDustPerfect(Main.MouseWorld, DustID.PurpleCrystalShard, speed, Scale: 1f);
                d.noGravity = true;
            }

            player.Hurt(
                PlayerDeathReason.ByCustomReason(
                    NetworkText.FromLiteral(player.name + " performed a dark ritual.")
                ),
                20,
                0
            );
        }

        public override bool CanUseItem(Player player)
        {
            // Remote copies of morale lag behind their owner, so don't let a stale value veto a
            // channel the owning client already decided to start.
            if (!PlayerUtils.IsLocalAuthority(player.whoAmI))
                return true;

            Morale moralePlayer = player.GetModPlayer<Morale>();
            return moralePlayer.CanUseMorale(moraleCost)
                && !player.HasBuff(ModContent.BuffType<HealingDebuff>());
        }
    }
}