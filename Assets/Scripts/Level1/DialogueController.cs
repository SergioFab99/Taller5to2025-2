using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
public class DialogueController : MonoBehaviour
{
    [SerializeField]string[] Npc1 = new string[1], Npc2 = new string[2], Npc3 = new string[2], Npc4 = new string[3],
        selection1 = new string[1], selection2 = new string[3], selection3 = new string[3], selection4 = new string[4];
    string[] row;
    [SerializeField] ActualDialogue actualDialogue;
    [SerializeField] TMP_Text combatSelectionTMP, peaceSelectionTMP, continueSelectionTMP;
    [SerializeField] GameObject dialoguePanel, combatButton, peaceButton, continueButton;
    [SerializeField] private PlayerCamera playerCamera;
    public bool npc1, npc2, npc3, npc4, canStartDialogue;

    /*[Serializable]
    public class Dialogues
    {
        public string[] Npc1 = new string[1];
        public string[]  Npc2, Npc3, Npc4, selection1, selection2, selection3, selection4;
    }
    [Serializable]
    public class DialoguesList
    {
        public Dialogues[] dialogues;
    }

    public DialoguesList myDialoguesList = new DialoguesList();*/

    private void Start()
    {
        DeActivateCombatDialogue();
        DeActivatePeaceDialogue();
        DeActivateContinueDialogue();
        DeActivateDialoguePanel();
        if (Time.timeScale == 0f || Time.timeScale == 1f)
        {
            ReadCSV();
        }
        npc1 = true;
        NPC1Dialogues();
    }

    void ReadCSV()
    {
        Debug.Log("ReadingCSV");
        TextAsset csvFile = Resources.Load<TextAsset>("DiálogosNPCSNivel1");
        if (csvFile == null)
        {
            Debug.LogError("Archivo 'Personajes.csv' no encontrado en Assets/Resources/");
            return;
        }

        string[] data = csvFile.text.Split(new string[] { ";", "\n" }, StringSplitOptions.None);
        //Debug.Log(csvFile);
        int tableSize = data.Length / 8 - 1;
        //myDialoguesList.dialogues = new Dialogues[tableSize];
        row = new string[tableSize];
        for(int i = 0; i < tableSize; i++)
        {
            //myDialoguesList.dialogues[i] = new Dialogues();
            //myDialoguesList.dialogues[0].Npc1[0] = data[8 * (0 + 1)];
            /*myDialoguesList.dialogues[i].Npc2[i] = data[4 * (i + 1) + 1];
            myDialoguesList.dialogues[i].Npc3[i] = data[4 * (i + 1) + 1];
            myDialoguesList.dialogues[i].Npc4[i] = data[4 * (i + 1) + 1];
            myDialoguesList.dialogues[i].selection1[i] = data[4 * (i + 1) + 1];
            myDialoguesList.dialogues[i].selection2[i] = data[4 * (i + 1) + 1];
            myDialoguesList.dialogues[i].selection3[i] = data[4 * (i + 1) + 1];
            myDialoguesList.dialogues[i].selection4[i] = data[4 * (i + 1) + 1];*/
            if (i < 1)
            {
                Npc1[i] = data[8 * (i + 1)];
                selection1[i] = data[8 * (i + 1) + 4];
            }                
            if (i < 2)
            {
                Npc2[i] = data[8 * (i + 1) + 1];
                Npc3[i] = data[8 * (i + 1) + 2];
            }
            if(i < 3)
            {
                Npc4[i] = data[8 * (i + 1) + 3];
                selection2[i] = data[8 * (i + 1) + 5];
                selection2[2] = data[8 * (2 + 1) + 6];
                selection3[i] = data[8 * (i + 1) + 6];
                selection3[2] = data[8 * (2 + 1) + 7];
            }
            if (i < 4)
            {
                selection4[i] = data[8 * (i + 1) + 7];
                selection4[2] = data[8 * (2 + 1) + 8];
                selection4[3] = data[8 * (3 + 1) + 8];
            }
        }
    }
    //Dialogues
    public void NPC1Dialogues()
    {
        if (!actualDialogue.isActiveAndEnabled || actualDialogue.isActiveAndEnabled)
        {
            for (int i = 0; i < Npc1.Length; i++)
            {
                actualDialogue.actualLines[i] = Npc1[i];
            }
        }        
        peaceSelectionTMP.text = selection1[0];
        canStartDialogue = true;
    }
    public void NPC2Dialogues()
    {
        if(!actualDialogue.isActiveAndEnabled || actualDialogue.isActiveAndEnabled)
        {
            for (int i = 0; i < Npc2.Length; i++)
            {
                actualDialogue.actualLines[i] = Npc2[i];
            }
        }
        canStartDialogue = true;
    }
    public void NPC3Dialogues()
    {
        if(!actualDialogue.isActiveAndEnabled || actualDialogue.isActiveAndEnabled)
        {
            for (int i = 0; i < Npc3.Length; i++)
            {
                actualDialogue.actualLines[i] = Npc3[i];
            } 
        }
        canStartDialogue = true;
    }
    public void NPC4Dialogues()
    {
        if (!actualDialogue.isActiveAndEnabled || actualDialogue.isActiveAndEnabled)
        {
            for (int i = 0; i < Npc4.Length; i++)
            {
                actualDialogue.actualLines[i] = Npc4[i];
            }
        }
        canStartDialogue = true;
    }
    //Selections
    public void Selection1()
    {
        peaceSelectionTMP.text = selection1[0];
    }
    public void Selection2()
    {
        continueSelectionTMP.text = selection2[0];
        peaceSelectionTMP.text = selection2[1];
        combatSelectionTMP.text = selection2[2];
    }
    public void Selection3()
    {
        continueSelectionTMP.text = selection3[0];
        peaceSelectionTMP.text = selection3[1];
        combatSelectionTMP.text = selection3[2];
    }
    public void Selection4Part1()
    {
        continueSelectionTMP.text = selection4[0];
        peaceSelectionTMP.text = selection4[1];
        combatSelectionTMP.text = selection4[3];
    }
    public void Selection4Part2()
    {
        continueSelectionTMP.text = selection4[2];
    }
    //Activates
    public void ActivateDialoguePanel()
    {
        dialoguePanel.SetActive(true);
    }
    public void ActivateCombatDialogue()
    {
        combatButton.SetActive(true);
    }
    public void ActivatePeaceDialogue()
    {
        peaceButton.SetActive(true);
    }
    public void ActivateContinueDialogue()
    {
        continueButton.SetActive(true);
    }
    //DeActivates
    public void DeActivateDialoguePanel()
    {
        dialoguePanel.SetActive(false);
    }
    public void DeActivateCombatDialogue()
    {
        combatButton.SetActive(false);
    }
    public void DeActivatePeaceDialogue()
    {
        peaceButton.SetActive(false);
    }
    public void DeActivateContinueDialogue()
    {
        continueButton.SetActive(false);
    }
    //PlayerCamera
    public void LockPlayerCamera()
    {
        playerCamera.SetLookLocked(true);
    }
    public void UnLockPlayerCamera()
    {
        playerCamera.SetLookLocked(false);
    }
}
