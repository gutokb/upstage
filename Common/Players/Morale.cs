using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static upstage.upstage;

namespace upstage.Common.Players
{

    public class Morale : ModPlayer
    {
        public int MoraleCur;

        public int MoraleCap = 0;

        public float MoraleBuffDuration, MoraleBuffDurationDef = 1f;
        private int MoraleDefMax = 100;

        public int MoraleMax, MoraleTrueMax;
        private int MoraleRegTimer, NearMissTimer;
        private float MoraleActivationDistance = 200f;

        private HashSet<int> NearMissCandidates = new HashSet<int>();
        private HashSet<int> NearMissHits = new HashSet<int>();

        public List<int> GreenBuffs = new List<int>();
        public List<int> BlueBuffs = new List<int>();
        public List<int> RedBuffs = new List<int>();

        public List<int>[] Buffs;
        public int[] BuffsNum;

        public int gBuffNum = 1, bBuffNum = 1, rBuffNum = 1;

        private bool NearMissPossible;

        public Morale()
        {
            Buffs = new List<int>[] {RedBuffs, GreenBuffs, BlueBuffs};
            BuffsNum = new int[] {rBuffNum, gBuffNum, bBuffNum};
        }

        public void GainMorale(int amount)
        {
            MoraleCur = System.Math.Min(MoraleCur + amount, MoraleTrueMax - MoraleCap);
            CombatText.NewText(Player.Hitbox, Color.Orange, amount);
        }

        public bool UseMorale(int amount)
        {
            if (amount <= MoraleCur)
            {
                MoraleCur = MoraleCur - amount;
                return true;
            }
            else
            {
                return false;
            }
        }

         public bool CanUseMorale(int amount)
        {
            if (amount <= MoraleCur)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private int EnemyNearby()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.lifeMax > 5) // only real enemies
                {
                    float distance = Vector2.Distance(Player.Center, npc.Center);

                    if (distance <= MoraleActivationDistance)
                    {
                        if (distance <= MoraleActivationDistance / 2)
                        {
                            return 2;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
            }
            return 0; // No enemies nearby
        }

        private void NearMiss()
        {
            int id;

            NearMissCandidates.RemoveWhere(i => !Main.projectile[i].active);
            NearMissHits.RemoveWhere(i => !Main.projectile[i].active);

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.hostile && !proj.friendly)
                {
                    float dist = Vector2.Distance(Player.Center, proj.Center);
                    id = proj.whoAmI;
                    if (dist <= MoraleActivationDistance / 3)
                    {

                        if (!NearMissCandidates.Contains(id))
                        {
                            NearMissCandidates.Add(id);
                        }
                    }
                    else
                    {
                        if (NearMissCandidates.Contains(id) && !NearMissHits.Contains(id))
                        {
                            GainMorale(20);
                            NearMissCandidates.Clear();
                            NearMissHits.Clear();
                            NearMissPossible = false;
                            return;
                        }
                    }
                }
            }
        }

        public override void Initialize()
        {
            MoraleMax = MoraleDefMax;
            MoraleBuffDuration = MoraleBuffDurationDef;
            MoraleCur = 0;
        }

        public override void ResetEffects()
        {
            ResetVariables();
        }

        public override void UpdateDead()
        {
            MoraleCur = 0;
            ResetVariables();
        }


        private void ResetVariables()
        {
            MoraleCap = 0;
            MoraleTrueMax = MoraleMax;
            MoraleBuffDuration = MoraleBuffDurationDef;
            GreenBuffs.Clear();
            BlueBuffs.Clear();
            RedBuffs.Clear();
        }



        public override void PostUpdateMiscEffects()
        {
            UpdateResource();
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            foreach (int Id in NearMissCandidates)
            {
                if (!NearMissHits.Contains(Id))
                {
                    NearMissHits.Add(Id);
                }
            }
            base.OnHurt(info);
        }


        private void UpdateResource()
        {

            if (MoraleCur < MoraleTrueMax - MoraleCap)
            {
                if (NearMissPossible)
                {
                    NearMiss();
                }
                else
                {
                    NearMissTimer++;
                    if (NearMissTimer >= 300)
                    {
                        NearMissTimer = 0;
                        NearMissPossible = true;
                    }

                }
                
                MoraleRegTimer += EnemyNearby();
                if (MoraleRegTimer > 60)
                {
                    MoraleCur++;
                    MoraleRegTimer = 0;
                }
                if (MoraleCur == MoraleTrueMax - MoraleCap)
                {
                    MoraleRegTimer = 0;
                }
            }
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            Morale clone = (Morale)targetCopy;
            clone.MoraleCur = MoraleCur;
            clone.MoraleMax = MoraleMax;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            Morale other = (Morale)clientPlayer;
            if (MoraleCur != other.MoraleCur || MoraleMax != other.MoraleMax)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MessageType.MoraleUpdate);
                packet.Write((byte)Player.whoAmI);
                packet.Write(MoraleCur);
                packet.Write(MoraleMax);
                packet.Send();
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.MoraleUpdate);
            packet.Write((byte)Player.whoAmI);
            packet.Write(MoraleCur);
            packet.Write(MoraleMax);
            packet.Send(toWho, fromWho);
        }

        public void Buffother(Player other, int Bufftype, int buffDuration, int Color)
        {
           Morale MoraleOther = other.GetModPlayer<Morale>();
            if (MoraleOther.Buffs[Color].Count >= BuffsNum[0])
            {
                other.ClearBuff(MoraleOther.Buffs[Color][0]);
               Main.NewText("hi");
            }
            other.AddBuff(Bufftype, buffDuration);
            foreach(int a in MoraleOther.Buffs[Color])
            {
                Main.NewText(a);
            }
        }
    }
}