using Lop.Survivor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisasterManager : MonoBehaviour
{
    [SerializeField] private List<Disaster> disasterList;
    private Dictionary<string, List<Disaster>> disastersByIsland = new Dictionary<string, List<Disaster>>();

    [SerializeField] private string IslandName;
    private List<int> predictedDisasterLevels = new List<int>(3);

    [SerializeField] private List<ScheduledData> scheduledDisaster = new List<ScheduledData>();
    private void Awake()
    {
        GroupDisastersByIsland();
    }

    private void Start()
    {
        TickManager.Instance.RegisterTickEvent(ActionType.DayInitialize, OnDayChanged);
    }

    private void OnDisable()
    {
        TickManager.Instance.DestroyTickEvent(ActionType.DayInitialize, OnDayChanged);
    }

    private void OnDayChanged()
    {
        TriggerDisasterForCurrentDay(IslandName);
    }


    public void GroupDisastersByIsland()
    {
        disastersByIsland.Clear();

        foreach (Disaster disasterPrefab in disasterList)
        {
            if (disasterPrefab.disasterData == null) continue;

            string islandName = disasterPrefab.disasterData.Island;

            if (!disastersByIsland.ContainsKey(islandName))
                disastersByIsland.Add(islandName, new List<Disaster>());

            disastersByIsland[islandName].Add(disasterPrefab);
        }
    }

    public void GenerateNewPredictionTable()
    {
        int currentDay = TimeManager.Instance.CurrentDay;
        System.Random seededRandom = new System.Random(currentDay);

        predictedDisasterLevels.Clear();

        for (int i = 0; i < 3; i++)
        {
            int targetDay = currentDay + i + 1;
            int disasterLevel = 0;

            ScheduledData scheduled = scheduledDisaster.FirstOrDefault(s => s.day == targetDay);
            if (scheduled != null)
            {
                disasterLevel = scheduled.level;
            }
            else
            {
                int rand = seededRandom.Next(0, 100);
                if (rand < 70) disasterLevel = 0;
                else if (rand < 90) disasterLevel = 1;
                else if (rand < 99) disasterLevel = 2;
                else disasterLevel = 3;
            }

            predictedDisasterLevels.Add(disasterLevel);
        }
    }

    public void TriggerDisasterForCurrentDay(string islandName)
    {
        Debug.Log("재해 실행");
        if (predictedDisasterLevels.Count == 0)
            GenerateNewPredictionTable();

        int curLevel = predictedDisasterLevels[0];
        predictedDisasterLevels.RemoveAt(0);

        AddNextDayPrediction();

        int curDay = TimeManager.Instance.CurrentDay;

        ScheduledData todayScheduled = scheduledDisaster.FirstOrDefault(s => s.day == curDay);
        if (todayScheduled != null)
        {
            SelectFinalDisasterAndTrigger(islandName, todayScheduled.level);
            return;
        }

        if (curLevel == 0)
            return;

        SelectFinalDisasterAndTrigger(islandName, curLevel);
    }

    private void AddNextDayPrediction()
    {
        int currentDay = TimeManager.Instance.CurrentDay;
        int targetDay = currentDay + 3;
        System.Random seededRandom = new System.Random(targetDay - 1);

        int disasterLevel = 0;

        ScheduledData scheduled = scheduledDisaster.FirstOrDefault(s => s.day == targetDay);
        if (scheduled != null)
        {
            disasterLevel = scheduled.level;
        }
        else
        {
            int rand = seededRandom.Next(0, 100);
            if (rand < 70) disasterLevel = 0;
            else if (rand < 90) disasterLevel = 1;
            else if (rand < 99) disasterLevel = 2;
            else disasterLevel = 3;
        }

        predictedDisasterLevels.Add(disasterLevel);
    }

    public void SelectFinalDisasterAndTrigger(string islandName, int predeterminedLevel)
    {
        if (!disastersByIsland.ContainsKey(islandName))
        {
            Debug.LogError($"'{islandName}' 섬에 할당된 재해가 없음");
            return;
        }

        List<Disaster> candidates = disastersByIsland[islandName]
            .Where(d => d.disasterData != null && d.disasterData.Level == predeterminedLevel)
            .ToList();

        if (candidates.Count == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, candidates.Count);
        Disaster selectedDisasterPrefab = candidates[randomIndex];

        Disaster instance = Instantiate(selectedDisasterPrefab);
        instance.StartDisaster();
    }
}
