namespace Games.OdeToPixelDays
{
	/// <summary>
	/// Ported from levels/Level44.as.
	/// </summary>
	public class Level44 : Level
	{
		public FlxText text;
		public FlxText poem;
		public bool fadeOut;

		public override void create()
		{
			gameSave = new FlxSave();
			gameSave.bind("save");
			gameSave.data.level = 46;
			levelwidth = 40;
			levelheight = 30;
			scale = new FlxPoint(1, 1);
			base.create();
			FlxG.bgColor = 4278190080;
			text = new FlxText(120, 220, 400, "Playtesters:\n\nTarik Kaya\nNazire Aslan\nCem Evin\nOrcun Nisli\nBurak Tezateser\nAli Bati\nUmut Dervis\nEmrah Ozer\nOzan Komurcu\nRefik Toksoy\nGokhan Yildiz\nCaglar Sahin\nMurat Kalkavan\nHande Basar\nOguzhan Tocan\nMelis Colak\nBurak Kadron\nEfe Alacamli\nOzan Yildiz\nDurmus Ali Collu\nAhmet Erdem\nYunus Emre Tekin\nCagatay Yildiz\nEmre Erdogan\nMelih Saglam\nOguz Demir\nMuhammed Cicek\nKutay Ata Sen\nBugrahan Memis\nHakan Yilmaz");
			text.color = 4289374890;
			text.alpha = 0;
			add(text);
			poem = new FlxText(25, 70, 400, "Hans will grow up\nBe a random guy on a street\nBut the pain in his throat\nIt's going to stay for a while.");
			poem.color = 4294967295;
			poem.size = 16;
			poem.alpha = 0;
			add(poem);
			player = new Hans(0, -200, scale);
			add(player);
		}

		public override void update()
		{
			base.update();
			player.y = -200;
			if (text.alpha < 1)
			{
				text.alpha += 0.01;
			}
			else if (text.y > -270)
			{
				text.y -= 0.15;
			}
			else if (text.y > -480)
			{
				text.y -= 0.15;
				if (poem.alpha < 1)
				{
					poem.alpha += 0.01;
				}
			}
			else if (text.y > -510)
			{
				text.y -= 0.2;
				if (poem.alpha > 0)
				{
					poem.alpha -= 0.01;
				}

				if (!fadeOut)
				{
					musicFadeOut();
					fadeOut = true;
				}
			}
			else
			{
				musicStop();
				nextLevel();
			}

			if (FlxG.keys.justPressed("ENTER"))
			{
				nextLevel();
			}
		}

		public override void nextLevel()
		{
			FlxG.switchState(new Menu());
		}
	}
}
