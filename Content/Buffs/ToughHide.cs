using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace upstage.Content.Buffs
{
	public class ToughHide : ModBuff
	{
		public override void Update(Player player, ref int buffIndex) {
			player.statDefense += player.statLife/50; 
		}
	}
}