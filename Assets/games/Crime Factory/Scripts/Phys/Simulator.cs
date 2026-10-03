using UnityEngine;
using System.Collections.Generic;
using Collection.Controls;

namespace Games.CrimeFactory
{
	public class Simulator {
	    public static bool IsPaused {
	        get { return Platformer.isPaused && isPaused; }
	    }
	    private static bool isPaused;
	    public static float dt;
	    public static bool isBodiesDirty;
	    public List<PhysBox> allObjects;
	    public List<DynamicPhysBox> dynamicObjects;
	    public List<DynamicSolidPhysBox> dynamicSolidObjects;
	    public List<PhysBox>[,] staticObjects;
	    public List<PhysBox> triggerObjects;
	    public List<HoldablePhysBox> ladders;
	    public List<PhysBox> walls;
	    public List<PhysBox> movingPlatforms;
	    public List<CharacterPhysBox> enemies;
	    public List<PhysBox> bombBoxes;
	    public List<PhysBox> spikes;
	    public List<PhysBox> spikeSolids;
	    public List<PhysBox> bossRoots;
	    public List<BossVulnerablePhysBox> bossVulnerablePoints;
	    public List<PhysBox> alsoPlatforms;
	    public List<NPC> npcs;
	    public List<DynamicPhysBox> barrels;
	    public CharacterPhysBox player;
	    public PlayerAnim playerAnim;
	    public DoorPhysBox door;
	    public DoorAnim doorAnim;
	    private float enemyHorAcc = 20f;
	    private float playerHorAcc = 80f;
	    private float playerVerAcc = 39f;
	    private float playerLadderClimbVerAcc = 4f;
	    private float playerJump = 10f;
	    private float frictionWhenTouchingUpWall = 0.5f;
	    private float gravity = -60f;
	    private const int OnAirLimit = 6;
	    public int restartCounter;
	    private float freezeScreen;
	    public static bool shouldInitPhys;
	    private const float BOMB_SPEED = 200f;
	    public const float MOVING_PLATFORM_SPEED = 3f;
	    public List<PhysBody> bodies;
	    private object body;
	    private bool inputFire2Old;
	    private bool inputBackOld;
	    public NPC npcTalking;
	    private const int BROAD_PHASE_SIZE = 9;
	    private int broadPhaseWidth = LevelEditor.level.GetLength(0) / BROAD_PHASE_SIZE + 1;
	    private int broadPhaseHeight = LevelEditor.level.GetLength(1) / BROAD_PHASE_SIZE + 1;
	    private List<Vector2> positionsDropMoney;
	    private int amountDropMoney;
	    private bool doesPlayerHaveShield;

	    public void InitPhysObjects() {
	        dynamicObjects = new List<DynamicPhysBox>();
	        dynamicSolidObjects = new List<DynamicSolidPhysBox>();
	        broadPhaseWidth = LevelEditor.level.GetLength(0) / BROAD_PHASE_SIZE + 1;
	        broadPhaseHeight = LevelEditor.level.GetLength(1) / BROAD_PHASE_SIZE + 1;
	        staticObjects = new List<PhysBox>[broadPhaseWidth, broadPhaseHeight];
	        for (int i = 0; i < broadPhaseWidth; i++) {
	            for (int j = 0; j < broadPhaseHeight; j++) {
	                staticObjects[i, j] = new List<PhysBox>();
	            }
	        }
	        triggerObjects = new List<PhysBox>();
	        allObjects = new List<PhysBox>();
	        walls = new List<PhysBox>();
	        movingPlatforms = new List<PhysBox>();
	        enemies = new List<CharacterPhysBox>();
	        ladders = new List<HoldablePhysBox>();
	        bombBoxes = new List<PhysBox>();
	        spikes = new List<PhysBox>();
	        spikeSolids = new List<PhysBox>();
	        bossRoots = new List<PhysBox>();
	        bossVulnerablePoints = new List<BossVulnerablePhysBox>();
	        alsoPlatforms = new List<PhysBox>();
	        npcTalking = null;
	        npcs = new List<NPC>();
	        barrels = new List<DynamicPhysBox>();
	        player = null;
	        door = null;
	        doorAnim = null;
	        object[] all = Object.FindObjectsOfType(typeof(GameObject));
	        foreach (object o in all) {
	            GameObject g = (GameObject)o;
	            PhysBox physObj = g.GetComponent<PhysBox>();
	            if (physObj != null) {
	                if (physObj.kind == PhysBox.Kind.Static) {
	                    if (physObj.parent == null) {
	                        int iBroadPhaseMin = GetBroadPhaseIndexForX(physObj.rect.xMin);
	                        int jBroadPhaseMin = GetBroadPhaseIndexForY(physObj.rect.yMin);
	                        int iBroadPhaseMax = GetBroadPhaseIndexForX(physObj.rect.xMax);
	                        int jBroadPhaseMax = GetBroadPhaseIndexForY(physObj.rect.yMax);
	                        for (int i = iBroadPhaseMin; i <= iBroadPhaseMax; i++) {
	                            for (int j = jBroadPhaseMin; j <= jBroadPhaseMax; j++) {
	                                staticObjects[i, j].Add(physObj);
	                            }
	                        }
	                    }
	                }
	                else if (physObj.kind == PhysBox.Kind.Dynamic) {
	                    dynamicObjects.Add(physObj as DynamicPhysBox);
	                }
	                else if (physObj.kind == PhysBox.Kind.Trigger) {
	                    triggerObjects.Add(physObj);
	                }
	                else if (physObj.kind == PhysBox.Kind.DynamicSolid) {
	                    dynamicSolidObjects.Add(physObj as DynamicSolidPhysBox);
	                    int iBroadPhaseMin = GetBroadPhaseIndexForX(physObj.rect.xMin);
	                    int jBroadPhaseMin = GetBroadPhaseIndexForY(physObj.rect.yMin);
	                    int iBroadPhaseMax = GetBroadPhaseIndexForX(physObj.rect.xMax);
	                    int jBroadPhaseMax = GetBroadPhaseIndexForY(physObj.rect.yMax);
	                    for (int i = iBroadPhaseMin; i <= iBroadPhaseMax; i++) {
	                        for (int j = jBroadPhaseMin; j <= jBroadPhaseMax; j++) {
	                            staticObjects[i, j].Add(physObj);
	                        }
	                    }
	                    dynamicObjects.Add(physObj as DynamicPhysBox);
	                }
	                allObjects.Add(physObj);
	                if (physObj.gameElement == PhysBox.GameElement.Player) {
	                    player = physObj as CharacterPhysBox;
	                    playerAnim = player.GetComponent<PlayerAnim>();
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.MovingPlatform) {
	                    movingPlatforms.Add(physObj);
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.EnemyLaser || physObj.gameElement == PhysBox.GameElement.EnemyCritter || physObj.gameElement == PhysBox.GameElement.EnemyJumper || physObj.gameElement == PhysBox.GameElement.EnemyPistol || physObj.gameElement == PhysBox.GameElement.EnemyBat || physObj.gameElement == PhysBox.GameElement.EnemyDiagonal) {
	                    CharacterPhysBox enemy = physObj as CharacterPhysBox;
	                    enemy.hp = enemy.hpMax;
	                    enemies.Add(enemy);
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.Wall || physObj.gameElement == PhysBox.GameElement.Indestructable) {
	                    walls.Add(physObj);
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.Ladder) {
	                    HoldablePhysBox ladder = physObj as HoldablePhysBox;
	                    ladders.Add(ladder);

	                    if (ladder.isAlsoPlatform) {
	                        alsoPlatforms.Add(ladder);
	                    }
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.Hanger) {
	                    ladders.Add(physObj as HoldablePhysBox);
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.Door) {
	                    door = physObj as DoorPhysBox;
	                    doorAnim = door.GetComponent<DoorAnim>();
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.BombBox)
	                {
	                    bombBoxes.Add(physObj);
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.Spike) {
	                    spikes.Add(physObj);
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.SpikeSolid) {
	                    spikeSolids.Add(physObj);
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.BossRoot || physObj.gameElement == PhysBox.GameElement.BossRootSpider) {
	                    bossRoots.Add(physObj);
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.BossVulnerable) {
	                    bossVulnerablePoints.Add(physObj as BossVulnerablePhysBox);
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.NPC) {
	                    npcs.Add(physObj.GetComponent<NPC>());
	                    physObj.GetComponent<NPC>().physBox = physObj as CharacterPhysBox;
	                }
	                else if (physObj.gameElement == PhysBox.GameElement.Barrel) {
	                    CharacterPhysBox barrel = physObj as CharacterPhysBox;
	                    barrel.hp = barrel.hpMax;
	                    barrels.Add(barrel);
	                }
	            }
	        }

	        CalculateBodies();

	        ObjectPool.bombPool.ResetObjs();
	        ObjectPool.laserEnemyPool.ResetObjs();
	        ObjectPool.firePlayerPool.ResetObjs();
	        ObjectPool.fireEnemyPool.ResetObjs();
	        ObjectPool.explosionPool.ResetObjs();
	        ObjectPool.deadPool.ResetObjs();
	        ObjectPool.moneyPool.ResetObjs();

	        positionsDropMoney = new List<Vector2>();
	    }

	    bool normalMode = true;
	    public void Update() {
	        if (restartCounter > 1 && TaloketoInputManager.GetButton("Back") && !inputBackOld) {
	            restartCounter = 0;
	            SaveSystem.LoadStats(out Platformer.money, out Platformer.numFireRateUpgrades, out Platformer.numShields, out Platformer.numBombs);
	        }

	        if (shouldInitPhys) {
	            InitPhysObjects();
	            shouldInitPhys = false;
	        }

	#if UNITY_EDITOR
	        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.nKey.wasPressedThisFrame)
	        {
	            normalMode = !normalMode;
	        }
	#endif

