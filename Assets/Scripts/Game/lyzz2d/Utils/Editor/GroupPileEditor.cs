/** 
 *文件名称:     GroupPileEditor.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-20 
 *描述:    
 *历史记录: 
*/

using Assets.Scripts.Game.lyzz2d.Utils.UI;
using UnityEditor;

namespace Assets.Scripts.Game.lyzz2d.Utils.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof (GroupPile))]
    public class GroupPileEditor : MahjongPileEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GroupPile group=target as GroupPile;
            float itemWitdh = EditorGUILayout.FloatField("组牌单位的宽度：", group.GroupItemWidth);
            float itemHeight = EditorGUILayout.FloatField("组牌单位的高度：", group.GroupItemHeight);
            int fourY=EditorGUILayout.IntField("组牌中的第四张，Y轴偏移",group.FourOffsetY);
            if (!itemHeight.Equals(group.GroupItemHeight))
            {
                group.GroupItemHeight = itemHeight;
            }
            if (!itemWitdh.Equals(group.GroupItemHeight))
            {
                group.GroupItemWidth = itemWitdh;
            }
            if(!fourY.Equals(group.FourOffsetY))
            {
                group.FourOffsetY = fourY;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
