using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour {

	[SerializeField]
	GameObject startCube;
	[SerializeField]
	Button left, right, levelButton;
	[SerializeField]
	Text levelText;

	int rotatedAmount = 0;

	// Use this for initialization
	void Start () {
		left.onClick.AddListener (leftMove);
		right.onClick.AddListener (rightMove);
		levelButton.onClick.AddListener (loadLevel);
	}

	void loadLevel(){
		Debug.Log(rotatedAmount + 1);
	}
	
	void leftMove(){
		rotatedAmount--;
		rotate ();
	}

	void rightMove(){
		rotatedAmount++;
		rotate ();
	}

	void rotate(){

		if (rotatedAmount < 0) {
			rotatedAmount = 3;
		}

		if (rotatedAmount > 3)
			rotatedAmount = 0;
		
		Vector3 rotation = Vector3.zero;

		switch (rotatedAmount) {
		case 0:
			rotation = new Vector3 (-10, 0, 0);
			break;
		case 1:
			rotation = new Vector3 (0, 90, -10);
			break;
		case 2:
			rotation = new Vector3 (10, 180, 0);
			break;
		case 3:
			rotation = new Vector3 (0, 270, 10);
			break;
		}

		levelText.DOFade (0, 0.2f);
		startCube.transform.DORotate (rotation, 1f);


		Sequence seq = DOTween.Sequence ();
		seq.PrependInterval (0.8f)
			.AppendCallback (fadeIn);
	}

	void fadeIn(){
		levelText.text = "Level " + (rotatedAmount + 1);
		levelText.DOFade (1, 0.2f);
	}
}
