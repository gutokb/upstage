using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Buffs.Aura
{
    public class EmeraldAura : ModBuff
    {
        public int MoraleCap = 20;
        private int Damage =11, Size = 240, TimerMax = 60;
        private Dust[] garbagebin = new Dust[90]; 

         public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;      
            Main.buffNoSave[Type] = false;    
            Main.buffNoTimeDisplay[Type] = true; 
               
     
        }
	
        public override void Update(Player player, ref int buffIndex)
        {
            Morale mplayer = player.GetModPlayer<Morale>();
            AuraP saplayer = player.GetModPlayer<AuraP>();
            saplayer.SetAura(Damage, Size);
            mplayer.MoraleCap = MoraleCap;
            player.AddBuff(Type, 2);
            for (int i = 0; i < 90; i++)
            {
                if (garbagebin[i] != null)
                {
                    garbagebin[i].active = false;
                }
                
            }
            for (int i = 0; i < 90; i++) 
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(player.Center + speed * Size, DustID.GemEmerald, Vector2.Zero, Scale: 1.5f);
                garbagebin[i] = d;
                d.noGravity = true;
            }
        }
        

            
    
	}
}