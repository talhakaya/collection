var Bathroom1 = new Phaser.Class({

	Extends: Phaser.Scene,

	initialize:

		function Bathroom1() {
			Phaser.Scene.call(this, { key: 'bathroom1' });

			this.width;
			this.height;
			this.timer;
			this.progress;
			this.progressOld;

			this.bgBlack;
			this.bg;
			this.curtain;
			this.lightbulb;
			this.toilet;
			this.light;
			this.player;
			this.rock0;
			this.rock1;
			this.rock2;
			this.text0;
			this.text1;
		},

	preload: function () {
		this.load.audio('Bathroom1', [
			'sounds/Bathroom1.ogg',
			'sounds/Bathroom1.mp3',
			'sounds/Bathroom1.m4a'
		]);
		this.load.audio('Outside', [
			'sounds/Outside.ogg',
			'sounds/Outside.mp3',
			'sounds/Outside.m4a'
		]);
		this.load.audio('HitStrong0', [
			'sounds/HitStrong0.ogg',
			'sounds/HitStrong0.mp3',
			'sounds/HitStrong0.m4a'
		]);
		this.load.image('black', 'images/black.png');
		this.load.image('sky', 'images/sky.png');
		this.load.image('arm1', 'images/bathroom1/arm.png');
		this.load.image('bg', 'images/bathroom1/bg.jpg');
		this.load.image('bg_broken', 'images/bathroom1/bg_broken.png');
		this.load.image('building0', 'images/bathroom1/building0.png');
		this.load.image('building1', 'images/bathroom1/building1.png');
		this.load.image('building2', 'images/bathroom1/building2.png');
		this.load.image('building3', 'images/bathroom1/building3.png');
		this.load.image('creep', 'images/bathroom1/creep.png');
		this.load.image('curtain0', 'images/bathroom1/curtain0.png');
		this.load.image('curtain1', 'images/bathroom1/curtain1.png');
		this.load.image('curtain2', 'images/bathroom1/curtain2.png');
		this.load.image('curtain3', 'images/bathroom1/curtain3.png');
		this.load.image('kid_brush', 'images/bathroom1/kid_brush.png');
		this.load.image('light', 'images/bathroom0/light.png');
		this.load.image('lightbulb1', 'images/bathroom1/lightbulb.png');
		this.load.image('road', 'images/bathroom1/road.png');
		this.load.image('sunshine', 'images/bathroom1/sunshine.png');
		this.load.image('toilet', 'images/bathroom1/toilet.png');
		this.load.image('kid_scared', 'images/player/kid_scared.png');
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
		this.load.image('rock0', 'images/forest/rock0.png');
		this.load.image('rock1', 'images/forest/rock1.png');
		this.load.image('rock2', 'images/forest/rock2.png');
	},

	create: function () {
		isMusicPlaying = false;
		isMovementLocked = false;
		isHitMade = false;
		canAnimatePlayer = true;
		this.physics.world.setBoundsCollision(true, true, true, true);

		this.sound.add('Bathroom1').play();

		this.width = WIDTH * 6;
		this.height = HEIGHT * 1.5;
    	this.cameras.main.setBounds(0, 0, this.width, this.height);

		var sky = this.add.image(this.width * 0.5 + 480 + 265 + 320, this.height * 0.5, 'sky');
		sky.setScale(this.width / 2 + 320, this.height / 256);
		
		this.add.image(1500, this.height - 200, 'building0').setOrigin(0.5, 1);
		this.add.image(2200, this.height - 220, 'building1').setOrigin(0.5, 1);
		this.add.image(2800, this.height - 220, 'building2').setOrigin(0.5, 1);
		this.add.image(3300, this.height - 200, 'building3').setOrigin(0.5, 1);

		var i;
		for (i = 1; i <= 5; i++) {
			this.add.image(i * 1015, this.height - 124, 'road');
		}

		this.bgBlack = this.add.sprite(sky.x - 320, sky.y, 'black');
		this.bgBlack.setScale(this.width / 16, this.height / 16);

		this.rock0 = this.add.image(bathroom1XRoadHit + 90, this.height - 100, 'rock0');
		this.rock1 = this.add.image(bathroom1XRoadHit + 90, this.height - 150, 'rock1');
		this.rock2 = this.add.image(bathroom1XRoadHit + 90, this.height - 200, 'rock2');

		this.add.image(bathroom1XJumpStart + 390, this.height, 'creep').setOrigin(0.5, 1).setScale(1.2);

		this.bg = this.add.sprite(960 * 0.5, this.height - 270, 'bg');
		this.cameras.main.startFollow(this.bg);
		this.cameras.main.setZoom(1.33);

		this.curtain = this.add.sprite(536, this.height - 540 + 264, 'curtain0');

		this.lightbulb = this.add.sprite(400, this.height - 540 + 44, 'lightbulb1');
		this.lightbulb.setOrigin(0.5, 0);

		this.toilet = this.add.sprite(480 + 255, this.height - 540 + 422, 'toilet');

		this.light = this.add.sprite(400, this.height - 270, 'light');
		this.light.setAlpha(0.4);
		this.light.blendMode = Phaser.BlendModes.ADD;
		this.light.setScale(1.3);
		
		this.player = this.physics.add.sprite(340, this.bg.y, 'kid_brush');

		this.arm = this.add.image(240 + 114, this.height - 520 + 272, 'arm1');
		this.arm.setOrigin(49 / 64, 34 / 70);
		this.arm.rotation = 0.01;
		
		this.tweens.add({
			targets: this.arm,
			rotation: -0.04,
			ease: 'Sine.easeInOut',
			duration: 50,
			delay: 0,
			repeat: 24,
			yoyo: true,
			repeatDelay: 50
		});

		this.progress = 0.0;
		this.progressOld = 0.0;
		this.timer = this.time.addEvent({ delay: 3000, callback: bathroom1Event0, callbackScope: this });
		cursors = this.input.keyboard.createCursorKeys();
    	spacebar = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.SPACE);

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
			key: 'attack3',
			frames: [
				{ key: 'kid_attack3' }
			],
			frameRate: 12,
			repeat: -1
		});

		this.anims.create({
			key: 'curtainBlow',
			frames: [
				{ key: 'curtain1' },
				{ key: 'curtain2' },
				{ key: 'curtain3' },
				{ key: 'curtain2' },
				{ key: 'curtain3' },
				{ key: 'curtain2' },
				{ key: 'curtain3' },
				{ key: 'curtain2' },
				{ key: 'curtain3' },
				{ key: 'curtain2' },
				{ key: 'curtain1' },
				{ key: 'curtain0' }
			],
			frameRate: 8
		});

		this.anims.create({
			key: 'curtainBlowSoft',
			frames: [
				{ key: 'curtain0' }
			],
			frameRate: 4,
			repeat: -1
		});

		bathroom1SetPlayerY(this, this.player);
	},

	update: function () {

		this.progress = this.timer.getProgress();
		if (this.progress > 0.8 && this.progressOld <= 0.8) {
			this.bg.setTexture('bg_broken');
			this.arm.destroy();
			this.bgBlack.destroy();
			this.player.setTexture('kid_scared');
			this.curtain.play('curtainBlow');
			this.curtain.anims.chain('curtainBlowSoft');
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
					x: 400 - 50 * (5 - i),
					ease: 'Sine.easeInOut',
					duration: 250,
					delay: 0 + 500 * (i - 1),
					yoyo: true
				});
				this.tweens.add({
					targets: this.light,
					x: 400 + 50 * (5 - i),
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
				x: 400,
				ease: 'Sine.easeInOut',
				duration: 250,
				delay: 500 * 5
			});
			this.tweens.add({
				targets: this.toilet,
				x: 224,
				y: this.height - 540 + 94,
				rotation: -4.5,
				ease: 'Sine.easeInOut',
				duration: 150
			});
			this.cameras.main.startFollow(this.player, false, 0.4, 0);
		}

		if (this.progress >= 1) {
			if (canAnimatePlayer && !isMovementLocked) {
				if (Phaser.Input.Keyboard.JustDown(cursors.left) || Phaser.Input.Keyboard.JustDown(cursors.right)) {
					if (this.text0) this.text0.destroy();
					this.player.play('walk');
				}
				else if (!cursors.right.isDown && !cursors.left.isDown){
					this.player.play('idle');
				}

				if (cursors.left.isDown && !cursors.right.isDown) {
					this.player.setVelocityX(-200);
					this.player.setScale(-1, 1);
				}
				else if (cursors.right.isDown && !cursors.left.isDown){
					this.player.setVelocityX(200);
					this.player.setScale(1, 1);
				}
				else {
					this.player.setVelocity(0);
				}
			}
			else if (!isHitMade && Phaser.Input.Keyboard.JustDown(spacebar)) {
				if (this.text1) this.text1.destroy();
				this.sound.add('HitStrong0').play();
				this.player.play('attack3');
				this.time.addEvent({ delay: 500, callback: bathroom1Event1, callbackScope: this });
				isHitMade = true;
				canAnimatePlayer = false;
				isMovementLocked = false;
				
				this.tweens.add({
					targets: this.rock0,
					rotation: '+=3',
					x: '+=10',
					y: '+=10',
					ease: 'Sine.easeInOut',
					duration: 100
				});
				this.tweens.add({
					targets: this.rock1,
					rotation: '+=4',
					x: '+=50',
					y: '+=70',
					ease: 'Sine.easeInOut',
					duration: 130
				});
				this.tweens.add({
					targets: this.rock2,
					rotation: '-=6',
					x: '-=70',
					y: '+=140',
					ease: 'Sine.easeInOut',
					duration: 150
				});
			}
			bathroom1SetPlayerY(this, this.player);
		}
		this.progressOld = this.progress;
	}

});

