using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TagDatabase", menuName = "Scriptable Objects/TagDatabase")]
public class TagDatabase : ScriptableObject
{
    public TagSO[] tags;

    private static TagDatabase _instance;
    public static TagDatabase Instance
    {
        get
        {
           #if UNITY_EDITOR
            if (_instance == null)
            {
                // busca en assets
                var guids = AssetDatabase.FindAssets("t:TagDatabase");
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    _instance = AssetDatabase.LoadAssetAtPath<TagDatabase>(path);
                }
            }
           #endif
           return _instance;
        }
        set => _instance = value;
    }


}

