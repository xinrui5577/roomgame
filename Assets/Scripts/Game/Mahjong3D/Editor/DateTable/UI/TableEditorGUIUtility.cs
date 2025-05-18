using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    public static class TableEditorGUIUtility
    {
        public static float WidthPerIndent { get { return 15.0f; } }
        
        public static Rect GetLabelRect(Rect controlRect)
        {
            var indentCorrection = EditorGUI.indentLevel * WidthPerIndent;
            return new Rect(controlRect.x, controlRect.y, EditorGUIUtility.labelWidth - indentCorrection, controlRect.height);
        }
    }
}
