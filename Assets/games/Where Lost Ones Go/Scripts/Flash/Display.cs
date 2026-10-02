using System;
using System.Collections.Generic;
using UnityEngine;

namespace Games.WhereLostOnesGo
{
	/// flash.geom.Point.
	public class Point
	{
		public double x;
		public double y;

		public Point(double x = 0, double y = 0)
		{
			this.x = x;
			this.y = y;
		}
	}

	/// <summary>
	/// A 2D affine transform in Flash's layout: (a b; c d) plus a translation, mapping a
	/// point as x' = a x + c y + tx, y' = b x + d y + ty. Y points down, as on the stage.
	/// </summary>
	public struct Matrix2D
	{
		public double a, b, c, d, tx, ty;

		public static readonly Matrix2D Identity = new Matrix2D { a = 1, d = 1 };

		/// How Flash builds a display object's matrix: scale, then rotate, then move.
		public static Matrix2D Of(DisplayObject o)
		{
			double r = o.rotation * Math.PI / 180;
			double cos = Math.Cos(r);
			double sin = Math.Sin(r);
			return new Matrix2D
			{
				a = cos * o.scaleX,
				b = sin * o.scaleX,
				c = -sin * o.scaleY,
				d = cos * o.scaleY,
				tx = o.x,
				ty = o.y,
			};
		}

		/// This matrix applied after the child's: parent * child.
		public Matrix2D Then(Matrix2D child)
		{
			return new Matrix2D
			{
				a = a * child.a + c * child.b,
				b = b * child.a + d * child.b,
				c = a * child.c + c * child.d,
				d = b * child.c + d * child.d,
				tx = a * child.tx + c * child.ty + tx,
				ty = b * child.tx + d * child.ty + ty,
			};
		}

		public Point Apply(double x, double y)
		{
			return new Point(a * x + c * y + tx, b * x + d * y + ty);
		}
	}

	/// <summary>
	/// flash.display.DisplayObject, for what the game uses: position, scale, rotation in
	/// degrees, alpha and visibility, a parent, localToGlobal, and the two events it listens
	/// for - ENTER_FRAME and CLICK.
	/// </summary>
	public class DisplayObject
	{
		public double x;
		public double y;
		public double scaleX = 1;
		public double scaleY = 1;
		public double rotation;
		public double alpha = 1;
		public bool visible = true;
		public DisplayObjectContainer parent;

		private readonly List<Action> clickListeners = new List<Action>();

		/// The stage, if this object is on the display list; null otherwise, as in Flash.
		public FlashStage stage
		{
			get
			{
				DisplayObject o = this;
				while (o.parent != null)
				{
					o = o.parent;
				}

				return o == FlashPlayer.root ? FlashPlayer.stage : null;
			}
		}

		/// Event.ENTER_FRAME: called once per frame, whether on the display list or not,
		/// until removed.
		public void addEventListener(string type, Action listener)
		{
			if (type == Event.ENTER_FRAME)
			{
				FlashPlayer.AddEnterFrame(listener);
			}
			else if (type == MouseEvent.CLICK && !clickListeners.Contains(listener))
			{
				clickListeners.Add(listener);
			}
		}

		public void removeEventListener(string type, Action listener)
		{
			if (type == Event.ENTER_FRAME)
			{
				FlashPlayer.RemoveEnterFrame(listener);
			}
			else if (type == MouseEvent.CLICK)
			{
				clickListeners.Remove(listener);
			}
		}

		internal void dispatchClick()
		{
			foreach (Action listener in clickListeners.ToArray())
			{
				listener();
			}
		}

		public Matrix2D concatenatedMatrix()
		{
			Matrix2D m = Matrix2D.Of(this);
			for (DisplayObject p = parent; p != null; p = p.parent)
			{
				m = Matrix2D.Of(p).Then(m);
			}

			return m;
		}

		public Point localToGlobal(Point point)
		{
			return concatenatedMatrix().Apply(point.x, point.y);
		}

		internal virtual void render(FlashRenderer renderer, Matrix2D matrix, double concatenatedAlpha)
		{
		}
	}

	/// flash.display.DisplayObjectContainer: children, drawn in order, later on top.
	public class DisplayObjectContainer : DisplayObject
	{
		internal readonly List<DisplayObject> children = new List<DisplayObject>();

		public int numChildren
		{
			get { return children.Count; }
		}

		public DisplayObject addChild(DisplayObject child)
		{
			if (child.parent != null)
			{
				child.parent.removeChild(child);
			}

			children.Add(child);
			child.parent = this;
			return child;
		}

		public DisplayObject removeChild(DisplayObject child)
		{
			if (children.Remove(child))
			{
				child.parent = null;
			}

			return child;
		}

		internal void renderChildren(FlashRenderer renderer, Matrix2D matrix, double concatenatedAlpha)
		{
			for (int i = 0; i < children.Count; i++)
			{
				DisplayObject child = children[i];
				if (child.visible)
				{
					child.render(renderer, matrix.Then(Matrix2D.Of(child)), concatenatedAlpha * child.alpha);
				}
			}
		}

		internal override void render(FlashRenderer renderer, Matrix2D matrix, double concatenatedAlpha)
		{
			renderChildren(renderer, matrix, concatenatedAlpha);
		}
	}

	/// flash.display.Sprite: a container with its own vector drawing underneath its children.
	public class Sprite : DisplayObjectContainer
	{
		public readonly Graphics graphics = new Graphics();

