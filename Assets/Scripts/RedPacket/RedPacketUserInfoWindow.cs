using YxFramwork.Common.Model;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketUserInfoWindow : YxNguiRedPacketWindow
    {
        public void OnChangeNickName()
        {
            var userInfo = UserInfoModel.Instance.UserInfo;
            var win=CreateOtherWindow("ChangeNickNameWindow");
            win.UpdateView(userInfo.NickM);
        }
    }
}
