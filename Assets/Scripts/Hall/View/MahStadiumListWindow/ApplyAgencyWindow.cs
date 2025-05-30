using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    /// <summary>
    /// 创建代理
    /// </summary>
    public class ApplyAgencyWindow : YxNguiWindow
    {
        public UILabel AgentId;
        public UILabel QqAccount;
        public UILabel MobileAccount;
        public UILabel VerificationCode;
        public UILabel ProvinceLable;
        public UILabel CityLable;
        public UILabel RegionLable;
        public UILabel NeedTime;
        public int TimeCtrl;
        public UISprite ServicePriceState;
        public UISprite RealAttestationState;
        public GameObject BuyCardsBtn;
        public GameObject RealAttestationBtn;
        public UILabel CashNum;
        public UILabel WeiChatAccount;

        private string _applyAgencyFunction;
        private string _telVerifyFunction;
        private int _timeCtrl;


        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<string,object>("Verify", obj => {
                RealAttestationBtn.SetActive(false);
                RealAttestationState.spriteName = "201";
            });
        }

        protected override void OnFreshView()
        {
            var dictionary = Data as Dictionary<string, object>;
            if (dictionary==null) return;
            _applyAgencyFunction = dictionary.ContainsKey("function") ? dictionary["function"].ToString() : "";
            var loginName = dictionary.ContainsKey("login_m") ? dictionary["login_m"].ToString() : "";
            var lBsData = dictionary.ContainsKey("LBSData") ? dictionary["LBSData"] : null;
            if (lBsData != null)
            {
                if (!(lBsData is Dictionary<string, object>)) return;
                var posData = lBsData as Dictionary<string, object>;
                var province = posData.ContainsKey("province") ? posData["province"] : null;
                if (province is Dictionary<string, object>)
                {
                    var provinceInfo = province as Dictionary<string, object>;
                    var provinceId = provinceInfo.ContainsKey("id") ? provinceInfo["id"].ToString() : "";
                    var provinceText = provinceInfo.ContainsKey("text") ? provinceInfo["text"].ToString() : "";
                    ProvinceLable.name = provinceId;
                    ProvinceLable.text = provinceText;
                }
                var city = posData.ContainsKey("city") ? posData["city"] : null;
                if (city is Dictionary<string, object>)
                {
                    var cityInfo = city as Dictionary<string, object>;
                    var cityId = cityInfo.ContainsKey("id") ? cityInfo["id"].ToString() : "";
                    var cityText = cityInfo.ContainsKey("text") ? cityInfo["text"].ToString() : "";
                    CityLable.name = cityId;
                    CityLable.text = cityText;
                }
                var region = posData.ContainsKey("region") ? posData["region"] : null;
               
                var regionInfo = region as Dictionary<string, object>;
                if (regionInfo==null) return;
                var regionId = regionInfo.ContainsKey("id") ? regionInfo["id"].ToString() : "";
                var regionText = regionInfo.ContainsKey("text") ? regionInfo["text"].ToString() : "";
                RegionLable.name = regionId;
                RegionLable.text = regionText;
            }
            var telVerify = dictionary.ContainsKey("telVerify") ? dictionary["telVerify"] : null;
            var telVerifyInfo = telVerify as Dictionary<string, object>;
            if (telVerifyInfo == null) return;
            _telVerifyFunction = telVerifyInfo.ContainsKey("function") ? telVerifyInfo["function"].ToString() : "";

            var idcardVerify = dictionary.ContainsKey("idcardVerify") && (bool)dictionary["idcardVerify"];
            RealAttestationState.spriteName = idcardVerify ? "201" : "202";
            RealAttestationBtn.SetActive(!idcardVerify);
            var cashEnough = dictionary.ContainsKey("cashEnough") && (bool)dictionary["cashEnough"];
            ServicePriceState.spriteName = cashEnough ? "201" : "202";
            BuyCardsBtn.SetActive(!cashEnough);

            CashNum.text = dictionary.ContainsKey("cashNum") ? dictionary["cashNum"].ToString() : "";
        }

        public void OnCreatAgency()
        {
            var dic = new Dictionary<string, object>();
            dic["proxy_id"] = AgentId.text;
            dic["province"] = ProvinceLable.name;
            dic["city"] = CityLable.name;
            dic["region"] = RegionLable.name;
            dic["qq_n"] = QqAccount.text;
            dic["mobile_n"] = MobileAccount.text;
            dic["verify"] = VerificationCode.text;
            dic["weiChat_n"] = WeiChatAccount.text;
            Facade.Instance<TwManager>().SendAction(_applyAgencyFunction, dic, data =>
            {
                Facade.EventCenter.DispatchEvent<string,object>("IdData");
                Facade.Instance<TwManager>().SendAction("optionSwitch", new Dictionary<string, object>(), OnUpdateLogo, false);
                Close();
            }, true, OnFreshData);
        }

        private static void OnUpdateLogo(object msg)
        {
            HallModel.Instance.Convert(msg);
            HallModel.Instance.Save();
            Facade.EventCenter.DispatchEvent<string, object>("HallWindow_hallMenuChange");
        }

        private void OnFreshData(object msg)
        {
            var data = msg as Dictionary<string, object>;
            if (data==null) return;
           
            var message = data.ContainsKey("errorMessage") ? data["errorMessage"].ToString() : "";
            var status = data.ContainsKey("success") && bool.Parse(data["success"].ToString());
            var callback = new YxMessageBoxData { Msg = message };
            YxMessageBox.Show(callback);
            if (!status) return;
            _timeCtrl = TimeCtrl;
            InvokeRepeating("ComputTime", 0, 1);
        }

        public void OnOpenIdcardVerify()
        {
            YxWindowManager.OpenWindow("AttestatWindow");
        }

        public void OnMobileSend()
        {
            if (MobileAccount.text.Equals(""))
            {
                var info = new YxMessageBoxData { Msg = "您填写的手机号为空请检查后重试" };
                YxMessageBox.Show(info);
                return;
            }
            if (_timeCtrl >0)
            {
                var info = new YxMessageBoxData { Msg = "您发送验证码过于频繁，请等待" };
                YxMessageBox.Show(info);
                return;
            }
            var dic = new Dictionary<string, object>();
            dic["mobile_n"] = MobileAccount.text;
            Facade.Instance<TwManager>().SendAction(_telVerifyFunction, dic, data =>
                {
                    _timeCtrl = TimeCtrl;
                    InvokeRepeating("ComputTime", 0, 1);
                });
          
        }

        private void ComputTime()
        {
            if (_timeCtrl <= 0)
            {
                NeedTime.text = "";
                CancelInvoke("ComputTime");
                return;
            }
            _timeCtrl--;
            NeedTime.text = _timeCtrl+"s";
        }
    }
}
