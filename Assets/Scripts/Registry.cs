using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Registry : ScriptableObject {
    public List<RocketData> rockets;
    public List<PayloadData> payloads;
    public List<DestinationData> destinations;
    public List<FlightPlan> flightPlans;
}