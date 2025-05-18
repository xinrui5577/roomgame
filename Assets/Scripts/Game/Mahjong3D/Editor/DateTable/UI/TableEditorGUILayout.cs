using UnityEngine;
using UnityEditor;
using System;
using Assets.Scripts.Game.Mahjong3D.Standard;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    public static class TableEditorGUILayout
    {
        public sealed class HorizontalScope : IGUILayoutScope
        {
            public float RemainingWidth { get { return mRemainingWidth; } }

            private Rect mScope;

            private Rect mLastRect;

            private float mTotalWidth;

            private float mRemainingWidth;

            public HorizontalScope(Rect scope)
            {
                mScope = scope;
                mLastRect = mScope;
                mLastRect.width = 0.0f;
                mTotalWidth = mScope.width;
                mRemainingWidth = mTotalWidth;
            }

            public HorizontalScope(Rect scope, GUIStyle style) : this(style.margin.Remove(scope))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    style.Draw(scope, false, false, true, false);
                }
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            public Rect GetLastRect()
            {
                return mLastRect;
            }

            public Rect GetRect(float width)
            {
                var current = new Rect(mLastRect.x + mLastRect.width, mScope.y, width, mScope.height);
                mLastRect = current;
                mRemainingWidth -= width;
                return current;
            }

            public Rect GetRectEven(int count)
            {
                return GetRect(mRemainingWidth / count);
            }

            public Rect GetRectFromEnd(float width)
            {
                GetRect(mRemainingWidth - width);
                return GetRect(width);
            }

            public Rect GetRectRatio(float ratio)
            {
                return GetRect(mScope.width * ratio);
            }

            public Rect GetRemainingRect()
            {
                return GetRect(mRemainingWidth);
            }

            public Rect GetScope()
            {
                return mScope;
            }

            public Rect GetLabelRect()
            {
                return GetRect(EditorGUIUtility.labelWidth - (EditorGUI.indentLevel * TableEditorGUIUtility.WidthPerIndent));
            }

            public Rect GetLabelRect(int indentLevel)
            {
                return GetRect(EditorGUIUtility.labelWidth - (indentLevel * TableEditorGUIUtility.WidthPerIndent));
            }
        }


        public sealed class VerticalScope : IGUILayoutScope
        {
            public float RemainingHeight { get { return mRemainingHeight; } }

            private Rect mScope;

            private Rect mLastRect;

            private float mTotalHeight;

            private float mRemainingHeight;

            public VerticalScope(Rect scope)
            {
                mScope = scope;
                mLastRect = mScope;
                mLastRect.height = 0.0f;
                mTotalHeight = mScope.height;
                mRemainingHeight = mTotalHeight;
            }

            public VerticalScope(Rect scope, GUIStyle style) : this(style.margin.Remove(scope))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    style.Draw(scope, false, false, true, false);
                }
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            public Rect GetLastRect()
            {
                return mLastRect;
            }

            public Rect GetRect(float height)
            {
                var current = new Rect(mScope.x, mLastRect.y + mLastRect.height, mScope.width, height);
                mLastRect = current;
                mRemainingHeight -= height;
                return current;
            }

            public Rect GetRectEven(int count)
            {
                return GetRect(mRemainingHeight / count);
            }

            public Rect GetRectFromEnd(float height)
            {
                GetRect(mRemainingHeight - height);
                return GetRect(height);
            }

            public Rect GetRectRatio(float ratio)
            {
                return GetRect(mScope.height * ratio);
            }

            public Rect GetRemainingRect()
            {
                return GetRect(mRemainingHeight);
            }

            public Rect GetScope()
            {
                return mScope;
            }

            public Rect GetSingleLineRect()
            {
                return GetRect(EditorGUIUtility.singleLineHeight);
            }
        }
    }
}