	        isPaused = false;
	        if (restartCounter == 0) {
	            player.gameObject.SetActive(true);
	            if (!player.isActiveAndEnabled || TaloketoInputManager.GetButton("Back")) {
	                Platformer.Restart();
	            }
	            else {
	                if (string.IsNullOrEmpty(door.nextLevel)) Platformer.NextLevel();
	                else Platformer.OpenLevel(door.nextLevel);
	            }
	        }
	        else if (restartCounter == 1) {
	            InitPhysObjects();
	            if (doesPlayerHaveShield) {
	                playerAnim.ShowShield(doesPlayerHaveShield);
	            }
	            else if (Platformer.numShields > 0) {
	                Platformer.numShields--;
	                SaveSystem.SaveNumShields(Platformer.numShields);
	                doesPlayerHaveShield = true;
	                playerAnim.ShowShield(doesPlayerHaveShield);
	            }
	        }
	        else if (normalMode
	#if UNITY_EDITOR
	 || (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
	#endif
	            ) {
	            if (freezeScreen > 0f) {
	                isPaused = true;
	                freezeScreen -= Time.fixedDeltaTime;
	            }
	            else {
	                if (LevelEditor.updateAlsoPlatforms) {
	                    LevelEditor.updateAlsoPlatforms = false;
	                    alsoPlatforms = new List<PhysBox>();
	                    foreach (HoldablePhysBox physObj in ladders) {
	                        if (physObj.isAlsoPlatform) {
	                            alsoPlatforms.Add(physObj);
	                        }
	                    }
	                }
	                foreach (HoldablePhysBox physObj in ladders) {
	                    physObj.isHoldableHanger = false;
	                }
	                TalkUI.instance.UpdateUI(dt, npcTalking != null ? npcTalking : null, player);
	                foreach (DynamicPhysBox physObj in dynamicObjects) {
	                    physObj.inputJumpOld = physObj.inputJump;
	                    physObj.inputOld = physObj.input;
	                }
	                foreach (NPC npc in npcs) {
	                    npc.UpdateNPC(dt);
	                }
	                if (player.isActiveAndEnabled) {
	                    if (npcTalking == null && (TaloketoInputManager.GetButton("Fire1") || TaloketoInputManager.GetAxis("FireAxis") < -0.5f) && player.fireTime <= 0f) {
	                        player.fireTime = 0.3f * Mathf.Pow(0.99f, Platformer.numFireRateUpgrades);
	                        FirePlayer(player.ladders.Count > 0 && player.ladderActiveClimbable == ClimbableTile.D);
	                    }
	                    if (npcTalking == null && Platformer.numBombs > 0 && (TaloketoInputManager.GetButton("Fire2") && !inputFire2Old)) {
	                        Platformer.numBombs--;
	                        DynamicPhysBox bomb = ObjectPool.bombPool.getPhys(player.rect.position) as DynamicPhysBox;
	                        if (!dynamicObjects.Contains(bomb)) {
	                            dynamicObjects.Add(bomb);
	                        }
	                    }
	                    if (player.fireCoolingTime > 0f) {
	                        player.fireCoolingTime -= dt;
	                    }
	                    if (npcTalking != null || player.fireTime > 0f || player.waitTime > 0f) {
	                        if (player.waitTime > 0f) {
	                            player.waitTime -= dt;
	                        }
	                        if (player.fireTime > 0f) {
	                            player.fireTime -= dt;
	                            if (player.fireTime <= 0f) {

	                            }
	                        }
	                        player.input = new Vector2(0f, 0f);
	                        player.inputJump = false;
	                        if (TaloketoInputManager.GetAxisRaw("Horizontal") != 0f) {
	                            player.isRight = TaloketoInputManager.GetAxisRaw("Horizontal") > 0f;
	                        }
	                    }
	                    else {
	                        player.input = new Vector2(TaloketoInputManager.GetAxisRaw("Horizontal") * playerHorAcc, TaloketoInputManager.GetAxisRaw("Vertical") * playerVerAcc);
	                        player.inputJump = TaloketoInputManager.GetButton("Jump");
	                        if (TaloketoInputManager.GetAxisRaw("Horizontal") != 0f) {
	                            player.isRight = TaloketoInputManager.GetAxisRaw("Horizontal") > 0f;
	                        }
	                    }
	                    if (player.fireTime <= 0f && player.waitTime <= 0f) {
	                        player.inputHoldUp = TaloketoInputManager.GetAxisRaw("Vertical") > 0f;
	                        player.inputHoldDown = TaloketoInputManager.GetAxisRaw("Vertical") < 0f;
	                    }
	                }

	                //enemies
	                foreach (CharacterPhysBox enemy in enemies) {
	                    if (!enemy.isActiveAndEnabled) continue;
	                    if (enemy.gameElement == PhysBox.GameElement.EnemyBat) {
	                        if (player.isActiveAndEnabled) {
	                            Vector2 deltaPos = player.rect.position + new Vector2(0f, 0.5f) - enemy.rect.position;
	                            if (Mathf.Abs(deltaPos.x) + Mathf.Abs(deltaPos.y) < 10f && deltaPos.y < 1f) {
	                                if (Mathf.Abs(deltaPos.x) > 0.1f) {
	                                    enemy.input = new Vector2(deltaPos.x > 0f ? 1f : -1f, enemy.input.y);
	                                }
	                                else {
	                                    enemy.input = new Vector2(0f, enemy.input.y);
	                                }
	                                if (Mathf.Abs(deltaPos.y) > 0.1f) {
	                                    enemy.input = new Vector2(enemy.input.x, deltaPos.y > 0f ? 1f : -1f);
	                                }
	                                else {
	                                    enemy.input = new Vector2(enemy.input.x, 0f);
	                                }
	                            }
	                            else {
	                                enemy.input = new Vector2(0f, 0f);
	                            }
	                        }
	                        enemy.vel = enemy.input * 4f;
	                    }
	                    else if (enemy.gameElement == PhysBox.GameElement.EnemyDiagonal) {
	                        EnemyDiagonalPhysBox enemyDiagonal = enemy as EnemyDiagonalPhysBox;
	                        enemyDiagonal.input = new Vector2(enemyDiagonal.enemyDiagonalIsRight ? 1f : -1f, enemyDiagonal.enemyDiagonalIsUp ? 1f : -1f);
	                        enemyDiagonal.vel = enemyDiagonal.input * 4f;
	                    }
	                    else {
	                        if (enemy.gameElement == PhysBox.GameElement.EnemyLaser || enemy.gameElement == PhysBox.GameElement.EnemyPistol) {
	                            if (player.isActiveAndEnabled && enemy.fireTime <= 0f && enemy.fireCoolingTime <= 0f && (enemy.isRight == (player.rect.x > enemy.rect.x)) && Mathf.Abs(enemy.rect.y - player.rect.y) < 1f) {
	                                if (enemy.gameElement == PhysBox.GameElement.EnemyLaser) {
	                                    enemy.fireTime = 1f;
	                                    AudioPlayer.instance.Play(AudioPlayer.instance.clipEnemyLaserBuild);
	                                }
	                                else {
	                                    enemy.fireTime = 0.2f;
	                                }
	                            }
	                        }
	                        if (enemy.fireCoolingTime > 0f) {
	                            enemy.fireCoolingTime -= dt;
	                        }
	                        if (enemy.waitTime > 0f || enemy.fireTime > 0f) {
	                            if (enemy.waitTime > 0f) {
	                                enemy.waitTime -= dt;
	                                if (enemy.waitTime <= 0f) {

	                                }
	                            }
	                            if (enemy.fireTime > 0f) {
	                                enemy.isRight = (player.rect.x > enemy.rect.x);
	                                enemy.fireTime -= dt;
	                                if (enemy.fireTime <= 0f) {
	                                    if (enemy.gameElement == PhysBox.GameElement.EnemyLaser) {
	                                        FireLaser(enemy, false);
	                                    }
	                                    else {
	                                        FirePistol(enemy);
	                                    }
	                                }
	                            }
	                            enemy.input = new Vector2(0f, 0f);
	                        }
	                        else {
	                            float dir = enemy.isRight ? 1f : -1f;
	                            List<PhysBox> collided = new List<PhysBox>();

	                            bool checkLimits = true;
	                            if (enemy.gameElement == PhysBox.GameElement.EnemyJumper && player.isActiveAndEnabled) {
	                                Vector2 deltaPos = player.rect.position - enemy.rect.position;
	                                if (Mathf.Abs(deltaPos.x) + Mathf.Abs(deltaPos.y) < 10f) {
	                                    checkLimits = false;
	                                    enemy.isRight = deltaPos.x > 0f;
	                                }
	                                else {
	                                    checkLimits = true;
	                                }
	                            }

	                            if (checkLimits && (!CanMove(enemy, collided, new Vector2(dir * 1f, 0f), walls) || CanMove(enemy, collided, new Vector2(dir * 1f, -1f), walls))) {
	                                enemy.isRight = !enemy.isRight;
	                                dir = enemy.isRight ? 1f : -1f;
	                                enemy.input = new Vector2(0f, 0f);
	                                enemy.inputJump = false;
	                                if (enemy.gameElement != PhysBox.GameElement.EnemyJumper) {
	                                    enemy.waitTime = Random.Range(0f, 3f);
	                                }
	                            }
	                            else {
	                                enemy.input = new Vector2(dir * enemyHorAcc, 0f);
	                                enemy.inputJump = enemy.gameElement == PhysBox.GameElement.EnemyJumper ? Random.value > 0.5f : false;
	                            }
	                        }
	                    }
	                    if (enemy.invincibilityTime > 0f)
	                    {
	                        enemy.invincibilityTime -= dt;
	                    }
	                }

	                if (player.invincibilityTime > 0f) {
	                    player.invincibilityTime -= dt;
	                }

	                //lasers of enemies
	                bool damagePlayer = false;
	                if (player.isActiveAndEnabled && player.invincibilityTime <= 0f) {
	                    foreach (PhysBox trigger in ObjectPool.laserEnemyPool.physBoxes) {
	                        if (!trigger.isActiveAndEnabled) continue;
	                        if (player.Overlaps(trigger)) {
	                            damagePlayer = true;
	                            break;
	                        }
	                    }
	                    if (!damagePlayer) {
	                        foreach (CharacterPhysBox enemy in enemies) {
	                            if (!enemy.isActiveAndEnabled || enemy.invincibilityTime > 0f) continue;
	                            if (enemy.Overlaps(player)) {
	                                damagePlayer = true;
	                                break;
	                            }
	                        }
	                    }
	                }

	                //fires of enemy
	                foreach (PhysBox trigger in ObjectPool.fireEnemyPool.physBoxes) {
	                    if (!trigger.isActiveAndEnabled) continue;
	                    trigger.rect.position += trigger.vel * dt;
	                    if (!damagePlayer && player.isActiveAndEnabled && player.invincibilityTime <= 0f && player.Overlaps(trigger)) {
	                        damagePlayer = true;
	                    }
	                    foreach (PhysBox bomb in ObjectPool.bombPool.physBoxes) {
	                        if (!bomb.isActiveAndEnabled) continue;
	                        if (bomb.Overlaps(trigger)) {
	                            bomb.vel = Geometry.normalizeVector2(trigger.vel, BOMB_SPEED) + new Vector2(0f, 1f);
	                        }
	                    }
	                    foreach (PhysBox wall in GetStaticObjectsForRect(trigger.rect)) {
	                        if (!wall.isActiveAndEnabled) continue;
	                        if (wall.Overlaps(trigger)) {
	                            trigger.GetComponent<FirePlayerAnim>().Die();
	                        }
	                    }
	                }
	                //money
	                bool shouldPlayCoinSound = false;
	                foreach (PhysBox trigger in ObjectPool.moneyPool.physBoxes) {
	                    if (!trigger.isActiveAndEnabled) continue;
	                    MoneyPhysBox physBox = (trigger as MoneyPhysBox);
	                    if (physBox.timerInvincibility > 0f) continue;
	                    if (physBox.timerDie <= 0f) {
	                        Die(trigger);
	                    }
	                    else {
	                        physBox.inputJump = physBox.timerJump > 2.5f;
	                        trigger.rect.position += trigger.vel * dt;
	                        if (player.isActiveAndEnabled && player.Overlaps(trigger)) {
	                            int amount = (trigger as MoneyPhysBox).amount;
	                            Platformer.money += amount;
	                            MoneyUI.instance.Add(amount);
	                            shouldPlayCoinSound = true;
	                            Die(trigger);
	                        }
	                    }
	                }
	                if (shouldPlayCoinSound) {
	                    AudioPlayer.instance.Play(AudioPlayer.instance.clipCoin);
	                }

	                //spikes
	                if (!damagePlayer && player.invincibilityTime <= 0f) {
	                    foreach (PhysBox trigger in spikes) {
	                        if (!trigger.isActiveAndEnabled || (trigger.physBody != null && trigger.physBody.deathTimer > 0f)) continue;
	                        if (player.isActiveAndEnabled && player.Overlaps(trigger, 0.3f)) {
	                            damagePlayer = true;
	                            break;
	                        }
	                    }
	                }

	                //spike solids
	                if (!damagePlayer && player.invincibilityTime <= 0f) {
	                    foreach (PhysBox trigger in spikeSolids) {
	                        if (!trigger.isActiveAndEnabled || (trigger.physBody != null && trigger.physBody.deathTimer > 0f)) continue;
	                        if (player.isActiveAndEnabled && player.Overlaps(trigger, 0.3f)) {
	                            damagePlayer = true;
	                            break;
	                        }
	                    }
	                }

	                //explosions
	                foreach (PhysBox trigger in ObjectPool.explosionPool.physBoxes) {
	                    if (!trigger.isActiveAndEnabled) continue;
	                    foreach (PhysBox enemy in enemies) {
	                        if (!enemy.isActiveAndEnabled) continue;
	                        if (enemy.Overlaps(trigger)) {
	                            Die(enemy);
	                            AudioPlayer.instance.Play(AudioPlayer.instance.clipEnemyDie);
	                        }
	                    }
	                    if (player.isActiveAndEnabled && player.invincibilityTime <= 0f && player.Overlaps(trigger)) {
	                        damagePlayer = true;
	                    }
	                    foreach (PhysBox wall in GetStaticObjectsForRect(trigger.rect)) {
	                        if (!wall.isActiveAndEnabled || wall.gameElement == PhysBox.GameElement.Indestructable || wall.gameElement == PhysBox.GameElement.BossVulnerable) continue;
	                        if (wall.Overlaps(trigger)) {
	                            Die(wall);
	                        }
	                    }
	                    foreach (PhysBox bomb in ObjectPool.bombPool.physBoxes) {
	                        if (!bomb.isActiveAndEnabled) continue;
	                        if (bomb.Overlaps(trigger)) {
	                            bomb.GetComponent<BombAnim>().timer = 0f;
	                        }
	                    }
	                }

	                if (damagePlayer) {
	                    if (doesPlayerHaveShield) {
	                        doesPlayerHaveShield = false;
	                        playerAnim.ShowShield(doesPlayerHaveShield);
	                        player.invincibilityTime = 3f;
	                    }
	                    else {
	                        player.GetComponent<PlayerAnim>().deathTimer = 0.5f;
	                        AudioPlayer.instance.Play(AudioPlayer.instance.clipPlayerDie);
	                        freezeScreen = 1f;
	                    }
	                }

	                //fires of player
	                foreach (PhysBox trigger in ObjectPool.firePlayerPool.physBoxes) {
	                    if (!trigger.isActiveAndEnabled) continue;
	                    trigger.rect.position += trigger.vel * dt;
	                    bool didFireHit = false;
	                    foreach (CharacterPhysBox enemy in enemies) {
	                        if (!enemy.isActiveAndEnabled || enemy.invincibilityTime > 0f) continue;
	                        if (enemy.Overlaps(trigger)) {
	                            Damage(enemy);
	                            trigger.GetComponent<FirePlayerAnim>().Die();
	                            didFireHit = true;
	                            break;
	                        }
	                    }
	                    if (didFireHit) continue;
	                    foreach (CharacterPhysBox barrel in barrels) {
	                        if (!barrel.isActiveAndEnabled) continue;
	                        if (barrel.Overlaps(trigger)) {
	                            Damage(barrel);
	                            trigger.GetComponent<FirePlayerAnim>().Die();
	                            didFireHit = true;
	                            break;
	                        }
	                    }
	                    if (didFireHit) continue;
	                    foreach (PhysBox spike in spikeSolids) {
	                        if (!spike.isActiveAndEnabled) continue;
	                        if (spike.Overlaps(trigger)) {
	                            trigger.GetComponent<FirePlayerAnim>().Die();
	                            didFireHit = true;
	                            break;
	                        }
	                    }
	                    if (didFireHit) continue;
	                    foreach (PhysBox bomb in ObjectPool.bombPool.physBoxes) {
	                        if (!bomb.isActiveAndEnabled) continue;
	                        if (bomb.Overlaps(trigger)) {
	                            bomb.vel = Geometry.normalizeVector2(trigger.vel, BOMB_SPEED) + new Vector2(0f, 1f);
	                        }
	                    }
	                    foreach (BossVulnerablePhysBox boss in bossVulnerablePoints) {
	                        if (!boss.isActiveAndEnabled || boss.invincibilityTime > 0f) continue;
	                        if (boss.Overlaps(trigger)) {
	                            trigger.GetComponent<FirePlayerAnim>().Die();
	                            Damage(boss);
	                            didFireHit = true;
	                            break;
	                        }
	                    }
	                    if (didFireHit) continue;
	                    foreach (PhysBox wall in GetStaticObjectsForRect(trigger.rect)) {
	                        if (!wall.isActiveAndEnabled) continue;
	                        if (wall.Overlaps(trigger)) {
	                            trigger.GetComponent<FirePlayerAnim>().Die();
	                            break;
	                        }
	                    }
	                }

	                //moving platforms
	                foreach (MovingPlatformPhysBox physObj in movingPlatforms) {
	                    if (!physObj.isActiveAndEnabled) continue;
	                    if (physObj.waitTime > 0f) {
	                        physObj.waitTime -= dt;
	                        physObj.vel = new Vector2(0f, 0f);
	                    }
	                    else {
	                        physObj.iBroadPhaseMin = GetBroadPhaseIndexForX(physObj.rect.xMin);
	                        physObj.jBroadPhaseMin = GetBroadPhaseIndexForY(physObj.rect.yMin);
	                        physObj.iBroadPhaseMax = GetBroadPhaseIndexForX(physObj.rect.xMax);
	                        physObj.jBroadPhaseMax = GetBroadPhaseIndexForY(physObj.rect.yMax);
	                        float dir = physObj.isRight ? 1f : -1f;
	                        List<PhysBox> collided = new List<PhysBox>();
	                        if (physObj.isUpDown) {
	                            if (!CanMove(physObj, collided, new Vector2(0f, dir), walls)) {
	                                physObj.isRight = !physObj.isRight;
	                                dir = physObj.isRight ? 1f : -1f;
	                                physObj.vel = new Vector2(0f, 0f);
	                                physObj.waitTime = 1f;
	                            }
	                            else {
	                                physObj.vel = new Vector2(0f, dir * physObj.speed);
	                            }
	                        }
	                        else {
	                            if (!CanMove(physObj, collided, new Vector2(dir, 0f), walls)) {
	                                physObj.isRight = !physObj.isRight;
	                                dir = physObj.isRight ? 1f : -1f;
	                                physObj.vel = new Vector2(0f, 0f);
	                                physObj.waitTime = 1f;
	                            }
	                            else {
	                                physObj.vel = new Vector2(dir * physObj.speed, 0f);
	                            }
	                        }
	                    }
	                }

	                foreach (DynamicSolidPhysBox physObj in dynamicSolidObjects) {
	                    physObj.iBroadPhaseMin = GetBroadPhaseIndexForX(physObj.rect.xMin);
	                    physObj.jBroadPhaseMin = GetBroadPhaseIndexForY(physObj.rect.yMin);
	                    physObj.iBroadPhaseMax = GetBroadPhaseIndexForX(physObj.rect.xMax);
	                    physObj.jBroadPhaseMax = GetBroadPhaseIndexForY(physObj.rect.yMax);
	                }

	                //door-player
	                bool doorIsOpen = bossRoots.Count == 0;
	                if (!doorIsOpen) {
	                    doorIsOpen = true;
	                    foreach (PhysBox bossRoot in bossRoots) {
	                        if (bossRoot != null && bossRoot.isActiveAndEnabled) {
	                            doorIsOpen = false;
	                            break;
	                        }
	                    }
	                }
	                doorAnim.isOpen = doorIsOpen;
	                if (doorIsOpen && player.isActiveAndEnabled) {
	                    if (door.Overlaps(player)) {
	                        if (Mathf.Abs(doorAnim.timer) < float.Epsilon) {
	                            AudioPlayer.instance.Play(AudioPlayer.instance.clipDoor);
	                        }
	                        doorAnim.timer += dt;
	                        if (doorAnim.timer > 0.5f) {
	                            restartCounter = -1;
	                            SaveSystem.SaveStats(Platformer.money, Platformer.numFireRateUpgrades, Platformer.numShields, Platformer.numBombs);
	                        }
	                        playerAnim.yScaleFactor = doorAnim.transform.localScale.y / doorAnim.localScale.y;
	                    }
	                    else {
	                        doorAnim.timer = Mathf.Max(0f, doorAnim.timer - 4f * dt);
	                        playerAnim.yScaleFactor = 1f;
	                    }
	                }

	                //bombBoxes
	                foreach (PhysBox trigger in bombBoxes) {
	                    if (!trigger.isActiveAndEnabled) continue;
	                    if (player.isActiveAndEnabled && player.Overlaps(trigger)) {
	                        AudioPlayer.instance.Play(AudioPlayer.instance.clipPickup);
	                        Die(trigger);
	                        Platformer.numBombs++;
	                    }
	                }

	                //bossVulnerablePoints
	                foreach (BossVulnerablePhysBox trigger in bossVulnerablePoints) {
	                    if (!trigger.isActiveAndEnabled) continue;
	                    if (trigger.invincibilityTime > 0f) {
	                        trigger.invincibilityTime -= dt;
	                    }
	                }

	                foreach (CharacterPhysBox barrel in barrels) {
	                    if (!barrel.isActiveAndEnabled) continue;
	                    if (barrel.invincibilityTime > 0f) {
	                        barrel.invincibilityTime -= dt;
	                    }
	                }

	                //npcs
	                foreach (NPC npc in npcs) {
	                    if (!npc.isActiveAndEnabled || npc.dontTalkAgain) continue;
	                    if (player.isActiveAndEnabled && npcTalking == null && player.rect.Overlaps(npc.GetRect())) {
	                        if (player.inputHoldUp || npc.autoTalk) {
	                            npcTalking = npc;
	                            npc.Talk();
	                        }
	                        else {
	                            TutorialManager.instance.ShowUp(npc.transform.position + new Vector3(0f, 2f, 0f));
	                        }
	                    }
	                }
	                if (npcTalking != null && npcTalking.state == NPC.State.None) {
	                    npcTalking = null;
	                }

	                //physics update
	                foreach (PhysBox physObj in allObjects) {
	                    if (!physObj.isActiveAndEnabled) continue;
	                    physObj.lastPos = physObj.rect.position;
	                }
	                foreach (MovingPlatformPhysBox physObj in movingPlatforms) {
	                    if (!physObj.isActiveAndEnabled) continue;
	                    physObj.rect.position += physObj.vel * dt;
	                    int iBroadPhaseMinNew = GetBroadPhaseIndexForX(physObj.rect.xMin);
	                    int jBroadPhaseMinNew = GetBroadPhaseIndexForY(physObj.rect.yMin);
	                    int iBroadPhaseMaxNew = GetBroadPhaseIndexForX(physObj.rect.xMax);
	                    int jBroadPhaseMaxNew = GetBroadPhaseIndexForY(physObj.rect.yMax);
	                    if (physObj.iBroadPhaseMin != iBroadPhaseMinNew || physObj.jBroadPhaseMin != jBroadPhaseMinNew || physObj.iBroadPhaseMax != iBroadPhaseMaxNew || physObj.jBroadPhaseMax != jBroadPhaseMaxNew) {
	                        for (int i = physObj.iBroadPhaseMin; i <= physObj.iBroadPhaseMax; i++) {
	                            for (int j = physObj.jBroadPhaseMin; j <= physObj.jBroadPhaseMax; j++) {
	                                staticObjects[i, j].Remove(physObj);
	                            }
	                        }
	                        for (int i = iBroadPhaseMinNew; i <= iBroadPhaseMaxNew; i++) {
	                            for (int j = jBroadPhaseMinNew; j <= jBroadPhaseMaxNew; j++) {
	                                staticObjects[i, j].Add(physObj);
	                            }
	                        }
	                    }
	                    foreach (PhysBox dynamicObj in dynamicObjects) {
	                        if (dynamicObj.Overlaps(physObj)) {
	                            ResolveCollision(dynamicObj, physObj, new Vector2(0f, 0f));
	                        }
	                    }
	                }

	                foreach (DynamicSolidPhysBox physObj in dynamicSolidObjects) {
	                    if (!physObj.isActiveAndEnabled) continue;
	                    int iBroadPhaseMinNew = GetBroadPhaseIndexForX(physObj.rect.xMin);
	                    int jBroadPhaseMinNew = GetBroadPhaseIndexForY(physObj.rect.yMin);
	                    int iBroadPhaseMaxNew = GetBroadPhaseIndexForX(physObj.rect.xMax);
	                    int jBroadPhaseMaxNew = GetBroadPhaseIndexForY(physObj.rect.yMax);
	                    if (physObj.iBroadPhaseMin != iBroadPhaseMinNew || physObj.jBroadPhaseMin != jBroadPhaseMinNew || physObj.iBroadPhaseMax != iBroadPhaseMaxNew || physObj.jBroadPhaseMax != jBroadPhaseMaxNew) {
	                        for (int i = physObj.iBroadPhaseMin; i <= physObj.iBroadPhaseMax; i++) {
	                            for (int j = physObj.jBroadPhaseMin; j <= physObj.jBroadPhaseMax; j++) {
	                                staticObjects[i, j].Remove(physObj);
	                            }
	                        }
	                        for (int i = iBroadPhaseMinNew; i <= iBroadPhaseMaxNew; i++) {
	                            for (int j = jBroadPhaseMinNew; j <= jBroadPhaseMaxNew; j++) {
	                                staticObjects[i, j].Add(physObj);
	                            }
	                        }
	                    }
	                }

	                //update position and check collisions FOR BODIES
	                foreach (PhysBody body in bodies) {
	                    if (body.deathTimer <= 0f)
	                    {
	                        foreach (PhysNode node in body.all) {
	                            if (node.physBox.isActiveAndEnabled && node.physBox.kind == PhysBox.Kind.Static) {
	                                node.iBroadPhaseMin = GetBroadPhaseIndexForX(node.physBox.rect.xMin);
	                                node.jBroadPhaseMin = GetBroadPhaseIndexForY(node.physBox.rect.yMin);
	                                node.iBroadPhaseMax = GetBroadPhaseIndexForX(node.physBox.rect.xMax);
	                                node.jBroadPhaseMax = GetBroadPhaseIndexForY(node.physBox.rect.yMax);
	                            }
	                        }
	                    }
	                }
	                foreach (PhysBody body in bodies)
	                {
	                    if (body.deathTimer > 0f)
	                    {
	                        float scale = 1f;
	                        if (body.deathTimer < 0.7f)
	                        {
	                            scale = Easing.BackEaseOut(body.deathTimer / 0.7f, 0f, 1f, 1f);
	                            if (body.deathTimer + dt >= 0.7f) {
	                                AudioPlayer.instance.Play(AudioPlayer.instance.clipBossDie1);
	                            }
	                        }
	                        List<PhysNode> nodesToCheck = new List<PhysNode>();
	                        nodesToCheck.Add(body.root);
	                        while (nodesToCheck.Count > 0)
	                        {
	                            List<PhysNode> newNodesToCheck = new List<PhysNode>();
	                            foreach (PhysNode node in nodesToCheck)
	                            {
	                                if (node.physBox.isActiveAndEnabled)
	                                {
	                                    node.physBox.visualPosOffset = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
	                                    node.physBox.visualScale = scale;
	                                }
	                                foreach (PhysNode child in node.children)
	                                {
	                                    newNodesToCheck.Add(child);
	                                }
	                            }
	                            nodesToCheck = newNodesToCheck;
	                        }

	                        body.deathTimer -= dt;
	                        if (body.deathTimer <= 0f)
	                        {
	                            body.root.physBox.gameObject.SetActive(false);
	                        }
	                    }
	                    else
	                    {
	                        List<PhysNode> nodesToCheck = new List<PhysNode>();
	                        nodesToCheck.Add(body.root);
	                        while (nodesToCheck.Count > 0)
	                        {
	                            List<PhysNode> newNodesToCheck = new List<PhysNode>();
	                            foreach (PhysNode node in nodesToCheck)
	                            {
	                                if (node.physBox.isActiveAndEnabled)
	                                {
	                                    UpdatePositionAndResolveCollision(node, node.physBox.vel);
	                                }
	                                foreach (PhysNode child in node.children)
	                                {
	                                    newNodesToCheck.Add(child);
	                                }
	                            }
	                            nodesToCheck = newNodesToCheck;
	                        }
	                    }
	                }
	                foreach (PhysBody body in bodies) {
	                    if (body.deathTimer <= 0f) {
	                        foreach (PhysNode node in body.all) {
	                            if (node.physBox.isActiveAndEnabled && node.physBox.kind == PhysBox.Kind.Static) {
	                                int iBroadPhaseMinNew = GetBroadPhaseIndexForX(node.physBox.rect.xMin);
	                                int jBroadPhaseMinNew = GetBroadPhaseIndexForY(node.physBox.rect.yMin);
	                                int iBroadPhaseMaxNew = GetBroadPhaseIndexForX(node.physBox.rect.xMax);
	                                int jBroadPhaseMaxNew = GetBroadPhaseIndexForY(node.physBox.rect.yMax);
	                                if (node.iBroadPhaseMin != iBroadPhaseMinNew || node.jBroadPhaseMin != jBroadPhaseMinNew || node.iBroadPhaseMax != iBroadPhaseMaxNew || node.jBroadPhaseMax != jBroadPhaseMaxNew) {
	                                    for (int i = node.iBroadPhaseMin; i <= node.iBroadPhaseMax; i++) {
	                                        for (int j = node.jBroadPhaseMin; j <= node.jBroadPhaseMax; j++) {
	                                            staticObjects[i, j].Remove(node.physBox);
	                                        }
	                                    }
	                                    for (int i = iBroadPhaseMinNew; i <= iBroadPhaseMaxNew; i++) {
	                                        for (int j = jBroadPhaseMinNew; j <= jBroadPhaseMaxNew; j++) {
	                                            staticObjects[i, j].Add(node.physBox);
	                                        }
	                                    }
	                                }
	                            }
	                        }
	                    }
	                }

	                foreach (DynamicPhysBox physObj in dynamicObjects) {
	                    PhysBox groundIfBodyOld = physObj.groundIfBody;
	                    physObj.groundIfBody = null;
	                    if (!physObj.isActiveAndEnabled) continue;
	                    //horizontal input
	                    if (System.Math.Abs(physObj.input.x) > float.Epsilon) {
	                        if ((physObj.vel.x > float.Epsilon) != (physObj.input.x > float.Epsilon)) {
	                            //horizontal friction
	                            bool isPositive = physObj.vel.x > 0f;
	                            physObj.vel = new Vector2(Mathf.Max(0f, Mathf.Abs(physObj.vel.x) - physObj.friction * dt) * (isPositive ? 1f : -1f), physObj.vel.y);
	                        }
	                        if (physObj.input.x > 0) {
	                            physObj.vel = new Vector2(Mathf.Min(physObj.vel.x + physObj.input.x * 0.01f, physObj.input.x), physObj.vel.y);
	                        }
	                        else {
	                            physObj.vel = new Vector2(Mathf.Max(physObj.vel.x + physObj.input.x * 0.01f, physObj.input.x), physObj.vel.y);
	                        }
	                    }
	                    else {
	                        //horizontal friction
	                        bool isPositive = physObj.vel.x > 0f;
	                        physObj.vel = new Vector2(Mathf.Max(0f, Mathf.Abs(physObj.vel.x) - physObj.friction * dt) * (isPositive ? 1f : -1f), physObj.vel.y);
	                    }
	                    if (physObj is CharacterPhysBox) {
	                        CharacterPhysBox character = (physObj as CharacterPhysBox);
	                        if (!character.onLadder) {
	                            if (character.dontGoOnLadderTimer > 0f) {
	                                character.dontGoOnLadderTimer -= dt;
	                            }
	                            if (character.dontGoOnLadderTimer <= 0f) {
	                                character.ladders.Clear();
	                                character.ladderClimbables.Clear();
	                                foreach (HoldablePhysBox trigger in ladders) {
	                                    if (!trigger.isActiveAndEnabled) continue;
	                                    if (character.Overlaps(trigger)) {
	                                        if (trigger.gameElement == PhysBox.GameElement.Ladder) {
	                                            if (character.inputHoldUp || character.inputHoldDown || trigger.isAutoHoldLadder) {
	                                                if (Mathf.Abs(character.input.x) <= float.Epsilon || ((character.input.x > 0f) != (character.rect.position.x > trigger.rect.position.x)))
	                                                    character.ladders.Add(trigger);
	                                                character.ladderClimbables.Add(ClimbableTile.None);
	                                            }
	                                        }
	                                        else if (trigger.gameElement == PhysBox.GameElement.Hanger) {
	                                            if (character.inputHoldUp || trigger.isAutoHoldLadder) {
	                                                character.ladders.Add(trigger);
	                                                character.ladderClimbables.Add(ClimbableTile.None);
	                                            }
	                                            if (physObj == player) trigger.isHoldableHanger = true;
	                                        }
	                                        else {
	                                            throw new System.NotImplementedException();
	                                        }
	                                    }
	                                    else if (trigger.isAlsoPlatform && character.inputHoldDown) {
	                                        Rectangle dynamicRect = character.rect.Copy();
	                                        float deltaY = -0.5f;
	                                        dynamicRect.y += deltaY;
	                                        if (dynamicRect.Overlaps(trigger.rect)) {
	                                            character.ladders.Add(trigger);
	                                            character.ladderClimbables.Add(ClimbableTile.None);
	                                            character.rect.y += deltaY;
	                                        }
	                                    }
	                                }
	                                if (character.inputHoldUp || character.inputHoldDown) {
	                                    foreach (PhysBox staticObj in GetStaticObjectsForRect(character.rect)) {
	                                        if (staticObj is ClimbablePhysBox) {
	                                            ClimbablePhysBox climbablePhysBox = (staticObj as ClimbablePhysBox);
	                                            if (!climbablePhysBox.isActiveAndEnabled || (!climbablePhysBox.isClimbableL && !climbablePhysBox.isClimbableD && !climbablePhysBox.isClimbableR)) continue;
	                                            if (climbablePhysBox.isClimbableL && character.rect.Overlaps(climbablePhysBox.GetClimbRect(ClimbableTile.L))) {
	                                                if (character.inputHoldUp != (character.rect.y > climbablePhysBox.rect.y)) {
	                                                    character.ladders.Add(climbablePhysBox);
	                                                    character.ladderClimbables.Add(ClimbableTile.L);
	                                                }
	                                            }
	                                            if (climbablePhysBox.isClimbableD && character.rect.Overlaps(climbablePhysBox.GetClimbRect(ClimbableTile.D))) {
	                                                if (character.inputHoldUp) {
	                                                    character.ladders.Add(climbablePhysBox);
	                                                    character.ladderClimbables.Add(ClimbableTile.D);
	                                                }
	                                            }
	                                            if (climbablePhysBox.isClimbableR && character.rect.Overlaps(climbablePhysBox.GetClimbRect(ClimbableTile.R))) {
	                                                if (character.inputHoldUp != (character.rect.y > climbablePhysBox.rect.y)) {
	                                                    character.ladders.Add(climbablePhysBox);
	                                                    character.ladderClimbables.Add(ClimbableTile.R);
	                                                }
	                                            }
	                                        }
	                                    }
	                                }
	                            }
	                            else {
	                                foreach (HoldablePhysBox trigger in ladders) {
	                                    if (trigger.gameElement == PhysBox.GameElement.Hanger && physObj.Overlaps(trigger)) {
	                                        trigger.isHoldableHanger = true;
	                                    }
	                                }
	                            }
	                        }
	                        else {
	                            character.ladders.Clear();
	                            character.ladderClimbables.Clear();
	                            if (character.inputJump && !character.inputJumpOld) {
	                                character.dontGoOnLadderTimer = 0.15f;
	                                if (character == player) {
	                                    AudioPlayer.instance.Play(AudioPlayer.instance.clipPlayerJump);
	                                }
	                            }
	                            else {
	                                foreach (HoldablePhysBox trigger in ladders) {
	                                    if (!trigger.isActiveAndEnabled) continue;
	                                    if (character.Overlaps(trigger)) {
	                                        character.ladders.Add(trigger);
	                                        character.ladderClimbables.Add(ClimbableTile.None);
	                                    }
	                                }
	                                foreach (PhysBox staticObj in GetStaticObjectsForRect(character.rect)) {
	                                    if (staticObj is ClimbablePhysBox) {
	                                        ClimbablePhysBox climbablePhysBox = (staticObj as ClimbablePhysBox);
	                                        if (!climbablePhysBox.isActiveAndEnabled || (!climbablePhysBox.isClimbableL && !climbablePhysBox.isClimbableD && !climbablePhysBox.isClimbableR)) continue;
	                                        if (climbablePhysBox.isClimbableL && character.rect.Overlaps(climbablePhysBox.GetClimbRect(ClimbableTile.L))) {
	                                            character.ladders.Add(climbablePhysBox);
	                                            character.ladderClimbables.Add(ClimbableTile.L);
	                                        }
	                                        if (climbablePhysBox.isClimbableD && character.rect.Overlaps(climbablePhysBox.GetClimbRect(ClimbableTile.D))) {
	                                            character.ladders.Add(climbablePhysBox);
	                                            character.ladderClimbables.Add(ClimbableTile.D);
	                                        }
	                                        if (climbablePhysBox.isClimbableR && character.rect.Overlaps(climbablePhysBox.GetClimbRect(ClimbableTile.R))) {
	                                            character.ladders.Add(climbablePhysBox);
	                                            character.ladderClimbables.Add(ClimbableTile.R);
	                                        }
	                                    }
	                                }
	                            }
	                        }
	                        character.onLadder = character.ladders.Count > 0;
	                    }


	                    if (System.Math.Abs(physObj.gravityMult) < float.Epsilon) {
	                        physObj.vel += physObj.input * dt;
	                    }
	                    else if (physObj is CharacterPhysBox && (physObj as CharacterPhysBox).onLadder) {
	                        CharacterPhysBox character = (physObj as CharacterPhysBox);
	                        if (character.ladderActiveClimbable != ClimbableTile.D)
	                        {
	                            character.airCounter = 0;
	                        }
	                        PhysBox ladderActive = null;
	                        ClimbableTile ladderActiveClimbable = ClimbableTile.None;
	                        PhysBox ladderClosest = null;
	                        ClimbableTile ladderClosestClimbable = ClimbableTile.None;
	                        for (int i = 0, len = character.ladders.Count; i < len; i++)
	                        {
	                            PhysBox ladder = character.ladders[i];
	                            ClimbableTile ladderClimbable = character.ladderClimbables[i];
	                            if (ladder == null || !ladder.isActiveAndEnabled) continue;
	                            if (ladder.gameElement == PhysBox.GameElement.Hanger)
	                            {
	                                ladderActive = ladder;
	                                ladderActiveClimbable = ladderClimbable;
	                                break;
	                            }
	                            else if (ladder.gameElement == PhysBox.GameElement.Ladder)
	                            {
	                                if (character.input.y != 0f)
	                                {
	                                    ladderActive = ladder;
	                                    ladderActiveClimbable = ladderClimbable;
	                                    break;
	                                }
	                                else
	                                {
	                                    if (ladderClosest == null)
	                                    {
	                                        ladderClosest = ladder;
	                                        ladderClosestClimbable = ladderClimbable;
	                                    }
	                                    else if (Geometry.lengthOfVector2(ladderClosest.rect.position - character.rect.position) > Geometry.lengthOfVector2(ladder.rect.position - character.rect.position))
	                                    {
	                                        ladderClosest = ladder;
	                                        ladderClosestClimbable = ladderClimbable;
	                                    }
	                                }
	                            }
	                            else if (ladderClimbable != ClimbableTile.None)
	                            {
	                                if (character.input.y != 0f && (ladderClimbable == ClimbableTile.L || ladderClimbable == ClimbableTile.R))
	                                {
	                                    ladderActive = ladder;
	                                    ladderActiveClimbable = ladderClimbable;
	                                }
	                                else if(character.input.x != 0f && ladderClimbable == ClimbableTile.D)
	                                {
	                                    ladderActive = ladder;
	                                    ladderActiveClimbable = ladderClimbable;
	                                }
	                                else
	                                {
	                                    if (ladderClosest == null)
	                                    {
	                                        ladderClosest = ladder;
	                                        ladderClosestClimbable = ladderClimbable;
	                                    }
	                                    else if (Geometry.lengthOfVector2(ladderClosest.rect.position - character.rect.position) > Geometry.lengthOfVector2(ladder.GetClimbRect(character.ladderClimbables[i]).position - character.rect.position))
	                                    {
	                                        ladderClosest = ladder;
	                                        ladderClosestClimbable = ladderClimbable;
	                                    }
	                                }
	                            }
	                            else
	                            {
	                                throw new System.NotImplementedException();
	                            }
	                        }

	                        if (ladderActive == null)
	                        {
	                            ladderActive = ladderClosest;
	                            ladderActiveClimbable = ladderClosestClimbable;
	                        }

	                        character.ladderActive = ladderActive;
	                        character.ladderActiveClimbable = ladderActiveClimbable;

	                        if (character.ladderActive != null && character.ladders.Count > 0)
	                        {
	                            character.rect.position += character.ladderActive.rect.position - character.ladderActive.lastPos;
	                        }

	                        if (ladderActive.gameElement == PhysBox.GameElement.Ladder)
	                        {
	                            //vertical input
	                            float velX = 0f;
	                            if (System.Math.Abs(dt) > float.Epsilon)
	                            {
	                                if ((ladderActive.rect.x - physObj.rect.x) > 0f)
	                                {
	                                    velX = Mathf.Min(6f, (ladderActive.rect.x - physObj.rect.x) / dt);
	                                }
	                                else
	                                {
	                                    velX = Mathf.Max(-6f, (ladderActive.rect.x - physObj.rect.x) / dt);
	                                }
	                            }
	                            physObj.vel = new Vector2(velX, physObj.input.y > 0f ? playerLadderClimbVerAcc : (physObj.input.y < 0f ? -playerLadderClimbVerAcc : 0f));
	                        }
	                        else if (ladderActive.gameElement == PhysBox.GameElement.Hanger)
	                        {
	                            float velX = 0f;
	                            float velY = 0f;
	                            if (System.Math.Abs(dt) > float.Epsilon)
	                            {
	                                if ((ladderActive.rect.x - physObj.rect.x) > 0f)
	                                {
	                                    velX = Mathf.Min(6f, (ladderActive.rect.x - physObj.rect.x) / dt);
	                                }
	                                else
	                                {
	                                    velX = Mathf.Max(-6f, (ladderActive.rect.x - physObj.rect.x) / dt);
	                                }
	                                if ((ladderActive.rect.y - physObj.rect.y) > 0f)
	                                {
	                                    velY = Mathf.Min(6f, (ladderActive.rect.y - physObj.rect.y) / dt);
	                                }
	                                else
	                                {
	                                    velY = Mathf.Max(-6f, (ladderActive.rect.y - physObj.rect.y) / dt);
	                                }
	                            }
	                            physObj.vel = new Vector2(velX, velY);
	                        }
	                        else if (ladderActiveClimbable != ClimbableTile.None)
	                        {
	                            ClimbablePhysBox climbablePhysBox = (ladderActive as ClimbablePhysBox);
	                            if (ladderActiveClimbable == ClimbableTile.R || ladderActiveClimbable == ClimbableTile.L)
	                            {
	                                float x = climbablePhysBox.GetClimbPlayerPosition(ladderActiveClimbable, physObj).x;
	                                //vertical input
	                                float velX = 0f;
	                                if (System.Math.Abs(dt) > float.Epsilon)
	                                {
	                                    if ((x - physObj.rect.x) > 0f)
	                                    {
	                                        velX = Mathf.Min(6f, (x - physObj.rect.x) / dt);
	                                    }
	                                    else
	                                    {
	                                        velX = Mathf.Max(-6f, (x - physObj.rect.x) / dt);
	                                    }
	                                }
	                                physObj.vel = new Vector2(velX, physObj.input.y > 0f ? playerLadderClimbVerAcc : (physObj.input.y < 0f ? -playerLadderClimbVerAcc : 0f));
	                            }
	                            else if (ladderActiveClimbable == ClimbableTile.D)
	                            {
	                                float y = climbablePhysBox.GetClimbPlayerPosition(ladderActiveClimbable, physObj).y;
	                                //horizontal input
	                                float velY = 0f;
	                                if (System.Math.Abs(dt) > float.Epsilon)
	                                {
	                                    if ((y - physObj.rect.y) > 0f)
	                                    {
	                                        velY = Mathf.Min(6f, (y - physObj.rect.y) / dt);
	                                    }
	                                    else
	                                    {
	                                        velY = Mathf.Max(-6f, (y - physObj.rect.y) / dt);
	                                    }
	                                }
	                                physObj.vel = new Vector2(physObj.input.x * 0.5f, velY);
	                            }
	                            else
	                            {
	                                throw new System.EntryPointNotFoundException();
	                            }
	                        }
	                        else
	                        {
	                            throw new System.NotImplementedException();
	                        }

	                        //if physbox wants to go down and is touching a ground, then stop holding onto the ladder
	                        if (character.input.y < 0f) {
	                            bool grounded = !CanMove(character, new List<PhysBox>(), new Vector2(0f, -0.1f), GetStaticObjectsForRect(character.rect));
	                            if (grounded) {
	                                character.ladderActive = null;
	                                character.ladders.Clear();
	                                character.onLadder = false;
	                            }
	                        }
	                    }
	                    else {
	                        List<PhysBox> grounds = new List<PhysBox>();
	                        bool grounded = !CanMove(physObj, grounds, new Vector2(0f, -0.1f), GetStaticObjectsForRect(physObj.rect))
	                            || !CanMoveDown(physObj, grounds, new Vector2(0f, -0.1f), alsoPlatforms);
	                        if (!grounded) {
	                            physObj.airCounter++;
	                        }
	                        else {
	                            physObj.airCounter = 0;
	                        }
	                        foreach (PhysBox g in grounds) {
	                            if ((physObj.groundIfBody == null || g == groundIfBodyOld) && g.physBody != null && g.rect.y < physObj.rect.y) {
	                                physObj.groundIfBody = g;
	                            }
	                            if (g is MovingPlatformPhysBox && movingPlatforms.Contains(g as MovingPlatformPhysBox)) {
	                                physObj.rect.position += g.vel * dt;
	                            }
	                        }
	                        if (physObj.groundIfBody != null)
	                        {
	                            physObj.rect.position += physObj.groundIfBody.rect.position - physObj.groundIfBody.lastPos;
	                        }

	                        //jump input
	                        bool justJumped = false;
	                        if (physObj.inputJump) {
	                            if (!grounded && physObj.airCounter > OnAirLimit) {
	                                if (physObj.vel.y > 1) {
	                                    physObj.vel += new Vector2(0f, playerVerAcc * dt);
	                                }
	                            }
	                            else if ((!physObj.inputJumpOld && physObj.inputJump) || (physObj.inputOld.y <= 0f && physObj.input.y > 0f)) {
	                                bool isOnLadder = false;
	                                bool isLadderDown = true;
	                                if (physObj is CharacterPhysBox) {
	                                    CharacterPhysBox character = (physObj as CharacterPhysBox);
	                                    isOnLadder = character.onLadder;
	                                    if (isOnLadder) {
	                                        isLadderDown = (character.ladderActiveClimbable == ClimbableTile.D);
	                                    }
	                                }
	                                if (grounded || !isOnLadder || !isLadderDown) {
	                                    justJumped = true;
	                                    physObj.vel = new Vector2(physObj.vel.x, playerJump);
	                                    physObj.airCounter = OnAirLimit + 1;
	                                    if (physObj == player) {
	                                        AudioPlayer.instance.Play(AudioPlayer.instance.clipPlayerJump);
	                                    }
	                                }
	                            }
	                        }
	                        //gravity
	                        if (!grounded && !justJumped && physObj.airCounter > OnAirLimit) {
	                            physObj.vel += new Vector2(0f, gravity * dt * physObj.gravityMult);
	                        }
	                        if (!physObj.isGroundedOld && grounded) {
	                            if (physObj == player) {
	                                AudioPlayer.instance.Play(AudioPlayer.instance.clipPlayerHitGround);
	                            }
	                            Color color = new Color(1f, 1f, 1f);
	                            if (grounds.Count > 0 && grounds[0].GetComponent<SpriteRenderer>() != null) {
	                                color = grounds[0].GetComponent<SpriteRenderer>().color;
	                            }
	                            Vector3 dustPos = new Vector3(physObj.rect.x, physObj.rect.yMin, 1f);
	                            Dust dust = ObjectPool.dustPool.get(dustPos).GetComponent<Dust>();
	                            dust.dir = 1f;
	                            dust.color = color;
	                            dust = ObjectPool.dustPool.get(dustPos).GetComponent<Dust>();
	                            dust.dir = -1f;
	                            dust.color = color;
	                        }
	                        physObj.isGroundedOld = grounded;
	                    }
	                    //check max speed
	                    bool onLadder = physObj is CharacterPhysBox && (physObj as CharacterPhysBox).onLadder;
	                    float maxSpeedHor = onLadder ? physObj.MaxSpeedHor * 0.5f : physObj.MaxSpeedHor;
	                    if (Mathf.Abs(physObj.vel.x) > maxSpeedHor) {
	                        physObj.vel = new Vector2(physObj.vel.x > 0f ? maxSpeedHor : -maxSpeedHor, physObj.vel.y);
	                    }
	                    float maxSpeedVer = onLadder ? playerLadderClimbVerAcc : physObj.MaxSpeedVer;
	                    if (Mathf.Abs(physObj.vel.y) > maxSpeedVer) {
	                        physObj.vel = new Vector2(physObj.vel.x, physObj.vel.y > 0f ? maxSpeedVer : -maxSpeedVer);
	                    }
	                    //update position and check collisions FOR INDIVIDUALS
	                    if (physObj.physBody == null) {
	                        UpdatePositionAndResolveCollision(physObj, physObj.vel);
	                    }
	                }

	                if (amountDropMoney > 0 && positionsDropMoney.Count > 0) DropMoney();
	            }
	        }
	        BombUI.numBombs = Platformer.numBombs;
	        restartCounter++;
	        inputFire2Old = TaloketoInputManager.GetButton("Fire2");
	        inputBackOld = TaloketoInputManager.GetButton("Back");
	    }

