using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;
using upstage.Content.Buffs;

namespace upstage.Content.Items.Tools
{

    public class HoneyGripVexillum : ModItem
    {

        public int MoraleCost = 20;
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
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
            int buffType = ModContent.BuffType<HoneyGrip>();
            int buffDuration = (int)(60 * 30 * MoralePlayer.MoraleBuffDuration);
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
                            MoralePlayer.Buffother(other,buffType, buffDuration, 2);
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

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-40f, -10f); 
        }
    }
}