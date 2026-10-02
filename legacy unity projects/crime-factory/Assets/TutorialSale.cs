using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialSale : MonoBehaviour {
    public TextMeshPro textAccept;
    public TextMeshPro textDecline;
    
    void Start()
    {
        textAccept.text = Localization.Get("ACCEPT");
        textDecline.text = Localization.Get("DECLINE");
    }
}
