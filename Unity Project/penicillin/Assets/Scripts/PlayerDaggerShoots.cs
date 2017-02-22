﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GLOBAL;

public class PlayerDaggerShoots : MonoBehaviour {
    public List<GameObject> projs = new List<GameObject>();
    public Transform spawn_point;
    PlayerMovement pm;
	PlayerAttack pa;
    int currAmmo;
    int switcheroonie;
    float timer = 0f;

    public void Awake() {
        pm = GetComponent<PlayerMovement>();
        pa = GetComponent<PlayerAttack>();
    }

    public void Start() {
        switcheroonie = 0;
        ReplenishAmmo();
    }

    public void Update() {
        if(timer < GAME.playerdagger_aspd) {
            timer += Time.deltaTime;
        }
    }    

    public void fire() {
        if (timer >= GAME.playerdagger_aspd && currAmmo > 0) {
            Debug.Log("check");
            currAmmo--;
            timer = 0f;
            var tempRot = projs[switcheroonie].transform.rotation.eulerAngles;
            var temp = Instantiate(projs[switcheroonie], transform.position, Quaternion.Euler(tempRot.x,tempRot.y + pm.getDir() > 0 ? 0f : 180f,tempRot.z)) as GameObject;
			switcheroonie = (switcheroonie + 1) % 2  + pa.GetWeapLevel(1) * 2;
            temp.GetComponent<MoveDir>().setDir(pm.getDir() > 0 ? Vector2.left * GAME.playerdagger_speed : Vector2.right * GAME.playerdagger_speed);
			temp.GetComponentInChildren<TakeDamage> ().SetDamage (GAME.WEAP_DAMAGE [1, pa.GetWeapLevel (1)]);
        }
    }


    public void ReplenishAmmo() {
        currAmmo = GAME.WEAP_DURABILITY[1, pa.GetWeapLevel(1)];
    }
}