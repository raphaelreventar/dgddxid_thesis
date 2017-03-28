﻿using UnityEngine;
using UnityEngine.UI;
using GLOBAL;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class StomachLevel_Global : MonoBehaviour {
    public int kills;
    public Text screenTimer, dialogueTextArea;
    public Slider enemyCountSlider;
    public static float globalTime;
    public Transform[] loadouts;
    public Transform[] health;
    public Transform player;
    public GameObject
        pill, // rlab pickup
        loadoutIndicator, // rlab red circle thing
        Shigellang_Dormant, // pink egg
        healthPickup, // health pickup
        healthPickupIndicator, // health pickup big-ass yellow circle thing
        dialogues, // dialogues manager
        c_controls, // controls canvas
        c_hud, // hud canvas
        bossSpawnDefense, // spawn controllers for boss fight
        angry, // angry penny face
        normal, // normal, smiling penny face
        hparr, // hp arrow
        hpicon, // hp icon
        rlarr, // rlab arrow
        rlicon; // rlab icon
    public GameObject[] spawnWaveLVLS;
    public int acidCycleCounter;
    bool bossFight;
    bool bossDormant;

    private float waveTimeInSeconds, waveTime, levelTime, hplifetime, plifetime, pillTimer, dialogueTimer, dialogueDelay;
    private bool waspill, freeze, w1, w3, dw1, bosserino;
    private Color defaultColor;
    private Vector3 defaultScale;
    private int waveCounter, cur_msg;
    private string[] msgs;
    private bool face;

    void Start() {
        enemyCountSlider.value = 0;
        waveCounter = 1;
        spawnWaveLVLS[waveCounter - 1].SetActive(true);
        for(int i = waveCounter + 1; i <= GAME.LevelStomach_WAVECOUNTER; i++) {
            spawnWaveLVLS[i - 1].SetActive(false);
        }
        enemyCountSlider.maxValue = GAME.NUM_BACTERIA_WAVE[waveCounter - 1];

        pill.SetActive(false);
        loadoutIndicator.SetActive(false);
        waspill = false; /* Did the pill spawn? */
        defaultScale = loadoutIndicator.transform.localScale; /* Editing the scale through script instead of animator */
        screenTimer.text = "";
        bossDormant = true;
        acidCycleCounter = 0; /* Using this counter to determine when to spawn health pickups */
        waveTime = 0; /* Time elapsed for each wave (resets every next wave) */
        hplifetime = 0; /* Lifetime of health pickup */
        plifetime = 0; /* Lifetime of pill */
        kills = 0;
        freeze = false;
        w1 = false;
        w3 = false;
        dw1 = false;
        face = true;
        bosserino = false;
        dialogueDelay = 2;

        /* For Dialogue */
        cur_msg = 0;

        ///// Debug
        //waveCounter = 3;
        //kills = GAME.NUM_BACTERIA_WAVE[2] - 1;
        kills = 19;

        msgs = new string[] {
            //introductory message
            "",
            "Our host's stomach has been infected by Shigella bacteria. Get rid of them before they pose a threat to our host's health!",
            "Be careful of the green acid! If you fall into the pit, you'll get damaged!",
            "To progress through the level, eliminate enough enemies as quick as you can. Keep an eye on your progress bar, as well as health pickups along the way.",
            "I know you can do this. Good luck!",
            "",

            //after finishing wave 1
            "Great work, Private!",
            "You managed to clear out the first wave of bacteria that's harming our host.",
            "The research lab is going to appear soon, as it's time for your host to take another dose of antibiotics. Don't miss it! Make sure you upgrade your weapons wisely!",
            "",

            //after finishing wave 3 ...... 9
            "Oh no... Is that what I think it is?",
            "It's Shigellang: The Indifferent!",
            "You have to help me stop him before he takes over our host! Quick, break its outer shell and defeat it once and for all!",
            "",

            //if success, cur_msg = 13
            "You did it! You stopped the Shigella invasion by properly using antibiotics!",
            "Our fight does not end here. More and more bacteria evolve as time passes, and many people still need to be educated on antibiotic misuse and abuse.",
            "I'm sure you'll do a great job of informing everyone! I believe in you, Private!",
            "",

            //if loss, cur_msg = 17
            "",
            "Oh no! Shigella have taken over our host; we've lost the battle!",
            "This is a minor victory for Shigella today, but in the grand scheme of things, humans are at a greater risk.",
            "More bacteria, not just Shigella, will continuously mutate and cause harm to humans.",
            "It's not yet too late for us to change our ways, Private!",
            "Try again; I'm sure you'll do a great job!",
            "" //automatically go to the main menu after this message
        };

        freeze = true;
        Dialogue();
    }

    public void NextMessage() {
        try {
            if (msgs[cur_msg] == "" || dialogueDelay > 1) {
                dialogueTextArea.text = msgs[++cur_msg];
                dialogueDelay = 0;
            }
            else return;
        }
        catch (Exception e) {
            throw (e);
        }
        //set Penny's face
        if (cur_msg == 1 || cur_msg == 2 || cur_msg == 10 || cur_msg == 11 || cur_msg == 12 || cur_msg == 19 || cur_msg == 20 || cur_msg == 21 || cur_msg == 22) {
            face = false;
        }
        else{
            face = true;
        }

        angry.SetActive(face ? false : true);
        normal.SetActive(face ? true : false);

        //events
        if (cur_msg == 5) {// first dialogue is over
            Dialogue();
            freeze = false;
        }
        if (cur_msg == 9) { // first wave is over, unfreeze time scale, spawn trigger
            Dialogue();
            freeze = false;

            {
                spawnWaveLVLS[waveCounter - 1].SetActive(false);
                pill.transform.position = loadouts[(int)UnityEngine.Random.Range(0, loadouts.Length)].transform.position; /* Randomize position */
                pill.SetActive(true); /* activate */
                screenTimer.color = Color.red; /* show lifetime timer */
                loadoutIndicator.SetActive(true);/* activate indicator */
                loadoutIndicator.transform.localScale = defaultScale;
                loadoutIndicator.transform.position = pill.transform.position; /* set indicator position to where pill is */
                plifetime = 0; /* set pill lifetime to 0 */
                waspill = true; /* pill spawned */
            }
        }

        if(cur_msg == 13) { // boss warning done
            Dialogue();
            waveCounter = 3;
            NextWaveStart();
            Time.timeScale = 0;
        }

        if(cur_msg == 24 || cur_msg == 17) {// win or lose, go to main menu
            SceneManager.LoadScene("MainMenu");
        }
    }

    void Dialogue() {
        // Set Penny's animation to idle
        dialogues.SetActive(dialogues.activeInHierarchy ? false : true);
        if (dialogues.activeInHierarchy) NextMessage();
        c_hud.SetActive(dialogues.activeInHierarchy ? false : true);
        c_controls.SetActive(dialogues.activeInHierarchy ? false : true);
        for(int i = 0; i < waveCounter; i++) {
            spawnWaveLVLS[i].SetActive(spawnWaveLVLS[i].activeInHierarchy ? false : true);
        }
        gameObject.GetComponent<ResitanceCalculator>().enabled = gameObject.GetComponent<ResitanceCalculator>().isActiveAndEnabled ? false : true;
        if (freeze) Time.timeScale = Time.timeScale == 1 ? 0 : 1;
    }

    void OnEnable() {
        Time.timeScale = 1;
        EnemyHealth.Dead += AddEnemyCount;
        bossFight = false;
    }

    void OnDisable() {
        EnemyHealth.Dead -= AddEnemyCount;
    }

    public void PennyDied() {
        cur_msg = 18;
        Dialogue();
    }

    public void PennyWon() {
        cur_msg = 13;
        Dialogue();
    }

    void Update() {
        /* Update Timers */
        globalTime += Time.deltaTime;
        if (dialogues.activeInHierarchy) dialogueDelay += Time.unscaledDeltaTime;

        /* Dialogues */
        /// Wave 1
        if(waveCounter == 1) {
            if (kills >= GAME.NUM_BACTERIA_WAVE[waveCounter - 1] && !dw1) {
                w1 = true;
                dw1 = true;
            }
        }
        
        // delay the dialogue for a second
        if (w1 && waveCounter == 1) {
            dialogueTimer += Time.deltaTime;
            if(dialogueTimer > 1) {
                freeze = true;
                Dialogue();
                w1 = false;
            }
        }

        /* Health Pickup Management */
        // Once the acid rises and subsides acidCyclesPerHealthPickup times, spawn the health pickup if it hasn't spawned yet
        if (acidCycleCounter == GAME.acidCyclesPerHealthPickup && !healthPickup.activeInHierarchy) {
            /* Health Pickup */
            healthPickup.SetActive(true);
            healthPickup.transform.position = health[(int)UnityEngine.Random.Range(0, health.Length)].transform.position;
            /* Indicator */
            healthPickupIndicator.SetActive(true);
            healthPickupIndicator.transform.localScale = defaultScale;
            healthPickupIndicator.transform.position = healthPickup.transform.position;
            hplifetime = 0; /* reset lifetime tracker */
        }
        // Health pickup indicator resizing and shite
        if (healthPickup.activeInHierarchy) {
            hplifetime += Time.deltaTime; /* update the hp lifetime while it's active */
            if (healthPickupIndicator.transform.localScale.x >= 0) healthPickupIndicator.transform.localScale -= new Vector3(GAME.loadoutIndicatorDecaySpeed, GAME.loadoutIndicatorDecaySpeed, 0);
            else healthPickupIndicator.SetActive(false);
            // If the player fails to pick up the health thingy within its lifteime delete it from the scene
            if (hplifetime > GAME.loadoutLifetime) {
                healthPickup.SetActive(false); /* despawn pickup */
                acidCycleCounter = 0; /* reset acid cycle counter for the next pickup */
            }

            /* Tracker */
            Vector3 hpup = healthPickup.transform.position; // position of health pickup
            Vector3 plpos = player.position; // penny's position
            Vector3 dirVec = new Vector3(hpup.x - plpos.x, hpup.y - plpos.y); // direction vector from health pickup to player
            float length = dirVec.magnitude;
            Vector3 norm = dirVec / length; // normal vector from penny to health pickup

            // check if far enough
            if(length > 1.3f) { // 1.3f is just the value from the tutorial level
                hpicon.transform.position = new Vector3(plpos.x + norm.x * .5f, plpos.y + norm.y * .5f); // place the health pick up icon
                hparr.transform.position = new Vector3(plpos.x + norm.x * .8f, plpos.y + norm.y * .8f); // place the arrow
                if (!hpicon.activeInHierarchy) {
                    hpicon.SetActive(true);
                    hparr.SetActive(true);
                }
            }
            else {
                hpicon.SetActive(false);
                hparr.SetActive(false);
            }
            // set the rotation of the arrow indicator
            float rad = Mathf.Atan2(dirVec.y, dirVec.x);
            hparr.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * rad);
        }
        else {
            hparr.SetActive(false);
            hpicon.SetActive(false);
        }

        ////////////////////////////////////////////////////////
        /* Research Lab Pill Management */
        if (!bossFight) {
            if (pill.activeInHierarchy) {
                plifetime += Time.deltaTime; /* update the pill lifetime while it's active */

                int timeRemaining = (int)GAME.PillPickupTimeLimit - (int)plifetime;                
                if (loadoutIndicator.transform.localScale.x >= 0) loadoutIndicator.transform.localScale -= new Vector3(GAME.loadoutIndicatorDecaySpeed, GAME.loadoutIndicatorDecaySpeed, 0);
                else {
                    loadoutIndicator.SetActive(false);
                }
                string min = Mathf.Floor( timeRemaining/ 60).ToString("00");
                string sec = ( timeRemaining % 60).ToString("00");
                screenTimer.text = min + ":" + sec;

                /* Tracker */
                Vector3 rlloc = pill.transform.position; // position of health pickup
                Vector3 plpos = player.position; // penny's position
                Vector3 dirVec = new Vector3(rlloc.x - plpos.x, rlloc.y - plpos.y); // direction vector from health pickup to player
                float length = dirVec.magnitude;
                Vector3 norm = dirVec / length; // normal vector from penny to health pickup

                // check if far enough
                if (length > 1.3f) { // 1.3f is just the value from the tutorial level
                    rlicon.transform.position = new Vector3(plpos.x + norm.x * .5f, plpos.y + norm.y * .5f); // place the health pick up icon
                    rlarr.transform.position = new Vector3(plpos.x + norm.x * .8f, plpos.y + norm.y * .8f); // place the arrow
                    if (!rlicon.activeInHierarchy) {
                        rlicon.SetActive(true);
                        rlarr.SetActive(true);
                    }
                }
                else {
                    rlicon.SetActive(false);
                    rlarr.SetActive(false);
                }
                // set the rotation of the arrow indicator
                float rad = Mathf.Atan2(dirVec.y, dirVec.x);
                rlarr.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * rad);
            }
            else {
                rlarr.SetActive(false);
                rlicon.SetActive(false);
            }

            if (plifetime > GAME.PillPickupTimeLimit || (!pill.activeInHierarchy && waspill)) {
                screenTimer.text = "";
                plifetime = 0; 
                screenTimer.color = defaultColor;
                pill.SetActive(false);
                waspill = false;
				NextWaveStart ();
            }
        }
        else {
            /* Spawn the health pick-up here cause it stops spawning after wave 3 */
        }
    }

    public void Reset() {
        Time.timeScale = 1;
        waveTimeInSeconds = 60 * GAME.waveTimeInMins;
        globalTime = 0;
        waveTime = 0;
        waveCounter = 1;
        kills = 0;
        pill.SetActive(false);
        enemyCountSlider.value = 0;
        enemyCountSlider.maxValue = GAME.NUM_BACTERIA_WAVE[0];
        screenTimer.color = defaultColor;
    }

    public void BossDialogue() {
        if(waveCounter > 3) {
            cur_msg = 8;
            Time.timeScale = 0;
            Dialogue();
        }
    }

    // int timeRemaining = (int)waveTimeInSeconds - (int)waveTime;

    public void AddEnemyCount(int points) {
        kills += points;
        if (kills >= enemyCountSlider.maxValue && !bossFight) {
            enemyCountSlider.value = enemyCountSlider.maxValue;
            if (!pill.activeInHierarchy && waveCounter != 1) {
                spawnWaveLVLS[waveCounter - 1].SetActive(false);
                pill.transform.position = loadouts[(int)UnityEngine.Random.Range(0, loadouts.Length)].transform.position; /* Randomize position */
                pill.SetActive(true); /* activate */
                screenTimer.color = Color.red; /* show lifetime timer */
                loadoutIndicator.SetActive(true);/* activate indicator */
                loadoutIndicator.transform.localScale = defaultScale;
                loadoutIndicator.transform.position = pill.transform.position; /* set indicator position to where pill is */
                plifetime = 0; /* set pill lifetime to 0 */
                waspill = true; /* pill spawned */
        	}
        }
        else {
            enemyCountSlider.value = kills;
        }
    }

    public void NextWaveStart() {
        waveCounter++;
		if (waveCounter > 3) {
			bossFight = true;
			//enable boss health bar, disable level timer
			screenTimer.gameObject.SetActive (false);
			if (enemyCountSlider.isActiveAndEnabled)
				enemyCountSlider.gameObject.SetActive (false);
			//spawn boss if not already there
			if (!Shigellang_Dormant.activeInHierarchy && bossDormant) {
				Shigellang_Dormant.SetActive (true);
				bossDormant = false;
			}
            bossSpawnDefense.SetActive(true);
		} 
		else {
			kills = 0;
			enemyCountSlider.value = 0;
			enemyCountSlider.maxValue = GAME.NUM_BACTERIA_WAVE [waveCounter - 1];
            foreach(var wave in spawnWaveLVLS) {
                wave.SetActive(false);
            }
            spawnWaveLVLS[waveCounter - 1].SetActive(true);
		}
    }

    
}
