﻿using StardewModdingAPI;

namespace Hikawa.Editors
{
	internal class EventEditor : IAssetEditor
	{
		public bool CanEdit<T>(IAssetInfo asset)
		{
			// tbd
			return false;
		}
		public void Edit<T>(IAssetData asset)
		{
			// tbd
		}
	}
}