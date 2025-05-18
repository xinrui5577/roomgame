using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    public static class TableEditorGUI
    {
        #region LabelFieldWithBackgroundColor
        /// <summary>
        /// LabelField用背景色
        /// </summary>
        public static void LabelFieldWithBackgroundColor(Rect position, GUIContent label, GUIContent label2, GUIStyle style, Color backgroundColor)
        {
            DrawBoxWithColor(position, backgroundColor);
            EditorGUI.LabelField(position, label, label2, style);
        }
        public static void LabelFieldWithBackgroundColor(Rect position, GUIContent label, GUIStyle style, Color backgroundColor)
        {
            DrawBoxWithColor(position, backgroundColor);
            EditorGUI.LabelField(position, label, style);
        }
        public static void LabelFieldWithBackgroundColor(Rect position, GUIContent label, Color backgroundColor)
        {
            LabelFieldWithBackgroundColor(position, label, EditorStyles.label, backgroundColor);
        }
        public static void LabelFieldWithBackgroundColor(Rect position, string label, string label2, GUIStyle style, Color backgroundColor)
        {
            LabelFieldWithBackgroundColor(position, new GUIContent(label), new GUIContent(label2), style, backgroundColor);
        }
        public static void LabelFieldWithBackgroundColor(Rect position, string label, string label2, Color backgroundColor)
        {
            LabelFieldWithBackgroundColor(position, label, label2, EditorStyles.label, backgroundColor);
        }
        public static void LabelFieldWithBackgroundColor(Rect position, string label, Color backgroundColor)
        {
            LabelFieldWithBackgroundColor(position, new GUIContent(label), backgroundColor);
        }
        #endregion

        #region CenterdLabelField
        /// <summary>
        /// 居中显示 label field
        /// </summary>
        public static void CenteredLabelField(Rect position, string label, Color backgroundColor)
        {
            CenteredLabelField(position, new GUIContent(label), backgroundColor);
        }

        public static void CenteredLabelField(Rect position, GUIContent label)
        {
            CenteredLabelField(position, label, Color.clear);
        }

        public static void CenteredLabelField(Rect position, GUIContent label, Color backgroundColor)
        {
            LabelFieldWithBackgroundColor(position, label, TableEditorStyles.MidLabel, backgroundColor);
        }

        public static void CenteredLabelField(Rect position, string label)
        {
            CenteredLabelField(position, new GUIContent(label), Color.clear);
        }

        public static void CenteredLabelField(Rect position, string label, string label2)
        {
            CenteredLabelField(position, new GUIContent(label), new GUIContent(label2), Color.clear);
        }

        public static void CenteredLabelField(Rect position, string label, string label2, Color backgroundColor)
        {
            CenteredLabelField(position, new GUIContent(label), new GUIContent(label2), backgroundColor);
        }

        public static void CenteredLabelField(Rect position, GUIContent label, GUIContent label2, Color backgroundColor)
        {
            LabelFieldWithBackgroundColor(position, label, label2, TableEditorStyles.MidLabel, backgroundColor);
        }
        #endregion

        public static void DrawBoxWithColor(Rect rect, Color color)
        {
            rect = EditorGUI.IndentedRect(rect);
            GUI.backgroundColor = color;
            GUI.Box(rect, GUIContent.none);
            GUI.backgroundColor = Color.white;
        }
    }
}
