using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Buffs
{
    public class SproutingAura : ModBuff
    {
        public int MoraleCap = 20;
        private int Damage = 6, Size = 200, TimerMax = 60;
        private Dust[] garbagebin = new Dust[75]; 

         public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;      
            Main.buffNoSave[Type] = false;    
            Main.buffNoTimeDisplay[Type] = true; 
               
     
        }
	
        public override void Update(Player player, ref int buffIndex)
        {
            Morale mplayer = player.GetModPlayer<Morale>();
            AuraP saplayer = player.GetModPlayer<AuraP>();
            saplayer.SetAura(6, 200f, 60);
            mplayer.MoraleCap = MoraleCap;
            player.AddBuff(ModContent.BuffType<SproutingAura>(), 2);
            for (int i = 0; i < 75; i++)
            {
                if (garbagebin[i] != null)
                {
                    garbagebin[i].active = false;
                }
                
            }
            for (int i = 0; i < 75; i++) 
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(player.Center + speed * 200, DustID.FireworkFountain_Green, Vector2.Zero, newColor: Color.LawnGreen, Scale: 1.5f);
                garbagebin[i] = d;
                d.noGravity = true;
            }
        }
        

            
    
	}
}