using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using upstage.Common.Players;

namespace upstage.Content.Buffs.Aura
{
    public class AmberAura : ModBuff
    {
        public int MoraleCap = 20;
        private int Damage = 16, Size = 140, TimerMax = 60;
        private Dust[] garbagebin = new Dust[75]; 

         public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;      
            Main.buffNoSave[Type] = true;    
            Main.buffNoTimeDisplay[Type] = true; 
               
     
        }
	
        public override void Update(Player player, ref int buffIndex)
        {
            Morale mplayer = player.GetModPlayer<Morale>();
            AuraP saplayer = player.GetModPlayer<AuraP>();
            saplayer.SetAura(Damage, Size);
            mplayer.MoraleCap = MoraleCap;
            player.AddBuff(Type, 2);
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
                Dust d = Dust.NewDustPerfect(player.Center + speed * Size, DustID.GemAmber, Vector2.Zero, Scale: 1.5f);
                garbagebin[i] = d;
                d.noGravity = true;
            }
        }
        

            
    
	}
}