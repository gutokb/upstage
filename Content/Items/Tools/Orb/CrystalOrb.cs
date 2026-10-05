using Terraria;
using Terraria.ID;
using upstage.Common.Players;

namespace upstage.Content.Items.Tools.Orb
{
    public class CrystalOrb : OrbItem
    {
        public override int MoraleCost => 20;
        public override float ParryAmount => 1f;
        public override int ParryGain => 0;
        public override bool SpawnsGore => false;

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
