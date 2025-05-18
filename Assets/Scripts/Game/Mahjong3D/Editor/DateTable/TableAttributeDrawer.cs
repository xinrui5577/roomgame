using UnityEngine;
using UnityEditor;
using System.Reflection;
using Assets.Scripts.Game.Mahjong3D.Standard;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    [CustomPropertyDrawer(typeof(TableAttribute))]
    public class TableAttributeDrawer : PropertyDrawer
    {
        private bool mshowTable = true;
        private const float mLabelWidth = 80.0f;
        private const float mPrefixLabelWidth = 40.0f;
        private readonly float mSingleHeight = EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TableAttribute tableAtt = attribute as TableAttribute;
            SerializedProperty tableRow = property.FindPropertyRelative("Rows");

            using (var verticalScope = new TableEditorGUILayout.VerticalScope(position))
            {
                using (var headerScope = new TableEditorGUILayout.HorizontalScope(verticalScope.GetSingleLineRect()))
                {
                    mshowTable = EditorGUI.Foldout(headerScope.GetRectRatio(0.8f), mshowTable, label, true);
                    EditorGUI.LabelField(headerScope.GetRect(mPrefixLabelWidth), new GUIContent("Size"));
                    tableRow.arraySize = EditorGUI.DelayedIntField(headerScope.GetRemainingRect(), GUIContent.none, tableRow.arraySize);
                }

                if (mshowTable)
                {
                    var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                    var list = DataTableEditorGUI.GetReorderableList(GetHashCode(), property.serializedObject,
                                                                tableRow,
                                                                tableAtt.RowType,
                                                                tableAtt.RowType.GetFields(bindingFlags));
                    list.DoList(verticalScope.GetRect(verticalScope.RemainingHeight - EditorGUIUtility.singleLineHeight * 2.0f));
                }
            }
        }

        /// <summary>
        /// 重写这个方法，以指定这个字段的GUI在像素中有多高。
        /// </summary> 
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (mshowTable)
            {
                int rowCount = property.FindPropertyRelative("Rows").arraySize;              
                rowCount = rowCount == 0 ? 1 : rowCount;
                float totalHeight = mSingleHeight
                                    + DataTableEditorGUI.ListHeaderHeight
                                    + (DataTableEditorGUI.ListElementHeight * rowCount)
                                    + DataTableEditorGUI.ListFooterHeight
                                    + 10.0f;
                return totalHeight;
            }
            else
            {
                return mSingleHeight;
            }
        }
    }
}