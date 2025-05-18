/** 
 *文件名称:     MahjongItemEditor.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-02 
 *描述:         麻将显示处理
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Editor.MahjongEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MahjongItem), true)]
    public class MahjongItemEditor : UIWidgetContainerEditor
    {
        private MahjongItem _item;
        private bool _valueChange;
        public override void OnInspectorGUI()
        {     
            _item=target as MahjongItem;
            _valueChange = false;
            EnumMahjongValue value = (EnumMahjongValue)EditorGUILayout.EnumPopup("麻将值", _item.SelfData.Value);
            EnumMahJongDirection direction = (EnumMahJongDirection)EditorGUILayout.EnumPopup("麻将方向", _item.SelfData.Direction);
            EnumMahJongAction action = (EnumMahJongAction)EditorGUILayout.EnumPopup("麻将动作", _item.SelfData.Action);
            EnumShowDirection showDirection=(EnumShowDirection)EditorGUILayout.EnumPopup("麻将实际显示方向", _item.SelfData.ShowDirection);
            int mahjongLayer=EditorGUILayout.IntField("基础层级", _item.SelfData.MahjongLayer);
            bool isLockValueType = EditorGUILayout.Toggle("是否锁定麻将值类型", _item.SelfData.LockValueType);
            if (!mahjongLayer.Equals(_item.SelfData.MahjongLayer))
            {
                _item.SelfData.MahjongLayer = mahjongLayer;
                _valueChange = true;
            }
            if (!_item.SelfData.ShowDirection.Equals(showDirection))
            {
                _item.SelfData.ShowDirection = showDirection;
                _valueChange = true;
            }
            if (!_item.SelfData.Value.Equals(value))
            {
                _item.SelfData.Value = value;
                _valueChange = true;
            }
            if (!_item.SelfData.Direction.Equals(direction))
            {
                _item.SelfData.Direction = direction;
                _valueChange = true;
            }
            if (!_item.SelfData.Action.Equals(action))
            {
                _item.SelfData.Action = action;
                _valueChange = true;
            }
            if (showDirection.Equals(EnumShowDirection.Oppset))
            {
                bool oppset = EditorGUILayout.Toggle("是否需要倒转牌的值", _item.SelfData.NeedOppsetValue);
                if (!_item.SelfData.NeedOppsetValue.Equals(oppset))
                {
                    _item.SelfData.NeedOppsetValue = oppset;
                    _valueChange = true;
                }
            }
            if (_valueChange)
            {
                _item.OnDataChange();
            }
            if (!isLockValueType.Equals(_item.SelfData.LockValueType))
            {
                _item.SelfData.LockValueType = isLockValueType;
            }
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
