using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameStateProvider gameStateProvider;
	public Camera cam;
	public float animationDuration = 2f;
	public List<CameraInfo> cameraPositions;
	private GameState.GameProgress targetProgress;
	private GameState gameState;

	void Start() {
		gameState = gameStateProvider.getGameState();
		setupAnimation(GameState.GameProgress.BEGINNING);
	}

	void Update() {
		if (gameState.getCurrentProgress() != targetProgress) {
			targetProgress = gameState.getCurrentProgress();
			setupAnimation(targetProgress);
		}
	}

	private void setupAnimation(GameState.GameProgress newProgress) {
		CameraInfo info = cameraPositions.FirstOrDefault(arg => arg.progress == newProgress);
		if (info == null) return;

		iTween.MoveTo(gameObject, iTween.Hash(
			"x", info.x,
			"time", animationDuration,
			"easetype", iTween.EaseType.easeInOutExpo
		));
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", cam.orthographicSize,
			"to", info.size,
			"time", animationDuration,
			"easetype", iTween.EaseType.easeInOutExpo,
			"onupdate", "updateCameraSize"
		));
	}

	public void updateCameraSize(float size) {
		cam.orthographicSize = size;
	}
}
