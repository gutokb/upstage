using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Debuffs
{
	public class HealingDebuff : ModBuff
	{
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;          
            Main.buffNoSave[Type] = false;    
            Main.buffNoTimeDisplay[Type] = false; 
            Main.pvpBuff[Type] = true;      
     
        }
	}
}