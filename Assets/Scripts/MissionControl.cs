using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MissionControl : MonoBehaviour {
    public Registry registry;
    public GameState gameState;
    public MissionPlanner missionPlanner;
    public Button startPlanningButton;

    void Start() {
        startPlanningButton.onClick.AddListener(() => startPlanningMission());
    }

    public void startPlanningMission() {
        missionPlanner.gameObject.SetActive(true);
        startPlanningButton.gameObject.SetActive(false);
    }

    public void cancelPlanningMission() {
        missionPlanner.gameObject.SetActive(false);
        startPlanningButton.gameObject.SetActive(true);
    }

    public void launchMission(MissionData mission) {
        missionPlanner.gameObject.SetActive(false);
        startPlanningButton.gameObject.SetActive(true);
        gameState.missions.Add(ActiveMission.create(mission, Time.realtimeSinceStartup));
        gameState.funds -= mission.getCost();
    }

    public MissionData buildMission(RocketData rocket, PayloadData payload, DestinationData destination) {
        var mission = new MissionData();
        mission.payloadData = payload;
        mission.rocketData = rocket;
        mission.destinationData = destination;
        return mission;
    }
}
