/** 
 *文件名称:     HeadItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-08-29 
 *描述:         回放记录中玩家头像信息
 *历史记录: 
*/

using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class HeadItem : YxView
    {
        public YxBaseTextureAdapter ShowHead;
        public UILabel ShowUserName;
        public UILabel ShowId;
        public UILabel ShowTotalScore;
        public Color SelfColor = Color.yellow;
        public bool SelfSpecialColorl;

        protected virtual void RefreshItem(Dictionary<string, object> dic)
        {
            var data = new HeadData(dic);
            RefreshItem(data);
        }

        protected virtual void RefreshItem(HeadData data)
        {
            ShowUserName.TrySetComponentValue(data.UserName);
            ShowId.TrySetComponentValue(data.UserId);
            ShowTotalScore.TrySetComponentValue(YxUtiles.GetShowNumber(data.UserScore).ToString(CultureInfo.InvariantCulture));
            if (SelfSpecialColorl)
            {
                if (data.UserId.Equals(App.UserId))
                {
                    if (ShowUserName)
                    {
                        ShowUserName.color = SelfColor;
                        ShowTotalScore.color = new Color32(180, 16, 16, 255);
                    }
                }
            }
            if (ShowHead)
            {
                PortraitDb.SetPortrait(data.HeadUrl, ShowHead, data.UserSex);
            }
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data == null)
            {
                return;
            }
            if (Data is HeadData)
            {
                RefreshItem(Data as HeadData);
            }
            else if (Data is Dictionary<string, object>)
            {
                RefreshItem((Dictionary<string, object>)Data);
            }

        }
    }

    public class HeadData
    {
        #region Keys
        public const string KeyName = "name";
        public const string KeyId = "id";
        public const string KeyScore = "gold";
        public const string KeyAvatar = "avatar";
        public const string KeyAvatarX = "avatar_x";
        public const string KeySex = "sex_i";
        public const string KeySeat = "seat";
        #endregion
        protected string _headUrl;
        protected string _userName;
        protected string _userId;
        protected long _userScore;
        protected int _userSex;
        protected int _seat;

        public string UserName
        {
            get { return _userName; }
        }

        public int UserSex
        {
            get { return _userSex; }
        }

        public string UserId
        {
            get
            {
                return _userId;
            }
        }

        public long UserScore
        {
            get { return _userScore; }
        }

        public string HeadUrl
        {
            get { return _headUrl; }
        }

        public int Seat
        {
            get { return _seat; }
        }

        public HeadData(Dictionary<string, object> dic)
        {
            DealInfo(dic);
        }
        /// <summary>
        /// 茶馆头像信息兼容处理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public HeadData(string id, object data)
        {
            DealInfo(data as Dictionary<string, object>);
            _userId = id;
        }

        protected virtual void DealInfo(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey(out _userId, KeyId);
            dic.TryGetValueWitheKey(out _userName, KeyName);
            dic.TryGetValueWitheKey(out _userScore, KeyScore);
            dic.TryGetValueWitheKey(out _userSex, KeySex, 1);
            if (dic.ContainsKey(KeyAvatar))
            {
                _headUrl = dic[KeyAvatar].ToString();
            }
            if (dic.ContainsKey(KeyAvatarX))
            {
                _headUrl = dic[KeyAvatarX].ToString();
            }
            if (dic.ContainsKey(KeySeat))
            {
                dic.TryGetValueWitheKey(out _seat, KeySeat);
            }
        }
    }
}
