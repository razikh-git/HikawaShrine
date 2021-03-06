﻿using StardewModdingAPI;

namespace Hikawa.Editors
{
	class TestEditor : IAssetEditor
	{
		private IModHelper _helper;

		public TestEditor(IModHelper helper)
		{
			_helper = helper;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetName.StartsWith(@"Portraits") ||
			       (asset.AssetName.StartsWith(@"Characters") && asset.AssetName.Split('\\').Length < 3);
		}

		public void Edit<T>(IAssetData asset)
		{
			Log.D($"Editing {asset.AssetName}");
		}
	}
}
