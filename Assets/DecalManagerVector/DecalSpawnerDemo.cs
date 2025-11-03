using UnityEngine;

public class DecalSpawnerDemo : MonoBehaviour
{
    public DecalProjectorCaller decalCaller;

    public float moveSpeed = 5f;

    private void Update()
    {
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.position += new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;

        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 origin = transform.position;
            Vector3 dir = transform.forward;
            decalCaller.ProyectarDecal(origin, dir);
        }
    }
}
