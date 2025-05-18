/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using UnityEngine;

namespace BestHTTP.Examples
{
    public static class GUIHelper
    {
        private static GUIStyle centerAlignedLabel;
        private static GUIStyle rightAlignedLabel;

        public static Rect ClientArea;

        private static void Setup()
        {
            // These has to be called from OnGUI
            if (centerAlignedLabel == null)
            {
                centerAlignedLabel = new GUIStyle(GUI.skin.label);
                centerAlignedLabel.alignment = TextAnchor.MiddleCenter;

                rightAlignedLabel = new GUIStyle(GUI.skin.label);
                rightAlignedLabel.alignment = TextAnchor.MiddleRight;
            }
        }

        public static void DrawArea(Rect area, bool drawHeader, Action action)
        {
            Setup();

            // Draw background
            GUI.Box(area, string.Empty);
            GUILayout.BeginArea(area);

            if (drawHeader)
            {
                GUIHelper.DrawCenteredText(SampleSelector.SelectedSample.DisplayName);
                GUILayout.Space(5);
            }

            if (action != null)
                action();

            GUILayout.EndArea();
        }

        public static void DrawCenteredText(string msg)
        {
            Setup();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(msg, centerAlignedLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void DrawRow(string key, string value)
        {
            Setup();

            GUILayout.BeginHorizontal();
            GUILayout.Label(key);
            GUILayout.FlexibleSpace();
            GUILayout.Label(value, rightAlignedLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }

    public class GUIMessageList
    {
        System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
        Vector2 scrollPos;

        public void Draw()
        {
            Draw(Screen.width, 0);
        }

        public void Draw(float minWidth, float minHeight)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinHeight(minHeight));
            for (int i = 0; i < messages.Count; ++i)
                GUILayout.Label(messages[i], GUILayout.MinWidth(minWidth));
            GUILayout.EndScrollView();
        }

        public void Add(string msg)
        {
            messages.Add(msg);
            scrollPos = new Vector2(scrollPos.x, float.MaxValue);
        }

        public void Clear()
        {
            messages.Clear();
        }
    }
}
