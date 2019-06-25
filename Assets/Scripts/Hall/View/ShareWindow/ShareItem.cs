using System.Collections.Generic;
using System.Globalization;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.ShareWindow
{
    // todo 临时使用
    public class ShareItem : MonoBehaviour {

        public int Id;

        public UILabel Title;

        public UILabel ItemStatus;

        public UISprite AwardType;

        public UILabel AwardNum;

        public GameObject Btn;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sp"></param>
        public void Init(SharePlat sp) {
            Id = (int)sp;
            var parm = new Dictionary<string, object>
                {
                {"option",1},
                {"bundle_id",Application.bundleIdentifier},
                {"share_plat",((int)sp).ToString(CultureInfo.InvariantCulture) },
            };
            Facade.Instance<TwManger>().SendAction("shareAwards", parm, str =>
            {
                YxWindowManager.HideWaitFor();
                YxDebug.Log("分享查询回数据:" + str.ToString());
                var data = (Dictionary<string, object>)str;
                SetDate(data["infoClient"].ToString(), data["awardCount"].ToString(),(bool)data["todayShare"],
                    int.Parse(data["awardTime"].ToString()), (bool)data["enableAward"]);
            });
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="title"></param>
        /// <param name="aNum"></param>
        /// <param name="finish"></param>
        /// <param name="tager"></param>
        /// <param name="enable"></param>
        public void SetDate(string title,string aNum,bool finish,int tager,bool enable) {
            finish = (finish && !enable);
            Title.text = title;
            AwardNum.text = aNum;
            ItemStatus.text = finish ? "[1C6004FF]已完成[-]" : "[9E1818FF]未完成[-]";
        }

        /// <summary>
        /// 分享成功
        /// </summary>
        public void OnShareSuccess(object msg) {
            //发送分享成功
            YxWindowManager.ShowWaitFor();
            var parm = new Dictionary<string, object>
                {
                {"option", 2},
                {"bundle_id", Application.bundleIdentifier},
                {"share_plat", Id.ToString(CultureInfo.InvariantCulture)},
            };
            Facade.Instance<TwManger>().SendAction("shareAwards", parm, str =>
            {
                YxWindowManager.HideWaitFor();
                var data = (Dictionary<string, object>)str;
                if ((bool)data["enableAward"])
                {
                    GetReward();
                }
                else
                {
                    SetDate(data["infoClient"].ToString(), data["awardCount"].ToString(), (bool)data["todayShare"],
                        int.Parse(data["awardTime"].ToString()), (bool)data["enableAward"]);
                }
            });
        }

        /// <summary>
        /// 获取奖励
        /// </summary>
        public void GetReward()
        {
            YxWindowManager.ShowWaitFor();
            var parm = new Dictionary<string, object>
                {
                {"option",3},
                {"bundle_id",Application.bundleIdentifier},
                {"share_plat",Id.ToString(CultureInfo.InvariantCulture) },
            };
            Facade.Instance<TwManger>().SendAction("shareAwards", parm, str =>
            {
                YxWindowManager.HideWaitFor();
                var data = (Dictionary<string, object>)str;
                YxMessageBox.Show(data["awardInfo"].ToString());
                UserController.Instance.GetBackPack();
                SetDate(data["infoClient"].ToString(), data["awardCount"].ToString(), (bool)data["todayShare"],
                    int.Parse(data["awardTime"].ToString()), (bool)data["enableAward"]);
            });
        }

    }
}
