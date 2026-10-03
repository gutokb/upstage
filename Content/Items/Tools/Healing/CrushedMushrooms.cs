using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.ModUtils;
using upstage.Common.Players;
using upstage.Content.Debuffs;

namespace upstage.Content.Items.Tools.Healing
{
    public class CrushedMushrooms : ModItem
    {

        float healRadius = 50f;
        int healAmount = 15;
        int moraleCost = 15;
        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing; 
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item4;
            Item.consumable = false; 
        }

        public override bool? UseItem(Player player)
        {
            // Cosmetic, so let every client draw it for whoever used the item.
            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust d = Dust.NewDustPerfect(player.Center, DustID.GoldCritter, speed * 5f, newColor: Color.GreenYellow, Scale: 1f);
                d.noGravity = true;
            }

            // UseItem runs for every player on every client, so the morale spend and the heal have
            // to be limited to the client that actually used the item.
            if (!PlayerUtils.IsLocalAuthority(player.whoAmI))
                return true;

            Morale moralePlayer = player.GetModPlayer<Morale>();
            moralePlayer.UseMorale(moraleCost);
            player.AddBuff(ModContent.BuffType<HealingDebuff>(), 3600);

            foreach (Player other in Main.ActivePlayers)
            {
                if (!PlayerUtils.CanHeal(player, other, includeSelf: true))
                    continue;

                if (Vector2.Distance(other.Center, player.Center) < healRadius)
                    PlayerUtils.HealPlayer(other, healAmount);
            }

            return true;
        }

        public override bool CanUseItem(Player player)
        {
            // Remote copies of morale lag behind their owner, so don't let a stale value veto a
            // use the owning client already decided to make.
            if (!PlayerUtils.IsLocalAuthority(player.whoAmI))
                return true;

            Morale moralePlayer = player.GetModPlayer<Morale>();
            return moralePlayer.CanUseMorale(moraleCost)
                && !player.HasBuff(ModContent.BuffType<HealingDebuff>());
        }
    }
}