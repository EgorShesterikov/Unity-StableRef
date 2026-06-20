#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utility.StableRef
{
    [CustomPropertyDrawer(typeof(StableRefSelectorAttribute))]
    public class StableRefSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.HelpBox(position, "[StableRefSelector] works only with [SerializeReference]", MessageType.Error);
                return;
            }

            bool hasValue = property.managedReferenceValue != null;
            var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            const float FoldoutW = 4f;
            
            var ev = Event.current;
            if (line.Contains(ev.mousePosition) &&
                (ev.type == EventType.ContextClick ||
                 (ev.type == EventType.MouseDown && ev.button == 1)))
            {
                StableRefContextMenu.ShowDirectMenu(property);
                ev.Use();
                return;
            }

            bool hasLabel = label != GUIContent.none && !string.IsNullOrEmpty(label.text);
            var controlLine = hasLabel
                ? EditorGUI.PrefixLabel(line, TruncatedLabel(label, EditorGUIUtility.labelWidth - 12f))
                : line;

            float btnX = hasLabel ? controlLine.x : controlLine.x + FoldoutW;
            float btnW = hasLabel ? controlLine.width : controlLine.width - FoldoutW;

            bool hasChildren = hasValue && HasVisibleChildren(property);
            if (hasChildren)
            {
                float foldoutX = hasLabel ? controlLine.x - 4f : controlLine.x;
                property.isExpanded = EditorGUI.Foldout(
                    new Rect(foldoutX, controlLine.y, FoldoutW, controlLine.height), property.isExpanded, GUIContent.none, true);
            }

            var btnRect = new Rect(btnX, controlLine.y, btnW, controlLine.height);

            GUIContent buttonLabel = StableRefHandler.BrokenLabelOverride ?? new GUIContent(GetCurrentLabel(property));
            StableRefHandler.BrokenLabelOverride = null;

            var prevColor = GUI.contentColor;
            if (StableRefHandler.BrokenColorOverride.HasValue) GUI.contentColor = StableRefHandler.BrokenColorOverride.Value;
            StableRefHandler.BrokenColorOverride = null;

            bool clicked = GUI.Button(btnRect, buttonLabel, EditorStyles.popup);
            GUI.contentColor = prevColor;
            if (clicked)
                StableRefSelectorWindow.Show(btnRect, property, StableRefPropertyUtils.GetEntries(property));

            if (hasChildren && property.isExpanded)
            {
                float yOff = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.indentLevel++;
                DrawChildren(new Rect(position.x, position.y + yOff, position.width, position.height - yOff), property);
                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
                return EditorGUIUtility.singleLineHeight;

            float h = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue != null && property.isExpanded)
            {
                var child = property.Copy();
                var end = property.GetEndProperty();
                if (child.NextVisible(true))
                    while (!SerializedProperty.EqualContents(child, end))
                    {
                        h += EditorGUI.GetPropertyHeight(child, true) + EditorGUIUtility.standardVerticalSpacing;
                        if (!child.NextVisible(false)) break;
                    }
            }

            return h;
        }

        private static bool HasVisibleChildren(SerializedProperty property)
        {
            var child = property.Copy();
            var end = property.GetEndProperty();
            return child.NextVisible(true) && !SerializedProperty.EqualContents(child, end);
        }

        private static string GetCurrentLabel(SerializedProperty property)
        {
            if (property.managedReferenceValue == null) return "None";
            var t = property.managedReferenceValue.GetType();
            var cat = t.GetCustomAttribute<StableRefCategoryAttribute>();
            return cat != null && StableRefSelectorWindow.ShowCategoryInLabel
                ? $"{cat.Category}/{t.Name}"
                : t.Name;
        }

        private static GUIContent TruncatedLabel(GUIContent label, float maxWidth)
        {
            var style = EditorStyles.label;
            if (style.CalcSize(label).x <= maxWidth) return label;
            float ellipsisW = style.CalcSize(new GUIContent("...")).x;
            var text = label.text;
            while (text.Length > 0 && style.CalcSize(new GUIContent(text)).x + ellipsisW > maxWidth)
                text = text.Substring(0, text.Length - 1);
            return new GUIContent(text + "...", label.tooltip);
        }

        private static void DrawChildren(Rect rect, SerializedProperty property)
        {
            var child = property.Copy();
            var end = property.GetEndProperty();
            float y = rect.y;
            if (!child.NextVisible(true)) return;
            while (!SerializedProperty.EqualContents(child, end))
            {
                float h = EditorGUI.GetPropertyHeight(child, true);
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, h), child, true);
                y += h + EditorGUIUtility.standardVerticalSpacing;
                if (!child.NextVisible(false)) break;
            }
        }
    }
}
#endif