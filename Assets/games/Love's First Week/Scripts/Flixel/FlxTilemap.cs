using System;
using UnityEngine;

namespace Games.LovesFirstWeek
{
	/// org.flixel.system.FlxTile: the stand-in object a tilemap hands to the collision
	/// code, one per tile *type*, moved to wherever the tile being tested is.
	public class FlxTile : FlxObject
	{
		public FlxTilemap tilemap;
		public uint index;
		public uint mapIndex;

		public FlxTile(FlxTilemap Tilemap, uint Index, double Width, double Height, bool Visible, uint AllowCollisions)
			: base(0, 0, Width, Height)
		{
			immovable = true;
			moves = false;
			tilemap = Tilemap;
			index = Index;
			visible = Visible;
			allowCollisions = AllowCollisions;
			mapIndex = 0;
		}
	}

	/// <summary>
	/// org.flixel.FlxTilemap, as far as the game uses it: a grid loaded from comma-separated
	/// numbers, Flixel's AUTO tiling (each solid tile picks one of 16 pictures from which of
	/// its four neighbours are solid), collision against objects, and getTile/setTile.
	///
	/// Drawing differs in mechanism only. Flixel blitted the visible tiles into a
	/// screen-sized buffer each time the camera moved; here the whole map is painted once
	/// into a texture and that is moved instead. setTile repaints it.
	/// </summary>
	public class FlxTilemap : FlxObject
	{
		public const uint OFF = 0;
		public const uint AUTO = 1;
		public const uint ALT = 2;

		public uint auto;
		public int widthInTiles;
		public int heightInTiles;
		public int totalTiles;

		protected Texture2D _tiles;
		protected int _tileWidth;
		protected int _tileHeight;
		protected uint[] _data;
		protected RectInt?[] _rects;
		protected FlxTile[] _tileObjects;
		protected uint _startingIndex;

		private bool _dirty;
		private Texture2D _buffer;
		private Sprite _bufferSprite;
		private FlxRenderer _renderer;

		public FlxTilemap()
		{
			auto = OFF;
			widthInTiles = 0;
			heightInTiles = 0;
			totalTiles = 0;
			_tileWidth = 0;
			_tileHeight = 0;
			immovable = true;
			moves = false;
			_startingIndex = 0;
		}

		/// Flixel's FlxTilemap.arrayToCSV: rows of Width comma-separated values.
		public static string arrayToCSV(int[] Data, int Width, bool Invert = false)
		{
			var csv = new System.Text.StringBuilder(Data.Length * 2);
			int Height = Data.Length / Width;
			for (int row = 0; row < Height; row++)
			{
				for (int column = 0; column < Width; column++)
				{
					int index = Data[row * Width + column];
					if (Invert)
					{
						if (index == 0) index = 1;
						else if (index == 1) index = 0;
					}

					if (column == 0)
					{
						if (row == 0) csv.Append(index);
						else csv.Append('\n').Append(index);
					}
					else
					{
						csv.Append(", ").Append(index);
					}
				}
			}

			return csv.ToString();
		}

