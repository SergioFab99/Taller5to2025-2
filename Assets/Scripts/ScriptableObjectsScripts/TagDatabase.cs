using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;




#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TagDatabase", menuName = "Scriptable Objects/TagDatabase")]
public class TagDatabase : ScriptableObject
{
 
    [TableList(ShowIndexLabels = true)]
    [Searchable]

    public List<TagSO> tags;

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


#if UNITY_EDITOR
    [Button("Add Tag")]
    private void AddTag()
    {
        // Create a temporary, unsaved instance of TagSO
        var newTag = ScriptableObject.CreateInstance<TagSO>();
        newTag.tagName = "NewTag";
        newTag.color = Color.white;

       
        tags.Add(newTag);
        SaveTagAsset(newTag);
    }

    

    public void OnTagAdded()
    {
        for (int i = 0; i < tags.Count; i++)
        {
            var tag = tags[i];
            if (tag != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(tag)))
            {
                // If the tag exists in memory but isn’t an asset yet → save it
                SaveTagAsset(tag);

            }
        }

    }

   
    private void SaveTagAsset(TagSO tag)
    {
        string folderPath = "Assets/ScriptableObjects/Tags";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Tags");
        }

        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{tag.tagName}.asset");
        AssetDatabase.CreateAsset(tag, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
}

