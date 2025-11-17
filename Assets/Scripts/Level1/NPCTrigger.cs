using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    [SerializeField] bool npc1, npc2, npc3, npc4;
    [SerializeField] DialogueController dialogueController;
    [SerializeField] GameObject spawnPointNpc2, spawnPointNpc3, spawnPointNpc4, prefabEnemy;
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
        Instantiate(prefabEnemy, spawnPointNpc2.transform.position, spawnPointNpc2.transform.rotation);
    }
    void SpawnEnemyNpc3()
    {
        Instantiate(prefabEnemy, spawnPointNpc3.transform.position, spawnPointNpc3.transform.rotation);
    }
    void SpawnEnemyNpc4()
    {
        Instantiate(prefabEnemy, spawnPointNpc4.transform.position, spawnPointNpc4.transform.rotation);
    }
}
