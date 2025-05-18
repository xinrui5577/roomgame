/*===================================================
 *文件名称:     DbsmjPlayerCtrl.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-28
 *描述:        	调兵山麻将玩家控制类
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.Mahjong2D.Game.Player.Dbsmj
{
    public class DbsmjPlayerCtrl : MahjongPlayerCtrl
    {
        #region UI Param
        #endregion

        #region Data Param
        #endregion

        #region Local Data
        public override void GetInCard(int value)
        {
            GetToken();
            LastGetValue = value;
            OnGetInCard(value);
        }
        #endregion

        #region Life Cycle
        #endregion

        #region Function
        #endregion
    }
}
