using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketGrabLogWindow : YxNguiRedPacketWindow
    {
        public UITexture Bg;
        public Texture2D NormalTexture;
        public Texture2D SelfTexture;
        public string SendActionKey;
        public string Type;

        public NguiLabelAdapter MySelfGold;
        public NguiTextureAdapter Head;
        public NguiLabelAdapter NickName;
        public NguiLabelAdapter RedPacketMoney;
        public NguiLabelAdapter InmineNum;
        public RedPacketGrabLogItem RedPacketGrabLogItem;
        public UIGrid RedPacketGrabLogGrid;
        public UIGrid RedPacketGrabLogHasSelfGrid;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redId = (int)Data;
            var dic = new Dictionary<string, object>();
            dic["redId"] = redId;
            dic["op"] = 2;
            if (!string.IsNullOrEmpty(Type))
            {
                dic[Type] = 2;
            }
            Facade.Instance<TwManager>().SendAction(SendActionKey, dic, FreshRedPacketLog);
        }

        private void FreshRedPacketLog(object obj)
        {
            var info = obj as Dictionary<string, object>;

            if (info == null) return;
            var data = new RedPacketLogData(info["data"]);
            PortraitDb.SetPortrait(data.Avatar, Head, 1);
            var str = string.Format("{0}的红包", data.NickName);
            NickName.TrySetComponentValue(str);
            RedPacketMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(data.RedPacketMoney, 0, "N0"));
            InmineNum.TrySetComponentValue(data.InmineNum);
            var detail = info["detail"];
            var detailList = detail as List<object>;
            if (detailList == null) return;
            var userInfo = UserInfoModel.Instance.UserInfo;
            object selfData = null;
            object fromDeathData = null;
            var index = 0;
            while (index < detailList.Count)
            {
                var list = detailList[index];
                var detailData = new RedPacketLogData(list);
                if (detailData.UserId == int.Parse(userInfo.UserId))
                {
                    selfData = list;
                    detailList.Remove(list);
                    index = 0;
                }
                else if (detailData.UserId == 1)
                {
                    fromDeathData = list;
                    detailList.Remove(list);
                    index = 0;
                }
                else
                {
                    index++;
                }
            }

            List<object> trueList = new List<object>();
            if (selfData != null)
            {
                trueList.Add(selfData);
                Bg.mainTexture = SelfTexture;
                var selfInfo = new RedPacketLogData(selfData);
                MySelfGold.gameObject.SetActive(true);
                MySelfGold.TrySetComponentValue(YxUtiles.GetShowNumberForm(selfInfo.GrabMoney));
            }
            else
            {
                MySelfGold.gameObject.SetActive(false);
                Bg.mainTexture = NormalTexture;
            }
            if (detailList.Count != 0)
            {
                trueList.AddRange(detailList);
            }

            if (fromDeathData != null)
            {

                trueList.Add(fromDeathData);
            }

            foreach (var list in trueList)
            {
                var detailData = new RedPacketLogData(list);
                RedPacketGrabLogItem item = null;
                if (selfData != null)
                {
                    detailData.IncludeMe = true;
                    item = YxWindowUtils.CreateItem(RedPacketGrabLogItem, RedPacketGrabLogHasSelfGrid.transform);
                }
                else
                {
                    detailData.IncludeMe = false;
                    item = YxWindowUtils.CreateItem(RedPacketGrabLogItem, RedPacketGrabLogGrid.transform);
                }
              
                item.UpdateView(detailData);
            }

            RedPacketGrabLogHasSelfGrid.repositionNow = true;
            RedPacketGrabLogGrid.repositionNow = true;
        }
    }

    public class RedPacketLogData
    {
        public string NickName;
        public int UserId;
        public string Avatar;
        public int Status;
        public int RedId;
        public string InmineNum;
        public long GrabMoney;
        public long RedPacketMoney;
        public string GrabTime;
        public bool IsInmine;
        public bool IsPaymine;//中雷
        public bool IsLuck;
        public bool IncludeMe;

        public RedPacketLogData(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null) return;
            if (data.ContainsKey("nick_m"))
            {
                NickName = data["nick_m"].ToString();
            }
            if (data.ContainsKey("user_id"))
            {
                UserId = int.Parse(data["user_id"].ToString());
            }
            if (data.ContainsKey("avatar_x"))
            {
                Avatar = data["avatar_x"].ToString();
            }
            if (data.ContainsKey("status_i"))
            {
                Status = int.Parse(data["status_i"].ToString());
            }
            if (data.ContainsKey("id"))
            {
                RedId = int.Parse(data["id"].ToString());
            }
            if (data.ContainsKey("inmine_num"))
            {
                InmineNum = data["inmine_num"].ToString();
            }
            if (data.ContainsKey("grab_a"))
            {
                GrabMoney = long.Parse(data["grab_a"].ToString());
            }
            if (data.ContainsKey("money_a"))
            {
                RedPacketMoney = long.Parse(data["money_a"].ToString());
            }
            if (data.ContainsKey("date_created"))
            {
                GrabTime = data["date_created"].ToString();
            }
            if (data.ContainsKey("is_inmine"))
            {
                IsInmine = int.Parse(data["is_inmine"].ToString()) == 1;
            }
            if (data.ContainsKey("is_paymine"))
            {
                IsPaymine = int.Parse(data["is_paymine"].ToString()) == 1;
            }
            if (data.ContainsKey("is_luck"))
            {
                IsLuck = int.Parse(data["is_luck"].ToString()) == 1;
            }
        }
    }
}
