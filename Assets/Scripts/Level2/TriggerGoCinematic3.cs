using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerGoCinematic3 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Cinematic3");
        }
    }
}
