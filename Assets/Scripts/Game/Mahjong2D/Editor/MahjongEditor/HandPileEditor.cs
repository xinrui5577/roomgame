/** 
 *文件名称:     HandPileEditor.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-20 
 *描述:         手牌Editor
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Editor.MahjongEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HandPile), true)]
    public class HandPileEditor : MahjongPileEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            HandPile _pile = target as HandPile;
            GetInCard getIn = EditorGUILayout.ObjectField(new GUIContent("新获得的手牌所在:"), _pile.NewCard,typeof (GetInCard), false) as GetInCard;
            if (getIn!=null&&!getIn.Equals(_pile.NewCard))
            {
                _pile.NewCard = getIn;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
