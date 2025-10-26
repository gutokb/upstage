using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;


namespace upstage.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class ReinforcedSlimeBreastplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Green;
            Item.defense = 5;
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

