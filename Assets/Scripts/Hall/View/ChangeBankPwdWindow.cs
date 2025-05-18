using YxFramwork.Controller;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 修改银行密码
    /// </summary>
    public class ChangeBankPwdWindow : ChangePwdWindow
    {
        protected override void SendAction()
        {
            UserController.Instance.ChangeBankPwd(OldPwd.value, NewPwd.value);
        }
       
    }
}
