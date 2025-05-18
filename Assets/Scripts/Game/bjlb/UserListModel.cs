using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.bjlb
{
    public class UserListModel : MonoBehaviour
    {
        public UILabel NameLabel;
        public UILabel CoinLabel;
        public int UserId;

        public void SetInfo(YxBaseGameUserInfo user, bool isBanker)
        {
            NameLabel.text = user.NickM;
            CoinLabel.text = YxUtiles.GetShowNumberForm(user.CoinA);

            if (isBanker)
            {
                NameLabel.color = Color.red;
                CoinLabel.color = Color.red;
                name = "banker";
            }
            UserId = user.Id;
        }

        public void SetInfo(string players)
        {
            var temp = players.Split(',');
            NameLabel.text = temp[0];
            
            CoinLabel.text = YxUtiles.GetShowNumberForm(long.Parse(temp[1])); //(Int32.Parse(temp[1]) / 10000) + "万";
        }
    }
}
