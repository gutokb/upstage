using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;
using upstage.Content.Debuffs;
using upstage.Content.Projectiles;

namespace upstage.Content.Items.Tools.Healing
{
    public class CupidBow : ModItem
    {

        int moraleCost = 15;
        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot; 
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item4;
            Item.consumable = false;
            Item.shoot = ModContent.ProjectileType<CupidArrow>();
            Item.shootSpeed = 10f;
        }

        public override bool? UseItem(Player player)
        {
            Morale moralePlayer = player.GetModPlayer<Morale>();
            if (moralePlayer.UseMorale(moraleCost))
            {
                return true;
            }
            return false;
            
        }

        public override bool CanUseItem(Player player)
        {
            Morale moralePlayer = player.GetModPlayer<Morale>();
            if (moralePlayer.CanUseMorale(moraleCost))
            {
                if (!player.HasBuff(ModContent.BuffType<HealingDebuff>()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}