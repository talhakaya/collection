var config = {
	type: Phaser.AUTO,
	width: WIDTH,
	height: HEIGHT,
	parent: 'game',
	scene: [Loading, Bathroom0, Bathroom1, Dialogue, Forest, Store, City, Road],
	physics: {
		default: 'arcade'
	}
};

var game = new Phaser.Game(config);

function isClose(obj0, obj1, dist) {
	return Phaser.Math.Distance.Between(obj0.x, obj0.y, obj1.x, obj1.y) < dist;
}

function HandleHitEffects(scene, thing) {
	if (thing.hp > 0) {
		moneyAmount += Phaser.Math.Between(3, 7);
		var i = 0;
		for (i = 0; i < 5; i++) {
			Particle(scene, thing.x, thing.y);
		}
		scene.tweens.add({
			targets: thing,
			rotation: Phaser.Math.FloatBetween(-0.02, 0.02) * (9 - thing.hp),
			scaleX: 1 + Phaser.Math.FloatBetween(-0.02, 0.02) * (9 - thing.hp),
			scaleY: 1 + Phaser.Math.FloatBetween(-0.02, 0.02) * (9 - thing.hp),
			ease: 'Sine.easeInOut',
			duration: 150,
			yoyo: true
		});
		scene.sound.add('HitWeak' + Phaser.Math.Between(0, 2)).play();
	}
	else {
		moneyAmount += Phaser.Math.Between(15, 65);
		var i = 0;
		for (i = 0; i < 20; i++) {
			Particle(scene, thing.x, thing.y);
		}
		scene.sound.add('HitStrong' + Phaser.Math.Between(0, 1)).play();
	}
}

function InitParticles(scene) {
	particles = new Array();
	var i = 0;
	for (i = 0; i < 100; i++) {
		if (Phaser.Math.FloatBetween(0, 1) > 0.5) {
			var p = scene.add.sprite(0, 0, 'money');
			p.setAlpha(0);
			particles.push(p);
		}
		else {
			var p = scene.add.sprite(0, 0, 'coin');
			p.setAlpha(0);
			particles.push(p);
		}
	}
}

function Particle(scene, x, y) {
	var p = particles[particleIndex];
	p.setAlpha(1);
	p.x = x + Phaser.Math.FloatBetween(-50, 50);
	p.y = y + Phaser.Math.FloatBetween(-50, 50);
	
	var dur = Phaser.Math.FloatBetween(250, 500)
	scene.tweens.add({
		targets: p,
		y: '-= ' + Phaser.Math.FloatBetween(0, 300),
		ease: 'Sine.easeInOut',
		duration: dur * 0.5,
		yoyo: true
	});
	if (Phaser.Math.FloatBetween(0, 1) > 0.5) {
		scene.tweens.add({
			targets: p,
			x: '+= ' + Phaser.Math.FloatBetween(0, 100),
			rotation: Phaser.Math.FloatBetween(-3, 3),
			alpha: 0,
			ease: 'Linear',
			duration: dur
		});
	}
	else {
		scene.tweens.add({
			targets: p,
			x: '-= ' + Phaser.Math.FloatBetween(0, 100),
			rotation: Phaser.Math.FloatBetween(-3, 3),
			alpha: 0,
			ease: 'Linear',
			duration: dur
		});
	}
	particleIndex++;
	if (particleIndex >= particles.length) particleIndex = 0;
}

function getAttackTime() {
	return Math.max(10, attackTimeBase + attackTimeLevel * attackTimeLevelInc);
}

function getMovementSpeed() {
	return Math.min(600, movementSpeedBase + movementSpeedLevel * movementSpeedLevelInc);
}