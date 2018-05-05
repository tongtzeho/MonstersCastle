using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskUI : MonoBehaviour {

	private UnityEngine.UI.Text ghostNum;
	private UnityEngine.UI.Text bruteKilled;
	private UnityEngine.UI.Text bruteMaxLevel;
	private UnityEngine.UI.Text gateHp;

	private string[] numberString = new string[32];
	private short prevGateHp = 8000;

	void Start () {
		for (int i = 0; i < numberString.Length; ++i) {
			numberString [i] = i.ToString ();
		}
		Transform taskText = transform.Find ("Task/TaskText");
		ghostNum = taskText.Find ("GhostNum").GetComponent<UnityEngine.UI.Text> ();
		bruteKilled = taskText.Find ("BruteKilled").GetComponent<UnityEngine.UI.Text> ();
		bruteMaxLevel = taskText.Find ("BruteMaxLevel").GetComponent<UnityEngine.UI.Text> ();
		gateHp = taskText.Find ("GateHp").GetComponent<UnityEngine.UI.Text> ();
		gateHp.text = prevGateHp.ToString ();
	}
	
	public void SetCurrentState(short ghostNum, short bruteKilled, short bruteMaxLevel, short gateHp) {
		this.ghostNum.text = numberString [ghostNum];
		this.bruteKilled.text = numberString [bruteKilled];
		this.bruteMaxLevel.text = numberString [bruteMaxLevel];
		if (gateHp != prevGateHp) {
			prevGateHp = gateHp;
			this.gateHp.text = gateHp.ToString ();
		}
	}
}