		internal override void render(FlashRenderer renderer, Matrix2D matrix, double concatenatedAlpha)
		{
			graphics.render(renderer, matrix, concatenatedAlpha);
			renderChildren(renderer, matrix, concatenatedAlpha);
		}
	}

	/// <summary>
	/// flash.display.MovieClip. The game's four library symbols - background, foreground,
	/// human, scorpion - are each one frame holding one bitmap, placed relative to the
	/// symbol's registration point as in the SWF.
	/// </summary>
	public class MovieClip : Sprite
	{
		protected void bitmapSymbol(string image, double left, double top)
		{
			graphics.drawBitmap(FlashAssets.GetTexture(image), left, top);
		}
	}

	/// <summary>
	/// flash.display.Graphics: a list of drawing commands, replayed every frame.
	///
	/// Covers fills of rectangles and circles, lines, and (for the library symbols) a
	/// bitmap. Colours are 0xRRGGBB with the alpha passed separately, as in Flash; the
	/// game passes 0xAARRGGBB values, whose top byte Flash ignored too.
	/// </summary>
	public class Graphics
	{
		internal enum Kind { Rect, Circle, Line, Bitmap }

		internal struct Command
		{
			public Kind kind;
			public double x, y, w, h;
			public double thickness;
			public uint color;
			public double alpha;
			public Texture2D texture;
		}

		private readonly List<Command> commands = new List<Command>();
		private bool filling;
		private uint fillColor;
		private double fillAlpha;
		private bool stroking;
		private double lineThickness;
		private uint lineColor;
		private double lineAlpha;
		private double penX;
		private double penY;

		public void clear()
		{
			commands.Clear();
			filling = false;
			stroking = false;
			penX = penY = 0;
		}

		public void beginFill(uint color, double alpha = 1)
		{
			filling = true;
			fillColor = color & 0xffffff;
			fillAlpha = alpha;
		}

		public void endFill()
		{
			filling = false;
		}

		public void lineStyle(double thickness, uint color = 0, double alpha = 1, bool pixelHinting = false)
		{
			stroking = true;
			lineThickness = thickness;
			lineColor = color & 0xffffff;
			lineAlpha = alpha;
		}

		public void drawRect(double x, double y, double width, double height)
		{
			if (filling)
			{
				commands.Add(new Command { kind = Kind.Rect, x = x, y = y, w = width, h = height, color = fillColor, alpha = fillAlpha });
			}
		}

		public void drawCircle(double x, double y, double radius)
		{
			if (filling && radius > 0)
			{
				commands.Add(new Command { kind = Kind.Circle, x = x, y = y, w = radius, color = fillColor, alpha = fillAlpha });
			}
		}

		public void moveTo(double x, double y)
		{
			penX = x;
			penY = y;
		}

		public void lineTo(double x, double y)
		{
			if (stroking)
			{
				commands.Add(new Command { kind = Kind.Line, x = penX, y = penY, w = x, h = y, thickness = lineThickness, color = lineColor, alpha = lineAlpha });
			}

			penX = x;
			penY = y;
		}

		internal void drawBitmap(Texture2D texture, double left, double top)
		{
			commands.Add(new Command { kind = Kind.Bitmap, x = left, y = top, w = texture.width, h = texture.height, texture = texture, alpha = 1 });
		}

		internal void render(FlashRenderer renderer, Matrix2D matrix, double concatenatedAlpha)
		{
			for (int i = 0; i < commands.Count; i++)
			{
				renderer.Draw(commands[i], matrix, concatenatedAlpha);
			}
		}
	}

	/// flash.text.TextFormat, the four properties the game sets.
	public class TextFormat
	{
		public string align = TextFormatAlign.LEFT;
		public uint color;
		public string font;
		public double size = 12;
	}

	public static class TextFormatAlign
	{
		public const string LEFT = "left";
		public const string CENTER = "center";
	}

	/// <summary>
	/// flash.text.TextField, as the game uses it: a box of text in one format.
	///
	/// The game's fields use the device font - it embeds Verdana but never sets
	/// embedFonts - and Flash Player does not apply alpha to device text, neither the
	/// field's own nor its parents'. So the flickering alpha the game sets on its text
	/// never showed, and the message box hid its text by emptying it rather than by its
	/// alpha. This draws text the same way: alpha is kept but not used.
	/// </summary>
	public class TextField : DisplayObject
	{
		/// Set false to draw text with its alpha, as it would look with an embedded font.
		public const bool DeviceFontIgnoresAlpha = true;

		public string text = "";
		public double width = 100;
		public double height = 100;
		public bool mouseEnabled = true;
		public bool selectable = true;
		public TextFormat defaultTextFormat = new TextFormat();

		internal override void render(FlashRenderer renderer, Matrix2D matrix, double concatenatedAlpha)
		{
			if (!string.IsNullOrEmpty(text))
			{
				renderer.DrawText(this, matrix, DeviceFontIgnoresAlpha ? 1 : concatenatedAlpha);
			}
		}
	}

	public static class Event
	{
		public const string ENTER_FRAME = "enterFrame";
	}

	public static class MouseEvent
	{
		public const string CLICK = "click";
	}

	/// flash.display.Stage: its size, and where the mouse is on it.
	public class FlashStage
	{
		public double stageWidth = 640;
		public double stageHeight = 360;
		public double mouseX;
		public double mouseY;
	}
}

namespace Games.WhereLostOnesGo
{
	/// Top-level ActionScript functions the game calls.
	public static class Flash
	{
		private static readonly System.Random rng = new System.Random();

		/// Math.random(): 0 <= n < 1.
		public static double random()
		{
			return rng.NextDouble();
		}
	}
}
