using UnityEngine;

/*===================================================
 *文件名称:     DbsmjPlayer.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-28
 *描述:        	调兵山麻将玩家
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.Mahjong2D.Game.Player.Dbsmj
{
    public class DbsmjPlayer : MahjongPlayer
    {
        #region UI Param
        #endregion

        #region Data Param
        #endregion

        #region Local Data
        #endregion

        #region Life Cycle
        public override void GetInCard(int value)
        {
            GetToken();
            LastGetValue = value;
            OnGetInCard(value);
        }
        #endregion

        #region Function
        #endregion
    }
}
