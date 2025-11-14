using UnityEngine;
using System.Collections;

public class ActivateOnClick : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Vector3 localOffset = new Vector3(0, 1, 0);

    private void Start()
    {
        if (targetObject != null)
            targetObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(ActivateSequence());
    }

    private IEnumerator ActivateSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (targetObject != null)
        {
            targetObject.transform.SetParent(transform);
            targetObject.transform.localPosition = localOffset;
            targetObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        if (targetObject != null)
            targetObject.SetActive(false);
    }
}
