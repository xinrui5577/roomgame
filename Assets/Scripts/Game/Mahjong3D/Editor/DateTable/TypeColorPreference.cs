using Assets.Scripts.Game.Mahjong3D.Standard;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    /// <summary>
    /// 类型颜色偏好设置。
    /// </summary>
    public static class TypeColorPreference
    {
        private enum ETypeCategory
        {
            /// <summary>
            /// The none
            /// </summary>
            None,
            /// <summary>
            /// The enum
            /// </summary>
            Enum,
            /// <summary>
            /// The structure
            /// </summary>
            Struct,
            /// <summary>
            /// The class
            /// </summary>
            Class,
        }

        /// <summary>
        /// Gets the current <see cref="TypeColorPreset"/>.
        /// </summary>
        /// <value>The current.</value>
        public static TypeColorPreset Current
        {
            get
            {
                if (_current)
                {
                    return _current;
                }
                else
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(EditorPrefs.GetString("TypeColorPreset", string.Empty));
                    _current = AssetDatabase.LoadAssetAtPath<TypeColorPreset>(assetPath);
                    return _current;
                }
            }
        }

        private static TypeColorPreset _current;
        /// <summary>
        /// 类型色预置编辑器
        /// </summary>
        private static UnityEditor.Editor _typeColorPresetEditor;
        /// <summary>
        /// 类型颜色预置唯一标识符
        /// </summary>
        private static string _typeColorPresetGuid;      

        /// <summary>
        ///得到类型的颜色。
        /// </summary> 
        public static Color GetTypeColor(Type type)
        {
            if (Current)
            {
                Color typeColor;
                if (SearchTypeColor(type, ETypeCategory.None, out typeColor))
                {
                    return typeColor;
                }

                if (type.IsEnum
                    && SearchTypeColor(type, ETypeCategory.Enum, out typeColor))
                {
                    return typeColor;
                }
                else if (type.IsValueType
                         && SearchTypeColor(type, ETypeCategory.Struct, out typeColor))
                {
                    return typeColor;
                }
                else if (type.IsClass
                         && SearchTypeColor(type, ETypeCategory.Class, out typeColor))
                {
                    return typeColor;
                }

                return Color.white;
            }
            else
            {
                return Color.white;
            }
        }

        private static bool SearchTypeColor(Type type, ETypeCategory category, out Color typeColor)
        {
            string matchTypeName;
            switch (category)
            {
                case ETypeCategory.None:
                    matchTypeName = type.Name;
                    break;
                case ETypeCategory.Enum:
                    matchTypeName = "Enum";
                    break;
                case ETypeCategory.Struct:
                    matchTypeName = "Struct";
                    break;
                case ETypeCategory.Class:
                    matchTypeName = "Class";
                    break;
                default:
                    typeColor = Color.white;
                    return false;
            }

            for (int i = 0; i < Current.TypeColorList.Count; i++)
            {
                if (string.Compare(Current.TypeColorList[i].TypeName, matchTypeName) == 0)
                {
                    typeColor = Current.TypeColorList[i].TypeColor;
                    return true;
                }
            }

            typeColor = Color.white;
            return false;
        }

    }    
}
