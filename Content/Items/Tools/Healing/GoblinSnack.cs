using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.ModUtils;
using upstage.Common.Players;
using upstage.Content.Debuffs;

namespace upstage.Content.Items.Tools.Healing
{
    public class GoblinSnack : ModItem
    {

      
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

            if (!PlayerUtils.IsLocalAuthority(player.whoAmI))
                return true;

            Morale moralePlayer = player.GetModPlayer<Morale>();
            moralePlayer.UseMorale(moraleCost);
            player.AddBuff(ModContent.BuffType<HealingDebuff>(), 3600);

            int type = ModContent.ItemType<WallChicken>();
            var source = player.GetSource_ItemUse(Item);

           
                int idx = Item.NewItem(source, player.Center, type);

                Main.item[idx].ownIgnore = player.whoAmI; // this player can't pick it up...
                Main.item[idx].ownTime = 1200;             // ...for 120 ticks


                Main.item[idx].velocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, -2f));

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, idx); // sync the velocity
            


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