using UnityEngine;

public class Checkpoint : MonoBehaviour {
    public int power;
    public float zoomBack;
    public float bounce;
    public int used;
    public Transform drain;
    Vector3 startPos;
    void Start() {
        startPos = drain.position;
        used = 0;
    }

    public void Depower() {
        ++used;
        drain.position = startPos - new Vector3(0, 0.25f*used, 0);
    }
}
