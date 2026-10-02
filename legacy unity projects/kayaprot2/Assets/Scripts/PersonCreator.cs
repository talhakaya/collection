using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonCreator : MonoBehaviour {
    public enum Kind {
        None,
        Guard,
    }
    public Kind kind;
    public Sprite spriteHead;
    public Tool[] tools;
    [SerializeField] private string setTagTo;
    [SerializeField] private GameObject personPrefab;

    void Awake () {
        GameObject go = Instantiate(personPrefab, transform.parent);
        if (transform.parent != null && transform.parent.GetComponent<PrefabCreator>() != null) {
            transform.parent.GetComponent<PrefabCreator>().objectCreated = go;
        }
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
        go.transform.localScale = new Vector3(1f, 1f, 1f);
        if (kind == Kind.None) {

        }
        else if (kind == Kind.Guard) {
            go.AddComponent<Guard>();
            go.GetComponent<Person>().isRight = false;
        }
        else {
            throw new System.NotImplementedException();
        }
        Person person = go.GetComponent<Person>();
        if (spriteHead != null) {
            person.tilts[0].GetComponent<SpriteRenderer>().sprite = spriteHead;
        }
        
        person.tools = new GameObject[tools.Length];
        for (int i = 0, len = tools.Length; i < len; i++) {
            person.tools[i] = Instantiate(Game.instance.allTools[(int)tools[i]], person.tilts[1].transform);
            person.tools[i].transform.localPosition = person.defGadgetPos;
            person.tools[i].transform.localEulerAngles = new Vector3(0f, 0f, person.defGadgetAngle);
            person.tools[i].transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (setTagTo != "") person.tag = setTagTo;
        Destroy(gameObject);
    }
}
