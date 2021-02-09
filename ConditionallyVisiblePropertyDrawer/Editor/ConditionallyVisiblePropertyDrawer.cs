using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityToolbag
{
    [CustomPropertyDrawer(typeof(ConditionallyVisibleAttribute))]
    public class ConditionallyVisiblePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldDisplay(property))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUI.PropertyField(position, property, label, includeChildren: true);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldDisplay(property) ? EditorGUI.GetPropertyHeight(property, label) : 0;
        }

        private static bool ShouldDisplay(SerializedProperty property)
        {
            var obj = property.serializedObject.targetObject;
            var field = obj.GetType().GetField(
                property.name, 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            var attr = field.GetCustomAttributes(typeof(ConditionallyVisibleAttribute)).First() as ConditionallyVisibleAttribute;
            var dependentProp = property.serializedObject.FindProperty(attr.propertyName);
            return dependentProp.boolValue;
        }
    }
}