using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    [SerializeField] bool npc1, npc2, npc3, npc4;
    [SerializeField] DialogueController dialogueController;
    [SerializeField] GameObject aggerssiveNpc2, aggerssiveNpc3, aggerssiveNpc4, passiveNpc2, passiveNpc3, passiveNpc4;
    [SerializeField] BoxCollider boxCollider;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (npc1)
            {
                dialogueController.npc1 = true;
                dialogueController.npc2 = false;
                dialogueController.npc3 = false;
                dialogueController.npc4 = false;
                dialogueController.NPC1Dialogues();                
            }
            if (npc2)
            {
                dialogueController.npc1= false;
                dialogueController.npc2 = true;
                dialogueController.npc3 = false;
                dialogueController.npc4 = false;
                dialogueController.NPC2Dialogues();
            }
            if (npc3)
            {
                dialogueController.npc1 = false;
                dialogueController.npc2 = false;
                dialogueController.npc3 = true;
                dialogueController.npc4 = false;
                dialogueController.NPC3Dialogues();
            }
            if (npc4)
            {
                dialogueController.npc1 = false;
                dialogueController.npc2 = false;
                dialogueController.npc3 = false;
                dialogueController.npc4 = true;
                dialogueController.NPC4Dialogues();
            }
            
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var canvas = GameObject.Find("Canvas (1)");
            var canInteract = canvas.transform.Find("InteractBackground").gameObject;
            if (Input.GetKey(KeyCode.E) && Time.timeScale == 1 && dialogueController.canStartDialogue)
            {
                Debug.Log("InteractionNPC");
                dialogueController.ActivateDialoguePanel();
                dialogueController.LockPlayerCamera();
                NPCInteraction.OnTriggerNpc = false;
                canInteract.SetActive(false);
                Time.timeScale = 0;
                boxCollider.enabled = false;
            }
        }
    }
    public void CombatOption()
    {
        Debug.Log("CombatWithNPC");
        if (dialogueController.npc2)
        {
            Invoke(nameof(SpawnEnemyNpc2), 1f);
        }
        if (dialogueController.npc3)
        {
            Invoke(nameof(SpawnEnemyNpc3), 1f);
        }
        if (dialogueController.npc4)
        {
            Invoke(nameof(SpawnEnemyNpc4), 1f);
        }
    }

    void SpawnEnemyNpc2()
    {
        SpawnEnemiesLvl1.enemiesCapacity++;
        aggerssiveNpc2.SetActive(true);
        passiveNpc2.SetActive(false);
    }
    void SpawnEnemyNpc3()
    {
        SpawnEnemiesLvl1.enemiesCapacity++;
        aggerssiveNpc3.SetActive(true);
        passiveNpc3.SetActive(false);
    }
    void SpawnEnemyNpc4()
    {
        SpawnEnemiesLvl1.enemiesCapacity++;
        aggerssiveNpc4.SetActive(true);
        passiveNpc4.SetActive(false);
    }
}
