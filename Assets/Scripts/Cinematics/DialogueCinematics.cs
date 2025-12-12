using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Cinemachine;
using UnityEngine.Playables;
public class DialogueCinematics : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    [SerializeField] private GameObject panelDialogues;
    [SerializeField] private PlayableDirector timeLineCinematic;
    [TextArea(4, 6)] public string[] actualLines;
    [TextArea(4, 6)] public string[] nameLines;
    [SerializeField] private CinemachineBrain cinemachineBrain;
    public float textSpeed = 0.02f;

    public int index;

    public bool didDialogueStart, thisIsCinematic1, thisIsCinematic2, thisIsCinematic3;
    bool canChangeLines;
    private void OnEnable()
    {
        didDialogueStart = false;
    }
    private void Start()
    {
        index = 0;
        cinemachineBrain = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>();
        timeLineCinematic = GetComponent<PlayableDirector>();
        panelDialogues.SetActive(false);
        ChangeName();
    }
    public void FixedUpdate()
    {
        if (Time.timeScale == 0f || Time.timeScale == 1f)
        {
            if (!didDialogueStart && panelDialogues.activeInHierarchy)
            {
                StartDialogue();
            }
            /*else if (dialogueText.text == actualLines[index])
            {
                NextDialogueLine();
            }*/
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && canChangeLines)
            {
                NextDialogueLine();
            }
            if (dialogueText.text == actualLines[index])
            {
                
            }
        }
    }
    public void ChangeBrainToEasyOut()
    {
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
    }
    public void ChangeBrainToCut()
    {
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
    }
    public void ActivePanel()
    {
        panelDialogues.SetActive(true);
    }
    public void DeActivePanel()
    {
        panelDialogues.SetActive(false);
    }
    public void StartDialogue()
    {
        didDialogueStart = true;
        if (Time.timeScale == 0f || Time.timeScale == 1f)
        {
            StartCoroutine(WriteLine());
        }
    }
    void ChangeName()
    {
        if (thisIsCinematic1)
        {
            if (index == 0 || index == 2 || index == 5 || index == 6 || index == 8)
            {
                nameText.text = nameLines[0];
            }
            if (index == 1 || index == 3 || index == 4 || index == 7)
            {
                nameText.text = nameLines[1];
            }
        }
        if (thisIsCinematic2)
        {
            if (index == 1 || index == 3 || index == 5 || index == 7 || index == 9)
            {
                nameText.text = nameLines[0];
            }
            if (index == 0 || index == 2 || index == 4 || index == 6 || index == 8)
            {
                nameText.text = nameLines[1];
            }
        }
        if (thisIsCinematic3)
        {
            if (index == 2 || index == 4)
            {
                nameText.text = nameLines[0];
            }
            if (index == 0 || index == 1)
            {
                nameText.text = nameLines[1];
            }
            if (index == 3 || index == 5)
            {
                nameText.text = nameLines[2];
            }
        }
        
    }
    public void PauseTimeLine()
    {
        canChangeLines = true;
        timeLineCinematic.Pause();       
    }
    public void NextDialogueLine()
    {
        canChangeLines = false;
        timeLineCinematic.Resume();        
        index++;
        ChangeName();
        if (index < actualLines.Length)
        {
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
