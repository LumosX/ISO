// Initially developed by Tom Kail at Inkle, with modifications by Luiz Wendt
// Ultimately fixed, adapted, and expanded by Lumos of ZeroVector
// Released under the MIT Licence as held at https://opensource.org/licenses/MIT

// Must be placed within a folder named "Editor"

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace ZeroVector.Common.ISO {

    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    /// <summary>
    /// Objects inheriting from InlineScriptableObject will be drawn in the inspector as "inlined", allowing you to
    /// display and edit their values directly under the object reference. In addition, this provides a button to
    /// create a new ScriptableObject if property is null.
    /// The same effects can be obtained for any ScriptableObject by attaching the "DrawInline" attribute to it.
    /// </summary>
    [CustomPropertyDrawer(typeof(DrawInlineAttribute))]
    [CustomPropertyDrawer(typeof(InlineScriptableObject), true)]
    public class InlineScriptableObjectDrawer : PropertyDrawer{

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var totalHeight = EditorGUIUtility.singleLineHeight + (property.isExpanded ? 5 : 0);
            
            // Lumos: Add some more free space at the top to make things cleaner
            totalHeight += EditorGUIUtility.standardVerticalSpacing;
            
            if (!AnyPropertyVisible(property) || !property.isExpanded)
                return totalHeight;

            var data = property.objectReferenceValue as ScriptableObject;
            if (data == null) return EditorGUIUtility.singleLineHeight;
            var serializedObject = new SerializedObject(data);
            var prop = serializedObject.GetIterator();
            if (prop.NextVisible(true)) {
                do {
                    if (prop.name == "m_Script") continue;
                    var subProp = serializedObject.FindProperty(prop.name);
                    var height = EditorGUI.GetPropertyHeight(subProp, null, true) +
                                 EditorGUIUtility.standardVerticalSpacing;
                    totalHeight += height;
                } while (prop.NextVisible(false));
            }

            // Add a tiny bit of height if open for the background
            totalHeight += EditorGUIUtility.standardVerticalSpacing;
            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            
            // Lumos: check type and attribute, and use defaults if none are present.
            var propertyObject = GetTargetObjectOfProperty(property);
            var isAttribute = attribute is DrawInlineAttribute && 
                              (propertyObject is ScriptableObject || propertyObject is null);
            var isInline = propertyObject is InlineScriptableObject || propertyObject is null;

            if (!isAttribute && !isInline) {
                EditorGUI.LabelField(position, "Error: Must derive from (Inline)ScriptableObject");
                EditorGUI.EndProperty();
                return;
            }

            var attr = isAttribute ? (DrawInlineAttribute)attribute : new DrawInlineAttribute();

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var labelW = EditorGUIUtility.labelWidth;
            var vertSep = EditorGUIUtility.standardVerticalSpacing;

            // When unassigned, show a "Create" button and allow assignment
            if (property.objectReferenceValue == null) {
                var nameLabel = attr.showName
                    ? string.IsNullOrEmpty(attr.nameOverride) ? null : new GUIContent(attr.nameOverride)
                    : GUIContent.none;
                EditorGUI.ObjectField(new Rect(position.x, position.y, position.width - 60, lineHeight), 
                    property, nameLabel);
                
                if (GUI.Button(new Rect(position.x + position.width - 58, position.y, 58, lineHeight), "Create")) {
                    var selectedAssetPath = "Assets";
                    var targetObj = property.serializedObject.targetObject;
                    if (targetObj is MonoBehaviour || targetObj is ScriptableObject) {
                        var scriptFile = targetObj is MonoBehaviour obj 
                            ? MonoScript.FromMonoBehaviour(obj)
                            : MonoScript.FromScriptableObject(targetObj as ScriptableObject);
                        selectedAssetPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(scriptFile));
                    }

                    var type = fieldInfo.FieldType;
                    if (type.IsArray) type = type.GetElementType();
                    else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                        type = type.GetGenericArguments()[0];
                    property.objectReferenceValue = CreateAssetWithSavePrompt(type, selectedAssetPath);
                }
            }
            // Otherwise, make foldouts and so on.
            else {
                // Lumos: This isn't how you make a foldout. By making a labelled foldout and then adjusting the width
                // of the property fields, you're making the field more indented than when there's no object assigned...
                // The initial version of this whole thing is also an example of bad design: sure, it works in the one
                // use case it's been tested in, but relying on x = 0 and screen.width completely breaks this when
                // it's being nested in other stuff, for instance.
                
                // This is the only information the foldout needs if you're making a field anyway.
                var foldoutRect = new Rect(position.x, position.y, labelW, lineHeight);
                property.isExpanded = AnyPropertyVisible(property) &&
                                      EditorGUI.Foldout(foldoutRect, property.isExpanded, "", true);
                
                var propRect = new Rect(position.x, position.y, position.width, lineHeight);
                var labelName = attr.showName
                    ? string.IsNullOrEmpty(attr.nameOverride) ? null : new GUIContent(attr.nameOverride)
                    : GUIContent.none;
                EditorGUI.PropertyField(propRect, property, labelName, true);

                if (GUI.changed) property.serializedObject.ApplyModifiedProperties();
                if (property.objectReferenceValue == null) GUIUtility.ExitGUI();

                if (property.isExpanded) {
                    var baseHeight = GetPropertyHeight(property, label); // Lumos: position.height is unreliable when nested
                    // Draw a background that shows us clearly which fields are part of the ScriptableObject
                    var boxRect = new Rect(position.x - 4, position.y + lineHeight + vertSep - 1, position.width + 8,
                        baseHeight - lineHeight - vertSep - 1);
                    GUI.Box(boxRect, "");

                    EditorGUI.indentLevel++;
                    var data = (ScriptableObject)property.objectReferenceValue;
                    var serializedObject = new SerializedObject(data);
                    
                    // Iterate over all the values and draw them
                    var prop = serializedObject.GetIterator();
                    var y = position.y + lineHeight + vertSep * 2;
                    if (prop.NextVisible(true)) {
                        do {
                            // Don't bother drawing the class file
                            if (prop.name == "m_Script") continue;
                            var height = EditorGUI.GetPropertyHeight(prop, new GUIContent(prop.displayName), true);
                            EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), prop, true);
                            y += height + vertSep;
                        } while (prop.NextVisible(false));
                    }

                    if (GUI.changed)
                        serializedObject.ApplyModifiedProperties();

                    EditorGUI.indentLevel--;
                }
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        // Creates a new ScriptableObject via the default Save File panel
        private static ScriptableObject CreateAssetWithSavePrompt(Type type, string path) {
            path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", "New " + type.Name + ".asset", "asset",
                "Enter a file name for the ScriptableObject.", path);
            if (path == "") return null;
            var asset = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            EditorGUIUtility.PingObject(asset);
            return asset;
        }
        
        private static bool AnyPropertyVisible(SerializedProperty property) {
            // Lumos: Since the attribute can be applied to everything, we must check the prop type
            var isCorrectReference = property.propertyType == SerializedPropertyType.ObjectReference;
            var data = isCorrectReference ? (ScriptableObject)property.objectReferenceValue : null;
            if (data == null) return false; // Lumos: This was throwing exceptions if not checked for null

            var serializedObject = new SerializedObject(data);

            var prop = serializedObject.GetIterator();

            // Ignore m_Script when looking for visible props
            while (prop.NextVisible(true)) {
                if (prop.name == "m_Script") continue;
                return true;
            }

            return false;
        }
        
        
        // HELPER METHODS BELOW RETRIEVED FROM https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs
        // BASED ON POST https://forum.unity.com/threads/get-a-general-object-value-from-serializedproperty.327098/#post-2309545
        // AND ADJUSTED AFTERWARDS.

        /// <summary>
        /// Gets the object the property represents.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private static object GetTargetObjectOfProperty(SerializedProperty prop) {
            if (prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements) {
                if (element.Contains("[")) {
                    var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal))
                        .Replace("[", "")
                        .Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }


        private static object GetValue_Imp(object source, string name) {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null) {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }


        private static object GetValue_Imp(object source, string name, int index) {
            if (!(GetValue_Imp(source, name) is IEnumerable enumerable)) return null;
            var enm = enumerable.GetEnumerator();

            for (var i = 0; i <= index; i++) {
                if (!enm.MoveNext()) return null;
            }

            return enm.Current;
        }
    }
}
