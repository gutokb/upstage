using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace upstage.Content
{
    public class MiscRecipes : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.Leather)
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}