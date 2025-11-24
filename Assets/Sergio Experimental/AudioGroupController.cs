using UnityEngine;

public class AudioGroupController : MonoBehaviour
{
    [Header("Audios de exteriores")]
    public AudioSource audio1;
    public AudioSource audio2;
    public AudioSource audio3;

    public void PlayAudio(int index)
    {
        
        audio1.Stop();
        audio2.Stop();
        audio3.Stop();

        switch (index)
        {
            case 1:
                audio1.Play();
                break;
            case 2:
                audio2.Play();
                break;
            case 3:
                audio3.Play();
                break;
            default:
                Debug.LogWarning("invalid audio index");
                break;
        }
    }
}
