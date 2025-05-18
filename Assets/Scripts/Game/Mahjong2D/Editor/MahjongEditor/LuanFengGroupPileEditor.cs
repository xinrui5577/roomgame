/** 
 *文件名称:     LuanFengGroupPileEditor.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2019-03-05 
 *描述:         乱风牌组处理Editor
 *历史记录: 
*/
using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using UnityEditor;

namespace Assets.Scripts.Game.Mahjong2D.Editor.MahjongEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LuanFengGroupPile))]
    public class LuanFengGroupPileEditor : GroupPileEditor
    {

    }
}
