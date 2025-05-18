using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketChangeUserInfoWindow : YxNguiRedPacketWindow
    {
        public UIInput UserName;
        public RedPacketHeadItem HeadItem;
        public UIGrid HeadGrid;
        public GameObject SelectObj;

        private string _avatar;
        protected override void OnStart()
        {
            base.OnStart();
            if (HeadItem && HeadGrid)
            {
                Facade.Instance<TwManager>().SendAction("RedEnvelope.getAvatars", new Dictionary<string, object>(), FreshHeads, true, null, false);
            }
        }

        private void FreshHeads(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null) return;
            var avatars = data["avatars"] as List<object>;
            if (avatars != null)
                foreach (var avatar in avatars)
                {
                    var head = avatar as string;
                    var item = YxWindowUtils.CreateItem(HeadItem, HeadGrid.transform);
                    item.UpdateView(head);
                }

            HeadGrid.repositionNow = true;
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data is string)
            {
                var nickName = Data as string;
                if (UserName)
                {
                    UserName.value = nickName;
                }
            }
        }

        public void OnSubmit()
        {
            var dic = new Dictionary<string, object>();
            dic["nickName"] = UserName.value;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.changeUserInfo", dic, null, true, null, false);
        }

        public void OnSend()
        {
            var dic = new Dictionary<string, object>();
            dic["avatar"] = _avatar;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.changeUserInfo", dic, null, true, null, false);
        }

        public void OnSelectHead(RedPacketHeadItem obj)
        {
            if (!SelectObj.activeSelf)
            {
                SelectObj.SetActive(true);
            }
            SelectObj.transform.parent = obj.transform;
            SelectObj.transform.localPosition = Vector3.zero;
            _avatar = obj.Avatar;
        }
    }
}