	    private void UpdatePositionAndResolveCollision(PhysBox physObj, Vector2 vel) {
	        List<PhysBox> collidedObjects = new List<PhysBox>();
	        Vector2 moveVectorX = new Vector2(vel.x, 0f) * dt;
	        if (!physObj.simplifiedPhysics || Mathf.Abs(moveVectorX.x) > float.Epsilon) {
	            if (CanMove(physObj, collidedObjects, moveVectorX, GetStaticObjectsForRect(physObj.rect))) {
	                physObj.rect.position += moveVectorX;
	            }
	            else {
	                ResolveCollisionX(physObj, collidedObjects, moveVectorX);
	            }
	            collidedObjects.Clear();
	        }
	        Vector2 moveVectorY = new Vector2(0f, vel.y) * dt;
	        if (!physObj.simplifiedPhysics || Mathf.Abs(moveVectorY.y) > float.Epsilon) {
	            if (CanMove(physObj, collidedObjects, moveVectorY, GetStaticObjectsForRect(physObj.rect)) && CanMoveDown(physObj, collidedObjects, moveVectorY, alsoPlatforms)) {
	                physObj.rect.position += moveVectorY;
	            }
	            else {
	                ResolveCollisionY(physObj, collidedObjects, moveVectorY);
	            }
	        }
	    }

