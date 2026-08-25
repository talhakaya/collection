var Forest = new Phaser.Class({

	Extends: Phaser.Scene,

	initialize:

		function Forest() {
			Phaser.Scene.call(this, { key: 'forest' });
			this.width;
			this.height;
			this.playerScale;
			this.attackTime;

			this.text;
			this.player;
			this.rocks;
			this.trees;
			this.treeCount;
			this.rockCount;
			this.treeGoal;
			this.rockGoal;
		},

	preload: function () {
		this.load.audio('Forest', [
			'sounds/Forest.ogg',
			'sounds/Forest.mp3',
			'sounds/Forest.m4a'
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
		this.load.image('yellow', 'images/yellow.png');
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
		this.load.image('dirt', 'images/forest/dirt.png');
		this.load.image('rock0', 'images/forest/rock0.png');
		this.load.image('rock1', 'images/forest/rock1.png');
		this.load.image('rock2', 'images/forest/rock2.png');
		this.load.image('trunk', 'images/forest/trunk.png');
		this.load.image('tree0', 'images/forest/tree0.png');
		this.load.image('tree1', 'images/forest/tree1.png');
		this.load.image('tree2', 'images/forest/tree2.png');
		this.load.image('detail0', 'images/forest/detail0.png');
		this.load.image('detail1', 'images/forest/detail1.png');
		this.load.image('detail2', 'images/forest/detail2.png');
		this.load.image('detail3', 'images/forest/detail3.png');
		this.load.image('detail4', 'images/forest/detail4.png');
		this.load.image('detail5', 'images/forest/detail5.png');
		this.load.image('detail6', 'images/forest/detail6.png');
	},

	create: function () {
		victory = false;
		if (music) music.stop();
		music = this.sound.add('Forest');
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

		this.treeCount = 0;
		this.rockCount = 0;
		this.treeGoal = 5;
		this.rockGoal = 5;
		this.width = WIDTH * 3;
		this.height = WIDTH * 3;
		this.add.image(this.width / 2, this.height / 2, 'yellow').setScale(this.width / 16, this.height / 16);

		var i;
		for (i = 0; i < 160; i++) {
			this.add.image(Phaser.Math.FloatBetween(0, this.width), Phaser.Math.FloatBetween(0, this.height), 'detail' + Phaser.Math.Between(0, 6));
		}
		this.trees = new Array();
		for (i = 0; i < 16; i++) {
			var tree = this.physics.add.sprite(Phaser.Math.FloatBetween(0, this.width), Phaser.Math.FloatBetween(0, this.height), 'tree' + Phaser.Math.Between(0, 2));
			tree.hp = 3;
			this.trees.push(tree);
		}
		this.rocks = new Array();
		for (i = 0; i < 16; i++) {
			var rock = this.physics.add.sprite(Phaser.Math.FloatBetween(0, this.width), Phaser.Math.FloatBetween(0, this.height), 'rock' + Phaser.Math.Between(0, 2));
			rock.hp = 3;
			this.rocks.push(rock);
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
				for (i = 0; i < this.rocks.length; i++) {
					var thing = this.rocks[i];
					if (isClose(this.player, thing, 150)) {
						thing.hp--;
						HandleHitEffects(this, thing);
						if (thing.hp <= 0) {
							this.rockCount++;
							this.add.sprite(thing.x, thing.y, 'dirt');
							thing.destroy();
							this.rocks.splice(i, 1);
						}
						else if (thing.hp > 1) {
							thing.setTint(0xff8888);
						}
						else {
							thing.setTint(0xff0000);
						}
					}
				}
				for (i = 0; i < this.trees.length; i++) {
					var thing = this.trees[i];
					if (isClose(this.player, thing, 150)) {
						thing.hp--;
						HandleHitEffects(this, thing);
						if (thing.hp <= 0) {
							this.treeCount++;
							this.add.sprite(thing.x, thing.y, 'trunk');
							thing.destroy();
							this.trees.splice(i, 1);
						}
						else if (thing.hp > 1) {
							thing.setTint(0xff8888);
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

		if (this.rockCount > 0 || this.treeCount > 0) {
			this.text.setText('Cash: ' + moneyAmount + '$\n\n' + 'Missions:\nRocks destroyed: ' + this.rockCount + ' / ' + this.rockGoal + '\nTrees destroyed: ' + this.treeCount + ' / ' + this.treeGoal);
			if (this.rockCount >= this.rockGoal && this.treeCount >= this.treeGoal) {
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

				this.time.addEvent({ delay: 2000, callback: forestEnd, callbackScope: this });
			}
		}
		else {
			this.text.setText('Cash: ' + moneyAmount + '$\n\n' + 'Missions:\nDestroy ' + this.rockGoal + ' rocks.\nDestroy ' + this.treeGoal + ' trees.');
		}
	}
});

function forestEnd() {
	dialogueLines = new Array(
		'You did a great job!',
		'I destroyed nature!',
		'Yes, that happened.',
		'Why did I do that?',
		'You had no option. I told you to do so.',
		'Why did I do what you told me?',
		'Because I am a creepy giant and you are scared.',
		'Oh okay.',
		'Also you are making crazy cash, dude.',
		'I do like that.',
		'Don\'t worry about the trees.\nWe will destroy some garbage now.',
		'That sounds better.',
		'...',
		'Hey, can I get dressed now?',
		'NO!',
		'Why?',
		'Garbage people don\'t get dressed. They only destroy.',
		'Don\'t call me a garbage person! Sounds bad!',
		'Nothing bad about being a garbage person.',
		'Yeah?',
		'Yeah! Be proud of your garbageness.',
		'Why?',
		'I will explain later.',
		'...',
		'Let\'s destroy!'
	);
	dialogueNextScene = 'city';
	this.scene.start('store');
}