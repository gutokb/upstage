using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;
using upstage.Content.Debuffs; 

namespace upstage.Content.Items.Tools.Healing
{
    public class CrushedMushrooms : ModItem
    {

        float healRadius = 50f;
        int healAmount = 15;
        int moraleCost = 30;
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
            Morale moralePlayer = player.GetModPlayer<Morale>();
            moralePlayer.UseMorale(30);
            player.AddBuff(ModContent.BuffType<HealingDebuff>(), 3600);
            
            for (int i = 0; i < 50; i++) 
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust d = Dust.NewDustPerfect(player.Center, DustID.GoldCritter, speed * 5f, newColor: Color.GreenYellow, Scale: 1f);
                d.noGravity = true;
            }

            foreach (Player other in Main.player)
            {
                if (Vector2.Distance(other.Center, player.Center) < healRadius)
                {
                    other.Heal(healAmount);
                }
            }

            return true;
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