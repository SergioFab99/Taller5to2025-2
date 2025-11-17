using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    [SerializeField] bool npc1, npc2, npc3, npc4;
    [SerializeField] DialogueController dialogueController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (npc1)
            {
                dialogueController.npc1 = true;
                dialogueController.NPC1Dialogues();
            }
            if (npc2)
            {
                dialogueController.npc2 = true;
                dialogueController.NPC2Dialogues();
            }
            if (npc3)
            {
                dialogueController.npc3 = true;
                dialogueController.NPC3Dialogues();
            }
            if (npc4)
            {
                dialogueController.npc4 = true;
                dialogueController.NPC4Dialogues();
            }
        }
    }

    
}
