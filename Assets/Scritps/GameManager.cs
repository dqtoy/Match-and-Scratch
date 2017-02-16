﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameType {
	Free,
	MatchThree
}

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public GameType gameType;

	public Rotator rotator;
	public Spawner spawner;

	public Animator animator;

	public int score = 0;

	public int currentLevel = 0;

	public float distanceOfPins = 8f;

	public Text scoreLabel;
	public Text maxScoreLabel;

	public bool gameHasEnded = false;

	/*** gameType = match-three ***/
	List<List<GameObject>> colorGroups;
	/*** 		*	*	* 		***/

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}

		int highscore = PlayerPrefs.GetInt("MaxScore");
		maxScoreLabel.text =  highscore == 0 ? "" : "Max: " + highscore.ToString();
		spawner.SetSpawnerType(gameType);
		switch (gameType) {
			case GameType.MatchThree:
				if (colorGroups == null) {
					colorGroups = new List<List<GameObject>>();
				}
				colorGroups.Clear();
			break;
		}
		spawner.SpawnNeedle();
	}

	public void EndGame() {
		if (gameHasEnded)
			return;

		rotator.enabled = false;
		spawner.enabled = false;

		if (score > PlayerPrefs.GetInt("MaxScore")) {
			PlayerPrefs.SetInt("MaxScore", score);
		}

		gameHasEnded = true;
		animator.SetTrigger ("EndGame");
	}

	void Update() {
		scoreLabel.text = score.ToString();
	}

	public void RestartLevel () {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex  );
		score = 0;
		gameHasEnded = false;
		currentLevel = 0;
	}

	public void EvaluatePinnedNeedle(GameObject needleToPin, GameObject needleDestiny = null) {
		if (needleDestiny == null) {
			CreateColorGroup(needleToPin);
			//Debug.Log ("<color=yellow> no hay needleDestiny</color>");
		}
		else if ( needleToPin.name.Split('-')[1] != needleDestiny.name.Split('-')[1] ){
			CreateColorGroup(needleToPin);
			//Debug.Log ("<color=yellow>needleDestiny != color</color>");
		}
		else {
			// Buscamos el grupo en el que ya esté el ultimo objeto que hemos tocado
			int colorGroupId = -1;
			for (int i = 0; i < colorGroups.Count && colorGroupId == -1; i++){
				if (colorGroups[i].Find(c => c.name == needleDestiny.name) ) {
					colorGroupId = i;
				}
			}

			// Si hemos localizado un grupo en el que ya existe el último tocado, metemos el nuevo es ese grupo.
			if (colorGroupId >= 0) {
				colorGroups[colorGroupId].Add(needleToPin);
			}
			else{// Si no hemos encontrado el ultimo objeto colisionado en ningún grupo... creamos unos nuevo con el objeto a pinear
				CreateColorGroup(needleToPin);
				Debug.Log ("<color=red>Error WTF(1): Esto no debería suceder</color>");
			}
		}

		EvaluateColorGroups();
	}

	void CreateColorGroup(GameObject go) {
		List<GameObject> goList = new List<GameObject>();
		goList.Add(go);
		colorGroups.Add(goList);
	}

	void EvaluateColorGroups() {
		List<int> groupsToDestroy = new List<int>();

		// Si encontramos un grupo de mas de dos miembreo del mismo color...
		for(int i = 0; i < colorGroups.Count; i++) {
			if (colorGroups[i].Count > 2) {
				// ...los objetos que contiene...
				StartCoroutine(DestroyElementsFromGroup(colorGroups[i]));
				// ...eliminamos el grupo.
				groupsToDestroy.Add(i);
			}			
		}

		foreach(int i in groupsToDestroy) {
			colorGroups.RemoveAt(i);
			score++;
		}

		//PrintColorGroupsLog();
	}

	IEnumerator DestroyElementsFromGroup(List<GameObject> listGo) {
		for( int i = 0; i < listGo.Count; i++) {
			listGo[i].GetComponent<ColorNeedle>().Autodestroy();
			yield return new WaitForSeconds(ColorNeedle.TIME_TO_DESTROY/listGo.Count);
		}
	}




	void PrintColorGroupsLog() {
		string log = "";
		for(int i = 0; i < colorGroups.Count; i++) {
			for( int j = 0; j < colorGroups[i].Count; j++) {
				log += (colorGroups[i][j].name + " ");
			}
			log += "\n";
		}
		Debug.Log (log);
	}
}
