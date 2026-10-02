using UnityEngine;
using System.Collections.Generic;

public class PhysNode {
    public PhysBox physBox;
    public PhysNode parent;
    public List<PhysNode> children;
    public Vector2 deltaPos;
    public int iBroadPhaseMin;
    public int jBroadPhaseMin;
    public int iBroadPhaseMax;
    public int jBroadPhaseMax;
}