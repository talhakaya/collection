var Store = new Phaser.Class({

	Extends: Phaser.Scene,

	initialize:

		function Store() {
			Phaser.Scene.call(this, { key: 'store' });
			this.shine0;
			this.shine1;
			this.shine2;
			this.zButton;
			this.xButton;
			this.text;
			this.isDone;
		},

	preload: function () {
		this.load.audio('Outside', [
			'sounds/Outside.ogg',
			'sounds/Outside.mp3',
			'sounds/Outside.m4a'
		]);
		this.load.audio('HitStrong1', [
			'sounds/HitStrong1.ogg',
			'sounds/HitStrong1.mp3',
			'sounds/HitStrong1.m4a'
		]);
		this.load.image('shine', 'images/shine.png');
	},

	create: function () {
		this.isDone = false;
		if (music) music.stop();
		music = this.sound.add('Outside');
		music.setLoop(true);
		music.play();
		this.shine2 = this.add.image(WIDTH / 2, HEIGHT / 2, 'shine').setScale(5).setAlpha(0.5);
		this.shine2.setTint(944149);
		this.shine2.blendMode = Phaser.BlendModes.ADD;

		this.shine1 = this.add.image(WIDTH / 2, HEIGHT / 2, 'shine').setScale(3);
		this.shine1.setTint(944149).setAlpha(0.75);
		this.shine1.blendMode = Phaser.BlendModes.ADD;

		this.shine0 = this.add.image(WIDTH / 2, HEIGHT / 2, 'shine').setScale(2.5);
		this.shine0.setTint(944149);

		this.text = this.add.text(370, 200, '', { align: 'left' }).setFont('32px Arial Black').setFill('#ffffff').setShadow(2, 2, "#944149", 2);

		this.zButton = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.Z);
		this.xButton = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.X);
	},

	update: function () {
		this.shine0.rotation -= 0.004;
		this.shine1.rotation += 0.002;
		this.shine2.rotation -= 0.001;

		if (moneyAmount >= attackTimePrice || moneyAmount >= movementSpeedPrice) {
			this.text.setText('Cash to spend: ' + moneyAmount + '$\n\nMovement speed: Level ' + movementSpeedLevel + '\nPress Z to upgrade for ' + movementSpeedPrice + '$\n\nHit rate: Level ' + attackTimeLevel + '\nPress X to upgrade for ' + attackTimePrice + '$\n\n');
			if (Phaser.Input.Keyboard.JustDown(this.zButton) && moneyAmount >= movementSpeedPrice) {
				moneyAmount -= movementSpeedPrice;
				movementSpeedLevel++;
				this.sound.add('HitStrong1').play();
			}
			if (Phaser.Input.Keyboard.JustDown(this.xButton) && moneyAmount >= attackTimePrice) {
				moneyAmount -= attackTimePrice;
				attackTimeLevel++;
				this.sound.add('HitStrong1').play();
			}
		}
		else if (!this.isDone) {
			this.text.setText('All cash spent!\nGood job!\nWho needs groceries anyway!');
			this.isDone = true;
			this.time.addEvent({ delay: 2000, callback: storeEnd, callbackScope: this });
		}
	}

});

function storeEnd() {
	this.scene.start('dialogue');
}