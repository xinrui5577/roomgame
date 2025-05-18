using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.sanpian.item;
using Assets.Scripts.Game.sanpian.Tool;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.sanpian.server
{
    public class BigResult : MonoBehaviour
    {
        public BigResultItem[] items;

        [SerializeField]
        private CompressImg Img;

        public void ShowBigResult(ISFSObject param)
        {
            gameObject.SetActive(true);
            ISFSArray users = param.GetSFSArray("users");
            for(int i=0;i<items.Length;i++)
            {
                items[i].SetValue(users.GetSFSObject(i),i);
            }
        }

        public void OnBackHallClick()
        {
            YxDebug.Log("返回大厅");
            App.QuitGameWithMsgBox();
        }

        public void OnClickShare()
        {
            YxWindowManager.ShowWaitFor();

            Facade.Instance<WeChatApi>().InitWechat(AppInfo.WxAppId);

            UserController.Instance.GetShareInfo((info) =>
            {
                YxWindowManager.HideWaitFor();
                Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        imageUrl = "file://" + imageUrl;
                    }
                    info.ImageUrl = imageUrl;
                    info.ShareType = ShareType.Image;
                    Facade.Instance<WeChatApi>().ShareContent(info, str =>
                    {
                        Dictionary<string, object> parm = new Dictionary<string, object>()
                        {
                            {"option",2},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",SharePlat.WxSenceTimeLine.ToString() },
                        };
                        Facade.Instance<TwManager>().SendAction("shareAwards", parm, null);
                    });
                });
            });
        }
    }
}
