using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.lhc
{
    public class BetPosItem : MonoBehaviour
    {
        public UILabel BetPos;
        public UILabel BetValue;
        public UISprite BetPosBg;

        public void InitView(string key, int betValue)
        {
            BetValue.text = YxUtiles.GetShowNumber(betValue).ToString();
            var gdata = App.GetGameData<LhcGameData>();
            foreach (var value in gdata.BetPosColors)
            {
                if (value.Value==int.Parse(key))
                {
                    BetPosBg.spriteName = value.Color;
                    int betName = 0;
                    bool isNum=int.TryParse(value.Pos,out betName);
                    BetPos.text = !isNum ? value.Pos : key;
                }
            }
        }
    }
}
