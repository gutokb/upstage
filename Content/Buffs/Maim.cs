using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Buffs
{
	public class Maim : ModBuff
	{
        public override void Update(Player player, ref int buffIndex)
        {
            MaimP Maimplayer = player.GetModPlayer<MaimP>();
            Maimplayer.Maiming = true;
		}
	}
}