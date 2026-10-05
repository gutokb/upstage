using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using upstage.Common.Players;

namespace upstage
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class upstage : Mod
	{
		internal enum MessageType : byte
		{
			MoraleUpdate
		}

		// Healing deliberately does NOT go through a custom packet. It uses vanilla's
		// MessageID.SpiritHeal via upstage.Common.ModUtils.PlayerUtils.HealPlayer, because the
		// server relaying that message is what makes the target's own client - the only authority
		// on its statLife - actually apply the heal instead of reverting it.

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType)
			{
				case MessageType.MoraleUpdate:
					byte playerID = reader.ReadByte();
					int newMorale = reader.ReadInt32();
					int newMoraleMax = reader.ReadInt32();
					int[] newSlots = Morale.ReadBannerSlots(reader);

					// A client may only report its own morale; the server trusts the connection,
					// not the payload.
					if (Main.netMode == NetmodeID.Server)
						playerID = (byte)whoAmI;

					if (playerID < Main.maxPlayers && Main.player[playerID].TryGetModPlayer(out Morale moralePlayer))
					{
						moralePlayer.MoraleCur = newMorale;
						moralePlayer.MoraleMax = newMoraleMax;
						moralePlayer.SetBannerSlots(newSlots);

						if (Main.netMode == NetmodeID.Server)
						{
							// Re-broadcast to all other clients
							ModPacket packet = GetPacket();
							packet.Write((byte)MessageType.MoraleUpdate);
							packet.Write(playerID);
							packet.Write(newMorale);
							packet.Write(newMoraleMax);
							moralePlayer.WriteBannerSlots(packet);
							packet.Send(-1, whoAmI);
						}
					}
					break;
			}
		}

	}
}
