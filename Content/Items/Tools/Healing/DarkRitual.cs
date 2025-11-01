using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;
using upstage.Content.Debuffs;

namespace upstage.Content.Items.Tools.Healing
{
    public class DarkRitual : ModItem
    {

        float healRadius = 50f;
        int healAmount = 15;
        int moraleCost = 15;
        int holdTimer = 0;

        private Player closest = null;
        private float closestDistance = 100f;
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
            if (player.channel && Main.mouseLeft)
            {
                if (holdTimer < 61)
                {
                    holdTimer++;

                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                        Dust d = Dust.NewDustPerfect(Main.MouseWorld + (speed * healRadius * ((60f - holdTimer) / 60f)), DustID.GoldCritter, Vector2.Zero, Scale: 1f);
                        d.noGravity = true;
                    }
                }

                if (holdTimer == 60)
                {
                    foreach (Player other in Main.player)
                    {
                        float distance = Vector2.Distance(other.Center, Main.MouseWorld);
                        if (distance < healRadius && distance < closestDistance && other != player)
                        {
                            closest = other;
                        }
                    }

                    if (closest != null)
                    {
                        closest.Heal(healAmount);
                        Morale moralePlayer = player.GetModPlayer<Morale>();
                        moralePlayer.UseMorale(moraleCost);
                        player.AddBuff(ModContent.BuffType<HealingDebuff>(), 3600);
                        player.Hurt(
                            PlayerDeathReason.ByCustomReason(
                                NetworkText.FromLiteral(player.name + " performed a dark ritual.")
                            ),
                            20,
                            0
                        );
                        closest = null;
                        closestDistance = 100f;
                    }
                }
            }
            else
            {
                holdTimer = 0;
                closest = null;
            }
        }

        public override bool CanUseItem(Player player)
        {
            Morale moralePlayer = player.GetModPlayer<Morale>();
            if (moralePlayer.CanUseMorale(moraleCost))
            {
                if (!player.HasBuff(ModContent.BuffType<HealingDebuff>()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}