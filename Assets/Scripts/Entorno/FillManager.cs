using UnityEngine;

public class FillManager : MonoBehaviour
{
    public float fillWidth;

    public Color fillColour, defaultColour;
    void Update()
    {
        if(gameObject.transform.position.x < fillWidth)
        {
            gameObject.GetComponent<Renderer>().material.color = fillColour;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = defaultColour;
        }
    }
}
