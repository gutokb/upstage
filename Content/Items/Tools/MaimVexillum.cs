using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;
using upstage.Content.Buffs;

namespace upstage.Content.Items.Tools
{

    public class MaimVexillum : ModItem
    {


        public int MoraleCost = 20;

        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 72;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.UseSound = SoundID.Item4;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 1, 50);
            Item.consumable = false;
        }

        public override bool? UseItem(Player player)
        {
            Morale MoralePlayer = player.GetModPlayer<Morale>();
            int buffType = ModContent.BuffType<Maim>();
            int buffDuration = (int)(60 * 20 * MoralePlayer.MoraleBuffDuration);
            int radius = 800;

            if (MoralePlayer.UseMorale(MoraleCost))
            {
                foreach (Player other in Main.player)
                {
                    if (other.active && !other.dead && other.team == player.team)
                    {
                        float distance = Vector2.Distance(player.Center, other.Center);
                        if (distance < radius)
                        {
                            other.AddBuff(buffType, buffDuration);
                        }
                    }
                }
            }



            return true;
        }

        public override bool CanUseItem(Player player)
        {
            Morale moralePlayer = player.GetModPlayer<Morale>();
            if (moralePlayer.CanUseMorale(MoraleCost))
            {
                return base.CanUseItem(player);
            }
            return false;
        }
    }
}