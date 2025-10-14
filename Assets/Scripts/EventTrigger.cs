using UnityEngine;
using UnityEngine.SceneManagement;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string playerTag = "Player";

    private void Awake()
    {
        Collider trigger = GetComponent<Collider>();
        if (trigger != null)
        {
            trigger.isTrigger = true;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[EventTrigger] Trigger entered by {other.name} (root: {other.transform.root.name}) on {name}.");

        CharacterController controller = other.GetComponent<CharacterController>()
                                         ?? other.GetComponentInParent<CharacterController>();

        bool tagMatches = !string.IsNullOrEmpty(playerTag) &&
                          (other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag));

        if (controller == null && !tagMatches)
        {
            Debug.Log($"[EventTrigger] {other.name} ignored (no CharacterController and tag '{playerTag}' not found).");
            return;
        }

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("[EventTrigger] No scene name set. Scene will not change.");
            return;
        }

        Debug.Log($"[EventTrigger] Loading scene '{sceneToLoad}'.");
        SceneManager.LoadScene(sceneToLoad);
    }
}
