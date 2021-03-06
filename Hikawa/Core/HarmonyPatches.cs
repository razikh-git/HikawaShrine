﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Tools;

using Harmony; // el diavolo

namespace Hikawa
{
	public static class HarmonyPatches
	{
		internal static void PerformHarmonyPatches() {
			var harmony = HarmonyInstance.Create(ModEntry.Instance.ModManifest.UniqueID);

			// Fix the stupid melee weapon cooldown red-square fill draw that isn't scaled to fit the inventory slot bounds
			Log.D($"Harmony patching 'MeleeWeapon.drawInMenu': '{nameof(MeleeWeapon_drawInMenu_Transpiler)}'");
			harmony.Patch(
				original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.drawInMenu),
					new []
					{
						typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float),
						typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool)
					}),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(MeleeWeapon_drawInMenu_Prefix)));

			// Add default warps for custom locations
			Log.D($"Harmony patching 'Utility.getDefaultWarpLocation': '{nameof(Utility_getDefaultWarpLocation_Prefix)}'");
			harmony.Patch(
				original: AccessTools.Method(typeof(Utility), nameof(Utility.getDefaultWarpLocation)),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Utility_getDefaultWarpLocation_Prefix)));

			// Mini-sit transpiler for blocking the drawing of player shadows while sitting
			Log.D($"Harmony patching 'SGame.DrawImpl()': '{nameof(SGame_DrawImpl_Transpiler)}'");
			harmony.Patch(
				original: AccessTools.Method(AccessTools.TypeByName("StardewModdingAPI.Framework.SGame"), "DrawImpl"),
				transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(SGame_DrawImpl_Transpiler)));

			return;
			// Check result of previous transpile
			harmony.Patch(
				original: AccessTools.Method(AccessTools.TypeByName("StardewModdingAPI.Framework.SGame"), "DrawImpl"),
				transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(SGame_DrawImpl_Transpiler_test)));
		}
		
		public static IEnumerable<CodeInstruction> SGame_DrawImpl_Transpiler_test(
			IEnumerable<CodeInstruction> instructions)
		{
			// Print the CIL we've manipulated in SGame_DrawImpl_Transpiler

			Log.W("Draw transpiler test");
			var start = -1;
			var end = -1;
			var il = instructions.ToList();
			for (var i = 0; i < il.Count - 5; ++i)
			{
				if (start == -1
				    && il[i].opcode == OpCodes.Nop
				    && il[i + 1].opcode == OpCodes.Call
				    && il[i + 1].operand.ToString()
				    == AccessTools.Method(typeof(MiniSit), nameof(MiniSit.CheckToHidePlayerShadow)).ToString()
				    && il[i + 2].opcode == OpCodes.Brtrue)
				{
					Log.W($"Origin: {i}");
					start = i;
				}

				if (start != -1
				    && end == -1
				    && (il[i].opcode == OpCodes.Leave_S || il[i].opcode == OpCodes.Endfinally))
				{
					Log.W($"End: {i}");
					end = i;
				}

				if (start != -1 && end != -1)
				{
					for (var j = start; j > 0; --j)
					{
						if (il[j].opcode == OpCodes.Br)
						{
							Log.W($"Start: {j}");
							start = j;
							break;
						}

						if (j == 0)
						{
							Log.W("Never found Start.");
						}
					}
					
					Log.W($"Instructions {start} to {end} [{end - start}]");
					var ilRange = il.GetRange(start, end - start);
					if (start != -1 && end != -1)
					{
						Log.D($"\nInstructions ({start} to {end} [{end - start}]):\n"
						      + ilRange.Aggregate("", (s, instruction) => $"{s}\n{instruction.opcode} {instruction.operand}"));
						start = end = -1;
					}
				}
			}
			return il.AsEnumerable();
		}
		
		public static IEnumerable<CodeInstruction> SGame_DrawImpl_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var il = instructions.ToList();
			for (var i = 0; i < il.Count - 5; ++i)
			{
				// Identify points in SGame.DrawImpl() where the player's shadow would be drawn
				// Conveniently these are also the only usages of Farmer.isRidingHorse in DrawImpl, so seek out these usages to override

				if (il[i].opcode != OpCodes.Ldloc_S
				    || il[i + 1].opcode != OpCodes.Callvirt
				    || il[i + 1].operand.ToString() != AccessTools.Method(
					    typeof(Farmer), nameof(Farmer.isRidingHorse)).ToString()
				    || il[i + 2].opcode != OpCodes.Brtrue)
					continue;

				Log.T("\nTranspiling instructions:"
				      + $"\nil[{i - 1}]: {il[i - 1].opcode} {il[i - 1].operand}"
				      + $"\nil[{i}]: {il[i].opcode} {il[i].operand}"
				      + $"\nil[{i + 1}]: {il[i + 1].opcode} {il[i + 1].operand}"
				      + $"\nil[{i + 2}]: {il[i + 2].opcode} {il[i + 2].operand}");

				// Add a check for MiniSit.IsPlayerSittingDown to prevent drawing the player's shadow
				il[i] = new CodeInstruction(OpCodes.Nop);
				il[i + 1] = new CodeInstruction(OpCodes.Call,
					AccessTools.Method(typeof(MiniSit), nameof(MiniSit.CheckToHidePlayerShadow)));
				il[i + 2].opcode = OpCodes.Brtrue;
			}
			return il.AsEnumerable();
		}

		public static IEnumerable<CodeInstruction> MeleeWeapon_drawInMenu_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var il = instructions.ToList();
			for (var i = 0; i < il.Count; ++i)
			{
				var instruction = il[i];

				//if (i < il.Count - 1 && il[i + 1].)

				yield return instruction;
			}
		}

		public static bool Utility_getDefaultWarpLocation_Prefix(string location_name, ref int x, ref int y)
		{
			try
			{
				// Ignore locations not added by Hikawa
				if (!ModConsts.DefaultWarps.ContainsKey(location_name)
				    || !location_name.StartsWith(ModConsts.ContentPrefix))
					return true;
				var position = ModConsts.DefaultWarps[location_name];
				x = position.X;
				y = position.Y;
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Exception in {nameof(Utility_getDefaultWarpLocation_Prefix)}:\n{e}");
				return true;
			}
		}

		public static bool MeleeWeapon_drawInMenu_Prefix(MeleeWeapon __instance, SpriteBatch spriteBatch, Vector2 location,
			float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			try
			{
				var cooldown = 0;
				var cooldownLimit = 0f;
				var coolDownLevel = 0f;
				var addedScale = 0f;
				switch (__instance.type.Value)
				{
					case 0:
					case 3:
						cooldownLimit = 1500f;
						if (MeleeWeapon.defenseCooldown > 0)
						{
							cooldown = MeleeWeapon.defenseCooldown;
						}
						addedScale = ModEntry.Instance.Helper.Reflection.GetField<float>
							(typeof(MeleeWeapon), "addedSwordScale").GetValue();
						break;

					case 2:
						cooldownLimit = 4000f;
						if (MeleeWeapon.clubCooldown > 0)
						{
							cooldown = MeleeWeapon.clubCooldown;
						}
						addedScale = ModEntry.Instance.Helper.Reflection.GetField<float>
							(typeof(MeleeWeapon), "addedClubScale").GetValue();
						break;

					case 1:
						cooldownLimit = 6000f;
						if (MeleeWeapon.daggerCooldown > 0)
						{
							cooldown = MeleeWeapon.daggerCooldown;
						}
						addedScale = ModEntry.Instance.Helper.Reflection.GetField<float>
							(typeof(MeleeWeapon), "addedDaggerScale").GetValue();
						break;
				}

				coolDownLevel = cooldown / cooldownLimit;
				if (cooldown > 0)
					Log.D($"Cooldown: {cooldown}/{cooldownLimit}, Level: {coolDownLevel}");

				var drawingAsDebris = drawShadow && drawStackNumber == StackDrawType.Hide;
				if (!drawShadow | drawingAsDebris)
				{
					addedScale = 0f;
				}

				spriteBatch.Draw(
					Tool.weaponsTexture,
					location + (__instance.type.Value == 1
						? new Vector2(42f, 21f)
						: new Vector2(32f, 32f)),
					Game1.getSourceRectForStandardTileSheet(
						Tool.weaponsTexture, __instance.IndexOfMenuItemView, 16, 16),
					color * transparency,
					0f,
					new Vector2(8f, 8f),
					4f * (scaleSize + addedScale),
					SpriteEffects.None,
					layerDepth);
				if (coolDownLevel > 0f && drawShadow && !drawingAsDebris)
				{
					spriteBatch.Draw(
						Game1.staminaRect,
						new Rectangle(
							(int) location.X,
							(int) location.Y + (64 - (int) (coolDownLevel * 64f)),
							64,
							(int) (coolDownLevel * 64f)),
						Color.Red * 0.66f);
				}

				return false;
			}
			catch (Exception e)
			{
				Log.E($"Exception in {nameof(MeleeWeapon_drawInMenu_Prefix)}:\n{e}");
				return true;
			}
		}
	}
}
