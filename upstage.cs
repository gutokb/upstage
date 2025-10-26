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

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType)
			{
				case MessageType.MoraleUpdate:
					byte playerID = reader.ReadByte();
					int newMorale = reader.ReadInt32();
					int newMoraleMax = reader.ReadInt32();

					if (Main.player[playerID].TryGetModPlayer(out Morale moralePlayer))
					{
						moralePlayer.MoraleCur = newMorale;
						moralePlayer.MoraleMax = newMoraleMax;

						if (Main.netMode == NetmodeID.Server)
						{
							// Re-broadcast to all other clients
							ModPacket packet = GetPacket();
							packet.Write((byte)MessageType.MoraleUpdate);
							packet.Write(playerID);
							packet.Write(newMorale);
							packet.Write(newMoraleMax);
							packet.Send(-1, whoAmI);
						}
					}
					break;
			}
		}

	}
}
