using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players; 
namespace upstage.Content.Items.Tools.Orb
{
    public class CrystalOrb : ModItem
    {

        private int ParryGain = 0;
        private int MoraleCost = 20;
        private float ParryAmount = 1f;

        public override void SetDefaults()
        {
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

        public override bool? UseItem(Player player)
        {
            // Access our custom morale system
            Morale moralePlayer = player.GetModPlayer<Morale>();
            DamageMods damageModsPlayer = player.GetModPlayer<DamageMods>();
            if (moralePlayer.UseMorale(MoraleCost))
            {
                damageModsPlayer.ParryGain = ParryGain;
                damageModsPlayer.ParryAmount = ParryAmount;
                damageModsPlayer.Parrying = true;
                damageModsPlayer.ParryTimer = Item.useTime;
                return true;
            }
            return false;
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

    }
}