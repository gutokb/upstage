using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Buffs
{
    public class CometSpeedP : ModPlayer
    {
        public bool cometspeeding = false;
        public override void PostUpdateRunSpeeds()
        {
            if (cometspeeding)
            {
                Player.runAcceleration *= 3f;
            }
        }
        
        public override void ResetEffects()
        {
            cometspeeding = false;
        }
    }
	public class CometSpeed : ModBuff, IBannerBuff
	{
	    public int BannerColor => 1;

		public override void Update(Player player, ref int buffIndex) {
            CometSpeedP sp = player.GetModPlayer<CometSpeedP>();
            sp.cometspeeding = true;
		}
	}
}