using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    private Transform startPos;
    private Vector2 direction;
    private Vector3 initialPos;

    [SerializeField] protected float damage;
    private Rigidbody2D rb;
    [SerializeField] protected float speed;
    private float currentSpeed;
    [SerializeField] private float maxFlySpeed = 20f;

    [SerializeField] private float maxDistance = 10f;
    private bool isReturning = false;

    [SerializeField] private bool isDestroy;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!isReturning)
        {

            float distanceTraveled = Vector3.Distance(transform.position, initialPos);

            if (distanceTraveled >= maxDistance)
            {
                StartReturn();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isReturning)
        {
            currentSpeed += speed * Time.fixedDeltaTime;

            if (currentSpeed > maxFlySpeed)
            {
                currentSpeed = maxFlySpeed;
            }

            rb.velocity = direction * currentSpeed;


            float distanceTraveled = Vector3.Distance(transform.position, initialPos);

            if (distanceTraveled >= maxDistance)
            {
                StartReturn();
            }
        }
    }

    public  void Init(Vector2 _direction,Transform _startPos)
    {
        direction = _direction;
        startPos = _startPos;
        currentSpeed = speed;
        initialPos = transform.position;


    }


    public void StartReturn()
    {
        rb.velocity = Vector2.zero;

        isReturning = true;

        StartCoroutine(ReturnToStart(0.4f)); 
    }

    private IEnumerator ReturnToStart(float duration)
    {
        yield return new WaitForSeconds(0.5f);

        float elapsedTime = 0f;
        Vector3 initialPos = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(initialPos, startPos.position, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        transform.position = startPos.position;

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().TakeDamage(1);
            if(isDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
