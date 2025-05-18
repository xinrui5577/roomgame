using Assets.Scripts.Game.lyzz2d.Utils.UI;
using UnityEditor;

namespace Assets.Scripts.Game.lyzz2d.Utils.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DefLayout), true)]
    public class DefLayoutEditor : UIWidgetContainerEditor
    {
        private DefLayout _layout;

        public override void OnInspectorGUI()
        {
            _layout = target as DefLayout;
            NGUIEditorTools.DrawProperty("起始位置", serializedObject, "pivot");
            NGUIEditorTools.DrawProperty("排序方向", serializedObject, "directon");
            int width = EditorGUILayout.IntField("宽度", _layout.Width);
            int height = EditorGUILayout.IntField("高度", _layout.Height);
            int maxPerLine = EditorGUILayout.IntField(_layout.directon.Equals(DefLayout.Directon.Horizontal)?"一行最大数量": "一列最大数量", _layout.maxPerLine);
            if(maxPerLine!=_layout.maxPerLine)
            {
                _layout.maxPerLine = maxPerLine;
            }
            if (width!=_layout.Width)
            {
                _layout.Width = width;
            }
            if (height!=_layout.Height)
            {
                _layout.Height = height;
            }
            NGUIEditorTools.DrawProperty("隐藏忽略", serializedObject, "HideWhenDisable");
            _layout.SortByGroup = EditorGUILayout.Toggle("是否按组显示布局", _layout.SortByGroup);
            if (_layout.SortByGroup)
            {
                NGUIEditorTools.DrawProperty("每组数量", serializedObject, "GroupNumber");
                NGUIEditorTools.DrawProperty("组横向间距", serializedObject, "GroupCellWidth");
                NGUIEditorTools.DrawProperty("组纵向间距", serializedObject, "GroupCellHeight");
            }
            serializedObject.ApplyModifiedProperties();
        }

    }
}
