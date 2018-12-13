using UnityEngine;

public class StatePrecondition : ScriptableObject {
    public string requiredFlag;
    public int minCount;

    public bool isMet(GameState gameState) {
        int flagCount = -1;
        gameState.progressFlags.TryGetValue(requiredFlag, out flagCount);
        return flagCount >= minCount;
    }
}
