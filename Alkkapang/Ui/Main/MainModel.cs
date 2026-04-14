using System;
using UnityEngine;
using UnityEngine.UI;

public class MainModel : ModelBase
{
    private const int SIZE = 5;
    private float[] pos = new float[SIZE];
    private float distance;
    public int TargetIndex { get; private set; } = 2;
    public float TargetPos { get; private set; }

    public Action<int, float> OnTabStateChanged;

    public void Init()
    {
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++)
        {
            pos[i] = distance * i;
        }
        TargetPos = pos[TargetIndex];
    }

    public void UpdateTargetByScrollValue(float scrollValue, float deltaX)
    {
        int newIndex = TargetIndex;
        float newPos = TargetPos;

        for (int i = 0; i < SIZE; i++)
        {
            if (scrollValue < pos[i] + distance * 0.5f && scrollValue > pos[i] - distance * 0.5f)
            {
                newIndex = i;
                newPos = pos[i];
                break;
            }
        }

        if (Mathf.Abs(deltaX) > 18f)
        {
            if (deltaX > 0 && newIndex > 0) newIndex--;
            else if (deltaX < 0 && newIndex < SIZE - 1) newIndex++;
            newPos = pos[newIndex];
        }

        TargetIndex = newIndex;
        TargetPos = newPos;
        OnTabStateChanged?.Invoke(TargetIndex, TargetPos);
    }

    public void UpdateTargetByIndex(int index)
    {
        TargetIndex = index;
        TargetPos = pos[index];
        OnTabStateChanged?.Invoke(TargetIndex, TargetPos);
    }

}
