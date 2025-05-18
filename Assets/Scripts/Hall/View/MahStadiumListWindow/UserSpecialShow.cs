using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class UserSpecialShow : MonoBehaviour
    {
        public UISprite[] Stars;
        public UILabel UserState;
        public UILabel AttestatLable;
        public string BlackName;
        public string NormalName;

        public string Attestated;
        public string NoAttestated;

        private static bool _isReal;

        public void Awake()
        {
            Facade.EventCenter.AddEventListener<string,object>("IdData", obj =>
            {
                GetData();
            });
            GetData();
        }

        private void GetData()
        {
            var dic = new Dictionary<string, object>();
//#if  UNITY_ANDROID
//            dic["youIdToken"] = UMPushAndroid.getRegistrationId();
//#endif
//            if (Application.platform == RuntimePlatform.IPhonePlayer)
//            {
//                dic["youIdToken"] = PlayerPrefs.GetString("uToken");
//            }
            Facade.Instance<TwManager>().SendAction("mahjongwm.wmIdData", dic, OnFreshData);
        }

        private void OnFreshData(object data)
        {
            var userData = data as Dictionary<string, object>;
            if (userData == null) return;
            _isReal = userData.ContainsKey("real_m") && bool.Parse(userData["real_m"].ToString());
            AttestatLable.text = _isReal ?string.Format("[{0}]已认证", Attestated) : string.Format("[{0}]未认证", NoAttestated);
            var agency = userData.ContainsKey("agency") ? userData["agency"].ToString() : "";
            var mahjong = userData.ContainsKey("mahjong") ? userData["mahjong"].ToString() : "";
            if (UserState != null) UserState.text = mahjong + " " + agency;
           
            var star = userData.ContainsKey("star") ? userData["star"].ToString() : "";
            if (star.Equals("")) return;
            var stars = star.Split(',');
            for (int i = 0; i < stars.Length; i++)
            {
                if (Stars[i] == null) return;
                Stars[i].spriteName = int.Parse(stars[i]) == 0 ? BlackName : NormalName;
            }
        }

        public void OnClickViefy()
        {
            if (_isReal)
            {
                var info = new YxMessageBoxData { Msg = "您已经实名验证..." };
                YxMessageBox.Show(info);
            }
            else
            {
                YxWindowManager.OpenWindow("AttestatWindow");
            }
        }
    }
}
