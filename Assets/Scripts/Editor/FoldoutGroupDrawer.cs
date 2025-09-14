using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FoldoutGroupAttribute))]
public class FoldoutGroupDrawer : PropertyDrawer
{
    private static readonly Dictionary<string, bool> foldoutStates = new();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (FoldoutGroupAttribute)attribute;

        if (!foldoutStates.ContainsKey(attr.GroupName))
            foldoutStates[attr.GroupName] = attr.Expanded;

        bool isFirst = IsFirstInGroup(property, attr.GroupName);

        if (isFirst)
        {
            // Dibujar solo el foldout en la primera propiedad
            foldoutStates[attr.GroupName] = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            foldoutStates[attr.GroupName],
            new GUIContent(attr.GroupName),
            true);
        }

        // Dibujar todos los campos si el grupo está abierto
        if (foldoutStates[attr.GroupName])
        {
            // Calcular el rect debajo del foldout para este campo
            float yOffset = isFirst ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing : 0f;
            Rect fieldRect = new Rect(
                position.x,
                position.y + yOffset,
                position.width,
                EditorGUI.GetPropertyHeight(property, label, true)
            );

            EditorGUI.PropertyField(fieldRect, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr = (FoldoutGroupAttribute)attribute;

        if (!foldoutStates.ContainsKey(attr.GroupName))
            foldoutStates[attr.GroupName] = attr.Expanded;

        bool isFirst = IsFirstInGroup(property, attr.GroupName);

        if (!foldoutStates[attr.GroupName])
        {
            // Cuando está cerrado, solo ocupa el alto del foldout (en la primera propiedad)
            return isFirst ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing : 0f;
        }

        // Si está abierto:
        return EditorGUI.GetPropertyHeight(property, label, true) +
               (isFirst ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing : 0f);
    }

    private bool IsFirstInGroup(SerializedProperty property, string groupName)
    {
        var iterator = property.serializedObject.GetIterator();
        if (iterator.NextVisible(true))
        {
            do
            {
                var field = property.serializedObject.targetObject.GetType()
                    .GetField(iterator.name,
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

                if (field == null) continue;

                var attrs = field.GetCustomAttributes(typeof(FoldoutGroupAttribute), false);
                if (attrs.Length > 0 && ((FoldoutGroupAttribute)attrs[0]).GroupName == groupName)
                {
                    return iterator.name == property.name;
                }
            }
            while (iterator.NextVisible(false));
        }
        return true;
    }
}
