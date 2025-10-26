using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace upstage.Content.Buffs
{
    public class LeavesDraft : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetJumpState<LeavesDraftJump>().Enable();
        }
    }

    public class LeavesDraftJump : ExtraJump
    {
        public override Position GetDefaultPosition()
        {
            return AfterBottleJumps;
        }

        public override float GetDurationMultiplier(Player player)
        {
            return 0.70f;
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            int offsetY = player.height;
            for(int i = 0; i<10; i++)
            {
                Dust d = Dust.NewDustDirect(player.position + new Vector2(0, offsetY), player.width, player.height, DustID.GrassBlades,-player.velocity.X * 0.5f, player.velocity.Y * 0.5f);
            }
        }
    }
}