		public FlxTilemap loadMap(string MapData, string TileGraphic, int TileWidth = 0, int TileHeight = 0, uint AutoTile = OFF, uint StartingIndex = 0, uint DrawIndex = 1, uint CollideIndex = 1)
		{
			auto = AutoTile;
			_startingIndex = StartingIndex;

			// Figure out the map dimensions based on the data string
			string[] rows = MapData.Split('\n');
			heightInTiles = rows.Length;
			var data = new System.Collections.Generic.List<uint>();
			int row = 0;
			while (row < rows.Length)
			{
				string[] columns = rows[row++].Split(',');
				if (columns.Length <= 1)
				{
					heightInTiles = heightInTiles - 1;
					continue;
				}

				if (widthInTiles == 0)
				{
					widthInTiles = columns.Length;
				}

				int column = 0;
				while (column < widthInTiles)
				{
					data.Add(uint.Parse(columns[column++].Trim()));
				}
			}

			_data = data.ToArray();

			// Pre-process the map data if it's auto-tiled
			totalTiles = widthInTiles * heightInTiles;
			if (auto > OFF)
			{
				_startingIndex = 1;
				DrawIndex = 1;
				CollideIndex = 1;
				for (int i = 0; i < totalTiles; i++)
				{
					autoTile(i);
				}
			}

			// Figure out the size of the tiles
			_tiles = FlxAssets.GetTexture(TileGraphic);
			_tileWidth = TileWidth;
			if (_tileWidth == 0)
			{
				_tileWidth = _tiles.height;
			}

			_tileHeight = TileHeight;
			if (_tileHeight == 0)
			{
				_tileHeight = _tileWidth;
			}

			// create some tile objects that we'll use for overlap checks (one for each tile)
			int l = (_tiles.width / _tileWidth) * (_tiles.height / _tileHeight);
			if (auto > OFF)
			{
				l++;
			}

			_tileObjects = new FlxTile[l];
			for (int i = 0; i < l; i++)
			{
				_tileObjects[i] = new FlxTile(this, (uint)i, _tileWidth, _tileHeight, i >= DrawIndex, i >= CollideIndex ? allowCollisions : NONE);
			}

			// Then go through and create the actual map
			width = widthInTiles * _tileWidth;
			height = heightInTiles * _tileHeight;
			_rects = new RectInt?[totalTiles];
			for (int i = 0; i < totalTiles; i++)
			{
				updateTile(i);
			}

			_dirty = true;
			return this;
		}

		/// A solid tile becomes 1 + a four-bit mask of its solid neighbours (up, right,
		/// down, left); the map's edges count as solid. Run in place and in order, so the
		/// tiles above and to the left have already been rewritten - which is fine, since
		/// every rewritten solid tile is still non-zero.
		protected void autoTile(int Index)
		{
			if (_data[Index] == 0)
			{
				return;
			}

			_data[Index] = 0;
			if (Index - widthInTiles < 0 || _data[Index - widthInTiles] > 0) // UP
			{
				_data[Index] += 1;
			}

			if (Index % widthInTiles >= widthInTiles - 1 || _data[Index + 1] > 0) // RIGHT
			{
				_data[Index] += 2;
			}

			if (Index + widthInTiles >= totalTiles || _data[Index + widthInTiles] > 0) // DOWN
			{
				_data[Index] += 4;
			}

			if (Index % widthInTiles <= 0 || _data[Index - 1] > 0) // LEFT
			{
				_data[Index] += 8;
			}

			if (auto == ALT && _data[Index] == 15) // The alternate algo checks for interior corners
			{
				if (Index % widthInTiles > 0 && Index + widthInTiles < totalTiles && _data[Index + widthInTiles - 1] <= 0)
				{
					_data[Index] = 1; // BOTTOM LEFT OPEN
				}

				if (Index % widthInTiles > 0 && Index - widthInTiles >= 0 && _data[Index - widthInTiles - 1] <= 0)
				{
					_data[Index] = 2; // TOP LEFT OPEN
				}

				if (Index % widthInTiles < widthInTiles - 1 && Index - widthInTiles >= 0 && _data[Index - widthInTiles + 1] <= 0)
				{
					_data[Index] = 4; // TOP RIGHT OPEN
				}

				if (Index % widthInTiles < widthInTiles - 1 && Index + widthInTiles < totalTiles && _data[Index + widthInTiles + 1] <= 0)
				{
					_data[Index] = 8; // BOTTOM RIGHT OPEN
				}
			}

			_data[Index] += 1;
		}

		/// Which rectangle of the tile sheet this map cell draws, or nothing.
		protected void updateTile(int Index)
		{
			FlxTile tile = _data[Index] < _tileObjects.Length ? _tileObjects[_data[Index]] : null;
			if (tile == null || !tile.visible)
			{
				_rects[Index] = null;
				return;
			}

			int rx = (int)(_data[Index] - _startingIndex) * _tileWidth;
			int ry = 0;
			if (rx >= _tiles.width)
			{
				ry = (rx / _tiles.width) * _tileHeight;
				rx %= _tiles.width;
			}

			_rects[Index] = new RectInt(rx, ry, _tileWidth, _tileHeight);
		}

