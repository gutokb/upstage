using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace upstage.Content.Items.Tools.Healing{
    public class WallChicken : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IgnoresEncumberingStone[Type] = true; // stone doesn't make it heavy
        }

        private int age;
        private const int Lifetime = 60 * 10;  // gone after 10 seconds
        private const int BlinkTime = 60 * 2;  // blink during the last 2 seconds

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 1;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            age++;

            if (age >= Lifetime)
            {
                // Only the server (or singleplayer) removes it, then tells clients
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int index = Item.whoAmI;
                    Item.TurnToAir();
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index);
                }
            }
        }


        public override bool ItemSpace(Player player) => true;

        public override bool CanPickup(Player player) => Item.ownIgnore != player.whoAmI;

        // Blink near the end so players know it's about to vanish
        public override Color? GetAlpha(Color lightColor)
        {
            if (age > Lifetime - BlinkTime && age % 10 < 5)
                return Color.Transparent;
            return null;
        }

        public override bool OnPickup(Player player)
        {
            player.Heal(50);
            return false;
        }
    }
}
