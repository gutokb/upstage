using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;


namespace upstage.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ReinforcedSlimeHelmet : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Green;
            Item.defense = 5;
        }



        public override void UpdateEquip(Player player)
        {
            Morale MoralePlayer = player.GetModPlayer<Morale>();
            MoralePlayer.MoraleTrueMax += 10;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ReinforcedSlimeBreastplate>() &&
                legs.type == ModContent.ItemType<ReinforcedSlimeLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            Morale MoralePlayer = player.GetModPlayer<Morale>();
            player.jumpSpeedBoost += 1f;
            MoralePlayer.MoraleBuffDuration += 0.15f;

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
                .AddIngredient(ItemID.SilverBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

}