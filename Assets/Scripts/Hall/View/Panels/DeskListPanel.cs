using System.Collections;
using Assets.Scripts.Hall.Controller;
using Assets.Scripts.Hall.Models;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.Panels
{
    /// <summary>
    /// 桌面列表窗口
    /// </summary>
    public class DeskListPanel : RoomListPanel
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            if (InitStateTotal < 3)
            {
                InitStateTotal = 3;
            }
        }

        public override void Init(object param)
        {
            base.Init(param);
            var deskData = param as DeskData;
            if (deskData == null) return;
            var gk = deskData.GameKey;
            App.LoadingGameKey = gk;
            //todo 发送请求
            HallController.Instance.SendGetDesks(deskData, UpdateView);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Data = null;
        }

        protected override void OnFreshView()
        {
            if (Data == null) return;
            base.OnFreshView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gk"></param>
        /// <returns></returns>
        protected override GameObject LoadListViewResource(string gk)
        {
            var prefix = App.Skin.GameInfo;
            var namePrefix = string.Format("{0}_{1}", prefix, gk);//gameinfo_gamekey
            var listName = string.Format("desklistview_{0}", gk);//roomlistview_gamekey
            var bundleName = string.Format("{0}/{1}", namePrefix, listName);//gameinfo_gamekey/roomlistview_gamekey
            return ResourceManager.LoadAsset(prefix, bundleName, listName);
        }

        protected override IList GetListData()
        {
            return GetData<IList>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gk"></param>
        protected override void ShowViewEx(string gk)
        {
            var w = GameObjectUtile.CreateMonoContainer<UIWidget>(transform);
            ExView = w.transform;
            var prefix = App.Skin.GameInfo;
            var namePrefix = string.Format("{0}_{1}", prefix, gk);//gameinfo_gamekey
            var viewExName = string.Format("desklistviewex_{0}", gk);//roomlistview_gamekey
            var bundleName = string.Format("{0}/{1}", namePrefix, viewExName);//gameinfo_gamekey/roomlistview_gamekey
            var pre = ResourceManager.LoadAsset(prefix, bundleName, viewExName);
            if (pre == null) { return; }
            pre = Instantiate(pre);
            GameObjectUtile.ResetTransformInfo(pre.transform, ExView.transform);
        }
    }
}