		public override void destroy()
		{
			_tileObjects = null;
			_data = null;
			_rects = null;
			_tiles = null;
			if (_renderer != null)
			{
				_renderer.Destroy();
				_renderer = null;
			}

			if (_bufferSprite != null) UnityEngine.Object.Destroy(_bufferSprite);
			if (_buffer != null) UnityEngine.Object.Destroy(_buffer);
			_bufferSprite = null;
			_buffer = null;
			base.destroy();
		}

		public uint getTile(int X, int Y)
		{
			return _data[Y * widthInTiles + X];
		}

		public uint getTileByIndex(int Index)
		{
			return _data[Index];
		}

		public bool setTile(int X, int Y, uint Tile, bool UpdateGraphics = true)
		{
			if (X >= widthInTiles || Y >= heightInTiles)
			{
				return false;
			}

			return setTileByIndex(Y * widthInTiles + X, Tile, UpdateGraphics);
		}

		public bool setTileByIndex(int Index, uint Tile, bool UpdateGraphics = true)
		{
			if (Index >= _data.Length)
			{
				return false;
			}

			bool ok = true;
			_data[Index] = Tile;

			if (!UpdateGraphics)
			{
				return ok;
			}

			_dirty = true;

			if (auto == OFF)
			{
				updateTile(Index);
				return ok;
			}

			// If this map is autotiled and it changes, locally update the arrangement
			int row = Index / widthInTiles - 1;
			int rowLength = row + 3;
			int column = Index % widthInTiles - 1;
			int columnHeight = column + 3;
			while (row < rowLength)
			{
				column = columnHeight - 3;
				while (column < columnHeight)
				{
					if (row >= 0 && row < heightInTiles && column >= 0 && column < widthInTiles)
					{
						int i = row * widthInTiles + column;
						autoTile(i);
						updateTile(i);
					}

					column++;
				}

				row++;
			}

			return ok;
		}

		public override bool overlaps(FlxBasic ObjectOrGroup, bool InScreenSpace = false, FlxCamera Camera = null)
		{
			if (ObjectOrGroup is FlxGroup)
			{
				bool results = false;
				FlxGroup group = (FlxGroup)ObjectOrGroup;
				for (int i = 0; i < group.members.Count; i++)
				{
					FlxBasic basic = group.members[i];
					if (basic is FlxObject)
					{
						if (overlapsWithCallback((FlxObject)basic))
						{
							results = true;
						}
					}
					else if (basic != null && overlaps(basic, InScreenSpace, Camera))
					{
						results = true;
					}
				}

				return results;
			}

			if (ObjectOrGroup is FlxObject)
			{
				return overlapsWithCallback((FlxObject)ObjectOrGroup);
			}

			return false;
		}

		/// <summary>
		/// Tests an object against every tile its box could be touching. The callback is
		/// one of FlxObject.separateX / separateY, which is how the map collides: each
		/// solid tile near the object takes a turn as a small immovable box.
		/// </summary>
		public bool overlapsWithCallback(FlxObject Object, Func<FlxObject, FlxObject, bool> Callback = null, bool FlipCallbackParams = false, FlxPoint Position = null)
		{
			bool results = false;

			double X = x;
			double Y = y;
			if (Position != null)
			{
				X = Position.x;
				Y = Position.y;
			}

			// Figure out what tiles we need to check against
			int selectionX = (int)FlxU.floor((Object.x - X) / _tileWidth);
			int selectionY = (int)FlxU.floor((Object.y - Y) / _tileHeight);
			int selectionWidth = selectionX + (int)FlxU.ceil(Object.width / _tileWidth) + 1;
			int selectionHeight = selectionY + (int)FlxU.ceil(Object.height / _tileHeight) + 1;

			// Then bound these coordinates by the map edges
			if (selectionX < 0) selectionX = 0;
			if (selectionY < 0) selectionY = 0;
			if (selectionWidth > widthInTiles) selectionWidth = widthInTiles;
			if (selectionHeight > heightInTiles) selectionHeight = heightInTiles;

			// Then loop through this selection of tiles and call FlxObject.separate() accordingly
			int rowStart = selectionY * widthInTiles;
			int row = selectionY;
			double deltaX = X - last.x;
			double deltaY = Y - last.y;
			while (row < selectionHeight)
			{
				int column = selectionX;
				while (column < selectionWidth)
				{
					bool overlapFound = false;
					FlxTile tile = _tileObjects[_data[rowStart + column]];
					if (tile.allowCollisions != 0)
					{
						tile.x = X + column * _tileWidth;
						tile.y = Y + row * _tileHeight;
						tile.last.x = tile.x - deltaX;
						tile.last.y = tile.y - deltaY;
						if (Callback != null)
						{
							if (FlipCallbackParams)
							{
								overlapFound = Callback(Object, tile);
							}
							else
							{
								overlapFound = Callback(tile, Object);
							}
						}
						else
						{
							overlapFound = Object.x + Object.width > tile.x && Object.x < tile.x + tile.width && Object.y + Object.height > tile.y && Object.y < tile.y + tile.height;
						}

						if (overlapFound)
						{
							results = true;
						}
					}

					column++;
				}

				rowStart += widthInTiles;
				row++;
			}

			return results;
		}

