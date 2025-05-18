/** 
 *文件名称:     TaiAnplayerCtrl.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-08-14 
 *描述:         台安玩家控制类
 *历史记录: 
*/

namespace Assets.Scripts.Game.Mahjong2D.Game.Player.TaiAn
{
    public class TaiAnplayerCtrl : MahjongPlayerCtrl
    {
        public override void GetInCard(int value)
        {
            GetToken();
            LastGetValue = value;
            OnGetInCard(value);
        }
    }
}
