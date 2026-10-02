using UnityEngine;
using System.Collections;

public class InGameText : MonoBehaviour {
    private TextMesh textMesh;
    private string text;
    
	void Awake () {
        textMesh = GetComponent<TextMesh>();
        text = textMesh.text;
    }
	
	void Update () {
        textMesh.text = (Game.instance.menu.gameObject.activeSelf ? "" : text);
	}
}
