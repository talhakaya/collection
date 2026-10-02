using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScalablePhysBox : PhysBox {
    public int scaleX {
        get { return _scaleX; }
        set {
            if (_scaleX != value) {
                _scaleX = value;
                rect.x = col + (_scaleX - 1) * 0.5f;
                rect.w = Mathf.Max(0f, widthInit + (_scaleX - 1));
                transform.localScale = new Vector3(visualScale * _scaleX, visualScale * _scaleY, 1f);
                UpdateProperties();
                forceOutlineUpdate = true;
                Update();
                if (this is ClimbablePhysBox) (this as ClimbablePhysBox).SetClimbable();
            }
        }
    }
    public int scaleY {
        get { return _scaleY; }
        set {
            if (_scaleY != value) {
                _scaleY = value;
                rect.y = row - (_scaleY - 1) * 0.5f;
                rect.h = Mathf.Max(0f, heightInit + (_scaleY - 1));
                transform.localScale = new Vector3(visualScale * _scaleX, visualScale * _scaleY, 1f);
                UpdateProperties();
                forceOutlineUpdate = true;
                Update();
                if (this is ClimbablePhysBox) (this as ClimbablePhysBox).SetClimbable();
            }
        }
    }
    private int _scaleX = 1;
    private int _scaleY = 1;

    private void Start() {
        Init();
        if (parent == null) {
            rect.x = col + (scaleX - 1) * 0.5f;
            rect.y = row - (scaleY - 1) * 0.5f;
        }
        else {
            rect.x = transform.position.x;
            rect.y = transform.position.y;
        }
    }

    private void Update() {
        transform.position = new Vector3(rect.x + visualPosOffset.x, rect.y + visualPosOffset.y, transform.position.z);
        if (updateScale) {
            transform.localScale = new Vector3(visualScale * _scaleX, visualScale * _scaleY, 1f);
        }
        else {
            updateScale = (visualScale != 1f);
        }
    }

    public override void SetProperty(string name, int val, int iDelta = 0, int jDelta = 0) {
        base.SetProperty(name, val, iDelta, jDelta);
        if (name == PROP_SCALE_X) {
            scaleX = val;
        }
        else if (name == PROP_SCALE_Y) {
            scaleY = val;
        }
    }

    protected override List<Property> GetPropertyTypes() {
        List<Property> res = base.GetPropertyTypes();
        res.Add(Property.GetCopy(PROP_SCALE_X));
        res.Add(Property.GetCopy(PROP_SCALE_Y));
        return res;
    }
}
