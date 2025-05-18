/** 
 *文件名称:     MahjongPileEditor.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-18 
 *描述:         麻将堆编辑器
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using Assets.Scripts.Game.Mahjong2D.Common.UI;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Editor.MahjongEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MahjongPile), true)]
    public class MahjongPileEditor : UIWidgetContainerEditor
    {
        protected DefLayout _layout;
        protected MahjongPile _pile;
        public override void OnInspectorGUI()
        {
            _pile=target as MahjongPile;
            EnumMahJongDirection direction = (EnumMahJongDirection)EditorGUILayout.EnumPopup("麻将方向", _pile.ItemDirection);
            EnumMahJongAction action = (EnumMahJongAction)EditorGUILayout.EnumPopup("麻将动作", _pile.ItemAction);
            EnumShowDirection show = (EnumShowDirection) EditorGUILayout.EnumPopup("麻将所属人", _pile.ItemShow);
            int baseLayer = EditorGUILayout.IntField("基础层级", _pile.BaseLayer);
            bool isAddLayer = EditorGUILayout.Toggle("层级是否递增", _pile.IsLayerAdd);
            float scaleX = EditorGUILayout.FloatField("X轴缩放",_pile.ItemScaleX);
            float scaleY = EditorGUILayout.FloatField("Y轴缩放", _pile.ItemScaleY);
            DefLayout layout = EditorGUILayout.ObjectField(new GUIContent("布局文件Layout"), _pile.Layout, typeof(DefLayout), true) as DefLayout;
            if (!direction.Equals(_pile.ItemDirection))
            {
                _pile.ItemDirection = direction;
            }
            if (!action.Equals(_pile.ItemAction))
            {
                _pile.ItemAction = action;
            }
            if(!show.Equals(_pile.ItemShow))
            {
                _pile.ItemShow = show;
            }
            if(!baseLayer.Equals(_pile.BaseLayer))
            {
                _pile.BaseLayer = baseLayer;
            }
            if(!isAddLayer.Equals(_pile.IsLayerAdd))
            {
                _pile.IsLayerAdd = isAddLayer;
            }
            if(!scaleX.Equals(_pile.ItemScaleX))
            {
                _pile.ItemScaleX = scaleX;
            }
            if(!scaleY.Equals(_pile.ItemScaleY))
            {
                _pile.ItemScaleY = scaleY;
            }
            if (layout!=null)
            {
                if (!layout.Equals(_pile.Layout))
                {
                    _pile.Layout = layout;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
