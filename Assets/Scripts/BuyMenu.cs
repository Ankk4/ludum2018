using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyMenu : MonoBehaviour {

    public GameObject gm;
    public GameObject buyMenu;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GameManager.GameState gs = gm.GetComponent<GameManager>().CurrentGameState;
        Debug.Log(gs);
        if(gs == GameManager.GameState.action) {
            buyMenu.SetActive(true);
        } else {
            buyMenu.SetActive(false);
        }
    }
}
