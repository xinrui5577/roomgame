/** 
 *文件名称:     ShenYangGroupPile.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-06-29 
 *描述:         沈阳麻将组牌的特殊处理
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using UnityEditor;

namespace Assets.Scripts.Game.Mahjong2D.Editor.MahjongEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ShenYangGroupPile))]
    public class ShenYangGroupPileEditor : GroupPileEditor
    {
    }
}
