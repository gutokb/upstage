using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.ModUtils;
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
            // Morale belongs to the owning client. UseItem runs for every player on every client,
            // so spending it anywhere else drains a copy we don't own.
            if (!PlayerUtils.IsLocalAuthority(player.whoAmI))
                return true;

            Morale moralePlayer = player.GetModPlayer<Morale>();
            return moralePlayer.UseMorale(moraleCost);
        }

        public override bool CanUseItem(Player player)
        {
            // Remote copies of morale lag behind their owner, so don't let a stale value veto a
            // shot the owning client already decided to take.
            if (!PlayerUtils.IsLocalAuthority(player.whoAmI))
                return true;

            Morale moralePlayer = player.GetModPlayer<Morale>();
            return moralePlayer.CanUseMorale(moraleCost)
                && !player.HasBuff(ModContent.BuffType<HealingDebuff>());
        }
    }
}