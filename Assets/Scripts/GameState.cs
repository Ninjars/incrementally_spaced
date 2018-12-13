using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : ScriptableObject {
    public string playerName = "Sergei";
    public float funds = 0;
    public Dictionary<string, int> progressFlags = new Dictionary<string, int>();
    public List<MissionState> mission = new List<MissionState>();
}
