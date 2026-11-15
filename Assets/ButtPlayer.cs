using UnityEngine;

public class ButtPlayer : MonoBehaviour
{
    public float groundVelocity;
    public float momentaLoss;
    private void OnTriggerStay2D(Collider2D collision) {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        if (rb != null) {
            groundVelocity = rb.linearVelocityX; }
    }

    void FixedUpdate() {
        groundVelocity /= momentaLoss;
    }
}
