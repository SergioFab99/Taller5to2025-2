using UnityEngine;

// Super básico: persigue al objeto con tag "Player".
public class SimpleChaseEnemy : MonoBehaviour
{
    public float speed = 3f;
    public string playerTag = "Player";

    Transform target;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null) target = player.transform;
    }

    void Update()
    {
        if (!target)
        {
            var player = GameObject.FindGameObjectWithTag(playerTag);
            if (player) target = player.transform;
            else return;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f; // plano XZ
        if (toTarget.sqrMagnitude < 0.0001f) return;

        Vector3 dir = toTarget.normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}
