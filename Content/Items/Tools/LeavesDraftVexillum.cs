using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;
using upstage.Content.Buffs;

namespace upstage.Content.Items.Tools
{

    public class LeavesDraftVexillum : ModItem
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
            int buffType = ModContent.BuffType<LeavesDraft>();
            int buffDuration = (int)(60 * 60 * MoralePlayer.MoraleBuffDuration);
            int radius = 800;

            if (MoralePlayer.UseMorale(20))
            {
                foreach (Player other in Main.player)
                {
                    if (other.active && !other.dead && other.team == player.team)
                    {
                        float distance = Vector2.Distance(player.Center, other.Center);
                        if (distance < radius)
                        {
                            MoralePlayer.Buffother(other,buffType, buffDuration, 1);
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

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {   
            if(player.direction == 1)
            {
                player.itemLocation += new Vector2(-40f, -20f);
            }
            else
            {
                player.itemLocation += new Vector2(40f, -20f);
            }
            base.UseStyle(player, heldItemFrame);
        }
    }
}