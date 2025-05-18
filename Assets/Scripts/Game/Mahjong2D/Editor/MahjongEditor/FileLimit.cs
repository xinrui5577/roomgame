using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Editor.MahjongEditor
{
    public class FileLimit 
    {
        [InitializeOnLoadMethod]
        private static void Limit()
        {
            Action OnEvent = delegate()
            {
                Event eve = Event.current;

                switch (eve.type)
                {
                    //case EventType.MouseMove:
                    //    eve.Use();
                    //    break;
                    case EventType.MouseDrag:
                        //LTools.LogSystem("red","不让你拖动，哈哈哈");
                        //eve.Use();
                        break;
                    //case EventType.DragPerform:
                    //    eve.Use();
                    //    break;
                    //case EventType.DragUpdated:
                    //    eve.Use();
                    //    break;
                    //case  EventType.DragExited:
                    //    eve.Use();
                    //    break;
                    default:
                        break;
                }

            };

            EditorApplication.hierarchyWindowItemOnGUI += delegate(int id, Rect re)
            {
                OnEvent();
            };

            EditorApplication.projectWindowItemOnGUI += delegate(string id, Rect re)
            {
                OnEvent();
            };
        }
    }
}