	    private void UpdatePositionAndResolveCollision(PhysNode physNode, Vector2 vel) {
	        List<PhysBox> collidedObjects = new List<PhysBox>();
	        Vector2 moveVectorX = new Vector2(vel.x, 0f) * dt;
	        if (Mathf.Abs(moveVectorX.x) > float.Epsilon) {
	            bool canMove = true;
	            List<PhysNode> nodesToCheck = new List<PhysNode>();
	            nodesToCheck.Add(physNode);
	            while (canMove && nodesToCheck.Count > 0) {
	                List<PhysNode> newNodesToCheck = new List<PhysNode>();
	                foreach (PhysNode node in nodesToCheck) {
	                    if (!CanMove(node.physBox, collidedObjects, moveVectorX, GetStaticObjectsForRect(node.physBox.rect))) {
	                        canMove = false;
	                        Vector2 pos = node.physBox.rect.position;
	                        ResolveCollisionX(node.physBox, collidedObjects, moveVectorX);
	                        moveVectorX = node.physBox.rect.position - pos;
	                        node.physBox.rect.position = pos;
	                        break;
	                    }
	                    collidedObjects.Clear();
	                    foreach (PhysNode child in node.children) {
	                        newNodesToCheck.Add(child);
	                    }
	                }
	                nodesToCheck = newNodesToCheck;
	            }
	        }
	        Vector2 moveVectorY = new Vector2(0f, vel.y) * dt;
	        if (Mathf.Abs(moveVectorY.y) > float.Epsilon) {
	            bool canMove = true;
	            List<PhysNode> nodesToCheck = new List<PhysNode>();
	            nodesToCheck.Add(physNode);
	            while (canMove && nodesToCheck.Count > 0) {
	                List<PhysNode> newNodesToCheck = new List<PhysNode>();
	                foreach (PhysNode node in nodesToCheck) {
	                    if (!CanMove(node.physBox, collidedObjects, moveVectorY, GetStaticObjectsForRect(node.physBox.rect))) {
	                        canMove = false;
	                        Vector2 pos = node.physBox.rect.position;
	                        ResolveCollisionY(node.physBox, collidedObjects, moveVectorX);
	                        moveVectorY = node.physBox.rect.position - pos;
	                        node.physBox.rect.position = pos;
	                        break;
	                    }
	                    collidedObjects.Clear();
	                    foreach (PhysNode child in node.children) {
	                        newNodesToCheck.Add(child);
	                    }
	                }
	                nodesToCheck = newNodesToCheck;
	            }
	        }
	        if (Mathf.Abs(moveVectorX.x) > float.Epsilon || Mathf.Abs(moveVectorY.y) > float.Epsilon) {
	            List<PhysNode> nodesToCheck = new List<PhysNode>();
	            nodesToCheck.Add(physNode);
	            while (nodesToCheck.Count > 0) {
	                List<PhysNode> newNodesToCheck = new List<PhysNode>();
	                foreach (PhysNode node in nodesToCheck) {
	                    node.physBox.rect.position += moveVectorX + moveVectorY;
	                    foreach (PhysNode child in node.children) {
	                        newNodesToCheck.Add(child);
	                    }
	                }
	                nodesToCheck = newNodesToCheck;
	            }
	        }
	    }

