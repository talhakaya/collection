using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collection.Controls;

namespace Games.Golfinity
{
	public class Map : MonoBehaviour
	{
	    public GameObject levelUiPrefab;
	    public LineRenderer[] paths;
	    public LineRenderer[] bgs;
	    public MapLockUi lockUi;
	    private List<MapLevelUi> levelUis;
	    private const int numLevelUis = 9;
	    private Vector3 mousePosOld;
	    private int holeNo;
	    private float xDelta;
	    private float inertia;
	    private float inputTime;

	    private void Awake()
	    {
	        levelUis = new List<MapLevelUi>();
	        for (int i = 0; i < numLevelUis; i++)
	        {
	            MapLevelUi levelUi = Instantiate(levelUiPrefab, transform).GetComponent<MapLevelUi>();
	            levelUi.transform.localPosition = new Vector3(0f, 0f, -1f - 0.3f * i);
	            levelUis.Add(levelUi);
	        }
	    }

	    public void Init(int holeNo)
	    {
	        this.holeNo = holeNo;
	        xDelta = 0f;
	        inertia = 0f;
	        Game.instance.transform.position = new Vector3(0f, 0f, Game.instance.transform.position.z);
	        Game.instance.ball.ResetBall();
	        Refresh(true);
	    }

	    private void Update()
	    {
	        if (UIReferences.cheatPopup.gameObject.activeSelf || UIReferences.levelScorePopup.gameObject.activeSelf || UIReferences.optionsPopup.gameObject.activeSelf || UIReferences.upgradePopup.gameObject.activeSelf || lockUi.isPaying)
	        {
	            return;
	        }
	        foreach (var level in levelUis)
	        {
	            level.uiUpdate = false;
	        }
	        inputTime += Time.deltaTime;
	#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
	        foreach (Touch touch in Input.touches)
	        {
	            switch (touch.phase)
	            {
	                case TouchPhase.Began:
	                    {
	                        inputTime = 0f;
	                        var level = GetMapUiWithPointer(touch.position);
	                        if (level != null)
	                        {
	                            level.OnOver(true);
	                            level.uiUpdate = true;
	                        }
	                        inertia = 0f;
	                    }
	                    break;
	                case TouchPhase.Stationary:
	                    {
	                        var level = GetMapUiWithPointer(touch.position);
	                        if (level != null)
	                        {
	                            level.OnOver(true);
	                            level.uiUpdate = true;
	                        }
	                        inertia = 0f;
	                    }
	                    break;
	                case TouchPhase.Moved:
	                    {
	                        var level = GetMapUiWithPointer(touch.position);
	                        if (level != null)
	                        {
	                            level.OnOver(true);
	                            level.uiUpdate = true;
	                        }
	                        else
	                        {
	                            MoveMap(touch.deltaPosition);
	                        }
	                        inertia = 0f;
	                    }
	                    break;
	                case TouchPhase.Ended:
	                    {
	                        if (inputTime <= 0.3f)
	                        {
	                            var level = GetMapUiWithPointer(touch.position);
	                            if (level != null)
	                            {
	                                level.OnClick();
	                                level.uiUpdate = true;
	                            }
	                        }
	                        inertia = GetDragAmount(touch.deltaPosition);
	                    }
	                    break;
	            }
	        }
	#else
	        if (TaloketoInputManager.GetMouseButtonDown(0))
	        {
	            inputTime = 0f;
	        }
	        if (TaloketoInputManager.GetMouseButton(0))
	        {
	            var level = GetMapUiWithPointer(TaloketoInputManager.mousePosition);
	            if (level != null)
	            {
	                level.OnOver(true);
	                level.uiUpdate = true;
	            }
	            else
	            {
	                if (mousePosOld != default) MoveMap(TaloketoInputManager.mousePosition - mousePosOld);
	            }
	            inertia = 0f;
	            mousePosOld = TaloketoInputManager.mousePosition;
	        }
	        if (TaloketoInputManager.GetMouseButtonUp(0))
	        {
	            if (inputTime <= 0.3f)
	            {
	                var level = GetMapUiWithPointer(TaloketoInputManager.mousePosition);
	                if (level != null)
	                {
	                    level.OnClick();
	                    level.uiUpdate = true;
	                }
	            }
	            inertia = GetDragAmount(TaloketoInputManager.mousePosition - mousePosOld);
	            mousePosOld = default;
	        }
	#endif

	        if (inertia != 0f)
	        {
	            ApplyInertia();
	        }

	        foreach (var level in levelUis)
	        {
	            if (!level.uiUpdate) level.OnOver(false);
	        }
	    }

