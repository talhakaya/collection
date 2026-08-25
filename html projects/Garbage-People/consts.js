var WIDTH = 1280;
var HEIGHT = 720;
var cursors;
var spacebar;
var dialogueLines = new Array(
		'yo',
		'hey',
		'all good?',
		'yea'
		);
var dialogueNextScene = 'bathroom0';
var music;
var victory;
var moneyAmount = 0;
var particles;
var particleIndex = 0;
var attackTimeBase = 60;
var attackTimeLevel = 0;
var attackTimeLevelInc = -2;
var attackTimePrice = 250;
var movementSpeedBase = 200;
var movementSpeedLevel = 0;
var movementSpeedLevelInc = 25;
var movementSpeedPrice = 250;
var isGameFinished = false;