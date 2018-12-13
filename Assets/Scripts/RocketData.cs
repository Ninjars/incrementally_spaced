using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketData : ScriptableObject {
    [Tooltip("User facing name of the rocket")]
    public string rocketName;
    [Tooltip("Funds required to launch the rocket")]
    public int launchCost;
    [Tooltip("Rocket launch power")]
    public int power;
}