var isMusicPlaying = false;
var isMovementLocked = false;
var isHitMade = false;
var canAnimatePlayer = true;
var bathroom1XStart = 240;
var bathroom1XEnd = 740;
var bathroom1XOutside = 830;
var bathroom1XRoadStart = 960;
var bathroom1XRoadHit = 3600;
var bathroom1XJumpStart = 3900;
var bathroom1XJumpEnd = 4000;
var bathroom1XLevelEnd = 4030;
function bathroom1SetPlayerY(scene, player) {
	isMovementLocked = false;
	if (player.x < bathroom1XStart) {
		player.x = bathroom1XStart;
	}
	else if (player.x > bathroom1XLevelEnd) {
		bathroom1End(scene);
		return;
		player.x = bathroom1XLevelEnd;
	}

	if (player.x < bathroom1XEnd) {
		player.y = scene.height - 540 + 400;
	}
	else if (player.x < bathroom1XOutside) {
		var animRatio = (player.x - bathroom1XEnd) / (bathroom1XOutside - bathroom1XEnd);
		player.y = scene.height - 540 + 400 + animRatio * 50;
	}
	else if (player.x < bathroom1XRoadStart) {
		if (!isMusicPlaying) {
			isMusicPlaying = true;
			music = scene.sound.add('Outside');
			music.setLoop(true);
			music.play();
		}
		var animRatio = (player.x - bathroom1XOutside) / (bathroom1XRoadStart - bathroom1XOutside);
		player.y = scene.height - 540 + 450 - animRatio * 100;
	}
	else if (player.x < bathroom1XRoadHit) {
		player.y = scene.height - 540 + 350;
	}
	else if (player.x < bathroom1XJumpStart) {
		player.y = scene.height - 540 + 350;
		if (!isHitMade) {
			scene.text1 = scene.add.text(player.x, player.y - 250, 'Press SPACE to hit', { align: 'center' }).setFont('32px Arial Black').setFill('#ffffff').setShadow(2, 2, "#888888", 2);
			//tuto show text
			player.play('idle');
			isMovementLocked = true;
			player.setVelocityX(0);
		}
	}
	else if (player.x < bathroom1XJumpEnd) {
		var animRatio = (player.x - bathroom1XJumpStart) / (bathroom1XJumpEnd - bathroom1XJumpStart);
		player.y = scene.height - 540 + 350 - 200 * (animRatio - animRatio * animRatio) - 40 * animRatio;
		isMovementLocked = true;
	}
	else {
		player.y = scene.height - 540 + 310;
	}

	if (player.x < bathroom1XOutside) {
		scene.cameras.main.setZoom(1.33);
	}
	else if (player.x < bathroom1XOutside + 500) {
		var animRatio = (player.x - bathroom1XOutside) / 500;
		scene.cameras.main.setZoom(1.33 - animRatio * 0.43);
	}
	else {
		scene.cameras.main.setZoom(0.9);
	}
}

