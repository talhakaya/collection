var Dialogue = new Phaser.Class({

	Extends: Phaser.Scene,

	initialize:

		function Dialogue() {
			Phaser.Scene.call(this, { key: 'dialogue' });
			this.textPlayer;
			this.textCreep;
			this.shine0;
			this.shine1;
			this.shine2;
			this.player;
			this.creep;
			this.timer;
			this.progress;
			this.progressOld;
		},

	preload: function () {
		this.load.audio('Dialogue', [
			'sounds/Dialogue.ogg',
			'sounds/Dialogue.mp3',
			'sounds/Dialogue.m4a'
		]);
		this.load.image('creep', 'images/bathroom1/creep.png');
		this.load.image('kid_idle0', 'images/player/kid_idle0.png');
		this.load.image('shine', 'images/shine.png');
	},

	create: function () {
		dialogueIndex = 0;
		if (music) music.stop();
		music = this.sound.add('Dialogue');
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
		//this.shine0.blendMode = Phaser.BlendModes.MULTIPLY;

		this.creep = this.add.image(640, HEIGHT, 'creep').setOrigin(0.5, 1);
		this.player = this.add.image(400, HEIGHT - 100, 'kid_idle0').setOrigin(0.5, 0.8).setScale(0.6);

		this.textPlayer = this.add.text(60, 330, '', { align: 'left' }).setFont('32px Arial Black').setFill('#eaffa2').setShadow(2, 2, "#944149", 2);

		this.textCreep = this.add.text(300, 230, '', { align: 'right' }).setFont('32px Arial Black').setFill('#a993d3').setShadow(2, 2, "#944149", 2);

    	spacebar = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.SPACE);
		dialogueSet(this);
	},

	update: function () {
		this.shine0.rotation -= 0.004;
		this.shine1.rotation += 0.002;
		this.shine2.rotation -= 0.001;

		if (Phaser.Input.Keyboard.JustDown(spacebar)) {
			dialogueSet(this);
		}
	}

});

var dialogueIndex;
function dialogueSet(scene) {
	if (dialogueIndex >= dialogueLines.length) {
		scene.textPlayer.setText('');
		scene.textCreep.setText('');
		dialogueEndAnim(scene);
	}
	else {
		if (dialogueIndex % 2 == 0) {
			scene.textPlayer.setText('');
			scene.textCreep.setText(dialogueLines[dialogueIndex]);
		}
		else {
			scene.textPlayer.setText(dialogueLines[dialogueIndex]);
			scene.textCreep.setText('');
		}
	}
	dialogueIndex++;
}

function dialogueEndAnim(scene) {
	scene.tweens.add({
		targets: scene.player,
		y: '-=400',
		rotation: -4.5,
		ease: 'Sine.easeInOut',
		duration: 1500
	});
	scene.tweens.add({
		targets: scene.shine0,
		y: '-=100',
		ease: 'Sine.easeInOut',
		duration: 1500
	});
	scene.tweens.add({
		targets: scene.shine1,
		y: '-=100',
		ease: 'Sine.easeInOut',
		duration: 1500
	});
	scene.tweens.add({
		targets: scene.shine2,
		y: '-=100',
		ease: 'Sine.easeInOut',
		duration: 1500
	});
	scene.tweens.add({
		targets: scene.creep,
		y: '+=200',
		rotation: 0.1,
		ease: 'Sine.easeInOut',
		duration: 1500
	});

	scene.time.addEvent({ delay: 1500, callback: dialogueEnd, callbackScope: scene });
}

function dialogueEnd() {
	this.scene.start(dialogueNextScene);
}