using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.fxmj
{
    public class FuXinUserInfoPanel : UserInfoPanel
    {
        public override void SetTing(bool state)
        {
            base.SetTing(state);
            _ting.GetComponent<UISprite>().spriteName = App.GetGameManager<Mahjong2DGameManager>().Xs == 1 ? ConstantData.XiaoSa : ConstantData.Ting;//阜新特有 没事别乱用
        }
    }
}
