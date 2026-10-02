using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCreator : MonoBehaviour {
    [SerializeField] private GameObject objectToCreate;
    [HideInInspector] public GameObject _objectCreated;
    [SerializeField] private bool guardDontMove;
    public GameObject objectCreated {
        get {
            return GetObject();
        }
        set {
            _objectCreated = value;
        }
    }
    private bool hasCreated;
    
	void Awake() {
        GetObject();
    }

    GameObject GetObject() {
        if (!hasCreated) {
            hasCreated = true;
            GameObject go = Instantiate(objectToCreate, transform);
            if (_objectCreated == null) _objectCreated = go;
            _objectCreated.transform.localPosition = new Vector3(0f, 0f, 0f);
            _objectCreated.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            _objectCreated.transform.localScale = new Vector3(1f, 1f, 1f);
            if (_objectCreated.GetComponent<Guard>() != null) {
                if (guardDontMove) {
                    _objectCreated.GetComponent<Guard>().dontMove = true;
                }
            }
        }
        return _objectCreated;
    }
}
