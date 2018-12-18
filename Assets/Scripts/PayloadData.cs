using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadData : ScriptableObject {
    [Tooltip("User facing name of the payload")]
    public string payloadName;
    [Tooltip("Payload weight, which affects the power of rocket required for the mission")]
    public int weight;
    [Tooltip("Funds received for launching mission")]
    public int launchValue;
    [Tooltip("Bonus for successful mission")]
    public int successBonus;
    [Tooltip("Destinations this payload can be delivered to")]
    public List<DestinationData> validDestinations;
    [Tooltip("Add or increment these values in the game state")]
    public List<string> completionFlags;
    [Tooltip("Game state flag conditions required to use this payload")]
    public List<ProgressPrecondition> preconditions;
    public Sprite icon;
}
