using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateProvider : MonoBehaviour {

	public GameState templateState;
	private GameState activeState;
	void Awake () {
		activeState = Instantiate(templateState);
	}

	public GameState getGameState() {
		return activeState;
	}
}
