#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


[CustomEditor(typeof(TagContainer))]
public class TagContainerEditor : Editor
{
    TagContainer tc;
    SerializedProperty tagsProp;
    TagDatabase db;

    void OnEnable()
    {
        tc = (TagContainer)target;
        tagsProp = serializedObject.FindProperty("tags");

        // intenta cargar la DB automáticamente
        var guids = AssetDatabase.FindAssets("t:TagDatabase");
        if (guids.Length > 0)
            db = AssetDatabase.LoadAssetAtPath<TagDatabase>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(tagsProp, true);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add existed tag"))
        {
            ShowAddTagMenu();
        }
        if (GUILayout.Button("delete all"))
        {
            tagsProp.ClearArray();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    void ShowAddTagMenu()
    {
        if (db == null || db.tags == null || db.tags.Length == 0)
        {
            EditorUtility.DisplayDialog("Tag Database", "Doesnt find TagDatabase or has any tags.", "OK");
            return;
        }

        GenericMenu menu = new GenericMenu();
        foreach (var tag in db.tags)
        {
            if (!tc.tags.Contains(tag))
            {
                var closure = tag;
                menu.AddItem(new GUIContent(tag.tagName), false, () =>
                {
                    Undo.RecordObject(tc, "Add Tag");
                    tc.tags.Add(closure);
                    EditorUtility.SetDirty(tc);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(tag.tagName + " (added)"));
            }
        }
        menu.ShowAsContext();
    }

}
#endif