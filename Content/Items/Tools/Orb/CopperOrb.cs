using Terraria;
using Terraria.ID;
using upstage.Common.Players;

namespace upstage.Content.Items.Tools.Orb
{
    public class CopperOrb : OrbItem
    {
        public override int MoraleCost => 7;
        public override float ParryAmount => 0.3f;
        public override int ParryGain => 5;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true; // equips into OrbSlot; parries with the parry key
        }
    }
}
