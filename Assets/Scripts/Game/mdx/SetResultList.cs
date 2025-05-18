using UnityEngine;

namespace Assets.Scripts.Game.mdx
{
    public class SetResultList : MonoBehaviour
    {

        public UISprite[] DiceSprites;

        public UISprite ResultMark;

        public UISprite BankerWinMark;


        public void SetItem(ResultData diceData)
        {
            var diceVals = diceData.DiceVals;
            int len = diceVals.Length;
            for (int i = 0; i < len; i++)
            {
                var sprite = DiceSprites[i];
                int curVal = diceVals[i];
                sprite.spriteName = string.Format("dice{0}", curVal);
                sprite.gameObject.SetActive(true);
            }
            switch (diceData.DiceType)
            {
                case 0:
                    ResultMark.spriteName = "da";
                    break;
                case 1:
                    ResultMark.spriteName = "xiao";
                    break;
                case 2:
                    ResultMark.spriteName = "baozi";
                    break;
            }
            BankerWinMark.spriteName = diceData.BnakerWin ? "zhuangying" : "zhuangshu";
        }
    }
}
