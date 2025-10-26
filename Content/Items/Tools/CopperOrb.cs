using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players; 
namespace upstage.Content.Items.Tools
{
    public class CopperOrb : ModItem
    {

        private int ParryGain = 10;
        private int MoraleCost = 10;
        private float ParryAmount = 0.5f;

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
                Gore gore = Gore.NewGoreDirect(player.GetSource_FromThis(), player.position, -player.velocity, Main.rand.Next(11, 14));
                gore.velocity.X = gore.velocity.X * 0.1f - player.velocity.X * 0.1f;
                gore.velocity.Y = gore.velocity.Y * 0.1f - player.velocity.Y * 0.05f;
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