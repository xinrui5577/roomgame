using System;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaSingleInfoWindow : YxWindow
    {
        private YxEUIType _uiType;

        public UILabel TeaId;
        public UILabel TeaName;
        public UILabel TeaRoomCount;
        public UILabel TeaOwnerId;
        public UILabel TeaSign;

        public string KeyTeaId = "_teaId";
        /// <summary>
        /// 用于存储茶馆ID的key
        /// </summary>
        private string SaveTeaId
        {
            get { return string.Format("{0}_{1}_{2}", Application.bundleIdentifier, App.UserId, KeyTeaId); }
        }

        private string _teaId;

        protected override void OnFreshView()
        {
            var teaInfo = Data as Dictionary<string, object>;
            if (teaInfo != null)
            {
                _teaId = teaInfo.ContainsKey("tea_id") ? teaInfo["tea_id"].ToString() : null;
                var teaName = teaInfo.ContainsKey("tea_name") ? teaInfo["tea_name"] : null;
                var groupSign = teaInfo.ContainsKey("group_sign") ? teaInfo["group_sign"] : null;
                var ownerId = teaInfo.ContainsKey("user_id") ? teaInfo["user_id"] : null;
                var numTotal = teaInfo.ContainsKey("num_c") ? teaInfo["num_c"] : null;
                var curRooms = teaInfo.ContainsKey("curRooms") ? teaInfo["curRooms"] : null;
                var roomNum = string.Format("{0}/{1}", curRooms, numTotal);

                if (!string.IsNullOrEmpty(_teaId ) && TeaId) TeaId.text = _teaId;
                if (teaName != null && TeaName) TeaName.text = teaName.ToString();
                if (TeaRoomCount) TeaRoomCount.text = roomNum;
                if (ownerId != null && TeaOwnerId) TeaOwnerId.text = ownerId.ToString();
                if (groupSign != null && TeaSign) TeaSign.text = groupSign.ToString();
            }
        }

        public void OpenFindTeaWindow()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["id"] = _teaId;
            Facade.Instance<TwManager>().SendAction("group.teaGetIn", dic, GetInTea);

        }

        private void GetInTea(object msg)
        {
            var dic = (Dictionary<string, object>)msg;
            var value = int.Parse(dic["mstatus"].ToString());
            if (value != 4)
            {
                var obj = YxWindowManager.OpenWindow("TeaPanel");
                var panel = obj.GetComponent<TeaPanel>();
                panel.UpdateView(dic);
                panel.SetTeaCode(int.Parse(_teaId));
                PlayerPrefs.SetString(SaveTeaId, _teaId);
                Close();
            }
            else
            {
                YxMessageBox.Show("您查找的麻将馆不存在或者您填写的麻将馆号不对");
            }
        }


        public override YxEUIType UIType
        {
            get { return _uiType; }
        }
    }
}
