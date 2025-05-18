/** 
 *文件名称:     ShenYangPlayerCtrl.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-06-15 
 *描述:         沈阳麻将当前玩家脚本控制类
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.Player.ShenYang
{
    public class ShenYangPlayerCtrl : MahjongPlayerCtrl
    {
        public override void ShowMenuByCheck(int checkType, int value, int seat)
        {
            HideMenu(); 
            CheckCard = value;
            if ((checkType & (int)EnumCpghMenuType.DanSelect) == (int)EnumCpghMenuType.DanSelect)//过蛋处理
            {
                GuoDanSelect.Instance.ShowDanSelect();
                return;
            }
            base.ShowMenuByCheck(checkType,value,seat);
        }

        protected override void OnMahjongClick(Transform item)
        {
            if (GuoDanSelect.Instance.DanSelect)
            {
                item.GetComponent<UserContorl>().OnRecoveryPos();
                YxDebug.LogError("现在处于蛋选择状态，不能打牌");
                return;
            }
            base.OnMahjongClick(item);
        }
    }
}
