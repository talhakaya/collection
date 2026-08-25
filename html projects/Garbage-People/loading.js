var Loading = new Phaser.Class({

	Extends: Phaser.Scene,

	initialize:

		function Loading() {
			Phaser.Scene.call(this, { key: 'loading' });
			this.shine0;
			this.shine1;
			this.shine2;
			this.logo;
			this.text;
		},

	preload: function () {
		this.load.image('logo', 'images/logo.png');
		this.load.image('shine', 'images/shine.png');
	},

	create: function () {
		this.shine2 = this.add.image(WIDTH / 2, HEIGHT / 2, 'shine').setScale(5).setAlpha(0.5);
		this.shine2.setTint(944149);
		this.shine2.blendMode = Phaser.BlendModes.ADD;

		this.shine1 = this.add.image(WIDTH / 2, HEIGHT / 2, 'shine').setScale(3);
		this.shine1.setTint(944149).setAlpha(0.75);
		this.shine1.blendMode = Phaser.BlendModes.ADD;

		this.shine0 = this.add.image(WIDTH / 2, HEIGHT / 2, 'shine').setScale(2.5);
		this.shine0.setTint(944149);

		this.logo = this.add.image(WIDTH / 2, HEIGHT * 2 / 5, 'logo');
		this.logo.blendMode = Phaser.BlendModes.ADD;

		if (isGameFinished) {
			this.text = this.add.text(440, 500, 'THANKS FOR PLAYING!!\n\nGame by Talha Kaya\n@taloketo\n\nSpace to restart', { align: 'center' }).setFont('32px Arial Black').setFill('#ffffff').setShadow(2, 2, "#944149", 2);
		}
		else {
			this.text = this.add.text(450, 530, 'Game by Talha Kaya\n@taloketo\n\nSpace to begin', { align: 'center' }).setFont('32px Arial Black').setFill('#ffffff').setShadow(2, 2, "#944149", 2);
		}

    	spacebar = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.SPACE);
	},

	update: function () {
		this.shine0.rotation -= 0.004;
		this.shine1.rotation += 0.002;
		this.shine2.rotation -= 0.001;

		if (Phaser.Input.Keyboard.JustDown(spacebar)) {
			this.scene.start('bathroom0');
			if (music) music.stop();
		}
	}

});