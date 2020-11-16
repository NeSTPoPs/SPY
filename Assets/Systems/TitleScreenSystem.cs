﻿using UnityEngine;
using FYFY;
using System.Collections.Generic;
using UnityEngine.UI;

public class TitleScreenSystem : FSystem {
	private GameData gameData;
	private GameObject campagneMenu;
	private GameObject campagneButton;

	public TitleScreenSystem(){
		gameData = GameObject.Find("GameData").GetComponent<GameData>();
		gameData.levelList = new List<string>();
		campagneMenu = GameObject.Find("CampagneMenu");
		campagneButton = GameObject.Find("Campagne");
		GameObjectManager.dontDestroyOnLoadAndRebind(GameObject.Find("GameData"));

		//Level to Load
		gameData.levelList.Add("Level 1");
		gameData.levelList.Add("Level 2");

		GameObject cList = GameObject.Find("CampagneList");

		GameObject button = Object.Instantiate<GameObject>(Resources.Load ("Prefabs/Button") as GameObject, cList.transform);
		button.transform.GetChild(0).GetComponent<Text>().text = "Level 1";
		button.GetComponent<Button>().onClick.AddListener(delegate{launchLevel(0);});

		button = Object.Instantiate<GameObject>(Resources.Load ("Prefabs/Button") as GameObject, cList.transform);
		button.transform.GetChild(0).GetComponent<Text>().text = "Level 2";
		button.GetComponent<Button>().onClick.AddListener(delegate{launchLevel(1);});

		button = Object.Instantiate<GameObject>(Resources.Load ("Prefabs/Button") as GameObject, cList.transform);
		button.transform.GetChild(0).GetComponent<Text>().text = "Level 3";
		button.GetComponent<Button>().onClick.AddListener(delegate{launchLevel(2);});

		button = Object.Instantiate<GameObject>(Resources.Load ("Prefabs/Button") as GameObject, cList.transform);
		button.transform.GetChild(0).GetComponent<Text>().text = "Level 4";
		button.GetComponent<Button>().onClick.AddListener(delegate{launchLevel(3);});

		campagneMenu.SetActive(false);

	}
	// Use this to update member variables when system pause. 
	// Advice: avoid to update your families inside this function.
	protected override void onPause(int currentFrame) {
	}

	// Use this to update member variables when system resume.
	// Advice: avoid to update your families inside this function.
	protected override void onResume(int currentFrame){
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
	}

	public void showCampagneMenu(){
		campagneMenu.SetActive(true);
		campagneButton.SetActive(false);
	}

	public void launchLevel(int level){
		gameData.levelToLoad = level;
		GameObjectManager.loadScene("MainScene");
	}
}