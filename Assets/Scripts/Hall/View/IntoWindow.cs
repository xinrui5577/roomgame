using UnityEngine;
using YxFramwork.Common.Utils;
using YxFramwork.Controller;
using YxFramwork.Framework;


public class IntoWindow:YxBaseWindow
{

    public GameObject BtnLogin;

    public GameObject BtnInto;

    public override WindowName WindowName
    {
        get { return WindowName.Into;}
    }

    protected override IBaseModel YxBaseModel
    {
        get { return null; }
    }

    public void OnApplicationQuit()
    {
        //Util.RemoveData("isInto");
    }


    protected override void OnCreate()
    {
        base.OnCreate();

     

        //if (!string.IsNullOrEmpty(App.UserName))
        //{
        //    PanelManager.ShowWindow(WindowName.GameHall.ToString());
        //    PanelManager.HiddenWindow(WindowName.LoadEntrance.ToString(),null);
        //}
        

        UIEventListener.Get(BtnInto).onClick = delegate 
        {
            string userName = Util.GetString("userName");

            if (!string.IsNullOrEmpty(userName))
            {
                UserController.Instance.Login(Util.GetString("userName"), Util.GetString("userPwd"), false);
            }
            else
            {
                this.HiddenWindow();
                PanelManager.ShowWindow(WindowName.Login);
            }
            
        };

        UIEventListener.Get(BtnLogin).onClick = delegate
        {
            this.HiddenWindow();
            this.PanelManager.ShowWindow(WindowName.Login);
        };
    }
}

