using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Buffs
{
    public class StarSpeedP : ModPlayer
    {
        public bool starspeeding = false;
        public override void PostUpdateRunSpeeds()
        {
            if (starspeeding)
            {
                Player.runAcceleration *= 3f;
            }
        }
        public override void ResetEffects()
        {
            starspeeding = false;
        }
    }
	public class StarSpeed : ModBuff
	{
		private int BColor = 1;
		public override void Update(Player player, ref int buffIndex) {
            Morale mother = player.GetModPlayer<Morale>();
            StarSpeedP sp = player.GetModPlayer<StarSpeedP>();
            mother.Buffs[BColor].Add(buffIndex);
            sp.starspeeding = true;
		}
	}
}