	    private MapUi GetMapUiWithPointer(Vector2 pointerPosition)
	    {
	        Vector3 position = Game.cam.ScreenToWorldPoint(pointerPosition);
	        List<MapUi> eligibles = new List<MapUi>();
	        foreach (var level in levelUis)
	        {
	            if (!level.interactable) continue;
	            if (Mathf.Abs(level.transform.position.x - position.x) < 6f && Mathf.Abs(level.transform.position.y - position.y) < 4f) eligibles.Add(level);
	        }
	        if (lockUi.gameObject.activeInHierarchy && lockUi.interactable && Mathf.Abs(lockUi.transform.position.x - position.x) < 6f && Mathf.Abs(lockUi.transform.position.y - position.y) < 4f) eligibles.Add(lockUi);
	        MapUi selected = null;
	        foreach (var level in eligibles)
	        {
	            if (selected == null || (selected.transform.position - position).magnitude > (level.transform.position - position).magnitude) selected = level;
	        }
	        return selected;
	    }

	    private void ApplyInertia()
	    {
	        const float power = 50f;
	        const float friction = 2f;
	        MoveMap(power * inertia * Time.deltaTime);
	        float inertiaSign = Mathf.Sign(inertia);
	        float inertiaAbs = Mathf.Abs(inertia);
	        inertiaAbs = Mathf.Max(0f, inertiaAbs - friction * Time.deltaTime);
	        inertia = inertiaAbs * inertiaSign;
	    }

	    private float GetDragAmount(Vector2 pointerDeltaPos)
	    {
	        Vector3 pos0 = Game.cam.ScreenToWorldPoint(Vector3.zero);
	        Vector3 pos1 = Game.cam.ScreenToWorldPoint(pointerDeltaPos);
	        return pos1.x - pos0.x;
	    }

	    private void MoveMap(float dragAmount)
	    {
	        xDelta += dragAmount;
	        int holeNoOld = holeNo;
	        while (xDelta < -Spline.distPerPoint)
	        {
	            if (holeNo % LevelGenerator.numLevelsPerColor == LevelGenerator.numLevelsPerColor - 1)
	            {
	                if (xDelta < -2f * Spline.distPerPoint)
	                {
	                    xDelta += 2f * Spline.distPerPoint;
	                    holeNo++;
	                }
	                break;
	            }
	            xDelta += Spline.distPerPoint;
	            holeNo++;
	        }
	        while (xDelta > Spline.distPerPoint)
	        {
	            if (holeNo % LevelGenerator.numLevelsPerColor == 0)
	            {
	                if (xDelta > 2f * Spline.distPerPoint)
	                {
	                    xDelta -= 2f * Spline.distPerPoint;
	                    holeNo--;
	                }
	                break;
	            }
	            xDelta -= Spline.distPerPoint;
	            holeNo--;
	        }
	        if (holeNo < 1)
	        {
	            holeNo = 1;
	            xDelta = Spline.distPerPoint;
	            inertia = 0f;
	        }
	        Refresh(holeNoOld != holeNo);
	    }

	    private void MoveMap(Vector2 pointerDeltaPos)
	    {
	        MoveMap(GetDragAmount(pointerDeltaPos));
	    }

