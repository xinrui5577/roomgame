using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.View;

namespace Assets.Scripts.Game.pdk.DDzGameListener.GpsPanel
{
    public class GpsInfListener : MonoBehaviour {
        readonly Dictionary<int,UserInfoStruct> _userinfoDic = new Dictionary<int, UserInfoStruct>();
        /// <summary>
        /// 左边玩家gps地址信息
        /// </summary>
        [SerializeField]
        protected UILabel LeftGpsInfo;
        /// <summary>
        /// 右边玩家Gps地址信息
        /// </summary>
        [SerializeField]
        protected UILabel RightGpsInfo;


        /// <summary>
        /// 距离信息
        /// </summary>
        [SerializeField]
        protected UILabel DistanceLabel;

        /// <summary>
        /// gps信息面板
        /// </summary>
       [SerializeField]
        protected GameObject GpsUiGob;

        [SerializeField] protected GameObject GpsBtn;

        void Awake()
        {
            PdkGameManager.AddOnGameInfoEvt(OnGameInfo);
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnUserJoinRoomEvt(OnUserJoinRoom);
            Ddz2RemoteServer.AddOnGpsInfoReceiveEvt(OnGpsInfoEvt);
        }

        private void OnUserJoinRoom(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if(!data.ContainsKey(RequestKey.KeyUser)) return;

            var userdata = data.GetSFSObject(RequestKey.KeyUser);
            _userinfoDic[userdata.GetInt(RequestKey.KeySeat)] = SetUserData(userdata);
        }

        void Start()
        {

        }

        private void OnGpsInfoEvt(object sender, DdzbaseEventArgs args)
        {
            var userData = args.IsfObjData;
            var userId = userData.GetInt("uid");

            foreach (UserInfoStruct userinfoStruct in _userinfoDic.Values.Where(userinfoStruct => userId == userinfoStruct.Id))
            {
                JustSetGpsData(userinfoStruct, userData);
                break;
            }
        }

        private void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            UserIdToContry.Clear();
            InitUserDic(args.IsfObjData);
        }

