using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UIBorder : MonoBehaviour
{
    [SerializeField] Image border;
    bool animated;
    [SerializeField] float timer, maxTimer;
    void Start()
    {
        border = GetComponent<Image>();
        
        if (Time.timeScale == 0 || Time.timeScale == 1)
        {
            //Debug.Log($"Star coroutine animate");
            //StartCoroutine(BorderAnimated());
        }
    }
    
    private void Update()
    {
        Animate();
    }
    IEnumerator BorderAnimated()
    {
        while (true)
        {
            yield return null;
            while (!animated)
            {
                Debug.Log($"coroutine exist");
                yield return new WaitForSecondsRealtime(0.2f);
                
            }            
        }
    }

    void Animate()
    {
        timer += Time.unscaledDeltaTime;
        border.fillAmount = timer / maxTimer;
        if(timer >= maxTimer)
        {
            timer = 0;
        }
    }
}
