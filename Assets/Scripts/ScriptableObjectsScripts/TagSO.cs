using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TagSO", menuName = "Scriptable Objects/TagSO")]
[InlineEditor(InlineEditorObjectFieldModes.Boxed)]
public class TagSO : ScriptableObject
{
    [Delayed]
    [OnValueChanged("OnTagNameChanged", true)]
    public string tagName;
    public Color color = Color.white;
    public override string ToString() => tagName;

#if UNITY_EDITOR

    public void OnTagNameChanged()
    {
        if (string.IsNullOrEmpty(tagName)) return;

        // solo si el asset ya está en disco
        string path = AssetDatabase.GetAssetPath(this);
        if (!string.IsNullOrEmpty(path))
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            if (fileName != tagName) // evita renombrar en bucle
            {
                AssetDatabase.RenameAsset(path, tagName);
                AssetDatabase.SaveAssets();
            }
        }
    }

    [Button("Eliminar")]
    private void DeleteSelf()
    {
        if (EditorUtility.DisplayDialog(
            "Eliminar Tag",
            $"¿Seguro que quieres eliminar '{tagName}'?",
            "Sí, eliminar",
            "Cancelar"))
        {
            string path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            ScriptableObject.DestroyImmediate(this, true);
        }
    }
#endif
}
