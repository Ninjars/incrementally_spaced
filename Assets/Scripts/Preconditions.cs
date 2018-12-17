using UnityEngine;

[System.Serializable]
public class ProgressPrecondition {
    public int requiredProgress;

    public bool isMet(GameState gameState) {
        return gameState.getCurrentProgressValue() >= requiredProgress;
    }
}
