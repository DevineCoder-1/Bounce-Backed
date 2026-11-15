using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerControls : MonoBehaviour {
    Rigidbody2D rb;
    SpriteRenderer sp;
    public Sprite dead, ground, air;
    [SerializeField] Volume volume;
    public AudioSource walk;
    public AudioSource jump;
    [SerializeField] LiftGammaGain gammaCain;
    //Here are some variables that control basic jumping and speed
    public float speed;
    public PhysicsMaterial2D pm2d;
    float cooldown;
    bool isDead;
    public Transform tracking;
    public float jumpCooldown;
    public float jumpForce;
    public ButtPlayer butt;
    //One NHS teacher once possessed about 4 TB or Checkpoints, I do not need to name who
    public Vector3 lastCPPos;
    Vector3 startPos;
    public Checkpoint lastCP;
    public GameObject CPTxt;
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        isDead = false;
        startPos = CPTxt.transform.position;
        CPTxt.transform.position = startPos + new Vector3(50000f,0,0);
        pm2d.bounciness = 0.2f;
        if (volume != null && volume.profile.TryGet(out gammaCain))
        {
            gammaCain.active = true;
            gammaCain.gamma.overrideState = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spikes") && !isDead)
        {
            isDead = true;
            StartCoroutine(dmg(0f, 2f, 2f, -5f));
            StartCoroutine(Death());
        }
        if (collision.CompareTag("Checkpoint"))
        {
            lastCPPos = collision.transform.position;
            lastCP = collision.GetComponent<Checkpoint>();
            if (lastCP.bounce > pm2d.bounciness)
            {
                
                StartCoroutine(Move());
                pm2d.bounciness = lastCP.bounce;
                tracking.position = new Vector3(tracking.position.x, tracking.position.y, lastCP.zoomBack);
            }
        }
    }
    public void SetGamma(float gammaValue)
    {
        gammaCain.active = true;
        gammaCain.lift.overrideState = true;
        Vector4 newGamma = new Vector4(1f, 1f, 1f, gammaValue);
        gammaCain.lift.Override(newGamma);
    }

    IEnumerator Move() {
        float offset = 50000;
        while(offset > 1) {
            CPTxt.transform.position= 
            startPos + new Vector3(offset, 0, 0);
            offset /= 1.1f;
            yield return new WaitForSeconds(0.02f);
        }
        offset = -1f;
        while (offset > -50000f)
        {
            CPTxt.transform.position =
            startPos + new Vector3(offset, 0, 0);
            offset *= 1.05f;
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator dmg(float t1, float t3, float t2, float multi)
    {
        float timing = 0;
        while (timing < t1)
        {
            SetGamma(multi * timing);
            timing += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timing = t2;
        SetGamma(multi * timing);
        yield return new WaitForSeconds(t3);
        while (timing > 0f)
        {
            SetGamma(multi * timing);
            timing -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        SetGamma(0);
    }

    IEnumerator Death() {
        yield return new WaitForSeconds(0.15f);
        sp.sprite = dead;
        yield return new WaitForSeconds(1f);
        transform.position = lastCPPos;
        lastCP.Depower();
        isDead = false;
    }

    // Update is called once per frame
    void FixedUpdate() {
        float groundVel = butt.groundVelocity;
        rb.linearVelocity = new Vector2(groundVel + Input.GetAxisRaw("Horizontal") * speed 
            * (isDead ? 0 : 1), rb.linearVelocity.y);
        RaycastHit2D onGround = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, 1 << 0);
        RaycastHit2D onGround2 = Physics2D.Raycast(transform.position + new Vector3(-0.35f, 0f, 0f), Vector2.down, 0.6f, 1 << 0);
        RaycastHit2D onGround3 = Physics2D.Raycast(transform.position + new Vector3(0.35f, 0f, 0f), Vector2.down, 0.6f, 1 << 0);
        if (Input.GetKey(KeyCode.Space) && (onGround || onGround2 || onGround3) &&
            cooldown < 0 && rb.linearVelocityY >= 0f) {
            cooldown = jumpCooldown;
            jump.Play();
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
        if(!isDead) {
            if(onGround || onGround2 || onGround3) {
                sp.sprite = ground;
            } else {
                sp.sprite = air;
            }
            if(Input.GetAxisRaw("Horizontal") != 0)
            {
                walk.volume = 1;
            } else
            {
                walk.volume = 0;
            }
        }
        cooldown -= 0.02f;
    }
}