		public override void draw()
		{
			if (_flickerTimer != 0)
			{
				_flicker = !_flicker;
				if (_flicker)
				{
					return;
				}
			}

			if (_tiles == null)
			{
				return;
			}

			if (_dirty || _buffer == null)
			{
				paint();
			}

			if (_renderer == null || !_renderer.Alive)
			{
				_renderer = FlxGame.NewRenderer("FlxTilemap");
				_renderer.sprite.sprite = _bufferSprite;
			}

			FlxCamera camera = FlxG.camera;
			double px = x - (int)(camera.scroll.x * scrollFactor.x);
			double py = y - (int)(camera.scroll.y * scrollFactor.y);
			px += px > 0 ? 0.0000001 : -0.0000001;
			py += py > 0 ? 0.0000001 : -0.0000001;

			_renderer.Present((int)px, (int)py, 1, 1, false, Color.white);
		}

		/// Paints every visible tile into one map-sized texture. Unity textures have their
		/// origin at the bottom-left, so rows are written upside down relative to the map.
		private void paint()
		{
			int pixelWidth = widthInTiles * _tileWidth;
			int pixelHeight = heightInTiles * _tileHeight;

			if (_buffer == null || _buffer.width != pixelWidth || _buffer.height != pixelHeight)
			{
				if (_bufferSprite != null) UnityEngine.Object.Destroy(_bufferSprite);
				if (_buffer != null) UnityEngine.Object.Destroy(_buffer);

				_buffer = new Texture2D(pixelWidth, pixelHeight, TextureFormat.RGBA32, false);
				_buffer.filterMode = FilterMode.Point;
				_buffer.wrapMode = TextureWrapMode.Clamp;
				// Pivot top-left, so the map is placed by its own (x, y) like everything else.
				_bufferSprite = Sprite.Create(_buffer, new Rect(0, 0, pixelWidth, pixelHeight), new Vector2(0f, 1f), FlxGame.PixelsPerUnit);
				if (_renderer != null && _renderer.Alive)
				{
					_renderer.sprite.sprite = _bufferSprite;
				}
			}

			Color32[] source = _tiles.GetPixels32();
			int sourceWidth = _tiles.width;
			int sourceHeight = _tiles.height;
			var target = new Color32[pixelWidth * pixelHeight];

			for (int row = 0; row < heightInTiles; row++)
			{
				for (int column = 0; column < widthInTiles; column++)
				{
					RectInt? rect = _rects[row * widthInTiles + column];
					if (rect == null)
					{
						continue;
					}

					RectInt r = rect.Value;
					for (int ty = 0; ty < _tileHeight; ty++)
					{
						int sourceRow = sourceHeight - 1 - (r.y + ty);
						int targetRow = pixelHeight - 1 - (row * _tileHeight + ty);
						Array.Copy(source, sourceRow * sourceWidth + r.x, target, targetRow * pixelWidth + column * _tileWidth, _tileWidth);
					}
				}
			}

			_buffer.SetPixels32(target);
			_buffer.Apply(false);
			_dirty = false;
		}
	}
}
