using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class TutorialManager : MonoBehaviour
	{
	    public static TutorialManager instance;
	    [SerializeField] private GameObject prefabTutorialUp;
	    [SerializeField] private GameObject prefabTutorialSale;
	    private GameObject tutorialUp;
	    private GameObject tutorialSale;
	    private float tutorialUpTimer;
	    private Salesperson salesperson;

	    void Awake()
	    {
	        instance = this;
	    }

	    void Update()
	    {
	        if (tutorialUpTimer < 0.1f) {
	            tutorialUpTimer += Time.deltaTime;
	            if (tutorialUpTimer >= 0.1f) {
	                if (tutorialUp != null) {
	                    tutorialUp.SetActive(false);
	                }
	            }
	        }

	        if (tutorialSale != null && tutorialSale.activeSelf) {
	            if (salesperson == null || salesperson.state != Salesperson.State.Selling) {
	                tutorialSale.SetActive(false);
	            }
	        }
	    }

	    public void ShowUp(Vector3 position) {
	        if (tutorialUp != null) {
	            tutorialUp.SetActive(true);
	        }
	        else {
	            tutorialUp = Instantiate(prefabTutorialUp, null);
	        }
	        tutorialUp.transform.position = position;
	        tutorialUpTimer = 0f;
	    }

	    public void ShowSaleUI(Salesperson salesperson) {
	        if (tutorialSale != null) {
	            tutorialSale.SetActive(true);
	        }
	        else {
	            tutorialSale = Instantiate(prefabTutorialSale, null);
	        }
	        this.salesperson = salesperson;
	        tutorialSale.transform.position = salesperson.transform.position + new Vector3(0f, 1.5f, 0f);
	    }
	}
}
