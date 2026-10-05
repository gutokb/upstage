using ExampleMod.Content.DamageClasses;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using upstage.Common.Players;
using static upstage.upstage;

namespace upstage.Common.Players
{
    /// <summary>
    /// Implemented by every banner buff so <see cref="Morale"/> can enforce the per-colour banner
    /// limit on a player.
    /// </summary>
    public interface IBannerBuff
    {
        /// <summary>
        /// 0 = Red, 1 = Green, 2 = Blue. Matches the colour indices used by <see cref="Morale"/>.
        /// </summary>
        int BannerColor { get; }
    }

    public class Morale : ModPlayer
    {
        public int MoraleCur;

        public int MoraleCap = 0;

        public float MoraleBuffDuration, MoraleBuffDurationDef = 1f;
        private int MoraleDefMax = 0;

        public int MoraleMax, MoraleTrueMax;
        private int MoraleRegTimer, NearMissTimer;
        private float MoraleActivationDistance = 200f;

        private HashSet<int> NearMissCandidates = new HashSet<int>();
        private HashSet<int> NearMissHits = new HashSet<int>();

        // Banner colours: 0 = Red, 1 = Green, 2 = Blue. See IBannerBuff.
        private const int BannerColorCount = 3;

        /// <summary>
        /// Reach of a banner. Teammates within this distance of a target supply the target's limit.
        /// </summary>
        public const float BannerRadius = 800f;

        private const int BannerSlotsDefault = 1;

        /// <summary>
        /// How many banners of each colour this player can hold at once, and how many it can
        /// support as a source. Starts at <see cref="BannerSlotsDefault"/> and is raised by gear
        /// through <see cref="AddBannerSlot"/>.
        /// </summary>
        public readonly int[] BannerSlots = { BannerSlotsDefault, BannerSlotsDefault, BannerSlotsDefault };

        private bool NearMissPossible;

        /// <summary>
        /// The morale ceiling in effect right now: what equipment grants, minus whatever an active
        /// aura caps away.
        /// <para/>
        /// Floored at 0 on purpose. An aura can cap more than the player's gear grants - every gem
        /// aura caps 20 while a single armour piece only grants 10 - and without this floor the
        /// ceiling goes negative, <see cref="GainMorale"/> clamps <see cref="MoraleCur"/> below
        /// zero, and every morale item locks up until the player slowly regenerates back above 0.
        /// </summary>
        public int MoraleEffectiveMax => System.Math.Max(0, MoraleTrueMax - MoraleCap);

        /// <summary>
        /// Whether this player's morale pool is large enough to carry <paramref name="buffType"/>'s
        /// cap and still have morale left to spend.
        /// <para/>
        /// Gated on the aura's own <see cref="IAuraBuff.MoraleCap"/> rather than a fixed number, so
        /// an aura can never cap away a player's entire pool. The comparison is strict: at equality
        /// the ceiling would be exactly 0, which freezes regeneration and makes the next
        /// <see cref="GainMorale"/> clamp the player down to nothing.
        /// </summary>
        public bool CanSustainAura(int buffType)
        {
            return ModContent.GetModBuff(buffType) is IAuraBuff aura
                && MoraleTrueMax > aura.MoraleCap;
        }

        public void GainMorale(int amount)
        {
            MoraleCur = System.Math.Min(MoraleCur + amount, MoraleEffectiveMax);
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
            for (int color = 0; color < BannerColorCount; color++)
                BannerSlots[color] = BannerSlotsDefault;
        }



        public override void PostUpdateMiscEffects()
        {
            UpdateResource();
            EnforceBannerLimit();
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

            if (MoraleCur < MoraleEffectiveMax)
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
                if (MoraleCur >= MoraleEffectiveMax)
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
            clone.SetBannerSlots(BannerSlots);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            Morale other = (Morale)clientPlayer;
            if (MoraleCur != other.MoraleCur || MoraleMax != other.MoraleMax || !SameBannerSlots(other))
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MessageType.MoraleUpdate);
                packet.Write((byte)Player.whoAmI);
                packet.Write(MoraleCur);
                packet.Write(MoraleMax);
                WriteBannerSlots(packet);
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
            WriteBannerSlots(packet);
            packet.Send(toWho, fromWho);
        }

