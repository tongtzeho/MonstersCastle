using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour {

	private Hashtable colliderTable = new Hashtable();
	private HashSet<Collider> headSet = new HashSet<Collider> ();

	// assigned in editor
	public CriticalHit criticalHit;
	public FadeImage hitImage;
	public FadeImage criticalHitImage;

	void Start () {
		colliderTable.Clear ();
		headSet.Clear ();
		GameObject bruteObject = GameObject.Find ("Brute");
		MonsterHP brute = bruteObject.GetComponent<Brute> ().monster;
		Collider[] bruteCollider = bruteObject.GetComponentsInChildren<Collider> ();
		for (int i = 0; i < bruteCollider.Length; ++i) {
			colliderTable.Add (bruteCollider [i], brute);
		}
		Collider bruteHead = bruteObject.GetComponentInChildren<SphereCollider> ();
		headSet.Add (bruteHead);
		Transform ghostPool = GameObject.Find ("GhostPool").transform;
		int ghostId = 0;
		while (true) {
			string ghostName = "Ghost" + ghostId.ToString ();
			Transform ghost = ghostPool.Find (ghostName);
			if (ghost == null) {
				break;
			} else {
				++ghostId;
				MonsterHP monster = ghost.gameObject.GetComponent<Ghost> ().monster;
				Collider[] ghostCollider = ghost.gameObject.GetComponents<Collider> ();
				for (int i = 0; i < ghostCollider.Length; ++i) {
					colliderTable.Add (ghostCollider [i], monster);
				}
				Collider ghostHead = ghost.gameObject.GetComponent<SphereCollider> ();
				headSet.Add (ghostHead);
			}
		}
	}

	public void HitCollider(short damage, short criticalDamage, Collider collider) {
		if (colliderTable.Contains (collider)) {
			if (headSet.Contains(collider)) {
				((MonsterHP)colliderTable [collider]).Hit (criticalDamage);
				criticalHit.Play ();
				criticalHitImage.Activate ();
			} else {
				((MonsterHP)colliderTable [collider]).Hit (damage);
				hitImage.Activate ();
			}
		}
	}
}
