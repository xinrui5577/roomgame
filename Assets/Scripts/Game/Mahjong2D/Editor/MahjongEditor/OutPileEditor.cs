/** 
 *文件名称:     OutPileEditor.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-20 
 *描述:         打牌处理
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Editor.MahjongEditor
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
            if (!trans.Equals(outPile.OutCard))
            {
                outPile.OutCard = trans;
            }
            float scaleX = EditorGUILayout.FloatField("打出牌横向倍数:", outPile.OutCardScaleX);
            float scaleY = EditorGUILayout.FloatField("打出牌纵向倍数:", outPile.OutCardScaleY);
            bool isFlyShowVoice = EditorGUILayout.Toggle("是否在飞到显示的位置显示声音:", outPile.FlyShowVoice);
            bool isDownShowVoice = EditorGUILayout.Toggle("是否在落到牌堆时显示声音:", outPile.DownShowVoice);
            bool isDirectLie= EditorGUILayout.Toggle("打出牌后是否直接放倒:", outPile.DirectLie);
            Vector3 twPos = EditorGUILayout.Vector3Field("二人场打出牌堆本地位置:",outPile.TwoPeoplePos);
            if (!outPile.OutCardScaleX.Equals(scaleX))
            {
                outPile.OutCardScaleX = scaleX;
            }
            if (!outPile.OutCardScaleY.Equals(scaleY))
            {
                outPile.OutCardScaleY = scaleY;
            }
            if (!outPile.FlyShowVoice.Equals(isFlyShowVoice))
            {
                outPile.FlyShowVoice = isFlyShowVoice;
            }
            if (!outPile.DownShowVoice.Equals(isDownShowVoice))
            {
                outPile.DownShowVoice = isDownShowVoice;
            }
            if (!outPile.DirectLie.Equals(isDirectLie))
            {
                outPile.DirectLie = isDirectLie;
            }
            if (!outPile.TwoPeoplePos.Equals(twPos))
            {
                outPile.TwoPeoplePos = twPos;
            }
            serializedObject.ApplyModifiedProperties();
            
        }
    }
}
