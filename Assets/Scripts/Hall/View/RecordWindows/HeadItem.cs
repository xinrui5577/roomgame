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
using Assets.Scripts.Common.components;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class HeadItem : YxView
    {
        public UITexture ShowHead;
        public UILabel ShowUserName;
        public UILabel ShowId;
        public UILabel ShowTotalScore;
        public Color SelfColor = Color.yellow;
        public bool SelfSpecialColorl;
        private HeadData _data;
        private void RefreshItem(Dictionary<string, object> dic)
        {
            _data=new HeadData(dic);
            YxTools.TrySetComponentValue(ShowUserName, _data.UserName);
            YxTools.TrySetComponentValue(ShowId, _data.UserId);
            YxTools.TrySetComponentValue(ShowTotalScore, _data.UserScore);
            if (SelfSpecialColorl)
            {
                if (_data.UserId.Equals(App.UserId))
                {
                    if (ShowUserName)
                    {
                        ShowUserName.color = SelfColor;
                    }
                }
            } 
            PortraitRes.SetPortrait(_data.HeadUrl, ShowHead,_data.UserSex);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if(Data==null)
            {
                return;
            }
            RefreshItem((Dictionary<string, object>) Data);
        }
    }

    public class HeadData
    {
        #region Keys
        private string _keyName= "name";
        private string _keyId= "id";
        private string _keyScore= "gold";
        private string _keyAvatar= "avatar_x";
        private string _keySex= "sex_i";
        #endregion
        private string _headUrl;
        private string _userName;
        private string _userId;
        private string _userScore;
        private int _userSex;

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

        public string UserScore
        {
            get { return _userScore; }
        }

        public string HeadUrl
        {
            get { return _headUrl; }
        }

        public HeadData(Dictionary<string,object>dic)
        {
            _userId = dic[_keyId].ToString();
            _userName = dic[_keyName].ToString();
            _userScore = dic[_keyScore].ToString();
            _userSex = dic.ContainsKey(_keySex) ? int.Parse(dic[_keySex].ToString()) : 1;
            _headUrl = dic.ContainsKey(_keyAvatar) ? dic[_keyAvatar].ToString() :"";
        }
    }
}
