using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    Vector2 start, end, direction;
    public int dir;
    public float maxSpeed;
    public float speedUpDist;
    Rigidbody2D rb;
    void Start()
    {
        start = startPoint.position;
        end = endPoint.position;
        direction = Vector2.ClampMagnitude(end - start, 1.0f);
        Debug.Log(direction.x + ", " + direction.y);
        dir = 1;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = Mathf.Min(Vector2.Distance(transform.position, start), Vector2.Distance(transform.position, end));
        rb.linearVelocity = Mathf.Max(Mathf.Min(1, dist / speedUpDist), 0.1f) * maxSpeed * direction * dir;
        if (transform.position.y - end.y > 0) dir = -1;
        if (start.y - transform.position.y > 0) dir = 1;
    }
}
