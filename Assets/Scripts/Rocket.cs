using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
	internal void onMissionComplete() {
		Destroy(gameObject);
	}

	internal void explode() {
		Destroy(gameObject);
	}
}
