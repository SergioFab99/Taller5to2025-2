using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public float sprintMultiplier = 1.8f;
    Rigidbody rb; Transform cam;

    void OnEnable(){ Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked; }
    void OnDisable(){ Cursor.visible = true; Cursor.lockState = CursorLockMode.None; }

    void Awake(){ rb = GetComponent<Rigidbody>(); rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; cam = Camera.main ? Camera.main.transform : null; }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal"), v = Input.GetAxisRaw("Vertical");
        Vector2 in2 = new Vector2(h, v); if (in2.sqrMagnitude > 1f) in2.Normalize();
        bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 f = cam ? cam.forward : Vector3.forward; f.y = 0; f.Normalize();
        Vector3 r = cam ? cam.right   : Vector3.right;   r.y = 0; r.Normalize();
        Vector3 dir = f * in2.y + r * in2.x; if (dir.sqrMagnitude < 1e-6f) return;
        float spd = speed * (sprint ? sprintMultiplier : 1f);
        Vector3 vel = rb.linearVelocity; Vector3 t = dir * spd; Vector3 dv = new(t.x - vel.x, 0f, t.z - vel.z);
        rb.AddForce(dv, ForceMode.VelocityChange);
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}
