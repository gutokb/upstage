using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players; // <-- so we can access your Morale class

namespace upstage.Content.Items.Tools.Misc
{
    public class MoraleResetter : ModItem
    {
      
        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp; // overhead "use" animation
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item4; // mystical sound
            Item.consumable = false; // not consumed when used
        }

        public override bool? UseItem(Player player) {
            // Access our custom morale system
            Morale moralePlayer = player.GetModPlayer<Morale>();

            moralePlayer.MoraleCur = 0;

            CombatText.NewText(player.Hitbox, Microsoft.Xna.Framework.Color.Red, "Morale Reset!");

            return true; // successful use
        }
    }
}