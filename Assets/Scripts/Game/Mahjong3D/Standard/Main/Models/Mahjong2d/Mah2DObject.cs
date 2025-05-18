using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class Mah2DObject : ObjectBase
    {
        public Image CardValue;
        public Image LaiziIcon;

        public MahjongSign Sign { get; set; }

        protected void Awake()
        {
            Sign = GetComponent<MahjongSign>();
        }

        public void SetOther()
        {
            Sign.OtherSign(Anchor.TopRight, true);
        }

        public void SetLaizi()
        {
            Sign.LaiziSign(true);
        }

        public void SetNumber(int num)
        {
            Sign.SetNumberSign(num);
        }
    }
}