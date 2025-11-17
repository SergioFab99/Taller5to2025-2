using UnityEngine;
using UnityEngine.SceneManagement;
public class NPCInteraction : MonoBehaviour
{
    public event StartFinish startFinish;
    public delegate void StartFinish();
    [SerializeField] private string nextScene;
    [SerializeField] private DialogueController dialogueController;
    int npcCount;
    void Start()
    {
        startFinish += FinishLevel1;
        var canvas = GameObject.Find("Canvas (1)");
        var canInteract = canvas.transform.Find("InteractBackground").gameObject;
        canInteract.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Npc"))
        {
            var canvas = GameObject.Find("Canvas (1)");
            var canInteract = canvas.transform.Find("InteractBackground").gameObject;
            canInteract.SetActive(true);
            /*if(Input.GetKeyDown(KeyCode.E) && Time.timeScale == 1)
            {
                Debug.Log("InteractionNPC");
                dialogueController.ActivateDialoguePanel();
                dialogueController.LockPlayerCamera();
                canInteract.SetActive(false);
                Time.timeScale = 0;
                other.enabled = false;
            }*/
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Npc"))
        {
            var canvas = GameObject.Find("Canvas (1)");
            var canInteract = canvas.transform.Find("InteractBackground").gameObject;
            if (Input.GetKey(KeyCode.E) && Time.timeScale == 1)
            {
                Debug.Log("InteractionNPC");
                dialogueController.ActivateDialoguePanel();
                dialogueController.LockPlayerCamera();
                canInteract.SetActive(false);
                Time.timeScale = 0;
                other.enabled = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
         if (other.gameObject.CompareTag("Npc"))
        {
            var canvas = GameObject.Find("Canvas (1)");
            var canInteract = canvas.transform.Find("InteractBackground").gameObject;
            canInteract.SetActive(false);
        }
    }
    public void NPCCount()
    {
        npcCount++;
        startFinish?.Invoke();
        Time.timeScale = 1;
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
        startFinish -= FinishLevel1;
    }
}
