using System;
using System.Collections.Generic;
using Assets.Scripts.Common.components;
using Assets.Scripts.Common.UI;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaRoomInfoItem : YxView
    {
        public UILabel RoomId;
        public UILabel GameNameAndRound;
        public UILabel DatalLabel;
        public UILabel[] Names;
        public UITexture[] Avatars;
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
            RealGameRound = roomData.GameRound;
            GameNameAndRound.text = roomData.GameName+" "+roomData.GameRound+ (roomData.IsQuan?"圈":"局");
            UseNum = roomData.UseNum;
            InfoStr = roomData.InfoStr;
            DatalLabel.text = roomData.CreateDt;
            bool overIconShow = roomData.OverIcon;
            OverIcon.SetActive(overIconShow);
            Ids = new string[roomData.UserInfos.Length];
            Golds = new string[roomData.UserInfos.Length];
            for (int i = 0; i < roomData.UserInfos.Length; i++)
            {
                Avatars[i].gameObject.SetActive(true);
                Names[i].text = roomData.UserInfos[i].UserName;
                string url = roomData.UserInfos[i].Avatar;
                PortraitRes.SetPortrait(url, Avatars[i], 1);
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
            Facade.Instance<TwManger>().SendAction("group.dissolveRoom", dic, BackJieSan);
        }

        private void BackJieSan(object msg)
        {
            TeaUtil.GetBackString(msg);
            RoomInfo.GetTableList();
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
        public string GameRound;
        public string UseNum;
        public int UserNum;
        public string InfoStr;
        public string CreateDt;
        public string GameRoomId;
        public bool OverIcon;
        public string GameKey;
        public int status;
        public TeaUserInfo[] UserInfos=new TeaUserInfo[0];
        /// <summary>
        ///  Key圈标识
        /// </summary>
        private string _keyQuan = "quan";
        /// <summary>
        /// Key消耗资源类型
        /// </summary>
        private string _keyGoldType= "goldtype";
        /// <summary>
        /// Key消耗资源类型
        /// </summary>
        private string _keyGoldMinValue = "mingold";
        /// <summary>
        /// Key消耗资源类型
        /// </summary>
        private string _keyGoldMaxValue = "maxgold";
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
        private const int _quanValue = 1;

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
            if (dic.ContainsKey("round_num"))
            {
                GameRound = (string)dic["round_num"];
            }
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
            ConvertData(dic);
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
            if (dic.ContainsKey("name"))
            {
                GameName = (string)dic["name"];
            }
            if (dic.ContainsKey("round"))
            {
                GameRound = dic["round"].ToString();
            }
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
                status = Convert.ToInt32(dic["status"]);
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
        }

        private void ConvertData(Dictionary<string,object> dic)
        {
            if (dic.ContainsKey(_keyQuan))
            {
                _isQuan = int.Parse(dic[_keyQuan].ToString()) == _quanValue;
            }
            else
            {
                _isQuan = false;
            }
            if(dic.ContainsKey(_keyGoldType))
            {
                _goldType = dic[_keyGoldType].ToString();
            }
            else
            {
                _goldType = "TempCoin";
            }

            if(dic.ContainsKey(_keyGoldMaxValue))
            {
                _goldMaxValue = float.Parse(dic[_keyGoldMaxValue].ToString());
            }
            else
            {
                _goldMaxValue = 0;
            }

            if (dic.ContainsKey(_keyGoldMinValue))
            {
                _golMinValue = float.Parse(dic[_keyGoldMinValue].ToString());
            }
            else
            {
                _golMinValue = 0;
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
        public static string SubId(string roomId)
        {
            if (roomId.Length<=6)
            {
                return roomId;
            }
            string str = roomId.Substring(roomId.Length - 6);
            return str;
        }

        public static void GetBackString(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            if (dic.ContainsKey("info"))
            {
                string infoStr = (string)dic["info"];
                if (infoStr != "")
                {
                    YxMessageBox.Show(infoStr);
                }
            }
        }
    }
}