	    private void ResolveCollision(PhysBox dynamicObj, List<PhysBox> collidedObjects, Vector2 moveVector, bool skipBodyParts = false) {
	        dynamicObj.rect.position += moveVector;
	        foreach (PhysBox collidedObj in collidedObjects) {
	            if (!skipBodyParts || collidedObj.physBody == null) ResolveCollision(dynamicObj, collidedObj, moveVector);
	        }
	    }

	    private void ResolveCollisionX(PhysBox dynamicObj, List<PhysBox> collidedObjects, Vector2 moveVector) {
	        dynamicObj.rect.position += moveVector;
	        foreach (PhysBox collidedObj in collidedObjects) {
	            ResolveCollisionX(dynamicObj, collidedObj, moveVector);
	        }
	    }

	    private void ResolveCollisionY(PhysBox dynamicObj, List<PhysBox> collidedObjects, Vector2 moveVector) {
	        dynamicObj.rect.position += moveVector;
	        foreach (PhysBox collidedObj in collidedObjects) {
	            ResolveCollisionY(dynamicObj, collidedObj, moveVector);
	        }
	    }

	    private void ResolveCollision(PhysBox dynamicObj, PhysBox collidedObj, Vector2 moveVector) {
	        List<Vector2> solutions = new List<Vector2>();
	        solutions.Add(new Vector2(dynamicObj.rect.x + collidedObj.rect.xMin - dynamicObj.rect.xMax, dynamicObj.rect.y));
	        solutions.Add(new Vector2(dynamicObj.rect.x + collidedObj.rect.xMax - dynamicObj.rect.xMin, dynamicObj.rect.y));
	        solutions.Add(new Vector2(dynamicObj.rect.x, dynamicObj.rect.y + collidedObj.rect.yMin - dynamicObj.rect.yMax));
	        solutions.Add(new Vector2(dynamicObj.rect.x, dynamicObj.rect.y + collidedObj.rect.yMax - dynamicObj.rect.yMin));

	        float minLen = 1000000;
	        int selected = -1;
	        for (int i = 0, len = solutions.Count; i < len; i++) {
	            float l = Geometry.lengthOfVector2(dynamicObj.rect.position - solutions[i]) + Geometry.lengthOfVector2(dynamicObj.rect.position - moveVector - solutions[i]);
	            if (minLen > l) {
	                minLen = l;
	                selected = i;
	            }
	        }
	        if (selected != -1) {
	            dynamicObj.rect.position = solutions[selected];
	        }
	    }

