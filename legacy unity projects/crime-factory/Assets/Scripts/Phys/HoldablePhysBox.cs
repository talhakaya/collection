using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldablePhysBox : PhysBox {
    public bool isAutoHoldLadder;
    [HideInInspector] public bool isAlsoPlatform;
    [HideInInspector] public bool isHoldableHanger;
}