	    public void Refresh(bool fullRefresh)
	    {
	        int colorIndex = (holeNo / LevelGenerator.numLevelsPerColor) % Game.instance.level.terrainColors.Length;
	        int holeModColor = holeNo % LevelGenerator.numLevelsPerColor;
	        bool usePath1 = holeModColor < LevelGenerator.numLevelsPerColor / 2;
	        if (usePath1)
	        {
	            transform.position = new Vector3(-holeModColor * Spline.distPerPoint + xDelta, transform.position.y, transform.position.z);
	        }
	        else
	        {
	            transform.position = new Vector3((LevelGenerator.numLevelsPerColor + 1 - holeModColor) * Spline.distPerPoint + xDelta, transform.position.y, transform.position.z);
	        }
	        if (!fullRefresh) return;
	        if (usePath1)
	        {
	            Color terrainColor = Game.instance.level.terrainColors[(colorIndex - 1 + Game.instance.level.terrainColors.Length) % Game.instance.level.terrainColors.Length];
	            Color skyColor = Game.instance.level.skyColors[(colorIndex - 1 + Game.instance.level.skyColors.Length) % Game.instance.level.skyColors.Length];
	            paths[0].startColor = terrainColor;
	            paths[0].endColor = terrainColor;
	            bgs[0].startColor = skyColor;
	            bgs[0].endColor = skyColor;
	            terrainColor = Game.instance.level.terrainColors[colorIndex];
	            skyColor = Game.instance.level.skyColors[colorIndex];
	            paths[1].startColor = terrainColor;
	            paths[1].endColor = terrainColor;
	            bgs[1].startColor = skyColor;
	            bgs[1].endColor = skyColor;
	        }
	        else
	        {
	            Color terrainColor = Game.instance.level.terrainColors[colorIndex];
	            Color skyColor = Game.instance.level.skyColors[colorIndex];
	            paths[0].startColor = terrainColor;
	            paths[0].endColor = terrainColor;
	            bgs[0].startColor = skyColor;
	            bgs[0].endColor = skyColor;
	            terrainColor = Game.instance.level.terrainColors[(colorIndex + 1) % Game.instance.level.terrainColors.Length];
	            skyColor = Game.instance.level.skyColors[(colorIndex + 1) % Game.instance.level.skyColors.Length];
	            paths[1].startColor = terrainColor;
	            paths[1].endColor = terrainColor;
	            bgs[1].startColor = skyColor;
	            bgs[1].endColor = skyColor;
	        }
	        int index = 0;
	        int positionCount = paths[0].positionCount;
	        int unbeatenLastHole = Game.GetUnbeatenLastHole();
	        for (int i = holeNo - (numLevelUis / 2); i < holeNo + numLevelUis - (numLevelUis / 2); i++)
	        {
	            Vector3 pos = paths[0].GetPosition((((i % LevelGenerator.numLevelsPerColor) + 1) * Spline.resolution + positionCount) % positionCount);
	            if (!usePath1)
	            {
	                if (i % LevelGenerator.numLevelsPerColor >= LevelGenerator.numLevelsPerColor * 0.25f) pos -= new Vector3((LevelGenerator.numLevelsPerColor + 1) * Spline.distPerPoint, 0f, 0f);
	            }
	            else
	            {
	                if (i % LevelGenerator.numLevelsPerColor >= LevelGenerator.numLevelsPerColor * 0.75f) pos -= new Vector3((LevelGenerator.numLevelsPerColor + 1) * Spline.distPerPoint, 0f, 0f);
	            }
	            bool unlocked = true;
	            if (i > 0 && i % LevelGenerator.numLevelsPerColor == 0)
	            {
	                unlocked = Game.instance.level.GetNumGoldToUnlock(i / LevelGenerator.numLevelsPerColor) == 0;
	            }
	            levelUis[index].SetValues(i, Game.GetNumStars(i), pos, i <= unbeatenLastHole && unlocked);
	            index++;
	        }
	        int lockIndex = Mathf.RoundToInt(1f * holeNo / LevelGenerator.numLevelsPerColor);
	        if (lockIndex < 1) lockUi.gameObject.SetActive(false);
	        else
	        {
	            int numGoldLeft = Game.instance.level.GetNumGoldToUnlock(lockIndex);
	            if (numGoldLeft == 0) lockUi.gameObject.SetActive(false);
	            else
	            {
	                int numGoldTotal = Game.instance.level.GetNumGoldTotalToUnlock(lockIndex);
	                lockUi.gameObject.SetActive(true);
	                bool showValues = Game.GetNumStars(lockIndex * LevelGenerator.numLevelsPerColor - 1) > 0;
	                bool interactable = showValues && Game.gold > 0;
	                lockUi.Init(showValues, interactable, lockIndex, numGoldLeft, numGoldTotal);
	            }
	        }
	    }
	}

}
