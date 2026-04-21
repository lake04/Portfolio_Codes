using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyObject : MonoBehaviour
{
    public float lifeTime = 1.5f;

    private void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    protected virtual void ReturnToPool()
    {
        Destroy(gameObject);
    }
}
