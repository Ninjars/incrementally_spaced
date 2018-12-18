using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
	internal void missionComplete() {
		Destroy(gameObject);
	}
}