        /// <summary>
        /// Adds banner slots of one colour. Call from equipment's UpdateEquip so the bonus is
        /// re-granted every frame, the same way MoraleTrueMax is.
        /// </summary>
        public void AddBannerSlot(int color, int amount = 1)
        {
            BannerSlots[color] += amount;
        }

        private bool SameBannerSlots(Morale other)
        {
            for (int color = 0; color < BannerColorCount; color++)
            {
                if (BannerSlots[color] != other.BannerSlots[color])
                    return false;
            }
            return true;
        }

        public void WriteBannerSlots(ModPacket packet)
        {
            for (int color = 0; color < BannerColorCount; color++)
                packet.Write((byte)BannerSlots[color]);
        }

        /// <summary>
        /// Reads the banner slots from a MoraleUpdate packet. Always consumes the bytes, even when the
        /// receiving player is missing, so the rest of the packet stays aligned.
        /// </summary>
        public static int[] ReadBannerSlots(BinaryReader reader)
        {
            int[] slots = new int[BannerColorCount];
            for (int color = 0; color < BannerColorCount; color++)
                slots[color] = reader.ReadByte();
            return slots;
        }

        public void SetBannerSlots(int[] slots)
        {
            for (int color = 0; color < BannerColorCount; color++)
                BannerSlots[color] = slots[color];
        }

        /// <summary>
        /// Applies a banner buff to <paramref name="other"/>. Call only from the user's client (see
        /// PlayerUtils.IsLocalAuthority). AddBuff on a remote player is relayed to its owner, which
        /// then enforces <see cref="EnforceBannerLimit"/> on its own copy.
        /// </summary>
        public void Buffother(Player other, int buffType, int buffDuration)
        {
            other.AddBuff(buffType, buffDuration);
        }

        /// <summary>
        /// Keeps only as many banners of each colour as <see cref="BannerLimitFor"/> allows, dropping
        /// the rest.
        /// <para/>
        /// Runs only on the player's own client. Buff timers already tick independently on every
        /// machine and ClearBuff sends nothing over the network, so the owning client is the one
        /// that has to enforce this. The banners kept are the ones with the most time left, which
        /// are the most recently applied because each banner grants a fixed duration.
        /// </summary>
        private void EnforceBannerLimit()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;

            for (int color = 0; color < BannerColorCount; color++)
            {
                List<(int type, int time)> held = new List<(int, int)>();
                for (int i = 0; i < Player.buffType.Length; i++)
                {
                    if (BannerColorOf(Player.buffType[i]) == color)
                        held.Add((Player.buffType[i], Player.buffTime[i]));
                }

                int allowed = BannerLimitFor(color);
                if (held.Count <= allowed)
                    continue;

                held.Sort((a, b) => b.time.CompareTo(a.time));

                // held is a copy, so clearing buffs from the player while looping is safe.
                for (int i = allowed; i < held.Count; i++)
                    Player.ClearBuff(held[i].type);
            }
        }

        /// <summary>
        /// How many banners of <paramref name="color"/> this player may hold: the highest slot count
        /// among living teammates within <see cref="BannerRadius"/>, this player included. Never below
        /// <see cref="BannerSlotsDefault"/>.
        /// </summary>
        private int BannerLimitFor(int color)
        {
            int limit = BannerSlotsDefault;
            foreach (Player other in Main.ActivePlayers)
            {
                if (other.dead || other.team != Player.team)
                    continue;

                if (Vector2.Distance(other.Center, Player.Center) > BannerRadius)
                    continue;

                limit = System.Math.Max(limit, other.GetModPlayer<Morale>().BannerSlots[color]);
            }
            return limit;
        }

        private static int BannerColorOf(int buffType)
        {
            ModBuff buff = ModContent.GetModBuff(buffType);
            return buff is IBannerBuff banner ? banner.BannerColor : -1;
        }

        public void FarmAura(int Bufftype)
        {
            AuraP AuraP = Player.GetModPlayer<AuraP>();
            if (AuraP.Aura)
            {
                Player.ClearBuff(AuraP.AuraId);
            }
            Player.AddBuff(Bufftype, 180);
            AuraP.AuraId = Bufftype;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(hit.DamageType != ModContent.GetInstance<MoraleDamageClass>())
            {
                MoraleCur -= MoraleCur < damageDone ? MoraleCur : damageDone;
            }
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}