function bathroom1Event0() {
	canAnimatePlayer = true;
	this.text0 = this.add.text(this.player.x - 250, this.player.y - 250, 'Press DIRECTION BUTTONS to move', { align: 'center' }).setFont('32px Arial Black').setFill('#ffffff').setShadow(2, 2, "#888888", 2);
}

function bathroom1Event1() {
	canAnimatePlayer = true;
}

function bathroom1End(scene) {
	dialogueLines = new Array(
		'You are the chosen one.',
		'Did you just break my bathroom wall?',
		'Yes, that happened.',
		'You are going to pay for that.',
		'We have bigger issues at hand.',
		'Like what?',
		'How do you feel these days?',
		'I feel OK?',
		'No, think about it. How do you feel?',
		'I dunno. Good?',
		'Don\'t you feel like garbage?',
		'Garbage?',
		'Yes. Don\'t you? Garbage?',
		'What do you mean by feeling like garbage?',
		'Feeling bad, aimless, stupid. Cmon, catch up with me!',
		'Oh, yeah, I guess I do kind of feel like that.',
		'That is why you have been chosen.',
		'To do what exactly?',
		'Destroy some garbage.',
		'Why would I do that?',
		'$$$$$ I will pay you handsomely. $$$$$',
		'Ohhh okay! But can I get dressed before we do that?',
		'No.',
		'...',
		'Let\'s go.'
		);
	dialogueNextScene = 'forest';
	scene.scene.start('dialogue');
}