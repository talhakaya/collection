var Bathroom0 = new Phaser.Class({

	Extends: Phaser.Scene,

	initialize:

		function Bathroom0() {
			Phaser.Scene.call(this, { key: 'bathroom0' });

			this.bg;
			this.lightbulb;
			this.light;
			this.eyes;
			this.kid;
			this.arm;
			this.black0;
			this.black1;
			this.timer;
			this.progress;
			this.progressOld;
		},

	preload: function () {
		this.load.audio('Bathroom0', [
			'sounds/Bathroom0.ogg',
			'sounds/Bathroom0.mp3',
			'sounds/Bathroom0.m4a'
		]);
		this.load.image('bathroom0bg', 'images/bathroom0/bg.jpg');
		this.load.image('lightbulb0', 'images/bathroom0/lightbulb.png');
		this.load.image('light', 'images/bathroom0/light.png');
		this.load.image('eyes', 'images/bathroom0/eyes.png');
		this.load.image('kid', 'images/bathroom0/kid.png');
		this.load.image('arm0', 'images/bathroom0/arm.png');
		this.load.image('wallSide', 'images/wallSide.png');
	},

	create: function () {
		this.physics.world.setBoundsCollision(true, true, true, true);

		this.sound.add('Bathroom0').play();

		var bgWidth = HEIGHT * 392 / 720;
		var bgBlackWidth = (WIDTH - bgWidth) / 2;

		this.bg = this.add.image(WIDTH * 0.5, HEIGHT * 0.5, 'bathroom0bg');
		this.bg.setDisplaySize(bgWidth, HEIGHT);

		this.lightbulb = this.add.sprite(WIDTH * 0.5, 0, 'lightbulb0');
		this.lightbulb.setOrigin(0.5, 0);

		this.light = this.add.sprite(WIDTH * 0.5, HEIGHT * 0.5, 'light');
		this.light.setAlpha(0.4);
		this.light.blendMode = Phaser.BlendModes.ADD;
		this.light.setScale(1.3);

		this.eyes = this.add.image(WIDTH * 0.5 - 12, HEIGHT - 439, 'eyes');

		this.kid = this.add.image(WIDTH * 0.5, HEIGHT, 'kid');
		this.kid.setOrigin(0.5, 1);

		this.arm = this.add.image(WIDTH * 0.5 + 124, HEIGHT - 520 + 254, 'arm0');
		this.arm.setOrigin(149 / 165, 151 / 169);
		this.arm.rotation = 0.01;

		this.black0 = this.add.image(bgBlackWidth * 0.5, HEIGHT * 0.5, 'wallSide');
		this.black0.setDisplaySize(bgBlackWidth, HEIGHT);
		this.black1 = this.add.image(WIDTH - bgBlackWidth * 0.5, HEIGHT * 0.5, 'wallSide');
		this.black1.setDisplaySize(-bgBlackWidth, HEIGHT);

		this.progress = 0.0;
		this.progressOld = 0.0;
		this.timer = this.time.addEvent({ delay: 10000, callback: bathroom0End, callbackScope: this });
	},

	update: function () {
		//Phaser.Math.FloatBetween(-0.03, 0.03);
		this.progress = this.timer.getProgress();
		if (this.progress > 0.0 && this.progressOld <= 0.0) {
			this.tweens.add({
				targets: this.arm,
				rotation: -0.04,
				ease: 'Sine.easeInOut',
				duration: 50,
				delay: 0,
				repeat: 12,
				yoyo: true,
				repeatDelay: 50
			});
		}
		else if (this.progress > 0.2 && this.progressOld <= 0.2) {
			var i;
			for (i = 1; i <= 4; i++) {
				this.tweens.add({
					targets: this.lightbulb,
					rotation: '+=0.' + (5 - i),
					ease: 'Sine.easeInOut',
					duration: 250,
					delay: 0 + 500 * (i - 1),
					yoyo: true
				});
				this.tweens.add({
					targets: this.lightbulb,
					rotation: '-=0.' + (5 - i),
					ease: 'Sine.easeInOut',
					duration: 250,
					delay: 250 + 500 * (i - 1),
					yoyo: true
				});
				this.tweens.add({
					targets: this.light,
					x: WIDTH / 2 - 50 * (5 - i),
					ease: 'Sine.easeInOut',
					duration: 250,
					delay: 0 + 500 * (i - 1),
					yoyo: true
				});
				this.tweens.add({
					targets: this.light,
					x: WIDTH / 2 + 50 * (5 - i),
					ease: 'Sine.easeInOut',
					duration: 250,
					delay: 250 + 500 * (i - 1),
					yoyo: true
				});
			}
			this.tweens.add({
				targets: this.lightbulb,
				rotation: 0,
				ease: 'Sine.easeInOut',
				duration: 250,
				delay: 500 * 5
			});
			this.tweens.add({
				targets: this.light,
				x: WIDTH / 2,
				ease: 'Sine.easeInOut',
				duration: 250,
				delay: 500 * 5
			});
		}
		else if (this.progress > 0.22 && this.progressOld <= 0.22) {
			this.tweens.add({
				targets: this.eyes,
				x: '-=5',
				ease: 'Sine.easeInOut',
				duration: 50
			});
		}
		else if (this.progress > 0.4 && this.progressOld <= 0.4) {
			this.tweens.add({
				targets: this.eyes,
				x: '+=5',
				ease: 'Sine.easeInOut',
				duration: 50
			});
		}
		else if (this.progress > 0.45 && this.progressOld <= 0.45) {
			this.tweens.add({
				targets: this.arm,
				rotation: -0.04,
				ease: 'Sine.easeInOut',
				duration: 50,
				delay: 0,
				repeat: 25,
				yoyo: true,
				repeatDelay: 50
			});
		}
		else if (this.progress > 0.7 && this.progressOld <= 0.7) {
			var i;
			for (i = 1; i <= 4; i++) {
				this.tweens.add({
					targets: this.lightbulb,
					rotation: '-=0.' + (5 - i),
					ease: 'Sine.easeInOut',
					duration: 250,
					delay: 0 + 500 * (i - 1),
					yoyo: true
				});
				this.tweens.add({
					targets: this.lightbulb,
					rotation: '+=0.' + (5 - i),
					ease: 'Sine.easeInOut',
					duration: 250,
					delay: 250 + 500 * (i - 1),
					yoyo: true
				});
				this.tweens.add({
					targets: this.light,
					x: WIDTH / 2 + 50 * (5 - i),
					ease: 'Sine.easeInOut',
					duration: 250,
					delay: 0 + 500 * (i - 1),
					yoyo: true
				});
				this.tweens.add({
					targets: this.light,
					x: WIDTH / 2 - 50 * (5 - i),
					ease: 'Sine.easeInOut',
					duration: 250,
					delay: 250 + 500 * (i - 1),
					yoyo: true
				});
			}
			this.tweens.add({
				targets: this.lightbulb,
				rotation: 0,
				ease: 'Sine.easeInOut',
				duration: 250,
				delay: 500 * 5
			});
			this.tweens.add({
				targets: this.light,
				x: WIDTH / 2,
				ease: 'Sine.easeInOut',
				duration: 250,
				delay: 500 * 5
			});
		}
		else if (this.progress > 0.72 && this.progressOld <= 0.72) {
			this.tweens.add({
				targets: this.eyes,
				x: '+=5',
				ease: 'Sine.easeInOut',
				duration: 50
			});
		}
		else if (this.progress > 0.9 && this.progressOld <= 0.9) {
			this.tweens.add({
				targets: this.eyes,
				x: '-=5',
				ease: 'Sine.easeInOut',
				duration: 50
			});
		}
		else if (this.progress > 0.95 && this.progressOld <= 0.95) {
			this.tweens.add({
				targets: this.arm,
				rotation: -0.04,
				ease: 'Sine.easeInOut',
				duration: 50,
				delay: 0,
				repeat: 5,
				yoyo: true,
				repeatDelay: 50
			});
		}
		this.progressOld = this.progress;
	}

});

function bathroom0End() {
	this.scene.start('bathroom1');
}