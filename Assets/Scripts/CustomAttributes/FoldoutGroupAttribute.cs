using UnityEngine;

public class FoldoutGroupAttribute : PropertyAttribute
{
    public string GroupName;
    public bool Expanded;

    public FoldoutGroupAttribute(string groupName, bool expanded = true)
    {
        GroupName = groupName;
        Expanded = expanded;
    }
}


