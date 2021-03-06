﻿using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using xTile.Dimensions;

namespace Hikawa
{
	internal class ModConsts
	{
		/* Mod data */
		// Directories
		internal const string ModName = "blueberry.Hikawa";
		internal const string ContentPrefix = ModName + ".";
		internal const string SaveDataKey = ModName;

		internal const string AssetsDir = "assets";
		internal static readonly string SpritesPath = Path.Combine(AssetsDir, "LooseSprites");
		internal static readonly string EventsPath = Path.Combine(AssetsDir, "Data", "Events");
		internal static readonly string ForagePath = Path.Combine(AssetsDir, "Data", "Locations");
		internal static readonly string JaContentPackPath = Path.Combine(AssetsDir, "ContentPack");
		internal static readonly string StringsPath = Path.Combine("i18n", "default");

		internal const string TilesheetPrefix = "z_hikawa";
		internal const string ExtraSpritesFile = TilesheetPrefix + "_extras";
		internal const string BuffIconSpritesFile = TilesheetPrefix + "_bufficons";
		internal const string ArcadeSpritesFile = TilesheetPrefix + "_arcade";
		internal const string IndoorsSpritesFile = TilesheetPrefix + "_indoors";
		internal const string BundlesSpritesFile = TilesheetPrefix + "_bundles";
		internal const string CrowSpritesFile = TilesheetPrefix + "_crows";
		internal const string CatSpritesFile = TilesheetPrefix + "_cats";
		internal const string BusSpritesFile = TilesheetPrefix + "_buses";


		/* Game objects */
		// Objects
		private const string ArcadeMinigameName = "LightGun";
		internal const string ArcadeMinigameId = ModName + ArcadeMinigameName;
		internal const string ArcadeObjectName = "Sailor V Arcade System";
		// NPCs
		internal const string ReiNpcId = ContentPrefix + "Rei";
		internal const string AmiNpcId = ContentPrefix + "Ami";
		internal const string UsaNpcId = ContentPrefix + "Usagi";
		internal const string GrampsNpcId = ContentPrefix + "Grandpa";
		internal const string YuuichiroNpcId = ContentPrefix + "Yuuichiro";
		// Maps
		internal const string ShrineMapId = ContentPrefix + "Shrine";
		internal const string HouseMapId = ContentPrefix + "House";
		internal const string TownSnippetId = ContentPrefix + "Town";
		internal const string TownJojaSnippetId = TownSnippetId + "." + "Joja";
		internal const string CorridorMapId = ContentPrefix + "Corridor";
		internal const string VortexMapId = ContentPrefix + "Vortex";
		// Tile actions
		internal const string ActionEma = ContentPrefix + "Ema";
		internal const string ActionShrineHall = ContentPrefix + "HallDoor";
		internal const string ActionShrineShop = ContentPrefix + "OmiyageyaShop";
		internal const string ActionShrineOffering = ContentPrefix + "Offering";
		internal const string ActionBackDoor = ContentPrefix + "BackDoor";
		internal const string ActionLockbox = ContentPrefix + "Lockbox";
		internal const string ActionWardrobe = ContentPrefix + "Wardrobe";
		internal const string ActionVortex = ContentPrefix + "Vortex";
		internal const string ActionFrogman = ContentPrefix + "Frogman";
		internal const string ActionArcade = ContentPrefix + ArcadeMinigameName;
		internal const int OfferingCostS = 75;
		internal const int OfferingCostM = 330;
		internal const int OfferingCostL = 825;

		// Coordinates
		internal const string DebugDefaultWarpTo = HouseMapId;
		
		// TODO: ONGOING: Fill in default warps for missing maps
		internal static readonly Dictionary<string, Location> DefaultWarps = new Dictionary<string, Location>
		{
			//{ ShrineMapId, new Location(39, 60) },
			{ ShrineMapId, new Location(45, 45) },
			//{ HouseMapId, new Location(5, 19) },
			{ HouseMapId, new Location(20, 15) },
			{ VortexMapId + 1, new Location(25, 40) },
			{ "Town", new Location(39, 15) },
			{ "BathHouse_Pool", new Location(15, 12) },
		};

		internal static readonly Location TotemWarpPosition = new Location(69, 45);
		internal static readonly Vector2 StoryStockPosition = new Vector2(20, 10);
		internal static readonly Vector2 ShrineSouvenirShopPosition = new Vector2(28, 42) * 64f;
		internal static readonly Location ArcadeMachinePosition = new Location(40, 16);
		internal static readonly List<Location> CrowTilePositions = new List<Location>
		{
			new Location()
		};
		internal static readonly List<Vector2> StoryPlantPositionsForFarmTypes = new List<Vector2>
		{
			new Vector2(42, 27), // Standard
			new Vector2(22, 31), // River
			new Vector2(38, 16), // Forest
			new Vector2(61, 30), // Hilltop
			new Vector2(47, 18), // Wilderness
			new Vector2(38, 40)  // Four Corners
		};

		// Values and things
		internal const string CommandPrefix = "bb";
		internal const int SharedBuffId = 870084643;
		internal const int BananaBegins = 3;
		internal const int BigBananaBonanza = 7;
	}
}
