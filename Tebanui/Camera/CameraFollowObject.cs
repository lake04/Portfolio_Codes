using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References1")]
    [SerializeField] private Transform playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float flipYRotationTime = 0.5f;

    private Coroutine turnCoroutine;

    private Player player;

    private bool isFacingRight;

    void Start()
    {
        player = playerTransform.gameObject.GetComponent<Player>();

        isFacingRight = player.isFacingRight;
    }

    void Update()
    {
        transform.position = player.transform.position;
    }

    public void CallTurn()
    {
        turnCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermintEndRotation();
        float yRotation = 0f;

        float elasedTime = 0f;
        while(elasedTime < flipYRotationTime)
        {
            elasedTime += Time.deltaTime;

            yRotation = Mathf.Lerp(startRotation, endRotationAmount,(elasedTime / flipYRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }
    }

    private float DetermintEndRotation()
    {
        isFacingRight = !isFacingRight;

        if(isFacingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}
