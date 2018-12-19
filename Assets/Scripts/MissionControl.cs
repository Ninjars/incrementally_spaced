using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class MissionControl : MonoBehaviour {
    public Registry registry;
    public GameStateProvider gameStateProvider;
    public MissionPlanner missionPlanner;
    public Button startPlanningButton;

    private GameState gameState;

    void Start() {
        startPlanningButton.onClick.AddListener(() => startPlanningMission());
        gameState = gameStateProvider.getGameState();
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
        Debug.Log("launchMission: " + mission);
        missionPlanner.gameObject.SetActive(false);
        startPlanningButton.gameObject.SetActive(true);
        
        FlightPlan flightPlan = registry.flightPlans.Where(plan => plan.destination == mission.destinationData).First();
        Rocket rocket = GameObject.Instantiate(mission.rocketData.rocketObject);
        
        var activeMission = new ActiveMission(this, rocket, mission, Time.realtimeSinceStartup, flightPlan);
        gameState.registerActiveMission(activeMission);
        gameState.funds -= mission.getCost();
        gameState.registerProgress(mission.destinationData.progressionValue);
        activeMission.launch();
    }

    internal void onMissionComplete(ActiveMission activeMission) {
        activeMission.getRocket().onMissionComplete();
        var data = activeMission.getMissionData();
        Debug.Log("onMissionComplete: " + data);
        var completionBonus = data.payloadData.successBonus;
        gameState.registerMissionCompletion(activeMission);
        gameState.funds += completionBonus;
    }

    public MissionData buildMission(RocketData rocket, PayloadData payload, DestinationData destination) {
        var mission = new MissionData();
        mission.payloadData = payload;
        mission.rocketData = rocket;
        mission.destinationData = destination;
        return mission;
    }

    public void debugAdvanceGameStage() {
        gameState.registerProgress(gameState.getCurrentProgressValue() + 10);
    }
}
