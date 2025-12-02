using UnityEngine;
using TMPro;
public class InteractText : MonoBehaviour
{
    
    private TMP_Text interactTMP;
    PlayerInputActions _inputActions;
    [SerializeField] string interactText, noFinal, mayus, quotes;
    [SerializeField] string[] interactTextSeparate;
    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
    }
    void Start()
    {        
        var input = _inputActions.Player;
        interactTMP = gameObject.GetComponent<TMP_Text>();
        interactText = input.Interact.ToString();
        interactTextSeparate = interactText.Split("/");
        noFinal = interactTextSeparate[3].Replace("]", string.Empty);
        mayus = noFinal.ToUpper();
        interactTMP.text = $"Press {quotes}{mayus}{quotes} to Interact";
    }

    private void OnDestroy()
    {
        //_inputActions.Disable();
        _inputActions.Dispose();
    }
    void Update()
    {
        
    }
}
