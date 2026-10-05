using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Buffs
{
    public class HoneyGripP : ModPlayer
    {
        public bool honeying = false;
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (honeying)
            {
                modifiers.Knockback *= 0;
            }
            base.ModifyHurt(ref modifiers);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            Player.velocity *= 0.5f;
            base.OnHurt(info);
        }

        public override void UpdateLifeRegen()
        {
            Player.lifeRegen += 2;
        }


        public override void ResetEffects()
        {
            honeying = false;
        }
    }
	public class HoneyGrip : ModBuff, IBannerBuff
	{
	    public int BannerColor => 2;

		public override void Update(Player player, ref int buffIndex) {
            HoneyGripP sp = player.GetModPlayer<HoneyGripP>();
            sp.honeying = true;
		}
	}
}