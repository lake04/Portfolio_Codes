using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepSeaFish : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float waveAmplitude = 0.5f;
    [SerializeField] private float waveFrequency = 2f;

    [SerializeField] private float xRange;
    private int nextMove = 1;

    private float waveTimer = 0f;
    private Vector3 startPos;
    private SpriteRenderer sp;

    void Start()
    {
        startPos = transform.position;
        sp = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MoveWave();
    }

    private void MoveWave()
    {

        waveTimer += Time.deltaTime * waveFrequency;
        float yOffset = Mathf.Cos(waveTimer) * waveAmplitude;

        Vector3 targetPos = new Vector3(
            transform.position.x + nextMove * moveSpeed * Time.deltaTime,
            startPos.y + yOffset,
            0
        );

        transform.position = targetPos;

        if (transform.position.x <= startPos.x - xRange)
        {
            nextMove = 1;
            sp.flipX = true;
        }
        else if (transform.position.x >= startPos.x + xRange)
        {
            nextMove = -1;
            sp.flipX = false;
        }

    }
}
