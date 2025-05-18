/** 
 *文件名称:     OutPileEditor.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-20 
 *描述:         打牌处理
 *历史记录: 
*/

using Assets.Scripts.Game.lyzz2d.Game.Item;
using Assets.Scripts.Game.lyzz2d.Utils.UI;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OutPile))]
    public class OutPileEditor : MahjongPileEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            OutPile outPile=target as OutPile;
            ThrowOutCard trans =EditorGUILayout.ObjectField(new GUIContent("打出的那张牌所在"), outPile.OutCard, typeof (ThrowOutCard), true) as ThrowOutCard;
            MahjongItem item = EditorGUILayout.ObjectField(new GUIContent("最后打出的牌："), outPile.OutCard.ThrowCard, typeof(MahjongItem), true) as MahjongItem;
            if (!trans.Equals(outPile.OutCard))
            {
                outPile.OutCard = trans;
            }
            float scaleX = EditorGUILayout.FloatField("打出牌横向倍数:", outPile.OutCardScaleX);
            float scaleY = EditorGUILayout.FloatField("打出牌纵向倍数:", outPile.OutCardScaleY);
            
            if (!outPile.OutCardScaleX.Equals(scaleX))
            {
                outPile.OutCardScaleX = scaleX;
            }
            if (!outPile.OutCardScaleY.Equals(scaleY))
            {
                outPile.OutCardScaleY = scaleY;
            }
            if (item==null)
            {
                outPile.OutCard.ThrowCard = null;
            }
            else if (!item.Equals(outPile.OutCard.ThrowCard))
            {
                outPile.OutCard.ThrowCard = item;
            }
            serializedObject.ApplyModifiedProperties();
            
        }
    }
}
 