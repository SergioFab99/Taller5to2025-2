using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolTestSpawner : MonoBehaviour
{
    public SimpleObjectPool[] objectPools;
    public float spawnInterval = 1f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnInterval)
        {
            timer = 0f;
            int randomPool = UnityEngine.Random.Range(0, objectPools.Length);
            SimpleObjectPool pool = objectPools[randomPool];

            Vector3 pos = new Vector3(
                UnityEngine.Random.Range(-8f, 8f),
                1,
                UnityEngine.Random.Range(-8f, 8f)
            );
            GameObject obj = pool.GetObject(pos);
            if (obj != null)
            {
                StartCoroutine(ReturnAfterSeconds(pool, obj, 3f));
            }
        }
    }

    private IEnumerator ReturnAfterSeconds(SimpleObjectPool pool, GameObject obj, float secs)
    {
        yield return new WaitForSeconds(secs);
        pool.ReturnObject(obj);
    }
}
