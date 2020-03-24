﻿using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace Hikawa.Editors
{
	internal class WorldEditor : IAssetEditor
	{
		private readonly IModHelper _helper;

		public WorldEditor()
		{
			_helper = ModEntry.Instance.Helper;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(@"Maps/Saloon") 
			       || asset.AssetNameEquals(@"Maps/Town");
		}

		public void Edit<T>(IAssetData asset)
		{
			Log.D($"Editing {asset.AssetName}",
				ModEntry.Instance.Config.DebugMode);

			if (asset.AssetNameEquals(@"Maps/Saloon"))
			{
				// todo: make arcade machine conditional based on story progression
				// have an overnight event adding it to the location
				
				// Saloon - Sailor V Arcade Machine
				var saloonMap = asset.GetData<Map>();
				var x = ModConsts.ArcadeMachinePosition.X;
				var y = ModConsts.ArcadeMachinePosition.Y;
				try
				{
					var tilesheetPath =
						_helper.Content.GetActualAssetKey(
							Path.Combine("assets", ModConsts.SpritesDirectory, 
								$"{ModConsts.ExtraSpritesFile}.png"));
					if (tilesheetPath == null)
						Log.E("WorldEditor failed to load extras tilesheet.");
					var tilesheetPng = _helper.Content.Load<Texture2D>(tilesheetPath);

					const BlendMode blendMode = BlendMode.Additive;
					var tileSheet = new TileSheet(
						ModConsts.ExtraSpritesFile,
						saloonMap,
						tilesheetPath,
						new Size(tilesheetPng.Width, tilesheetPng.Height),
						new Size(16, 16));
					saloonMap.AddTileSheet(tileSheet);
					saloonMap.LoadTileSheets(Game1.mapDisplayDevice);
					var layer = saloonMap.GetLayer("Front");
					StaticTile[] tileAnim =
					{
						new StaticTile(layer, tileSheet, blendMode, 0),
						new StaticTile(layer, tileSheet, blendMode, 2)
					};

					layer.Tiles[x, y] = new AnimatedTile(
						layer, tileAnim, 1000);
					layer = saloonMap.GetLayer("Buildings");
					layer.Tiles[x, y + 1] = new StaticTile(
						layer, tileSheet, blendMode, 1);
					layer.Tiles[x, y + 1].Properties.Add("Action",
						new PropertyValue(ModConsts.ArcadeMinigameId));
					layer = saloonMap.GetLayer("Back");
					layer.Tiles[x, y + 2] = new StaticTile(
						layer, saloonMap.GetTileSheet("1"), blendMode, 1272);
				}
				catch (Exception ex)
				{
					Log.E("WorldEditor failed to patch Saloon:\n" + ex);
				}
			}
			else if (asset.AssetNameEquals(@"Maps/Town"))
			{
				/*
				try
				{
					var tilesheetPath =
						_helper.Content.GetActualAssetKey(
							Path.Combine("assets", "Maps", $"{ModConsts.OutdoorsTilesheetFile}.png"));
					if (tilesheetPath == null)
						Log.E("WorldEditor failed to load outdoors tilesheet.");
					var tilesheetPng = _helper.Content.Load<Texture2D>(tilesheetPath);

					var townMap = asset.GetData<Map>();
					var newMap = _helper.Content.Load<Map>(
						Path.Combine("assets", "Maps", $"{ModConsts.TownSnippetId}.tbin"));
					if (newMap == null)
					{
						Log.E($"WorldEditor failed to load {ModConsts.TownSnippetId} snippet.");
						return;
					}

					// Tilesheets
					var tilesheet = new TileSheet(
						ModConsts.OutdoorsTilesheetFile,
						townMap,
						tilesheetPath,
						new Size(tilesheetPng.Width, tilesheetPng.Height),
						new Size(16, 16));
					townMap.AddTileSheet(tilesheet);
					townMap.LoadTileSheets(Game1.mapDisplayDevice);

					// Joja stockade
					//todo: file check for !explosion flag

					// Apply tilesheets to snippet
					var tilesheetNames = new List<string>();
					foreach (var snippetTilesheet in newMap.TileSheets.ToList())
						tilesheetNames.Add(snippetTilesheet.Id);
					foreach (var tilesheetName in tilesheetNames)
					{
						var townTilesheet = townMap.GetTileSheet(tilesheetName);
						var newTilesheet = newMap.GetTileSheet(tilesheetName);
						Log.D("Image sources: " +
						      $"town: {townTilesheet?.ImageSource}, " +
						      $"snippet: {newTilesheet?.ImageSource}");
						if (newTilesheet != null)
							newTilesheet.ImageSource = townTilesheet?.ImageSource;
					}
					// Add townInteriors tilesheet for Joja supplies
					if (true)
					{
						townMap.AddTileSheet(new TileSheet(
							"z_vanilla_interior",
							townMap,
							"Maps/townInterior",
							new Size(512, 1088),
							new Size(16, 16)));
						townMap.LoadTileSheets(Game1.mapDisplayDevice);
					}

					// Patch in the map snippet
					const int xOffset = 7;
					var w = townMap.DisplayWidth;
					var h = townMap.DisplayHeight;
					foreach (var newLayer in newMap.Layers)
					{
						// Add layers
						var layer = townMap.GetLayer(newLayer.Id);
						if (layer == null)
						{
							layer = new Layer(
								newLayer.Id, 
								townMap,
								newLayer.LayerSize, 
								newLayer.TileSize);
							townMap.AddLayer(layer);
							Log.D($"Added new layer {layer.Id} to {townMap.Id}.");
						}

						// Replace tiles
						for (var y = 0; y < h; ++y)
						{
							for (var x = 0; x < w; ++x)
							{
								Log.W($"Tilesheet IDs:");
								Log.W($"town: {layer.Tiles[x, y].TileSheet.Id}, " +
								      $"snippet: {newLayer.Tiles[x, y].TileSheet.Id}");
								layer.Tiles[x, y] = newLayer.Tiles[x + xOffset, y];
							}
						}
					}

					// Entry warping
					newMap.Properties.TryGetValue("Warp", out var warpValue);
					Log.D($"Snippet Warp:  {warpValue}");
					if (warpValue == null)
						Log.E($"WorldEditor failed to read Warp properties from {ModConsts.TownSnippetId} snippet.");
					var currentOffset = 0;
					warpValue = warpValue.ToString().Replace("-90", (currentOffset++).ToString());
					townMap.Properties["Warp"] += ' ' + warpValue;
					Log.D($"Town New Warp: {warpValue}");
				}
				catch (Exception ex)
				{
					Log.E("WorldEditor failed to patch Town:\n" + ex);
				}
				*/
			}
		}
	}
}