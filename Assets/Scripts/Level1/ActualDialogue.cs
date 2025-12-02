using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ActualDialogue : MonoBehaviour
{
    public TMP_Text dialogueText;
    public DialogueController dialoguesController;
    [TextArea(4, 6)] public string[] actualLines = new string[4];

    public float textSpeed = 0.02f;

    public int index;

    public bool didDialogueStart;
    private void OnEnable()
    {
        didDialogueStart = false;
        if (dialoguesController.npc1)
        {
            dialoguesController.NPC1Dialogues();
        }
        if (dialoguesController.npc2)
        {
            dialoguesController.NPC2Dialogues();
        }
        if (dialoguesController.npc3)
        {
            dialoguesController.NPC3Dialogues();
        }
        if (dialoguesController.npc4)
        {
            dialoguesController.NPC4Dialogues();
        }       
    }
    private void Start()
    {
        if (dialoguesController.npc1)
        {
            dialoguesController.NPC1Dialogues();
        }
        if (dialoguesController.npc2)
        {
            dialoguesController.NPC2Dialogues();
        }
        if (dialoguesController.npc3)
        {
            dialoguesController.NPC3Dialogues();
        }
        if (dialoguesController.npc4)
        {
            dialoguesController.NPC4Dialogues();
        }
    }
    public void Update()
    {
        if (Time.timeScale == 0f || Time.timeScale == 1f)
        {
            if (!didDialogueStart)
            {
                StartDialogue();
            }
            /*else if (dialogueText.text == actualLines[index])
            {
                NextDialogueLine();
            }*/
            else if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
            {
                ActivateOptions();
                StopAllCoroutines();
                dialogueText.text = actualLines[index];
            }
            if(dialogueText.text == actualLines[index])
            {
                ActivateOptions();
            }
        }
    }

    public void StartDialogue()
    {
        didDialogueStart = true;
        if (Time.timeScale == 0f || Time.timeScale == 1f)
        {
            StartCoroutine(WriteLine());
        }
    }
    public void NextDialogueLine()
    {
        index++;
        if (index < actualLines.Length)
        {
            dialoguesController.DeActivatePeaceDialogue();
            dialoguesController.DeActivateContinueDialogue();
            dialoguesController.DeActivateCombatDialogue();
            StartCoroutine(WriteLine());
        }

        if (index >= actualLines.Length)
        {
            Finish();
        }
    }
    public void Finish()
    {
        index = 0;
        StopAllCoroutines();
        Time.timeScale = 1f;
        dialoguesController.DeActivateContinueDialogue();
        dialoguesController.DeActivateCombatDialogue();
        dialoguesController.DeActivatePeaceDialogue();
        dialoguesController.DeActivateDialoguePanel();
        dialoguesController.npc1 = false;
        dialoguesController.npc2 = false;
        dialoguesController.npc3 = false;
        dialoguesController.npc4 = false;
        dialoguesController.canStartDialogue = false;
        dialoguesController.UnLockPlayerCamera();
    }
    void ActivateOptions()
    {
        if(index == 0)
        {
            if (dialoguesController.npc1)
            {
                dialoguesController.Selection1();
                dialoguesController.ActivatePeaceDialogue();
                dialoguesController.DeActivateContinueDialogue();
                dialoguesController.DeActivateCombatDialogue();
            }
            if (dialoguesController.npc2)
            {
                dialoguesController.Selection2();
                dialoguesController.ActivateContinueDialogue();
                dialoguesController.DeActivatePeaceDialogue();
                dialoguesController.DeActivateCombatDialogue();
            }
            if (dialoguesController.npc3)
            {
                dialoguesController.Selection3();
                dialoguesController.ActivateContinueDialogue();
                dialoguesController.DeActivatePeaceDialogue();
                dialoguesController.DeActivateCombatDialogue();
            }
            if (dialoguesController.npc4)
            {
                dialoguesController.Selection4Part1();
                dialoguesController.ActivateContinueDialogue();
                dialoguesController.DeActivatePeaceDialogue();
                dialoguesController.DeActivateCombatDialogue();
            }
        }
        if(index == 1)
        {
            if (dialoguesController.npc2)
            {
                dialoguesController.DeActivateContinueDialogue();
                dialoguesController.ActivatePeaceDialogue();
                dialoguesController.ActivateCombatDialogue();
            }
            if (dialoguesController.npc3)
            {
                dialoguesController.DeActivateContinueDialogue();
                dialoguesController.ActivatePeaceDialogue();
                dialoguesController.ActivateCombatDialogue();
            }
            if (dialoguesController.npc4)
            {
                dialoguesController.Selection4Part2();
                dialoguesController.ActivateContinueDialogue();
                dialoguesController.DeActivateCombatDialogue();
                dialoguesController.ActivatePeaceDialogue();
            }
        }       
        if(index == 2)
        {
            if (dialoguesController.npc4)
            {
                dialoguesController.DeActivateContinueDialogue();
                dialoguesController.ActivateCombatDialogue();
                dialoguesController.DeActivatePeaceDialogue();
            }
        }        
    }
    private IEnumerator WriteLine()
    {
        if (Time.timeScale == 0f || Time.timeScale == 1f)
        {
            dialogueText.text = string.Empty;
            foreach (char letter in actualLines[index].ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSecondsRealtime(textSpeed);
            }
        }
    }
}
