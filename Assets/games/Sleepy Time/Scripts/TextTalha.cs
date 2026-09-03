using TMPro;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// Ported from TextTalha.as - every piece of text in the game. It is a display object
	/// like any other: it sways, blinks, and punches its scale up when something happens, so
	/// it is 3D TextMeshPro under a FlashObject rather than canvas UI.
	///
	/// The shadow is a second field, not TMP's underlay: field2 carries its own alpha (half
	/// of field1's, re-rolled every frame while blinking), which an underlay cannot do. Note
	/// that the original adds it *after* field1, so the shadow draws on top - kept.
	///
	/// The wide box is load-bearing. field1 is stage-width and shifted left by half of that,
	/// with centred text, so the line centres on the object's own origin; narrow the box and
	/// nothing lands where the source expects.
	/// </summary>
	public class TextTalha : FlashObject
	{
		public static int scaleUpConstant = 1000;

		public string text;
		public int scaleUpCounter;
		public int rotationTimeCurrent = 0;
		public float rotationMax = 15f;
		public bool rotatingToMax = false;
		public bool rotating = true;
		public FlashText field2;
		public FlashText field1;
		public Color color1;
		public bool blinking = false;

		/// White, as flash's 16777215.
		public static TextTalha New(string text = "", uint color = 16777215)
		{
			TextTalha talha = NewNode("TextTalha").AddComponent<TextTalha>();
			talha.text = "";
			talha.text = text;
			talha.color1 = ToColor(color);

			talha.field1 = FlashText.New("field1");
			talha.field1.color = talha.color1;
			talha.field1.width = Main.stageWidth;
			talha.field1.height = 40f;
			talha.field1.x = -Main.stageWidth / 2f;
			talha.field1.y = -15f;

			talha.field2 = FlashText.New("field2");
			talha.field2.color = talha.color1;
			talha.field2.alpha = 0.5f;
			talha.field2.width = Main.stageWidth;
			talha.field2.height = 40f;
			talha.field2.x = talha.field1.x + 2f;
			talha.field2.y = talha.field1.y + 2f;

			// TextFormat("Victor's Pixel Font", 30), centre aligned.
			talha.field1.size = 30f;
			talha.field2.size = 30f;
			talha.field1.align = TextAlignmentOptions.Top;
			talha.field2.align = TextAlignmentOptions.Top;

			talha.addChild(talha.field1);
			talha.addChild(talha.field2);
			talha.Tick();
			return talha;
		}

		public void Tick()
		{
			field1.text = text;
			field2.text = text;

			scaleUpCounter -= GameManager.dt;
			if (scaleUpCounter <= 0)
			{
				scaleUpCounter = 0;
				scaleX = scaleY = 1f;
			}
			else
			{
				scaleX = scaleY = 1f + scaleUpCounter / 1000f;
			}

			if (rotating)
			{
				if (rotatingToMax)
				{
					rotationTimeCurrent += GameManager.dt;
					if (rotationTimeCurrent >= GameManager.rhythm / 2f)
					{
						rotatingToMax = false;
					}
				}
				else
				{
					rotationTimeCurrent -= GameManager.dt;
					if (rotationTimeCurrent <= -GameManager.rhythm / 2f)
					{
						rotatingToMax = true;
					}
				}

				// Divided by rhythm and then by 2, so the sway is a quarter of rotationMax -
				// a good deal gentler than Citmap.rotate's. That is what the source says.
				rotation = rotationMax * (rotationTimeCurrent / (float)GameManager.rhythm / 2f);
			}

			if (blinking)
			{
				field1.alpha = GameManager.blink(field1.alpha, 0.4f, 0.6f);
				field2.alpha = field1.alpha / 2f;
			}
		}

		public void changeShadow()
		{
			if (Random.value < 0.25f)
			{
				field2.x = field1.x + 2f;
				field2.y = field1.y + 2f;
			}
			else
			{
				field2.x = field1.x - 2f + 4f * Random.value;
				field2.y = field1.y - 2f + 4f * Random.value;
			}
		}

		private static Color ToColor(uint rgb)
		{
			return new Color(
				((rgb >> 16) & 0xFF) / 255f,
				((rgb >> 8) & 0xFF) / 255f,
				(rgb & 0xFF) / 255f);
		}
	}
}
