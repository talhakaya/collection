var Road = new Phaser.Class({

	Extends: Phaser.Scene,

	initialize:

		function City() {
			Phaser.Scene.call(this, { key: 'road' });
			this.width;
			this.height;
			this.playerScale;
			this.attackTime;

			this.text;
			this.player;
			this.buildings;
			this.buildingCount;
			this.buildingGoal;
		},

	preload: function () {
		this.load.audio('Skyscraper', [
			'sounds/Skyscraper.ogg',
			'sounds/Skyscraper.mp3',
			'sounds/Skyscraper.m4a'
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
		this.load.image('building0', 'images/bathroom1/building0.png');
		this.load.image('building1', 'images/bathroom1/building1.png');
		this.load.image('building2', 'images/bathroom1/building2.png');
		this.load.image('building3', 'images/bathroom1/building3.png');
		this.load.image('mess', 'images/road/mess.png');
	},

	create: function () {
		victory = false;
		if (music) music.stop();
		music = this.sound.add('Skyscraper');
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

		this.buildingCount = 0;
		this.buildingGoal = 15;
		this.width = WIDTH * 3;
		this.height = WIDTH * 3;
		this.add.image(this.width / 2, this.height / 2, 'white').setScale(this.width / 16, this.height / 16).setTint(0x525252);

		var i;
		for (i = 0; i < 160; i++) {
			this.add.image(Phaser.Math.FloatBetween(0, this.width), Phaser.Math.FloatBetween(0, this.height), 'detail' + Phaser.Math.Between(0, 6));
		}
		this.buildings = new Array();
		for (i = 0; i < 32; i++) {
			var building = this.physics.add.sprite(Phaser.Math.FloatBetween(0, this.width), Phaser.Math.FloatBetween(0, this.height), 'building' + Phaser.Math.Between(0, 3));
			building.hp = 10;
			this.buildings.push(building);
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
				for (i = 0; i < this.buildings.length; i++) {
					var thing = this.buildings[i];
					if (isClose(this.player, thing, 300)) {
						thing.hp--;
						HandleHitEffects(this, thing);
						if (thing.hp <= 0) {
							this.buildingCount++;
							this.add.sprite(thing.x, thing.y, 'mess');
							thing.destroy();
							this.buildings.splice(i, 1);
						}
						else if (thing.hp > 8){
							thing.setTint(0xffeeee);
						}
						else if (thing.hp > 7){
							thing.setTint(0xffdddd);
						}
						else if (thing.hp > 6){
							thing.setTint(0xffcccc);
						}
						else if (thing.hp > 5){
							thing.setTint(0xffaaaa);
						}
						else if (thing.hp > 4){
							thing.setTint(0xff9999);
						}
						else if (thing.hp > 3){
							thing.setTint(0xff6666);
						}
						else if (thing.hp > 2){
							thing.setTint(0xff4444);
						}
						else if (thing.hp > 1){
							thing.setTint(0xff2222);
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

		if (this.buildingCount > 0) {
			this.text.setText('Cash: ' + moneyAmount + '$\n\n' + 'Missions:\nBuildings destroyed: ' + this.buildingCount + ' / ' + this.buildingGoal);
			if (this.buildingCount >= this.buildingGoal) {
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

				this.time.addEvent({ delay: 2000, callback: roadEnd, callbackScope: this });
			}
		}
		else {
			this.text.setText('Cash: ' + moneyAmount + '$\n\n' + 'Missions:\nDestroy ' + this.buildingGoal + ' buildings.');
		}
	}
});

function roadEnd() {
	isGameFinished = true;
	dialogueLines = new Array(
		'You must be so tired!',
		'You said we were going to destroy some garbage.',
		'Yes.',
		'We have destroyed everything BUT garbage!',
		'Yes, that happened.',
		'I shouldn\'t have trusted you in the first place.',
		'If you didn\'t do what I said\nI would have crushed you with my hand.',
		'Okay okay, I understand the power dynamics here.',
		'But aren\'t you proud?\nWe have achieved so much!',
		'Not sure I would call it an achievement.',
		'You have made some cash.',
		'Indeed I did.',
		'You have made a friend.',
		'Friend?',
		'Of course! I am your friend.',
		'Okay.',
		'Okay!?',
		'Just please don\'t kill me.',
		'No promises.'
	);
	dialogueNextScene = 'loading';
	this.scene.start('dialogue');
}