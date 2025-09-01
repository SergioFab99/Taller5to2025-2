using UnityEngine;

[CreateAssetMenu(fileName = "TagSO", menuName = "Scriptable Objects/TagSO")]
public class TagSO : ScriptableObject
{
    public string tagName;
    public Color color = Color.white;
    public override string ToString() => tagName;

}
