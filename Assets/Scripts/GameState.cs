﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : ScriptableObject {
    public const int STAGE_ATMOSPHERE = 10;
    public const int STAGE_LOW_ORBIT = 20;
    public const int STAGE_SPACE_STATION = 30;
    public const int STAGE_HIGH_ORBIT = 40;
    public const int STAGE_MOON = 50;
    public string playerName = "Sergei";
    public float funds = 0;
    private List<ActiveMission> missions = new List<ActiveMission>();
    private Dictionary<PayloadData, int> deliveredPayloads = new Dictionary<PayloadData, int>();

    private int totalLaunchAttempts = 0;
    private int totalMissionSuccesses = 0;
    private int progressStage = 0;
    private GameProgress currentProgress = GameProgress.BEGINNING;

    public void registerProgress(int stage) {
        Debug.Log("register progress " + stage);
        progressStage = Math.Max(stage, progressStage);
        currentProgress = updateCurrentProgress();
    }

    public List<ActiveMission> GetActiveMissions() {
        return missions;
    }

    public void registerActiveMission(ActiveMission mission) {
        totalLaunchAttempts++;
        missions.Add(mission);
    }

    public void registerMissionFailure(ActiveMission mission) {
        missions.Remove(mission);
    }

    public void registerMissionCompletion(ActiveMission mission) {
        totalMissionSuccesses++;
        missions.Remove(mission);
        var payload = mission.getMissionData().payloadData;

        int currentCount;
        deliveredPayloads.TryGetValue(payload, out currentCount);
        deliveredPayloads[payload] = currentCount + 1;
    }

    public int getDeliveredPayloadCount(PayloadData payload) {
        if (payload == null) return 0;
        int currentCount;
        deliveredPayloads.TryGetValue(payload, out currentCount);
        return currentCount;
    }

    internal int getTotalLaunchCount() {
        return totalLaunchAttempts;
    }

    public GameProgress getCurrentProgress() {
        return currentProgress;
    }

    public int getCurrentProgressValue() {
        return progressStage;
    }

    private GameProgress updateCurrentProgress() {
        if (progressStage == 0) {
            return GameProgress.BEGINNING;
        }
        if (progressStage <= STAGE_ATMOSPHERE) {
            return GameProgress.ATMOSPHERE;
        }
        if (progressStage <= STAGE_LOW_ORBIT) {
            return GameProgress.LOW_ORBIT;
        }
        if (progressStage <= STAGE_SPACE_STATION) {
            return GameProgress.SPACE_STATION;
        }
        if (progressStage <= STAGE_HIGH_ORBIT) {
            return GameProgress.HIGH_ORBIT;
        }
        if (progressStage <= STAGE_MOON) {
            return GameProgress.MOON;
        }
        return GameProgress.COMPLETE;
    }

    public enum GameProgress {
        BEGINNING,
        ATMOSPHERE,
        LOW_ORBIT,
        SPACE_STATION,
        HIGH_ORBIT,
        MOON,
        COMPLETE,
    }
}
