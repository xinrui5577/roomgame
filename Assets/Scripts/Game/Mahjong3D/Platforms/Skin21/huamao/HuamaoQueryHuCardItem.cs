using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class HuamaoQueryHuCardItem : QueryHuCardItem
    {
        public Text CardValue;

        public override void SetData(int card, int num)
        {
            Any.gameObject.SetActive(card == 0);

            CardImg.gameObject.SetActive(card != 0);

            if (card != 0)
            {
                CardImg.sprite = GameCenter.Assets.GetMahjongSprite(card);
                CardImg.SetNativeSize();

                CardValue.text = GameCenter.Shortcuts.MahjongQuery.Query(card) + "张";
            }
            else
            {
                //任意牌
                CardValue.text = GameCenter.DataCenter.LeaveMahjongCnt.ToString() + "张";
            }
        }

        public override int Rate
        {
            set
            {
                var text = value != 0 ? value : 1;
                RateValue.text = text + "倍";
            }
        }
    }
}