        private void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            UserIdToContry.Clear();
            InitUserDic(args.IsfObjData);
        }

        //静态信息-----------------------start
        [SerializeField]
        protected UITexture HeadTextureLeft;

        [SerializeField]
        protected UITexture HeadTextureRight;

        [SerializeField]
        protected UILabel NameLabelLeft;

        [SerializeField]
        protected UILabel NameLabelRight;

        [SerializeField] protected string Infostr1 = "只有是3人房间，且人满状态下才能调用gps距离";
        //显示GpsInfo
        public void OnShowGpsInfo()
        {
            var leftSeat = App.GetGameData<GlobalData>().GetLeftPlayerSeat;
            var rightSeat = App.GetGameData<GlobalData>().GetRightPlayerSeat;
            if (!_userinfoDic.ContainsKey(leftSeat) || !_userinfoDic.ContainsKey(rightSeat))
            {
                YxMessageBox.Show(Infostr1);
                return;
            }

            GpsUiGob.SetActive(true);

            DDzUtil.LoadRealHeadIcon(_userinfoDic[leftSeat].HeadImgAvatar, _userinfoDic[leftSeat].Sex, HeadTextureLeft);
            DDzUtil.LoadRealHeadIcon(_userinfoDic[rightSeat].HeadImgAvatar, _userinfoDic[rightSeat].Sex, HeadTextureRight);

            NameLabelLeft.text = _userinfoDic[leftSeat].Name;
            NameLabelRight.text = _userinfoDic[rightSeat].Name;

            foreach (var seat in _userinfoDic.Keys)
            {
                if (seat == leftSeat)
                {
   /*                 LeftGpsInfo.text = string.Format("IP:{0}\nID:{1}\n所在地:{2}\n{3}", _userinfoDic[seat].Ip,
                        _userinfoDic[seat].Id, _userinfoDic[seat].Country, (_userinfoDic[seat].GpsX!=-1f) ? "已经提供位置信息" : "未提供位置信息\n请开启位置服务,并给予应用相应权限");*/


                    LeftGpsInfo.text = string.Format("所在地:{0}\n{1}", 
                   _userinfoDic[seat].Country, (_userinfoDic[seat].GpsX != -1f) ? "已经提供位置信息" : "未提供位置信息");

                }else if (seat == rightSeat)
                {
/*                    RightGpsInfo.text = string.Format("IP:{0}\nID:{1}\n所在地:{2}\n{3}", _userinfoDic[seat].Ip,
                        _userinfoDic[seat].Id, _userinfoDic[seat].Country, (_userinfoDic[seat].GpsX != -1f) ? "已经提供位置信息" : "未提供位置信息\n请开启位置服务,并给予应用相应权限");*/

                    RightGpsInfo.text = string.Format(" 所在地:{0}\n{1}",
                        _userinfoDic[seat].Country, (_userinfoDic[seat].GpsX != -1f) ? "已经提供位置信息" : "未提供位置信息");
                }
            }

            if (_userinfoDic[leftSeat].GpsX == -1 || _userinfoDic[rightSeat].GpsX == -1)
            {
                DistanceLabel.text = "距离：0 米";
                return;
            }

            var distance = Distince(_userinfoDic[leftSeat].GpsX, _userinfoDic[leftSeat].GpsY, _userinfoDic[rightSeat].GpsX, _userinfoDic[rightSeat].GpsY);

            string des = "";
            if (distance < 1000)
            {
                if (distance < 100)
                {
                    des = "<=100m";
                }
                else
                {
                    des = string.Format("距离：{0:F}米", distance);
                }

            }
            else
            {
                des = string.Format("距离：{0:F} 千米", distance / 1000f);
            }
            DistanceLabel.text = des;
        }

        /// <summary>
        /// 隐藏GpsInfo
        /// </summary>
        public void OnCloseGpsInfo()
        {
            GpsUiGob.SetActive(false);
        }


        class UserInfoStruct
        {
            public int Id;

            /// <summary>
            /// 地里位置
            /// </summary>
            public string Country;
            /// <summary>
            /// Ip地址
            /// </summary>
            public string Ip;

            /// <summary>
            /// 纬度
            /// </summary>
            public float GpsX=-1;

            /// <summary>
            /// 经度
            /// </summary>
            public float GpsY=-1;

            public string HeadImgAvatar="";

            public short Sex = 0;

            public string Name = "";
        }

        //userid对应的玩家地理位置
        private static readonly Dictionary<int ,string> UserIdToContry = new Dictionary<int, string>();
        /// <summary>
        /// 记录地理位置信息
        /// </summary>
        /// <param name="userData"></param>
        private void SetContryInfo(ISFSObject userData)
        {
            if (userData.ContainsKey(RequestKey.KeyId) && userData.ContainsKey("country"))
            {
                UserIdToContry[userData.GetInt(RequestKey.KeyId)] = userData.GetUtfString("country");
            }
        }

        /// <summary>
        /// 根据玩家id获得地理位置信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetContryInfo(int userId)
        {
            if (UserIdToContry.ContainsKey(userId)) return UserIdToContry[userId];
            return "";
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="userData"></param>
        UserInfoStruct SetUserData(ISFSObject userData)
        {
            SetContryInfo(userData);

            var userinfoStruct = new UserInfoStruct();

            if (userData.ContainsKey(RequestKey.KeyId))
                userinfoStruct.Id = userData.GetInt(RequestKey.KeyId);

            if (userData.ContainsKey("ip"))
                userinfoStruct.Ip = userData.GetUtfString("ip");

            if (userData.ContainsKey("country"))
                userinfoStruct.Country = userData.GetUtfString("country");

            if (userData.ContainsKey(RequestKey.KeyName))
                userinfoStruct.Name = userData.GetUtfString(RequestKey.KeyName);

            //获取gpsx; gpsy
            if ((userData.ContainsKey("gpsx") && userData.ContainsKey("gpsy")) ||
                (userData.ContainsKey("x") && userData.ContainsKey("y")))
            {
                userinfoStruct.GpsX = userData.ContainsKey("gpsx") ? userData.GetFloat("gpsx") : userData.GetFloat("x");
                userinfoStruct.GpsY = userData.ContainsKey("gpsy") ? userData.GetFloat("gpsy") : userData.GetFloat("y");
            }
            else
            {
                userinfoStruct.GpsX = -1f;
                userinfoStruct.GpsY = -1f;
            }

            if (userData.ContainsKey(NewRequestKey.KeyAvatar))
            {
               userinfoStruct.HeadImgAvatar =  userData.GetUtfString(NewRequestKey.KeyAvatar);
            }

            if (userData.ContainsKey(NewRequestKey.KeySex))
            {
                userinfoStruct.Sex = userData.GetShort(NewRequestKey.KeySex);
            }
            return userinfoStruct;
        }


        private void JustSetGpsData(UserInfoStruct userinfoStruct,ISFSObject userData)
        {

            if (userData.ContainsKey("ip"))
                userinfoStruct.Ip = userData.GetUtfString("ip");

            if (userData.ContainsKey("country"))
                userinfoStruct.Country = userData.GetUtfString("country");

            //获取gpsx; gpsy
            if ((userData.ContainsKey("gpsx") && userData.ContainsKey("gpsy")) ||
                (userData.ContainsKey("x") && userData.ContainsKey("y")))
            {
                userinfoStruct.GpsX = userData.ContainsKey("gpsx") ? userData.GetFloat("gpsx") : userData.GetFloat("x");
                userinfoStruct.GpsY = userData.ContainsKey("gpsy") ? userData.GetFloat("gpsy") : userData.GetFloat("y");
            }
            else
            {
                userinfoStruct.GpsX = -1f;
                userinfoStruct.GpsY = -1f;
            }
        }


        private void InitUserDic(ISFSObject data)
        {
            if (data.ContainsKey(RequestKey.KeyUser))
            {
                SetContryInfo(data.GetSFSObject(RequestKey.KeyUser));
            }

            if (!data.ContainsKey(RequestKey.KeyUserList))
            {
                Debug.LogError("此isfobj data  不能获得玩家其他玩家的数据集合");
                return;
            }
            //其他玩家数据集合
            var otherUsers = data.GetSFSArray(RequestKey.KeyUserList);
            foreach (ISFSObject user in otherUsers)
            {
                if (user.ContainsKey(RequestKey.KeySeat))
                {
                    _userinfoDic[user.GetInt(RequestKey.KeySeat)] = SetUserData(user);
                }
            }

            if (data.ContainsKey(NewRequestKey.KeyPlayerNum) && data.GetInt(NewRequestKey.KeyPlayerNum) < 3)
            {
                GpsBtn.SetActive(false);
            }
            else
            {
                GpsBtn.SetActive(true);
            }
        }


        /// <summary>
        /// 根据Gps信息获得2个点之间距离
        /// </summary>
        /// <param name="a1">n</param>
        /// <param name="a2">e</param>
        /// <param name="b1">n</param>
        /// <param name="b2">e</param>
        /// <returns></returns>
        public static double Distince(float a1, float a2, float b1, float b2)
        {
            double R = 6371004;
            double PI_RM = 180 / Math.PI;
            double C = 1 - (Math.Pow((Math.Sin((90 - a2) / PI_RM) * Math.Cos(a1 / PI_RM) - Math.Sin((90 - b2) / PI_RM) * Math.Cos(b1 / PI_RM)), 2) + Math.Pow((Math.Sin((90 - a2) / PI_RM) * Math.Sin(a1 / PI_RM) - Math.Sin((90 - b2) / PI_RM) * Math.Sin(b1 / PI_RM)), 2) + Math.Pow((Math.Cos((90 - a2) / PI_RM) - Math.Cos((90 - b2) / PI_RM)), 2)) / 2;
            return R * Math.Acos(C);
        }
    }
}
