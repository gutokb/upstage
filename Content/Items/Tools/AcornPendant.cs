using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;
using upstage.Content.Buffs;

namespace upstage.Content.Items.Tools
{

public class AcornPendant : ModItem
    {
    

        
     public override void SetDefaults() {
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

        public override bool? UseItem(Player player) {
            Morale MoralePlayer = player.GetModPlayer<Morale>();
            int buffType = ModContent.BuffType<SproutingAura>();

            if (MoralePlayer.MoraleTrueMax > 10)
            {

                player.AddBuff(buffType, 60);
 
            }



            return true;
        }
    }
}