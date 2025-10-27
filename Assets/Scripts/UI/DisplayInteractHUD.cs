using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DisplayInteractHUD : MonoBehaviour
{
    Coroutine coroutine;
    bool startCoroutine;
    [SerializeField] float timeBetween;
    [SerializeField] bool starRay;
    PlayerCombat playerCombat;
    public DefaultGrabThrowSettings DefaultGrabThrowSettings;
    public Transform cam;
    void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
        cam = playerCombat.cam;
        DefaultGrabThrowSettings = playerCombat.DefaultGrabThrowSettings;
        coroutine = StartCoroutine(Display(timeBetween));
    }

    void Update()
    {
        
    }
    IEnumerator Display(float timeBetween)
    {
        while (!startCoroutine)
        {
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, DefaultGrabThrowSettings.grabRange))
            {
                if (hit.collider.CompareTag("Interactuable") || hit.collider.CompareTag("Grabbable"))
                {
                    var canvas = GameObject.Find("Canvas");
                    var canInteract = canvas.transform.Find("InteractBackground").gameObject;
                    Debug.Log("Sí hay");
                    Debug.Log(canInteract);
                    canInteract.SetActive(true);
                    yield return new WaitForSeconds(timeBetween);
                }
                else
                {
                    var canvas = GameObject.Find("Canvas");
                    var canInteract = canvas.transform.Find("InteractBackground").gameObject;
                    Debug.Log("No hay");
                    Debug.Log(canInteract);
                    canInteract.SetActive(false);
                    yield return new WaitForSeconds(timeBetween);
                }
            }
            yield return new WaitForSeconds(0f);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactuable"))
        {

        }
    }
}
