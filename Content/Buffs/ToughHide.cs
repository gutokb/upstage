using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Buffs
{
	public class ToughHide : ModBuff
	{
		private int BColor = 2;
		public override void Update(Player player, ref int buffIndex) {
			Morale mother = player.GetModPlayer<Morale>();
			mother.Buffs[BColor].Add(Type);
			player.statDefense += player.statLife / 50;
			
		}
	}
}