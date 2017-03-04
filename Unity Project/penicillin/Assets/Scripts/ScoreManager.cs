﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GLOBAL;

public class ScoreManager : MonoBehaviour {

	public int researchPoints;
    public int totalResearchPoints;
    public Text text;

    void Start () {
		researchPoints = 0; // PlayerPrefs.GetInt (GAME.PLAYER_PREFS_RP, 0);
        totalResearchPoints = 0;
		//AddPoints (400);
	}

    void Update() {
		text.text = researchPoints.ToString("D8");
        //AddPoints(1);
    }


	public void SaveData(){
		PlayerPrefs.SetInt (GAME.PLAYER_PREFS_RP, researchPoints);
	}


    public void AddPoints(int val) {
        researchPoints += val;
        totalResearchPoints += val;
        text.text = researchPoints.ToString("D8");
		Debug.Log (researchPoints);
    }

    public void DeductPoints(int val) {
        researchPoints -= val;
        text.text = researchPoints.ToString("D8");
    }
}
