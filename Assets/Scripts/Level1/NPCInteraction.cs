using UnityEngine;
using UnityEngine.SceneManagement;
public class NPCInteraction : MonoBehaviour
{
    public event StartFinish startFinish;
    public delegate void StartFinish();
    [SerializeField] private string nextScene;
    int npcCount;
    void Start()
    {
        startFinish += FinishLevel1;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Npc"))
        {
            npcCount++;
            Destroy(other.gameObject);
            startFinish?.Invoke();
        }
    }

    void FinishLevel1()
    {
        if(npcCount >= 4)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
    private void OnDestroy()
    {
        startFinish += FinishLevel1;
    }
}
