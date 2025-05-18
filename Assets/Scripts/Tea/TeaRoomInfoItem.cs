using System;
using System.Collections.Generic;
using Assets.Scripts.Common.UI;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaRoomInfoItem : YxView
    {
        public UILabel RoomId;
        public UILabel GameNameAndRound;
        public UILabel DatalLabel;
        public UILabel DatalUpdateLabel;
        public UILabel[] Names;
        public YxBaseTextureAdapter[] Avatars;
        public UILabel Index;
        public GameObject JieSanBt;
        public GameObject ChaKanBt;
        public GameObject OverIcon;
        public string TeaId;
        public TeaRoomInfo RoomInfo;
        public string RealRoomId;

        public string UseNum;
        public string InfoStr;
        public string[] Ids;
        public string[] Golds;
        public string RealGameName;
        public string RealGameRound;
        [Tooltip("头像显示布局")]
        public MulLayout HeadLayout;

        protected override void OnFreshView()
        {
            var roomData = GetData<RoomInfoData>();
            if (roomData == null) return;
            RealRoomId = roomData.RoomId;
            RoomId.text = TeaUtil.SubId(roomData.RoomId);
            RealGameName = roomData.GameName;
            RealGameRound =string.Format("{0}{1}", roomData.GameRound, roomData.IsQuan ? "圈" : "局") ;
            GameNameAndRound.text =string.Format("{0} {1}", roomData.GameName, RealGameRound);
            UseNum = roomData.UseNum;
            InfoStr = roomData.InfoStr;
            DatalLabel.text = roomData.CreateDt;
            if(DatalUpdateLabel) DatalUpdateLabel.text= roomData.UpdateDt;
            bool overIconShow = roomData.OverIcon;
            OverIcon.SetActive(overIconShow);
            Ids = new string[roomData.UserInfos.Length];
            Golds = new string[roomData.UserInfos.Length];
            for (int i = 0; i < roomData.UserInfos.Length; i++)
            {
                Avatars[i].gameObject.SetActive(true);
                Names[i].text = roomData.UserInfos[i].UserName;
                string url = roomData.UserInfos[i].Avatar;
                PortraitDb.SetPortrait(url, Avatars[i], 1);
                Golds[i] = roomData.UserInfos[i].Gold;
                Ids[i] = roomData.UserInfos[i].Id;
            }
            if(HeadLayout!=null)
            {
                HeadLayout.ResetLayout();
            }
        }

        public void SetIndex(int index)
        {
            Index.text = index + "";
        }

        public void ClickJieSan()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj = RealRoomId;
            dic["roomId"] = obj;
            Facade.Instance<TwManager>().SendAction("group.dissolveRoom", dic, BackJieSan);
        }

        private void BackJieSan(object msg)
        {
            TeaUtil.GetBackString(msg);
            RoomInfo.DangQianClick();
        }

        public void ClickChaKan()
        {
            RoomInfo.CreateUserInfoWindow(this);
        }
    }

    

    public class RoomInfoData
    {
        public string RoomId;
        public string GameName;
        public string CurRound;
        public string GameRound;
        public string UseNum;
        public int UserNum;
        public string InfoStr;
        public string CreateDt;
        public string UpdateDt;
        public string GameRoomId;
        public bool OverIcon;
        public string GameKey;
        public int Status;
        public string LimitGold;
        public int Index;
        /// <summary>
        /// 下注信息
        /// </summary>
        public string AnteInfo;
        public TeaUserInfo[] UserInfos=new TeaUserInfo[0];
        /// <summary>
        ///  Key圈标识
        /// </summary>
        private const string KeyQuan = "quan";
        /// <summary>
        /// Key消耗资源类型
        /// </summary>
        private const string KeyGoldType= "goldtype";
        /// <summary>
        /// Key消耗资源类型
        /// </summary>
        private const string KeyGoldMinValue = "mingold";
        /// <summary>
        /// Key消耗资源类型
        /// </summary>
        private const string KeyGoldMaxValue = "maxgold";
        /// <summary>
        /// Key 玩家数量
        /// </summary>
        private const string KeyUserNum = "user_num";
        /// <summary>
        /// 是否为圈玩法
        /// </summary>
        private bool _isQuan;
        /// <summary>
        /// 消耗资源类型
        /// </summary>
        private string _goldType;
        /// <summary>
        /// 消耗货币最大值
        /// </summary>
        private float _goldMaxValue;

        /// <summary>
        /// Key消耗资源类型
        /// </summary>
        private float _golMinValue;
        /// <summary>
        /// 圈玩法标记值
        /// </summary>
        private const int QuanValue = 1;

        public bool IsQuan
        {
            get
            {
                return _isQuan;
            }
        }

        public float GoldMax
        {
            get
            {
                return _goldMaxValue;
            }
        }

        public float GoldMin
        {
            get { return _golMinValue; }
        }

        public string GoldType
        {
            get { return _goldType; }
        }

        public void ParseData(Dictionary<string, object> dic,bool overIconShow=false)
        {
            if (dic.ContainsKey("room_id"))
            {
                RoomId = (string)dic["room_id"]; 
            }
            if (dic.ContainsKey("game_name"))
            {
                GameName = (string)dic["game_name"];
            }

            string roundNum = "";
            if (dic.ContainsKey("round_num"))
            {
                roundNum = (string)dic["round_num"];
            }
            string roundRnum = "";
            if (dic.ContainsKey("round_rnum"))
            {
                roundRnum = (string)dic["round_rnum"];
            }
            GameRound = !string.IsNullOrEmpty(roundRnum) ? string.Format("{0}/{1}", roundRnum, roundNum) : roundNum;
            if (dic.ContainsKey("use_num"))
            {
                UseNum = (string)dic["use_num"];
            }
            if (dic.ContainsKey("info_str"))
            {
                InfoStr = (string)dic["info_str"];
            }
            if (dic.ContainsKey("create_dt"))
            {
                CreateDt = (string)dic["create_dt"];
            }
            if (dic.ContainsKey("update_dt"))
            {
                UpdateDt= (string)dic["update_dt"];
            }
            ConvertData(dic);
            dic.TryGetValueWitheKey(out UserNum,KeyUserNum);
            if (dic.ContainsKey("userinfo"))
            {
                
                Dictionary<string, object> userDic = (Dictionary<string, object>) dic["userinfo"];
                int index = 0;
                UserInfos = new TeaUserInfo[userDic.Count];
                for (int i = 0; i < UserInfos.Length; i++)
                {
                    UserInfos[i]=new TeaUserInfo();
                }
                foreach (KeyValuePair<string, object> userInfo in userDic)
                {
                    Dictionary<string, object> user=(Dictionary<string, object>) userInfo.Value;
                    if (user.ContainsKey("name"))
                    {
                        UserInfos[index].UserName = (string)user["name"];
                    }
                    if (user.ContainsKey("avatar"))
                    {
                        UserInfos[index].Avatar = (string)user["avatar"];
                    }
                    if (user.ContainsKey("gold"))
                    {
                        var goldObj = user["gold"];
                        UserInfos[index].Gold = goldObj.ToString();
                    }
                    UserInfos[index].Id = userInfo.Key;                   
                    index++;
                }
                if(UserNum==0)
                {
                    UserNum = index;
                }
            }
            OverIcon = overIconShow;
        }

        public void ParseGameServerData(Dictionary<string, object> dic, bool overIconShow = false)
        {
            if (dic.ContainsKey("rndId"))
            {
                RoomId = dic["rndId"].ToString();
            }

            if (dic.ContainsKey("-ante"))
            {
                AnteInfo = string.Format("{0}{1}{2}",",", "底分:",YxUtiles.GetShowNumber(int.Parse(dic["-ante"].ToString())));
            }

            var limiteGold = 0;

            if (dic.ContainsKey("-limiteGold"))
            {
                limiteGold=int.Parse(dic["-limiteGold"].ToString());
            }

            var gUpLimite = 0;
            if (dic.ContainsKey("-gUpLimit"))
            {
                gUpLimite= int.Parse(dic["-gUpLimit"].ToString());
            }

            var maxLimit=Math.Max(limiteGold, gUpLimite);

            LimitGold = maxLimit==0 ? ",无限制" : string.Format("{0}{1}{2}", ",", YxUtiles.GetShowNumber(maxLimit), "准");
           
            if (dic.ContainsKey("name"))
            {
                GameName =(string)dic["name"];
            }
            string round = "";
            if (dic.ContainsKey("round"))
            {
                 round = dic["round"].ToString();
            }
            if (dic.ContainsKey("cround"))
            {
                CurRound = dic["cround"].ToString();
            }
            GameRound = !string.IsNullOrEmpty(CurRound) ? string.Format("{0}/{1}", CurRound, round) : round;
            if (dic.ContainsKey("capacity"))
            {
                UserNum = Convert.ToInt32(dic["capacity"]);
            }
            if (dic.ContainsKey("info"))
            {
                InfoStr = (string)dic["info"];
            }
            if (dic.ContainsKey("id"))
            {
                GameRoomId = dic["id"].ToString();
            }
            if (dic.ContainsKey("status"))
            {
                Status = Convert.ToInt32(dic["status"]);
            }
            if (dic.ContainsKey("gamekey"))
            {
                GameKey = (string) dic["gamekey"];
            }
            ConvertData(dic);
            if (dic.ContainsKey("users"))
            {
                List<object> usersObj = (List<object>) dic["users"];
                UserInfos = new TeaUserInfo[usersObj.Count];
                for (int i = 0; i < UserInfos.Length; i++)
                {
                    UserInfos[i] = new TeaUserInfo();
                }
                for (int i = 0; i < usersObj.Count; i++)
                {
                    UserInfos[i].UserName = usersObj[i].ToString();
                }
            }
            if (dic.ContainsKey("avatars"))
            {
                List<object> usersObj = (List<object>)dic["avatars"];
                for (int i = 0; i < usersObj.Count; i++)
                {
                    UserInfos[i].Avatar = usersObj[i].ToString();
                }
            }

            if (dic.ContainsKey("index"))
            {
                Index = int.Parse(dic["index"].ToString());
            }
        }

        private void ConvertData(Dictionary<string,object> dic)
        {
            if (dic.ContainsKey(KeyQuan))
            {
                _isQuan = int.Parse(dic[KeyQuan].ToString()) == QuanValue;
            }
            else
            {
                _isQuan = false;
            }
            if(dic.ContainsKey(KeyGoldType))
            {
                _goldType = dic[KeyGoldType].ToString();
            }
            else
            {
                _goldType = "TempCoin";
            }

            if(dic.ContainsKey(KeyGoldMaxValue))
            {
                _goldMaxValue = float.Parse(dic[KeyGoldMaxValue].ToString());
            }
            else
            {
                _goldMaxValue = 0;
            }

            if (dic.ContainsKey(KeyGoldMinValue))
            {
                _golMinValue = float.Parse(dic[KeyGoldMinValue].ToString());
            }
            else
            {
                _golMinValue = 0;
            }
        }
    }

    public class SatisticsData
    {
        public int UserId;
        public string UserName;
        public int Total;
        public Dictionary<string, object> GameList = new Dictionary<string, object>();

        public SatisticsData(Dictionary<string, object> info)
        {
            if (info.ContainsKey("userid"))
            {
                UserId = int.Parse(info["userid"].ToString());
            }

            if (info.ContainsKey("nickname"))
            {
                UserName = info["nickname"].ToString();
            }

            if (info.ContainsKey("total"))
            {
                Total = int.Parse(info["total"].ToString());
            }

            if (info.ContainsKey("data"))
            {
                GameList = info["data"] as Dictionary<string,object>;
            }
        }
    }

    public class TeaUserInfo
    {
        public string UserName;
        public string Avatar;
        public string Gold;
        public string Id;
    }

    public class TeaUtil
    {
        /// <summary>
        /// Key茶馆ID
        /// </summary>
        public const string KeyTeaId = "id";
        /// <summary>
        /// Key 茶馆保存Key
        /// </summary>
        public const string KeySaveTeaKey = "TeaId";
        /// <summary>
        /// Key茶馆申请请求
        /// </summary>
        public const string KeyTeaApplyJoinAction= "group.teaHouseApply";
        /// <summary>
        /// Key查找房间请求
        /// </summary>
        public const string KeyFindTeaHouseAction = "group.teaGetIn";
        /// <summary>
        /// Key 茶馆加入状态（0未申请 1已申请 2邀请状态 3已加入 4黑名单）
        /// </summary>
        public const string KeyTeaHouseStatus = "mstatus";
        /// <summary>
        /// Key 默认茶馆窗口名称
        /// </summary>
        public const string KeyDefTeaHouseName = "TeaPanel";
        /// <summary>
        /// Key茶馆交互message 内容
        /// </summary>
        public const string KeyBackStringContent = "info";
        /// <summary>
        /// Key茶馆交互message 内容分割标识
        /// </summary>
        public const string KeyBackStringSplitFlag = ";";
        /// <summary>
        /// Key茶馆交互message 内容分割替换标识
        /// </summary>
        public const string KeyBackStringReplaceFlag = ";\n";

        public static string SubId(string roomId)
        {
            if (roomId.Length<=6)
            {
                return roomId;
            }
            string str = roomId.Substring(roomId.Length - 6);
            return str;
        }

        /// <summary>
        /// 茶馆交互提示相关
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgBoxName"></param>
        public static void GetBackString(object msg,string msgBoxName=null)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            if (dic.ContainsKey(KeyBackStringContent))
            {
                string infoStr = (string)dic[KeyBackStringContent];
                if (!string.IsNullOrEmpty(infoStr))
                {
                    if (infoStr.Contains(KeyBackStringSplitFlag)) infoStr = infoStr.Replace(KeyBackStringSplitFlag, KeyBackStringReplaceFlag);
                    YxMessageBox.Show(null, msgBoxName, infoStr);
                }
            }
        }

        /// <summary>
        /// 申请加入茶馆
        /// </summary>
        /// <param name="teaId">茶馆ID</param>
        /// <param name="msgBoxName">提示框名称</param>
        /// <param name="successCall">成功回调</param>
        public static void ApplyJoinTeaHouse(string teaId,string msgBoxName = null, TwCallBack successCall=null)
        {
            var dic=new Dictionary<string, object>();
            dic[KeyTeaId] = teaId;
            Facade.Instance<TwManager>().SendAction(KeyTeaApplyJoinAction,dic,message =>
            {
                if (successCall!=null)
                {
                    successCall(message);
                }
                GetBackString(message, msgBoxName);
            });
        }

        /// <summary>
        /// 查找茶馆房间
        /// </summary>
        /// <param name="teaId">茶馆ID</param>
        /// <param name="msgBoxName">提示框名称</param>
        /// <param name="successCall">成功回调</param>
        /// <param name="openTeapanel"></param>
        /// <param name="defTeaHouseName">默认茶馆窗口名称</param>
        public static void FindTeaHouse(string teaId,string msgBoxName=null, TwCallBack successCall = null,bool openTeapanel=true,string defTeaHouseName=KeyDefTeaHouseName)
        {
            var dic = new Dictionary<string, object>();
            dic[KeyTeaId] = teaId;
            Facade.Instance<TwManager>().SendAction(KeyFindTeaHouseAction, dic, message =>
            {
                if (successCall!=null)
                {
                    successCall(message);
                }
                if (openTeapanel)
                {
                    Dictionary<string, object> successDic = (Dictionary<string, object>)message;
                    YxWindow window = YxWindowManager.GetWindowInstance<TeaPanel>(defTeaHouseName);
                    if (window == null)
                    {
                        window = YxTools.OpenWindowWithData(null, defTeaHouseName, successDic);
                    }
                    else
                    {
                        window.UpdateView(successDic);
                    }
                    if (window)
                    {
                        TeaPanel panel = window.GetComponent<TeaPanel>();
                        panel.SetTeaCode(int.Parse(teaId));
                        Util.SetString(KeySaveTeaKey, teaId);
                    }
                }
            });
        }

        /// <summary>
        /// 当前的茶馆ID，为茶馆部分公用字段，以Teapanel部分数据为数据源
        /// </summary>
        /// <returns></returns>
        public static string CurTeaId
        {
            get { return TeaPanel.CurTeaId.ToString(); }
        }

        /// <summary>
        /// 当前茶馆的授权开房状态 0.关闭 1.开启
        /// </summary>
        public static int CurPersonCtrlStatus
        {
            get { return TeaPanel.PersonCtrl; }
        }

        /// <summary>
        /// 是否只有馆主可以开房 0：茶馆成员可开房 其它默认为馆主开房
        /// </summary>
        public static int OnlyOwener
        {
            get { return TeaPanel.OnlyOwner; }
        }

        /// <summary>
        /// 当前茶馆名称
        /// </summary>
        public static string CurTeaName
        {
            get { return TeaPanel.CurTeaName; }
        }

        /// <summary>
        /// 当前茶馆馆主ID
        /// </summary>
        public static string CurTeaOwenerId
        {
            get { return TeaPanel.CurTeaOwnerId.ToString(); }
        }

    }
}