	    private void ResolveCollisionX(PhysBox dynamicObj, PhysBox collidedObj, Vector2 moveVector) {
	        List<Vector2> solutions = new List<Vector2>();
	        solutions.Add(new Vector2(dynamicObj.rect.x + collidedObj.rect.xMin - dynamicObj.rect.xMax, dynamicObj.rect.y));
	        solutions.Add(new Vector2(dynamicObj.rect.x + collidedObj.rect.xMax - dynamicObj.rect.xMin, dynamicObj.rect.y));

	        float minLen = 1000000;
	        int selected = -1;
	        for (int i = 0, len = solutions.Count; i < len; i++) {
	            float l = Geometry.lengthOfVector2(dynamicObj.rect.position - solutions[i]) + Geometry.lengthOfVector2(dynamicObj.rect.position - moveVector - solutions[i]);
	            if (minLen > l) {
	                minLen = l;
	                selected = i;
	            }
	        }
	        if (selected != -1) {
	            dynamicObj.rect.position = solutions[selected];
	            dynamicObj.vel = new Vector2(0f, dynamicObj.vel.y);
	            if (dynamicObj is EnemyDiagonalPhysBox) {
	                (dynamicObj as EnemyDiagonalPhysBox).enemyDiagonalIsRight = (selected == 1);
	            }
	        }
	    }

