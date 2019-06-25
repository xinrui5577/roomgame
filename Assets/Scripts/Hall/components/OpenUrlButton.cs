using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using System.Text;

namespace Assets.Scripts.Hall.Components
{
    public class OpenUrlButton : MonoBehaviour
    {
        public string Url;
        public bool UseToekn;
        public bool UseGameHost;
        public string[] ArgNames;//['token']
        public string[] ArgValues;//['LoginInfo.ctoken']

        public void OnClickOpenUrl()
        {
            var url = Url;
            if (UseGameHost)
            {
                url = App.Config.GetUrlWithServer("index.php/mobile/download/proxy");
            }
            url = url + (url.IndexOf("?") > 0 ? '&' : '?');
            if (UseToekn)
            {
                string ctoken = LoginInfo.Instance.ctoken;
                string userid = LoginInfo.Instance.user_id;
                url = url + string.Format("token={0}&userid={1}", ctoken, userid);
            }
            if (ArgNames != null)
            {
                for (var i = 0; i < ArgNames.Length; i++)
                {
                    if (ArgNames[i] != null && ArgValues.Length > i)
                    {
                        //ArgValues['user.coin'];
                        var value = ArgValues[i];
                        if (value.IndexOf('.') > 0)
                        {
                            //user
                            //coin
                            //LoginInfo.Instance.token;
                        }
                        url = url + "&" + ArgNames[i] + "=" + value;
                        //call function();
                    }
                }
            }
            Application.OpenURL(url);
        }
    }
}
