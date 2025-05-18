using System;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Tool;


namespace Assets.Scripts.Game.brnn
{
    public class UserListModel : MonoBehaviour
    {
        public UILabel NameLabel;
        public UILabel CoinLabel;
        public int UserId;

        public virtual void SetInfo(YxBaseUserInfo brnnUser, bool isBanker = false,int rank = 0)
        {
            NameLabel.text = isBanker ? "[FF0000]" + brnnUser.NickM : brnnUser.NickM;
            CoinLabel.text = (isBanker ? "[FF0000]" : "") + YxUtiles.ReduceNumber((int)brnnUser.CoinA);
        }
        public virtual void SetInfo(string players,int rank)
        {
            var temp = players.Split(',');
            NameLabel.text = temp[0];
            CoinLabel.text = YxUtiles.ReduceNumber(int.Parse(temp[1]));
        }
    }
}