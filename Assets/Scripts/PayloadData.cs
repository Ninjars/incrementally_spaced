using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadData : ScriptableObject {
    [Tooltip("User facing name of the payload")]
    public string payloadName;
    [Tooltip("Payload weight, which affects the power of rocket required for the mission")]
    public int weight;
    [Tooltip("Cost to launch this payload")]
    public int cost;
    [Tooltip("Increase in funds on successful mission")]
    public int reward;
    [Tooltip("Add or increment these values in the game state")]
    public List<string> completionFlags;
    [Tooltip("Game state flag conditions required to use this payload")]
    public List<StatePrecondition> preconditions;
}
