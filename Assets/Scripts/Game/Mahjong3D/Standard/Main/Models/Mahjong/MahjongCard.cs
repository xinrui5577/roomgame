using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongCard : MonoBehaviour
    {
        public MeshRenderer Mahjong;
        public SpriteRenderer CardValue;

        private int mValue;

        public int Value
        {
            get { return mValue; }
            set
            {
                mValue = value;
                if (mValue >= (int)MahjongValue.Wan_1)
                {
                    var sprite = GameCenter.Assets.GetMahjongSprite(value);
                    if (sprite != null)
                    {
                        CardValue.sprite = sprite;
                    }
                }
            }
        }

        public void SetMahjongColor(MahjongColor skin)
        {
            CardValue.color = Color.white;
            switch (skin)
            {
                case MahjongColor.Normal:
                    Mahjong.material = GameCenter.Assets.MahjongNormal;
                    break;
                case MahjongColor.Golden:
                    Mahjong.material = GameCenter.Assets.MahjongGolden;
                    break;
                case MahjongColor.Gray:
                    Mahjong.material = GameCenter.Assets.MahjongGray;
                    CardValue.color = new Color(170 / 255f, 170 / 255f, 170 / 255f);
                    break;
                case MahjongColor.Blue:
                    Mahjong.material = GameCenter.Assets.MahjongBlue;
                    break;
            }
        }
    }
}