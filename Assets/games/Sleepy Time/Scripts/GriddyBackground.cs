using System.Collections.Generic;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// The 9x9 grid of squares behind the dialogue and score screens. Ported from
	/// GriddyBackground.as.
	///
	/// Every cell is two squares stacked: a fixed one behind and an animated one in front,
	/// with the two colours chosen by a coin flip so each cell fades between them in its own
	/// direction. The back square is created and then thrown away - createSquare adds it to
	/// the display list, so the return value is only needed for the front one.
	///
	/// The colours are borrowed at random from scenes 1-5, which is why the grid never looks
	/// the same twice.
	///
	/// This is where the 800x450 stage is hardcoded in the original: the grid is centred with
	/// literal (800 - 45 * 9) / 2 and (450 - 45 * 9) / 2.
	/// </summary>
	public class GriddyBackground : FlashObject
	{
		public static float SquareEdgeFactor;
		public static float SquareXFactor;
		public static float SquareYFactor;
		public static int AlphaPeriod = 2000;
		public static int NumberOfSquaresPerEdge = 9;

		public List<List<int>> squaresPeriods;
		public List<List<bool>> squaresAlphaGoingUps;
		public List<List<FlashBitmap>> squares;

		public static GriddyBackground New()
		{
			GriddyBackground grid = NewNode("GriddyBackground").AddComponent<GriddyBackground>();
			grid.squares = new List<List<FlashBitmap>>();
			grid.squaresPeriods = new List<List<int>>();
			grid.squaresAlphaGoingUps = new List<List<bool>>();

			for (int column = 0; column < 9; column++)
			{
				grid.squares.Add(new List<FlashBitmap>());
				grid.squaresPeriods.Add(new List<int>());
				grid.squaresAlphaGoingUps.Add(new List<bool>());

				for (int row = 0; row < 9; row++)
				{
					int flip = FlashMath.round(Random.value);
					grid.createSquare(column, row, 2 - flip);
					grid.squares[column].Add(grid.createSquare(column, row, 1 + flip));
					grid.squaresPeriods[column].Add(FlashMath.round(2000f * (0.4f + 0.6f * Random.value)));
					grid.squaresAlphaGoingUps[column].Add(false);
				}
			}

			return grid;
		}

		public void Tick()
		{
			for (int column = 0; column < 9; column++)
			{
				for (int row = 0; row < 9; row++)
				{
					// _temp_3 / _temp_4 in the decompiled source; the lines that follow each
					// one read squares[column][row].alpha back, which names them.
					if (squaresAlphaGoingUps[column][row])
					{
						squares[column][row].alpha += GameManager.dt / (float)squaresPeriods[column][row];
						if (squares[column][row].alpha >= 1f)
						{
							squaresAlphaGoingUps[column][row] = false;
						}
					}
					else
					{
						squares[column][row].alpha -= GameManager.dt / (float)squaresPeriods[column][row];
						if (squares[column][row].alpha <= 0f)
						{
							squaresAlphaGoingUps[column][row] = true;
						}
					}
				}
			}
		}

		public FlashBitmap createSquare(int column, int row, int color)
		{
			FlashBitmap square = FlashBitmap.New(
				SleepyAssets.GetSprite("img/scene" + (1 + FlashMath.round(4f * Random.value)) + "/color" + color + ".png"));
			addChild(square);
			square.scaleX = square.scaleY = 45f;
			square.x = (800 - 45 * 9) / 2 + column * 45;
			square.y = (450 - 45 * 9) / 2 + row * 45;
			return square;
		}
	}
}
