using System.Globalization;
using UnityEngine.UI;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.UI
{
    public class UserDefLable : Text
    {
        private long Glod;
        //金币 显示为 **万
        public long GlodText
        {
            get { return Glod; }
            set
            {
                if(value==Glod)return;
                Glod = value;
                text = YxUtiles.GetShowNumber(Glod).ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
