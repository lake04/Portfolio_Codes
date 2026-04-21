using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected float damage;
    public Rigidbody2D rb;
    [SerializeField] protected float speed;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("AutoDestroy", 2f);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void Init(Vector2 _Direction)
    {
        rb.AddForce(_Direction * speed, ForceMode2D.Impulse);

        if(_Direction.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void AutoDestroy()
    {
        Destroy(gameObject);
    }
}
