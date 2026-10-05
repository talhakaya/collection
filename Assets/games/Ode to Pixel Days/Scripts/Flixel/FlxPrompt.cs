using Collection.Controls;
using UnityEngine;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// In the collection: a sprite with a button prompt over it. Not part of Flixel.
	///
	/// The game's tutorial pictures had the keys drawn into them. The keys are gone from the
	/// pictures (the grey ellipse behind them stays) and the prompt is drawn here instead, in
	/// the collection's glyphs, for whatever device the player is using. The game's own
	/// 320x240 picture is too small for those glyphs, so they go over it at the window's
	/// resolution (InputPromptOverlay).
	///
	/// It is still a sprite in every other way: it scrolls, fades and is hidden with its
	/// state like one, and the prompt follows.
	/// </summary>
	public class FlxPrompt : FlxSprite
	{
		/// What to show: see InputPrompts for the {tokens}.
		public string template;

		/// How tall a key cap is drawn, in game pixels.
		public double keyHeight;

		private InputPromptOverlay.Label _label;

		public FlxPrompt(double X, double Y, string Graphic, string Template, double KeyHeight) : base(X, Y, Graphic)
		{
			template = Template;
			keyHeight = KeyHeight;
		}

		public override void destroy()
		{
			if (_label != null)
			{
				_label.Destroy();
				_label = null;
			}

			base.destroy();
		}

		protected override void present(double pointX, double pointY)
		{
			base.present(pointX, pointY);

			if (_label == null || !_label.Alive)
			{
				_label = FlxGame.NewPromptLabel();
			}

			// White, with the sprite's own fade.
			Color color = renderColor();
			_label.Show(template, (int)pointX + frameWidth * 0.5, (int)pointY + frameHeight * 0.5, keyHeight, new Color(1f, 1f, 1f, color.a), true);
		}
	}
}
