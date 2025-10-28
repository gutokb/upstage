using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;


namespace upstage.Content.Items.Armor.GrassSuit
{
    [AutoloadEquip(EquipType.Body)]
    public class GrassSuitBreastplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Green;
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            Morale MoralePlayer = player.GetModPlayer<Morale>();
            MoralePlayer.MoraleTrueMax += 20;
        }


        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Gel, 10)
                .AddIngredient(ItemID.Leather, 5)
                .AddIngredient(ItemID.SilverBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe().AddIngredient(ItemID.Gel, 10)
                .AddIngredient(ItemID.Leather, 5)
                .AddIngredient(ItemID.SilverBar,10)
				.AddTile(TileID.Anvils)
				.Register();
        }
    }

}

