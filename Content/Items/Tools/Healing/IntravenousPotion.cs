using upstage.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;
using upstage.Content.Debuffs;

namespace upstage.Content.Items.Tools.Healing
{
	public class IntravenousPotion : ModItem
	{
        private int moraleCost = 20;

		public override void SetDefaults()
		{
			Item.damage = 8;
			Item.knockBack = 4f;
			Item.useStyle = ItemUseStyleID.Rapier; // Makes the player do the proper arm motion
			Item.useAnimation = 20;
			Item.useTime = 32;
			Item.width = 32;
			Item.height = 32;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.autoReuse = false;
			Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
			Item.noMelee = true; // The projectile will do the damage and not the item

			Item.rare = ItemRarityID.White;
			Item.value = Item.sellPrice(0, 0, 0, 10);

			Item.shoot = ModContent.ProjectileType<IVPotionProjectile>(); // The projectile is what makes a shortsword work
			Item.shootSpeed = 1f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values
		}

        public override bool? UseItem(Player player)
		{
			Morale moralePlayer = player.GetModPlayer<Morale>();
			if (moralePlayer.UseMorale(moraleCost))
			{
				return base.UseItem(player);
			}
			return false;
            
        }
		
		public override bool CanUseItem(Player player)
        {
            Morale moralePlayer = player.GetModPlayer<Morale>();
            if (moralePlayer.MoraleCur >= moraleCost)
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