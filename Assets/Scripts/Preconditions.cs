using System;
using UnityEngine;

[System.Serializable]
public class PayloadPrecondition {
    public PayloadData payload;
    public int count;
    public Comparison comparison;
    public int minProgressValue = 0;

    public bool isMet(GameState gameState) {
        try {
            return gameState.getCurrentProgressValue() >= minProgressValue && payloadConditionIsMet(gameState);
        } catch(NullReferenceException e) {
            return gameState.getCurrentProgressValue() >= minProgressValue;
        }
    }

    private bool payloadConditionIsMet(GameState gameState) {
        int payloadCount = gameState.getDeliveredPayloadCount(payload);
        switch (comparison) {
            case Comparison.AT_LEAST: 
                return payloadCount >= count;
            case Comparison.EXACTLY:
                return payloadCount == count;
            default:
                throw new ArgumentException("unhandled comaprison " + comparison);
        }
    }

    public enum Comparison {
        AT_LEAST,
        EXACTLY
    }
}
