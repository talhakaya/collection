var City = new Phaser.Class({

	Extends: Phaser.Scene,

	initialize:

		function City() {
			Phaser.Scene.call(this, { key: 'city' });
			this.width;
			this.height;
			this.playerScale;
			this.attackTime;

			this.text;
			this.player;
			this.signs;
			this.signCount;
			this.signGoal;
			this.radios;
			this.radioCount;
			this.radioGoal;
			this.cars;
			this.carCount;
			this.carGoal;
		},

	preload: function () {
		this.load.audio('City', [
			'sounds/City.ogg',
			'sounds/City.mp3',
			'sounds/City.m4a'
		]);
		this.load.audio('HitStrong0', [
			'sounds/HitStrong0.ogg',
			'sounds/HitStrong0.mp3',
			'sounds/HitStrong0.m4a'
		]);
		this.load.audio('HitStrong1', [
			'sounds/HitStrong1.ogg',
			'sounds/HitStrong1.mp3',
			'sounds/HitStrong1.m4a'
		]);
		this.load.audio('HitWeak0', [
			'sounds/HitWeak0.ogg',
			'sounds/HitWeak0.mp3',
			'sounds/HitWeak0.m4a'
		]);
		this.load.audio('HitWeak1', [
			'sounds/HitWeak1.ogg',
			'sounds/HitWeak1.mp3',
			'sounds/HitWeak1.m4a'
		]);
		this.load.audio('HitWeak2', [
			'sounds/HitWeak2.ogg',
			'sounds/HitWeak2.mp3',
			'sounds/HitWeak2.m4a'
		]);
		this.load.audio('Victory', [
			'sounds/Victory.ogg',
			'sounds/Victory.mp3',
			'sounds/Victory.m4a'
		]);
		this.load.image('money', 'images/money.png');
		this.load.image('coin', 'images/coin.png');
		this.load.image('white', 'images/white.png');
		this.load.image('kid_idle0', 'images/player/kid_idle0.png');
		this.load.image('kid_walk0', 'images/player/kid_walk0.png');
		this.load.image('kid_walk1', 'images/player/kid_walk1.png');
		this.load.image('kid_walk2', 'images/player/kid_walk2.png');
		this.load.image('kid_walk3', 'images/player/kid_walk3.png');
		this.load.image('kid_walk4', 'images/player/kid_walk4.png');
		this.load.image('kid_walk5', 'images/player/kid_walk5.png');
		this.load.image('kid_attack0', 'images/player/kid_attack0.png');
		this.load.image('kid_attack1', 'images/player/kid_attack1.png');
		this.load.image('kid_attack2', 'images/player/kid_attack2.png');
		this.load.image('kid_attack3', 'images/player/kid_attack3.png');
		this.load.image('detail0', 'images/forest/detail0.png');
		this.load.image('detail1', 'images/forest/detail1.png');
		this.load.image('detail2', 'images/forest/detail2.png');
		this.load.image('detail3', 'images/forest/detail3.png');
		this.load.image('detail4', 'images/forest/detail4.png');
		this.load.image('detail5', 'images/forest/detail5.png');
		this.load.image('detail6', 'images/forest/detail6.png');
		this.load.image('junk', 'images/city/junk.png');
		this.load.image('radio', 'images/city/radio.png');
		this.load.image('sign', 'images/city/sign.png');
		this.load.image('car', 'images/city/car.png');
	},

	create: function () {
		victory = false;
		if (music) music.stop();
		music = this.sound.add('City');
		music.setLoop(true);
		music.play();

		this.anims.create({
			key: 'idle',
			frames: [
				{ key: 'kid_idle0' }
			],
			frameRate: 12,
			repeat: -1
		});

		this.anims.create({
			key: 'walk',
			frames: [
				{ key: 'kid_walk0' },
				{ key: 'kid_walk1' },
				{ key: 'kid_walk2' },
				{ key: 'kid_walk3' },
				{ key: 'kid_walk4' },
				{ key: 'kid_walk5' }
			],
			frameRate: 8,
			repeat: -1
		});

		this.anims.create({
			key: 'attack0',
			frames: [
				{ key: 'kid_attack0' }
			],
			frameRate: 12,
			repeat: -1
		});

		this.anims.create({
			key: 'attack1',
			frames: [
				{ key: 'kid_attack1' }
			],
			frameRate: 12,
			repeat: -1
		});

		this.anims.create({
			key: 'attack2',
			frames: [
				{ key: 'kid_attack2' }
			],
			frameRate: 12,
			repeat: -1
		});

		this.anims.create({
			key: 'attack3',
			frames: [
				{ key: 'kid_attack3' }
			],
			frameRate: 12,
			repeat: -1
		});

		this.signCount = 0;
		this.carCount = 0;
		this.radioCount = 0;
		this.signGoal = 5;
		this.carGoal = 5;
		this.radioGoal = 5;
		this.width = WIDTH * 3;
		this.height = WIDTH * 3;
		this.add.image(this.width / 2, this.height / 2, 'white').setScale(this.width / 16, this.height / 16).setTint(0x838383);

		var i;
		for (i = 0; i < 160; i++) {
			this.add.image(Phaser.Math.FloatBetween(0, this.width), Phaser.Math.FloatBetween(0, this.height), 'detail' + Phaser.Math.Between(0, 6));
		}
		this.signs = new Array();
		for (i = 0; i < 16; i++) {
			var sign = this.physics.add.sprite(Phaser.Math.FloatBetween(0, this.width), Phaser.Math.FloatBetween(0, this.height), 'sign');
			sign.hp = 2;
			this.signs.push(sign);
		}
		this.cars = new Array();
		for (i = 0; i < 16; i++) {
			var car = this.physics.add.sprite(Phaser.Math.FloatBetween(0, this.width), Phaser.Math.FloatBetween(0, this.height), 'car');
			car.hp = 5;
			this.cars.push(car);
		}
		this.radios = new Array();
		for (i = 0; i < 16; i++) {
			var radio = this.physics.add.sprite(Phaser.Math.FloatBetween(0, this.width), Phaser.Math.FloatBetween(0, this.height), 'radio');
			radio.hp = 3;
			this.radios.push(radio);
		}

		InitParticles(this);

		this.playerScale = 0.8;
		this.attackTime = 0;
		this.player = this.physics.add.sprite(this.width / 2, this.height / 2, 'kid_idle0').setOrigin(0.5, 0.5).setScale(this.playerScale);
		this.cameras.main.startFollow(this.player);
		this.physics.world.setBoundsCollision(true, true, true, true);
		this.cameras.main.setBounds(0, 0, this.width, this.height);
		this.cameras.main.setZoom(0.8);

		this.text = this.add.text(-100, -50, '', { align: 'left' }).setFont('32px Arial Black').setFill('#ffffff').setShadow(2, 2, "#444444", 2).setScrollFactor(0);

		cursors = this.input.keyboard.createCursorKeys();
		spacebar = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.SPACE);
	},

	update: function () {
		if (victory) return;
		var speed = getMovementSpeed();
		if ((cursors.left.isDown || cursors.right.isDown) == (cursors.up.isDown || cursors.down.isDown)) {
			speed *= 144 / 200;
		}

		if (this.attackTime <= 0) {
			if (Phaser.Input.Keyboard.JustDown(cursors.left) || Phaser.Input.Keyboard.JustDown(cursors.right) || Phaser.Input.Keyboard.JustDown(cursors.up) || Phaser.Input.Keyboard.JustDown(cursors.down)) {
				this.player.play('walk');
			}
			else if (!cursors.right.isDown && !cursors.left.isDown && !cursors.up.isDown && !cursors.down.isDown) {
				this.player.play('idle');
			}

			if (cursors.left.isDown) {
				this.player.setVelocityX(-speed);
				this.player.setScale(-this.playerScale, this.playerScale);
			}
			else if (cursors.right.isDown) {
				this.player.setVelocityX(speed);
				this.player.setScale(this.playerScale, this.playerScale);
			}
			else {
				this.player.setVelocityX(0);
			}

			if (cursors.up.isDown) {
				this.player.setVelocityY(-speed);
			}
			else if (cursors.down.isDown) {
				this.player.setVelocityY(speed);
			}
			else {
				this.player.setVelocityY(0);
			}
		}

		if (this.attackTime <= 0) {
			if (spacebar.isDown) {
				this.player.setVelocityX(0);
				this.player.setVelocityY(0);
				this.attackTime = getAttackTime();
				this.player.play('attack' + Phaser.Math.Between(0, 3));
				var i;
				for (i = 0; i < this.signs.length; i++) {
					var thing = this.signs[i];
					if (isClose(this.player, thing, 150)) {
						thing.hp--;
						HandleHitEffects(this, thing);
						if (thing.hp <= 0) {
							this.signCount++;
							this.add.sprite(thing.x, thing.y, 'junk');
							thing.destroy();
							this.signs.splice(i, 1);
						}
						else {
							thing.setTint(0xff0000);
						}
					}
				}
				for (i = 0; i < this.radios.length; i++) {
					var thing = this.radios[i];
					if (isClose(this.player, thing, 150)) {
						thing.hp--;
						HandleHitEffects(this, thing);
						if (thing.hp <= 0) {
							this.radioCount++;
							this.add.sprite(thing.x, thing.y, 'junk');
							thing.destroy();
							this.radios.splice(i, 1);
						}
						else if (thing.hp > 1) {
							thing.setTint(0xff8888);
						}
						else {
							thing.setTint(0xff0000);
						}
					}
				}
				for (i = 0; i < this.cars.length; i++) {
					var thing = this.cars[i];
					if (isClose(this.player, thing, 150)) {
						thing.hp--;
						HandleHitEffects(this, thing);
						if (thing.hp <= 0) {
							this.carCount++;
							this.add.sprite(thing.x, thing.y, 'junk');
							thing.destroy();
							this.cars.splice(i, 1);
						}
						else if (thing.hp > 3) {
							thing.setTint(0xffcccc);
						}
						else if (thing.hp > 2) {
							thing.setTint(0xff8888);
						}
						else if (thing.hp > 1) {
							thing.setTint(0xff4444);
						}
						else {
							thing.setTint(0xff0000);
						}
					}
				}
			}
		}
		else {
			this.attackTime -= 1;
		}

		if (this.signCount > 0 || this.radioCount > 0 || this.carCount > 0) {
			this.text.setText('Cash: ' + moneyAmount + '$\n\n' + 'Missions:\nSigns destroyed: ' + this.signCount + ' / ' + this.signGoal + '\nRadios destroyed: ' + this.radioCount + ' / ' + this.radioGoal + '\nCars destroyed: ' + this.carCount + ' / ' + this.carGoal);
			if (this.signCount >= this.signGoal && this.radioCount >= this.radioGoal && this.carCount >= this.carGoal) {
				victory = true;
				music.stop();
				this.sound.add('Victory').play();

				this.tweens.add({
					targets: this.player,
					rotation: Phaser.Math.FloatBetween(-25, 25),
					scaleX: Phaser.Math.FloatBetween(-25, 25),
					scaleY: Phaser.Math.FloatBetween(-25, 25),
					ease: 'Sine.easeInOut',
					duration: 2000
				});

				this.time.addEvent({ delay: 2000, callback: cityEnd, callbackScope: this });
			}
		}
		else {
			this.text.setText('Cash: ' + moneyAmount + '$\n\n' + 'Missions:\nDestroy ' + this.signGoal + ' signs.\nDestroy ' + this.radioGoal + ' radios.\nDestroy ' + this.carGoal + ' cars.');
		}
	}
});

function cityEnd() {
	dialogueLines = new Array(
		'My little destroyer! My garbage person!',
		'Did I just destroy people\'s cars?',
		'Yes, that happened.',
		'Well I wish that didn\'t happen!',
		'Well, it already happened. Move on.',
		'At least the cash is good I guess.',
		'My garbage person likes cash.',
		'Please don\'t call me that...',
		'That is who you are! Be proud!',
		'Why?',
		'Because. There are a lot of garbage people\non earth that do not know that they are garbage.\nYou know you are garbage.\nThat makes you a good person.',
		'Really?',
		'Yeah I mean that is my theory at the moment.',
		'Good to know I guess.',
		'Ready for one last mission for today?',
		'Please don\'t be private property...',
		'We are going to destroy houses.',
		'Of course...',
		'Let\'s go!'
	);
	dialogueNextScene = 'road';
	this.scene.start('store');
}