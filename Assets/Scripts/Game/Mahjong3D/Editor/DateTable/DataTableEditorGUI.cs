using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    public class DataTableEditorGUI
    {
        /// <summary>
        /// 列表头的高度。
        /// </summary>
        public const float ListHeaderHeight = 18.0f;
        /// <summary>
        /// 列表元素的高度。
        /// </summary>      
        public const float ListElementHeight = 18.0f;
        /// <summary>
        /// 列表页脚的高度。
        /// </summary>
        public const float ListFooterHeight = 13.0f;
        /// <summary>
        /// 索引的宽度。
        /// </summary>
        public const float IndexWidth = 45.0f;
        /// <summary>
        /// 填充的内容。
        /// </summary>     
        public const float ContentPaddingRight = 16.0f;

        private static Dictionary<int, ReorderableList> _reorderableListStorage = new Dictionary<int, ReorderableList>();

        public static ReorderableList GetReorderableList(int drawerHashCode, SerializedObject serializedObject, SerializedProperty elements, Type elementType, FieldInfo[] fieldsInfo = null)
        {
            if (_reorderableListStorage.ContainsKey(drawerHashCode))
            {
                _reorderableListStorage[drawerHashCode].serializedProperty = elements;
                return _reorderableListStorage[drawerHashCode];
            }
            else
            {
                _reorderableListStorage.Add(drawerHashCode, CreateReorderableList(serializedObject, elements, elementType, fieldsInfo));
                return _reorderableListStorage[drawerHashCode];
            }
        }

        /// <summary>
        /// 创建可重新排序的列表
        /// </summary>
        private static ReorderableList CreateReorderableList(SerializedObject serializedObject, SerializedProperty elements, Type elementType, FieldInfo[] fieldsInfo)
        {
            ReorderableList reorderableList = new ReorderableList(serializedObject, elements)
            {
                headerHeight = ListHeaderHeight,
                elementHeight = ListElementHeight,
                footerHeight = ListFooterHeight
            };

            reorderableList.drawHeaderCallback += rect => DrawHeader_Table(rect, elements, fieldsInfo);
            reorderableList.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                DrawElement_Table(rect, index, isActive, isFocused, reorderableList, fieldsInfo);
            };
            return reorderableList;
        }

        private static void DrawHeader_Table(Rect rect, SerializedProperty elements, FieldInfo[] fieldsInfo)
        {
            if (fieldsInfo.Length == 0)
            {
                Debug.LogWarning("Field names are missing.");
                return;
            }

            using (var headerScope = new TableEditorGUILayout.HorizontalScope(rect))
            {
                TableEditorGUI.CenteredLabelField(headerScope.GetRect(IndexWidth), "Index", Color.clear);

                float headerFieldWidth = (headerScope.RemainingWidth - ContentPaddingRight) / fieldsInfo.Length;

                for (int i = 0; i < fieldsInfo.Length; i++)
                {
                    if (GUI.Button(headerScope.GetRect(headerFieldWidth), fieldsInfo[i].Name, TableEditorStyles.ToolbarButton))
                    {
                    }
                }
            }
        }

        private static void DrawElement_Table(Rect rect, int index, bool isActive, bool isFocused, ReorderableList reorderableList, FieldInfo[] fieldsInfo)
        {
            if (index >= reorderableList.count)
            {
                return;
            }
            rect.height = EditorGUIUtility.singleLineHeight;
            using (var elementScope = new TableEditorGUILayout.HorizontalScope(rect))
            {
                float prefixLabelWidth = IndexWidth - TableEditorGUIUtility.WidthPerIndent;
                EditorGUI.HandlePrefixLabel(elementScope.GetScope(), elementScope.GetRect(prefixLabelWidth), new GUIContent(index.ToString()));
                float columnWidth = (elementScope.RemainingWidth - ContentPaddingRight) / fieldsInfo.Length;
                for (int i = 0; i < fieldsInfo.Length; i++)
                {
                    if (SupportTypeChecker.CheckTypeIsSupported(fieldsInfo[i].FieldType))
                    {
                        EditorGUI.PropertyField(elementScope.GetRect(columnWidth),
                            reorderableList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative(fieldsInfo[i].Name), GUIContent.none);
                    }
                    else
                    {
                        SupportTypeChecker.DrawNotSupportedLabelGUI(elementScope.GetRect(rect.width - prefixLabelWidth), fieldsInfo[i].FieldType);
                    }
                }
                if (GUI.Button(elementScope.GetRemainingRect(), TableEditorStyles.ToolbarMinus, TableEditorStyles.RL_Element))
                {
                    reorderableList.serializedProperty.DeleteArrayElementAtIndex(index);
                }
            }
        }
    }
}
