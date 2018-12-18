using System;
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

    private int progressStage = 0;
    private GameProgress currentProgress = GameProgress.BEGINNING;

    public void registerProgress(int stage) {
        Debug.Log("register progress " + stage);
        progressStage = Math.Max(stage, progressStage);
        currentProgress = updateCurrentProgress();
    }

    public void registerActiveMission(ActiveMission mission) {
        missions.Add(mission);
    }

    public void registerMissionCompletion(ActiveMission mission) {
        missions.Remove(mission);
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
