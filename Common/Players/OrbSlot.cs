using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace upstage.Common.Players
{
	/// <summary>
	/// Base class for orbs. An orb is an accessory that only fits <see cref="OrbSlot"/>; while it's
	/// equipped, pressing the parry key (<see cref="OrbSlotSystem.ParryKeybind"/>) spends morale and
	/// starts a parry with the orb's stats.
	/// </summary>
	public abstract class OrbItem : ModItem
	{
		public abstract int MoraleCost { get; }

		/// <summary>Fraction of incoming damage blocked; 1 makes the parry a full dodge.</summary>
		public abstract float ParryAmount { get; }

		/// <summary>Morale gained when the parry blocks a hit.</summary>
		public abstract int ParryGain { get; }

		/// <summary>Ticks the parry stays up; also the cooldown before the next parry.</summary>
		public virtual int ParryDuration => 10;

		public virtual bool SpawnsGore => true;

		public override void UpdateAccessory(Player player, bool hideVisual)
			=> player.GetModPlayer<DamageMods>().EquippedOrb = this;

		// Keeps orbs out of the vanilla accessory slots. OrbSlot.CanAcceptItem narrows the modded
		// slots down to the orb slot.
		public override bool CanEquipAccessory(Player player, int slot, bool modded)
			=> modded;

		/// <summary>
		/// Starts a parry if the cooldown is over and the player can pay the morale cost. Call only
		/// on the player's own client.
		/// </summary>
		public void TryParry(Player player)
		{
			DamageMods damageMods = player.GetModPlayer<DamageMods>();
			if (damageMods.ParryCooldown > 0)
				return;

			if (!player.GetModPlayer<Morale>().UseMorale(MoraleCost))
				return;

			SoundEngine.PlaySound(SoundID.Item4, player.position);

			if (SpawnsGore)
			{
				Gore gore = Gore.NewGoreDirect(player.GetSource_Accessory(Item), player.position, -player.velocity, Main.rand.Next(11, 14));
				gore.velocity.X = gore.velocity.X * 0.1f - player.velocity.X * 0.1f;
				gore.velocity.Y = gore.velocity.Y * 0.1f - player.velocity.Y * 0.05f;
			}

			damageMods.ParryGain = ParryGain;
			damageMods.ParryAmount = ParryAmount;
			damageMods.Parrying = true;
			damageMods.ParryTimer = ParryDuration;
			damageMods.ParryCooldown = ParryDuration;
		}
	}

	/// <summary>
	/// Accessory slot that only holds orbs (<see cref="OrbItem"/>). For players wearing morale gear it
	/// takes the place of the vanilla mount slot on the misc equipment page; see
	/// <see cref="OrbSlotSystem"/> for how the vanilla slot is hidden.
	/// </summary>
	public class OrbSlot : ModAccessorySlot
	{
		// Vanilla's misc equipment column (Main.DrawInventory, EquipPage 2): x = screenWidth - 92,
		// y = mH + 174 + row * 47, with the mount slot on row 3.
		private const int MountSlotRow = 3;

		// Main.mH (the minimap's push-down offset) is private, but the misc column is placed with it.
		private static readonly FieldInfo MapHeightOffset =
			typeof(Main).GetField("mH", BindingFlags.NonPublic | BindingFlags.Static);

		/// <summary>
		/// Whether <paramref name="player"/>'s mount slot is replaced by the orb slot: true while
		/// they wear any morale gear.
		/// </summary>
		public static bool ReplacesMountSlot(Player player)
			=> player.GetModPlayer<Morale>().MoraleTrueMax > 0;

		public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
			=> checkItem.ModItem is OrbItem;

		// Right-click equipping an orb puts it in this slot first.
		public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo)
			=> item.ModItem is OrbItem;

		// Looked up per call: a ModAccessorySlot is created once at load time, so caching the
		// Morale player in a field would read the wrong player (or none).
		public override bool IsEnabled()
			=> ReplacesMountSlot(Player);

		// Hide the slot without morale gear, but keep it visible while it still holds an orb so
		// the player can take the orb back out after removing their armour.
		public override bool IsVisibleWhenNotEnabled()
			=> !FunctionalItem.IsAir;

		// While enabled, sit exactly on the vanilla mount slot. While disabled but still holding an
		// orb, fall back to the normal accessory column so it doesn't overlap the mount slot, which
		// is visible again at that point.
		public override Vector2? CustomLocation
			=> IsEnabled() ? MountSlotPosition() : null;

		// A custom-location slot is drawn on every equipment page, so keep it to the misc page.
		public override bool IsHidden()
			=> IsEnabled() && Main.EquipPage != 2;

		// Only the functional square fits in the mount slot's place.
		public override bool DrawVanitySlot => false;
		public override bool DrawDyeSlot => false;

		// Icon textures. Nominal image size is 32x32. Will be centered on the slot.
		public override string FunctionalTexture => "upstage/Content/Items/Tools/Orb/CopperOrb";

		private static Vector2 MountSlotPosition()
		{
			int mapHeightOffset = (int)MapHeightOffset.GetValue(null);
			return new Vector2(Main.screenWidth - 92, mapHeightOffset + 174 + MountSlotRow * 47);
		}
	}

	/// <summary>
	/// Registers the parry key, and hides and disables the vanilla mount slot (and its dye slot) for
	/// players whose mount slot is replaced by <see cref="OrbSlot"/>. Any mount already in the slot
	/// stays there untouched and comes back once the player takes their morale gear off.
	/// </summary>
	[Autoload(Side = ModSide.Client)]
	public class OrbSlotSystem : ModSystem
	{
		private const int MountSlotIndex = 3;

		/// <summary>
		/// Triggers the equipped orb's parry. Defaults to R, which is also vanilla's Quick Mount key;
		/// players can rebind either under Controls.
		/// </summary>
		public static ModKeybind ParryKeybind { get; private set; }

		public override void Load()
		{
			ParryKeybind = KeybindLoader.RegisterKeybind(Mod, "Parry", "R");

			On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += SkipHiddenSlotDraw;
			On_ItemSlot.Handle_ItemArray_int_int += SkipHiddenSlotHandle;
			On_Player.QuickMount_GetItemToUse += IgnoreHiddenMountSlot;
		}

		public override void Unload()
		{
			ParryKeybind = null;
		}

		private static bool IsHiddenMountSlot(int context, int slot)
		{
			if (slot != MountSlotIndex)
				return false;

			if (context != ItemSlot.Context.EquipMount && context != ItemSlot.Context.EquipMiscDye)
				return false;

			return OrbSlot.ReplacesMountSlot(Main.LocalPlayer);
		}

		private static void SkipHiddenSlotDraw(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig,
			SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor)
		{
			if (IsHiddenMountSlot(context, slot))
				return;

			orig(spriteBatch, inv, context, slot, position, lightColor);
		}

		// Handle does clicking, hovering and tooltips; skipping it makes the hidden slot inert.
		private static void SkipHiddenSlotHandle(On_ItemSlot.orig_Handle_ItemArray_int_int orig,
			Item[] inv, int context, int slot)
		{
			if (IsHiddenMountSlot(context, slot))
				return;

			orig(inv, context, slot);
		}

		// Quick Mount checks the mount slot before the inventory. Hiding the slot from that check
		// keeps mounts carried in the inventory, and dismounting, working as normal.
		private static Item IgnoreHiddenMountSlot(On_Player.orig_QuickMount_GetItemToUse orig, Player self)
		{
			if (!OrbSlot.ReplacesMountSlot(self))
				return orig(self);

			Item mount = self.miscEquips[MountSlotIndex];
			self.miscEquips[MountSlotIndex] = new Item();
			try
			{
				return orig(self);
			}
			finally
			{
				self.miscEquips[MountSlotIndex] = mount;
			}
		}
	}
}
