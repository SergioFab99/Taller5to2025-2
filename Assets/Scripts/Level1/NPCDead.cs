using UnityEngine;

public class NPCDead : MonoBehaviour
{
    HealthController health;
    [SerializeField]GameObject letter;
    [SerializeField] NPCInteraction npcInteraction;
    void Start()
    {
        health = GetComponent<HealthController>();
        npcInteraction = GameObject.FindWithTag("Player").GetComponent<NPCInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(health.health <= 0)
        {
            letter.SetActive(true);
            letter.transform.position = transform.position;
        }*/
    }

    private void OnDestroy()
    {
        letter.transform.position = transform.position;
        letter.SetActive(true);
        npcInteraction.NPCCount();
    }
}
