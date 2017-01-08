﻿using UnityEngine;
using System.Collections;
using PlayGroup;

public class DoorController: Photon.PunBehaviour {
    private Animator animator;
    private BoxCollider2D boxColl;
    private bool isOpened = false;

    public float maxTimeOpen = 5;
    private float timeOpen = 0;
    private int numOccupiers = 0;
    
    void Start() {
        animator = gameObject.GetComponent<Animator>();
        boxColl = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update() {
        waitUntilClose();
    }

    public void BoxCollToggleOn() {
        boxColl.enabled = true;
    }

    public void BoxCollToggleOff() {
        boxColl.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D coll) {
		if(!isOpened && coll.gameObject.layer == 8 || isOpened && coll.gameObject.layer == 12) {
            Open();
        }
        numOccupiers++;
    }

    void OnTriggerExit2D(Collider2D coll) {
        numOccupiers--;
    }

    private void waitUntilClose() {
        if(isOpened) { //removed numOccupies condition for time being
            timeOpen += Time.deltaTime;

            if(timeOpen >= maxTimeOpen) {
                Close();
            }
        }else {
            timeOpen = 0;
        }
    }
    
    public void PlayOpenSound() {
        SoundManager.Play("AirlockOpen");
    }

    public void PlayCloseSound() {
        SoundManager.Play("AirlockClose");
    }

    public void PlayCloseSFXshort() {
        SoundManager.Play("AirlockClose", time:0.6f);
    }

    void OnMouseDown() {
        if(PlayerManager.PlayerScript != null) {
            if(PlayerManager.PlayerScript.DistanceTo(transform.position) <= 2) {
                if(isOpened) {
                    photonView.RPC("Close", PhotonTargets.All);
                } else {
                    photonView.RPC("Open", PhotonTargets.All);
                }
            }
        }
    }

    [PunRPC]
    public void Open() {
        isOpened = true;
        animator.SetBool("open", true);
    }

    [PunRPC]
    public void Close() {
        isOpened = false;
        animator.SetBool("open", false);
    }
}
