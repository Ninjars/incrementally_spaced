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

		if (Time.time > startTime + animationDuration) {
			animating = false;
		}
		if (animating) {
			updatePosition();
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

		animating = true;

		startPosition = transform.position;
		endPosition = new Vector3(info.x, transform.position.y, transform.position.z);

		startSize = cam.orthographicSize;
		endSize = info.size;

		startTime = Time.time;
	}
}
