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

	private bool animating = false;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private float startSize;
	private float endSize;
	private float startTime;

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

	private void updatePosition() {
		float deltaT = Time.time - startTime;
		float fraction = deltaT / animationDuration;
		transform.position = Vector3.Lerp(startPosition, endPosition, fraction);
		cam.orthographicSize = Mathf.Lerp(startSize, endSize, fraction);
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