	    private void ResolveCollisionY(PhysBox dynamicObj, PhysBox collidedObj, Vector2 moveVector) {
	        List<Vector2> solutions = new List<Vector2>();
	        solutions.Add(new Vector2(dynamicObj.rect.x, dynamicObj.rect.y + collidedObj.rect.yMin - dynamicObj.rect.yMax));
	        solutions.Add(new Vector2(dynamicObj.rect.x, dynamicObj.rect.y + collidedObj.rect.yMax - dynamicObj.rect.yMin));

	        float minLen = 1000000;
	        int selected = -1;
	        for (int i = 0, len = solutions.Count; i < len; i++) {
	            float l = Geometry.lengthOfVector2(dynamicObj.rect.position - solutions[i]) + Geometry.lengthOfVector2(dynamicObj.rect.position - moveVector - solutions[i]);
	            if (minLen > l) {
	                minLen = l;
	                selected = i;
	            }
	        }
	        if (selected != -1) {
	            dynamicObj.rect.position = solutions[selected];
	            dynamicObj.vel = new Vector2(dynamicObj.vel.x, Mathf.Max(0f, dynamicObj.vel.y - frictionWhenTouchingUpWall));
	            if (dynamicObj is EnemyDiagonalPhysBox) {
	                (dynamicObj as EnemyDiagonalPhysBox).enemyDiagonalIsUp = (selected == 1);
	            }
	        }
	    }

	    private bool CanMove(PhysBox dynamicObj, List<PhysBox> collidedObjects, Vector2 deltaPos, List<PhysBox> checkObjects) {
	        Rectangle newRect = dynamicObj.rect.Copy();
	        newRect.position += deltaPos;
	        foreach (PhysBox physObj in checkObjects) {
	            if (!physObj.isActiveAndEnabled || dynamicObj == physObj || (dynamicObj.physBody != null && dynamicObj.physBody == physObj.physBody)) continue;
	            if (newRect.Overlaps(physObj.rect)) {
	                collidedObjects.Add(physObj);
	            }
	        }
	        return collidedObjects.Count == 0;
	    }

	    private bool CanMoveDown(PhysBox dynamicObj, List<PhysBox> collidedObjects, Vector2 deltaPos, List<PhysBox> checkObjects) {
	        if (deltaPos.y >= 0f) return true;
	        Rectangle newRect = dynamicObj.rect.Copy();
	        newRect.position += deltaPos;
	        foreach (PhysBox physObj in checkObjects) {
	            if (!physObj.isActiveAndEnabled || (dynamicObj.physBody != null && dynamicObj.physBody == physObj.physBody)) continue;
	            Rectangle checkRect = physObj.rect.Copy();
	            checkRect.w = 1f;
	            checkRect.h = 0.1f;
	            checkRect.y += 0.45f;
	            if (newRect.Overlaps(checkRect) && dynamicObj.rect.y > checkRect.y + dynamicObj.rect.w * 0.5f) {
	                collidedObjects.Add(physObj);
	            }
	        }
	        return collidedObjects.Count == 0;
	    }

	    private List<PhysBox> GetStaticObjectsForRect(Rectangle rect) {
	        int iMin = GetBroadPhaseIndexForX(rect.xMin - 0.5f);
	        int iMax = GetBroadPhaseIndexForX(rect.xMax + 0.5f);
	        int jMin = GetBroadPhaseIndexForY(rect.yMin - 0.5f);
	        int jMax = GetBroadPhaseIndexForY(rect.yMax + 0.5f);
	        if (iMin == iMax && jMin == jMax) return staticObjects[iMin, jMin];
	        List<PhysBox> res = new List<PhysBox>();
	        for (int i = iMin; i <= iMax; i++) {
	            for (int j = jMin; j <= jMax; j++) {
	                foreach (PhysBox physBox in staticObjects[i, j]) {
	                    if (!res.Contains(physBox)) {
	                        res.Add(physBox);
	                    }
	                }
	            }
	        }
	        return res;
	    }

	    private int GetBroadPhaseIndexForX(float x) {
	        return Mathf.Clamp(Mathf.FloorToInt(x / BROAD_PHASE_SIZE), 0, broadPhaseWidth - 1);
	    }

	    private int GetBroadPhaseIndexForY(float y) {
	        return Mathf.Clamp(Mathf.FloorToInt(y / BROAD_PHASE_SIZE), 0, broadPhaseHeight - 1);
	    }

