using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketMainWindow : YxNguiRedPacketWindow
    {
        public RedPacketItem OtherRedPacketItem;
        public RedPacketItem MySelfRedPacketItem;
        public RedPacketGrabedTipItem RedPacketGrabedTipItem;
        public UITable RedPacketGrid;
        public UIScrollBar ScrollBar;
        public float FreshRequestTime;
        public float FreshScrollBarTime;

        private int _lastRedId;
        private static int _lastGrabedId = -1;
        private bool _fresh = true;
        private bool _hasRequestBack;

        private Coroutine _historyRequest;
        private Coroutine _freshScrollbar;

        protected override void OnShow()
        {
            base.OnShow();
            _fresh = true;

            if (_historyRequest != null)
            {
                StopCoroutine(_historyRequest);
            }
            if (_freshScrollbar != null)
            {
                StopCoroutine(_freshScrollbar);
            }
            _historyRequest = StartCoroutine("RequestRedPacketHistory");
            _freshScrollbar = StartCoroutine("FreshScrollBar");
        }

        protected override void OnHide()
        {
            base.OnHide();
            _fresh = false;
            StopCoroutine(_historyRequest);
            StopCoroutine(_freshScrollbar);
        }


        IEnumerator RequestRedPacketHistory()
        {
            while (_fresh)
            {
                yield return new WaitForSeconds(FreshRequestTime);
                if (!_hasRequestBack)
                {
                    var dic = new Dictionary<string, object>();
                    dic["lastRedId"] = _lastRedId;
                    if (_lastGrabedId != -1)
                    {
                        dic["lastGrabedId"] = _lastGrabedId;
                    }

                    _hasRequestBack = true;
                    Facade.Instance<TwManager>().SendAction("RedEnvelope.redEnvelopeHistory", dic, FreshRedPacketView, true, null, false);
                }
               
            }
        }

        IEnumerator FreshScrollBar()
        {
            while (_fresh)
            {
                yield return new WaitForSeconds(FreshScrollBarTime);
                if (RedPacketGrid.transform.childCount > 5)
                {
                    ScrollBar.value = 1;
                }
            }
        }


        private void FreshRedPacketView(object obj)
        {
            var info = obj as Dictionary<string, object>;
            if (info == null) return;
            if (info.ContainsKey("autofootTime"))
            {
                FreshScrollBarTime = float.Parse(info["autofootTime"].ToString());
            }
            if (info.ContainsKey("noticeInterval"))
            {
                FreshRequestTime = float.Parse(info["noticeInterval"].ToString());
            }
            var data = info["data"] as List<object>;
            bool includeMe = false;
            if (data != null && data.Count != 0)
            {
                var userInfo = UserInfoModel.Instance.UserInfo;

                for (int i = data.Count - 1; i >= 0; i--)
                {
                    var redPacketData = new RedPacketData(data[i]);
                    RedPacketItem item = null;
                    if (redPacketData.UserId == int.Parse(userInfo.UserId))
                    {
                        includeMe = true;
                        item = YxWindowUtils.CreateItem(MySelfRedPacketItem, RedPacketGrid.transform);
                    }
                    else
                    {
                        item = YxWindowUtils.CreateItem(OtherRedPacketItem, RedPacketGrid.transform);
                    }

                    _lastRedId = redPacketData.RedId;
                    item.UpdateView(redPacketData);
                    Facade.Instance<MusicManager>().Play("warning");
                }
            }

            bool selfGrabEdLog = false;
            if (info.ContainsKey("selfGrabedData"))
            {
                var selfGrabedData = info["selfGrabedData"] as Dictionary<string, object>;
                if (selfGrabedData != null)
                {
                    if (selfGrabedData["lastGrabedId"] != null)
                    {
                        _lastGrabedId = int.Parse(selfGrabedData["lastGrabedId"].ToString());
                    }

                    if (selfGrabedData.ContainsKey("data"))
                    {
                        var grabedDatas = selfGrabedData["data"] as List<object>;
                        if (grabedDatas != null)
                        {
                            if (grabedDatas.Count > 0)
                            {
                                selfGrabEdLog = true;
                            }
                            foreach (var grabedData in grabedDatas)
                            {
                                var grabedInfo = grabedData as Dictionary<string, object>;

                                if (grabedInfo != null)
                                {
                                    var item = YxWindowUtils.CreateItem(RedPacketGrabedTipItem, RedPacketGrid.transform);
                                    item.UpdateView(grabedInfo);
                                }
                            }
                        }
                    }
                }
            }

            if (RedPacketGrid)
            {
                RedPacketGrid.repositionNow = true;
            }
            if (data != null && data.Count != 0 && Math.Abs(ScrollBar.value - 1) <= 0.2 || includeMe || selfGrabEdLog)
            {
                if (gameObject.activeSelf)
                {
                    StartCoroutine("FreshScrollView");
                }
            }

            _hasRequestBack = false;
        }

        IEnumerator FreshScrollView()
        {
            yield return new WaitForSeconds(0.1f);

            if (RedPacketGrid.transform.childCount > 4)
            {
                ScrollBar.value = 1;
                ScrollBar.ForceUpdate();
            }
        }

        public void OpenUrl(string objName)
        {
            if (!string.IsNullOrEmpty(objName))
            {
                Application.OpenURL(objName);
            }
        }
    }

    public class RedPacketData
    {
        public string NickName;
        public int UserId;
        public string Avatar;
        public int RedId;
        public long RedMoney;
        public string InmineNum;
        public List<string> SpecialInfos = new List<string>();

        public RedPacketData(object data)
        {
            var info = data as Dictionary<string, object>;
            if (info == null) return;
            if (info.ContainsKey("nick_m"))
            {
                NickName = info["nick_m"].ToString();
            }
            if (info.ContainsKey("user_id"))
            {
                UserId = int.Parse(info["user_id"].ToString());
            }

            if (info.ContainsKey("avatar_x"))
            {
                Avatar = info["avatar_x"].ToString();
            }

            if (info.ContainsKey("id"))
            {
                RedId = int.Parse(info["id"].ToString());
            }

            if (info.ContainsKey("money_a"))
            {
                RedMoney = long.Parse(info["money_a"].ToString());
            }

            if (info.ContainsKey("inmine_num"))
            {
                InmineNum = info["inmine_num"].ToString();
            }

            if (info.ContainsKey("special_info"))
            {
                var specialInfos = info["special_info"] as List<object>;
                if (specialInfos != null && specialInfos.Count != 0)
                {
                    foreach (var specialInfo in specialInfos)
                    {
                        var str = specialInfo as string;
                        SpecialInfos.Add(str);
                    }
                }
            }
        }
    }
}
