using System;
using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    public static class SupportTypeChecker
    {     
        // 不支持类型的集合。    
        private static Type[] _notSupportingTypes = new Type[]
        {
            typeof(Rect),
            typeof(Bounds)
        };
      
        public static bool CheckTypeIsSupported(Type type, bool showLog = false)
        {
            for (int i = 0; i < _notSupportingTypes.Length; i++)
            {
                if (type == _notSupportingTypes[i])
                {
                    if (showLog)
                    {
                        Debug.LogWarningFormat("{0} is not supported.", type);
                    }

                    return false;
                }
            }
            return true;
        }

        public static void DrawNotSupportedLabelGUI(Rect rect, Type type)
        {
            EditorGUI.LabelField(rect, string.Format("{0} is not supported.", type.ToString()), EditorStyles.boldLabel);
        }
    }
}