	    private void FireLaser(CharacterPhysBox firer, bool isVertical) {
	        AudioPlayer.instance.Play(AudioPlayer.instance.clipEnemyLaser);
	        firer.waitTime = 3f;
	        firer.fireCoolingTime = 3f;
	        int xDir = isVertical ? 0 : (firer.isRight ? 1 : -1);
	        int yDir = isVertical ? (firer.isRight ? 1 : -1) : 0;
	        int x = Mathf.RoundToInt(firer.transform.position.x) + xDir;
	        int y = Mathf.RoundToInt(firer.transform.position.y) + yDir;
	        int xFirst = x;
	        int yFirst = y;
	        bool hitWall = false;
	        while (x >= 0 && x < LevelEditor.level.GetLength(0) && y >= 0 && y < LevelEditor.level.GetLength(1)) {
	            if (LevelEditor.level[x, y] != null && LevelEditor.level[x, y].GetComponent<PhysBox>().kind == PhysBox.Kind.Static) {
	                hitWall = true;
	                break;
	            }
	            Rectangle r = new Rectangle(new Vector2(x, y), new Vector2(0.5f, 0.5f));
	            foreach (PhysBox physObj in GetStaticObjectsForRect(r)) {
	                if (!physObj.isActiveAndEnabled) continue;
	                if (physObj.rect.Overlaps(r)) {
	                    hitWall = true;
	                    break;
	                }
	            }
	            if (hitWall) break;
	            x += xDir;
	            y += yDir;
	        }
	        if (x == xFirst && y == yFirst) return;
	        x -= xDir;
	        y -= yDir;
	        Vector3 pos = new Vector3((x + xFirst) * 0.5f, (y + yFirst) * 0.5f, -1);
	        pos += new Vector3(firer.transform.position.x - Mathf.Round(firer.transform.position.x), firer.transform.position.y - Mathf.Round(firer.transform.position.y), 0f);
	        PhysBox fire = ObjectPool.laserEnemyPool.getPhys(pos);
	        if (isVertical) {
	            fire.rect.w = 0.5f;
	            fire.rect.h = Mathf.Abs(y - yFirst) + 1;
	            fire.GetComponent<FireEnemyAnim>().isVertical = true;
	            fire.transform.localScale = new Vector3(1f, fire.rect.h, 1f);
	        }
	        else {
	            fire.rect.w = Mathf.Abs(x - xFirst) + 1;
	            fire.rect.h = 0.5f;
	            fire.GetComponent<FireEnemyAnim>().isVertical = false;
	            fire.transform.localScale = new Vector3(fire.rect.w, 1f, 1f);
	        }
	    }

	    private void FirePistol(CharacterPhysBox firer) {
	        AudioPlayer.instance.Play(AudioPlayer.instance.clipEnemyShoot);
	        firer.waitTime = 0.5f;
	        firer.fireCoolingTime = 2f;
	        Vector3 pos = firer.rect.position;
	        PhysBox fire = ObjectPool.fireEnemyPool.getPhys(pos);

	        if (firer.isRight)
	        {
	            fire.GetComponent<FirePlayerAnim>().angle = 0f;
	        }
	        else
	        {
	            fire.GetComponent<FirePlayerAnim>().angle = 180f;
	        }
	    }

	    private void FirePlayer(bool isFiringDown) {
	        AudioPlayer.instance.Play(AudioPlayer.instance.clipPlayerShoot);
	        Vector3 pos = player.rect.position;
	        PhysBox fire = ObjectPool.firePlayerPool.getPhys(pos);


	        if (isFiringDown) {
	            fire.GetComponent<FirePlayerAnim>().angle = -90f;
	        }
	        else {
	            bool isRight = player.isRight;
	            if (player.ladders.Count > 0) {
	                if (player.ladderActiveClimbable == ClimbableTile.L) {
	                    isRight = false;
	                }
	                else if (player.ladderActiveClimbable == ClimbableTile.R) {
	                    isRight = true;
	                }
	            }
	            if (isRight) {
	                fire.GetComponent<FirePlayerAnim>().angle = 0f;
	            }
	            else {
	                fire.GetComponent<FirePlayerAnim>().angle = 180f;
	            }
	        }
	    }

	    private void Die(PhysBox something) {
	        something.gameObject.SetActive(false);
	        if (something.GetComponent<CharacterPhysBox>() != null) {
	            CharacterPhysBox character = something.GetComponent<CharacterPhysBox>();
	            if (character.moneyToDrop > 0) {
	                amountDropMoney += character.moneyToDrop;
	                positionsDropMoney.Add(something.transform.position);
	            }
	        }
	        if (something.GetComponent<SpriteRenderer>() != null) {
	            Dead dead = ObjectPool.deadPool.get(something.transform.position).GetComponent<Dead>();
	            dead.color = something.GetComponent<SpriteRenderer>().color;
	            dead.GetComponent<SpriteRenderer>().sprite = something.GetComponent<SpriteRenderer>().sprite;
	            dead.scale = something.transform.localScale;
	        }
	    }

	    private void DropMoney() {
	        List<int> moneyKinds = new List<int>() {
	            20,
	            15,
	            10,
	            5,
	            3,
	            2,
	            1
	        };
	        int moneyLeft = amountDropMoney;
	        for (int i = 0, len = moneyKinds.Count; i < len; i++) {
	            int moneyKind = moneyKinds[i];
	            int numMoney = moneyLeft;
	            if (moneyKind != 1) {
	                int maxNumOfThisKind = Mathf.Max(0, moneyLeft / moneyKind);
	                numMoney = UnityEngine.Random.Range(0, maxNumOfThisKind);
	                moneyLeft -= numMoney * moneyKind;
	            }
	            for (int j = 0; j < numMoney; j++) {
	                MoneyPhysBox money = ObjectPool.moneyPool.getPhys(positionsDropMoney[Random.Range(0, positionsDropMoney.Count)]) as MoneyPhysBox;
	                if (!dynamicObjects.Contains(money)) {
	                    dynamicObjects.Add(money);
	                }
	                money.vel = new Vector2(0f, Random.Range(1f, 2f));
	                money.input = money.inputFirst = new Vector2(Random.Range(-money.MaxSpeedHor, money.MaxSpeedHor), Random.Range(2f, money.MaxSpeedVer));
	                money.timerInvincibility = 0.25f;
	                money.SetAmount(moneyKind);
	            }
	        }
	        amountDropMoney = 0;
	        positionsDropMoney.Clear();
	    }

	    private void Damage(CharacterPhysBox something) {
	        something.hp--;
	        something.invincibilityTime = something.invincibilityTimeMax;
	        if (something.hp <= 0) {
	            freezeScreen = 0.1f;
	            AudioPlayer.instance.Play(AudioPlayer.instance.clipEnemyDie);
	            Die(something);
	        }
	    }

	    private void Damage(BossVulnerablePhysBox something) {
	        something.hp--;
	        something.invincibilityTime = 0.5f;
	        if (something.hp <= 0) {
	            freezeScreen = 0.1f;
	            AudioPlayer.instance.Play(AudioPlayer.instance.clipEnemyDie);
	            Die(something);
	            if (something.physBody != null) {
	                if (something.physBody.root.physBox.gameElement == PhysBox.GameElement.BossRoot || something.physBody.root.physBox.gameElement == PhysBox.GameElement.BossRootSpider) {
	                    int numAliveVulnerables = 0;
	                    foreach (PhysBox bossVulnerable in something.physBody.root.physBox.bossVulnerablePoints) {
	                        if (bossVulnerable.isActiveAndEnabled) {
	                            numAliveVulnerables++;
	                        }
	                    }
	                    BossAnim bossAnim = something.physBody.root.physBox.GetComponent<BossAnim>();
	                    if (numAliveVulnerables == 0) {
	                        something.physBody.deathTimer = 3f;
	                        AudioPlayer.instance.Play(AudioPlayer.instance.clipBossDie0);
	                        LevelEditor. OnBossKilled();
	                    }
	                    else if (bossAnim != null) {
	                        if (numAliveVulnerables == 1) {
	                            bossAnim.phase = BossAnim.Phase.Phase2;
	                            bossAnim.animTimerHor = 0f;
	                            bossAnim.animTimerVer = 0f;
	                        }
	                        bossAnim.hurtTimer = Random.Range(3f, 8f);
	                        bossAnim.animTimerHor = 0f;
	                        bossAnim.animTimerVer = 0f;
	                    }
	                }
	            }
	        }
	    }

	    private void CalculateBodies() {
	        bodies = new List<PhysBody>();
	        foreach (PhysBox physBox in allObjects) {
	            physBox.physBody = null;
	            physBox.physNode = null;
	            if (physBox.parent != null) {
	                PhysBox parent = physBox.parent;
	                while (parent.parent != null) {
	                    parent = parent.parent;
	                }
	                bool doesBodyExist = false;
	                foreach (PhysBody body in bodies) {
	                    if (body.root.physBox == parent) {
	                        doesBodyExist = true;
	                        PhysNode node = new PhysNode();
	                        node.physBox = physBox;
	                        body.all.Add(node);
	                        break;
	                    }
	                }
	                if (!doesBodyExist) {
	                    PhysBody body = new PhysBody();
	                    PhysNode root = new PhysNode();
	                    root.physBox = parent;
	                    body.root = root;
	                    body.all = new List<PhysNode>();
	                    body.all.Add(root);

	                    PhysNode node = new PhysNode();
	                    node.physBox = physBox;
	                    body.all.Add(node);
	                    bodies.Add(body);
	                }
	            }
	        }

	        foreach (PhysBody body in bodies) {
	            body.root.physBox.bossVulnerablePoints = new List<PhysBox>();
	            foreach (PhysNode node0 in body.all) {
	                node0.physBox.physBody = body;
	                node0.physBox.physNode = node0;
	                node0.children = new List<PhysNode>();
	                if (node0.physBox.gameElement == PhysBox.GameElement.BossVulnerable) {
	                    body.root.physBox.bossVulnerablePoints.Add(node0.physBox);
	                    (node0.physBox as BossVulnerablePhysBox).hp = 3;
	                }
	                foreach (PhysNode node1 in body.all) {
	                    if (node0.physBox.parent == node1.physBox) {
	                        node0.parent = node1;
	                    }
	                    else if (node0.physBox == node1.physBox.parent) {
	                        node0.children.Add(node1);
	                    }
	                }
	                if (node0.physBox is ClimbablePhysBox) {
	                    (node0.physBox as ClimbablePhysBox).SetClimbable();
	                }
	                if (node0.physBox.kind == PhysBox.Kind.Static) {
	                    node0.iBroadPhaseMin = GetBroadPhaseIndexForX(node0.physBox.rect.xMin);
	                    node0.jBroadPhaseMin = GetBroadPhaseIndexForY(node0.physBox.rect.yMin);
	                    node0.iBroadPhaseMax = GetBroadPhaseIndexForX(node0.physBox.rect.xMax);
	                    node0.jBroadPhaseMax = GetBroadPhaseIndexForY(node0.physBox.rect.yMax);

	                    for (int i = node0.iBroadPhaseMin; i <= node0.iBroadPhaseMax; i++) {
	                        for (int j = node0.jBroadPhaseMin; j <= node0.jBroadPhaseMax; j++) {
	                            staticObjects[i, j].Add(node0.physBox);
	                        }
	                    }
	                }
	            }
	        }
	    }
	}
}
