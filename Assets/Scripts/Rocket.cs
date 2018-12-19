using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

	public ExplosionMat explosion;
	private ExplosionMat explosionInstance;

	internal void onMissionComplete() {
		Destroy(gameObject);
	}

	internal void explode() {
		explosionInstance = Instantiate(explosion);
		explosionInstance._alpha = 0;
		explosionInstance.transform.position = transform.position;
		GetComponentInChildren<SpriteRenderer>().enabled = false;
		
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", 1,
			"to", 0,
			"time", 2,
			"easetype", iTween.EaseType.easeOutSine,
			"onupdate", "updateExplosionAlpha",
			"oncomplete", "explosionKill"
		));
	}

	public void explosionKill() {
		Destroy(gameObject);
		Destroy(explosionInstance.gameObject);
	}

	public void updateExplosionAlpha(float alpha) {
		explosionInstance._alpha = alpha;
	}
}
