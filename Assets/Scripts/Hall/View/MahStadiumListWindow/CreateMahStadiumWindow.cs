using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    /// <summary>
    /// 创建麻将馆
    /// </summary>
    public class CreateMahStadiumWindow : YxNguiWindow
    {
        public UILabel MahName;
        public UILabel QqAccount;
        public UILabel MobileAccount;
        public UILabel VerificationCode;
        public UILabel ManagerCode;

        public UILabel CommunityLable;
        public UILabel NeedTime;
        public int TimeCtrl;
        public UIGrid GameListGrid;
        public GameObject GameListBg;

        public UIGrid MahTypeGrid;
        public MahTypeItem MahTypeItem;
        public UISprite ServicePriceState;
        public UISprite RealAttestationState;
        public GameObject BuyCardsBtn;
        public GameObject RealAttestationBtn;
        public UILabel CashNum;
        public UILabel MahPassword;
        public UILabel AgainMahPassword;
        public UILabel WeiChatAccount;

        public MahInfoItem MahInfoItem;

        public UILabel MahProvin;
        public UILabel MahCity;
        public UILabel MahRegion;

        public UIGrid MahProvinceGrid;
        public UIGrid MahCityGrid;
        public UIGrid MahRegionGrid;

        public UIScrollView ProvinScrollView;
        public UIScrollView CityScrollView;
        public UIScrollView RegionScrollView;

        public GameObject ProvinceBg;
        public GameObject CityBg;
        public GameObject RegionBg;

        private string _creatMahFunction;
        private string _telVerifyFunction;
        private int _timeCtrl;

        private string _province = "";
        private string _city = "";
        private string _region = "";
        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<string, object>("Verify", obj =>
            {
                RealAttestationBtn.SetActive(false);
                RealAttestationState.spriteName = "201";
            });
        }

        protected override void OnFreshView()
        {
            var dictionary = Data as Dictionary<string, object>;
            if (dictionary == null) return;

            _creatMahFunction = dictionary.ContainsKey("function") ? dictionary["function"].ToString() : "";
            var telVerify = dictionary.ContainsKey("telVerify") ? dictionary["telVerify"] : null;
            if (telVerify is Dictionary<string, object>)
            {
                var telVerifyInfo = telVerify as Dictionary<string, object>;
                _telVerifyFunction = telVerifyInfo.ContainsKey("function") ? telVerifyInfo["function"].ToString() : "";
            }

            var idcardVerify = dictionary.ContainsKey("idcardVerify") && (bool)dictionary["idcardVerify"];
            RealAttestationState.spriteName = idcardVerify ? "201" : "202";
            RealAttestationBtn.SetActive(!idcardVerify);
            var cashEnough = dictionary.ContainsKey("cashEnough") && (bool)dictionary["cashEnough"];
            ServicePriceState.spriteName = cashEnough ? "201" : "202";
            BuyCardsBtn.SetActive(!cashEnough);

            CashNum.text = dictionary.ContainsKey("cashNum") ? dictionary["cashNum"].ToString() : "";

            if (!dictionary.ContainsKey("gameList")) return;
            var gameListData = dictionary["gameList"];
            if (!(gameListData is List<object>)) return;
            var gameLists = gameListData as List<object>;
            while (GameListGrid.transform.childCount > 0)
            {
                DestroyImmediate(GameListGrid.transform.GetChild(0).gameObject);
            }
            foreach (var gameList in gameLists)
            {

                var obj = YxWindowUtils.CreateItem(MahInfoItem, GameListGrid.transform);
                obj.gameObject.SetActive(true);
                if (!(gameList is Dictionary<string, object>)) continue;
                var gameData = gameList as Dictionary<string, object>;
                var gameId = gameData.ContainsKey("game_id") ? gameData["game_id"].ToString() : "";
                var gameM = gameData.ContainsKey("game_m") ? gameData["game_m"].ToString() : "";
                obj.name = gameId;
                obj.InitMahInfoData(gameM);
                NguiAddOnClick(obj.gameObject, (msg) =>
                {
                    var item = YxWindowUtils.CreateItem(MahTypeItem, MahTypeGrid.transform);
                    NguiAddOnClick(item.DeleButton.gameObject, (info) =>
                    {
                        DestroyImmediate(item.gameObject);
                        obj.gameObject.SetActive(true);
                        MahTypeGrid.repositionNow = true;
                        GameListGrid.repositionNow = true;
                    }, item.name);
                    item.name = obj.name;
                    item.InitData(gameM);
                    obj.gameObject.SetActive(false);
                    MahTypeGrid.repositionNow = true;
                    GameListGrid.repositionNow = true;
                }, obj.name);
            }
            GameListGrid.repositionNow = true;
        }

        public void OnCreatMahStadiumBtn()
        {
            var games = "";
            for (var i = 0; i < MahTypeGrid.transform.childCount; i++)
            {
                games = games + MahTypeGrid.transform.GetChild(i).name + ",";
            }
            var dic = new Dictionary<string, object>();
     
            dic["proxy_id"] = ManagerCode.text;
            dic["name_m"] = MahName.text;
            dic["province"] = _province;
            dic["city"] = _city;
            dic["region"] = _region;
            dic["address"] = CommunityLable.text;
            dic["qq_n"] = QqAccount.text;
            dic["mobile_n"] = MobileAccount.text;
            dic["gameid_s"] = games;
            dic["verify"] = VerificationCode.text;
            dic["password"] = MahPassword.text;
            dic["againPassword"] = AgainMahPassword.text;
            dic["wechat_n"] = WeiChatAccount.text;
            Facade.Instance<TwManager>().SendAction(_creatMahFunction, dic, data =>
            {
                Facade.EventCenter.DispatchEvent<string, object>("IdData");
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
            if (data == null) return;
            var message = data.ContainsKey("errorMessage") ? data["errorMessage"].ToString() : "";
            var status = data.ContainsKey("success") && bool.Parse(data["success"].ToString());
            var info = new YxMessageBoxData { Msg = message };

            YxMessageBox.Show(info);
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
            if (_timeCtrl > 0)
            {
                var info = new YxMessageBoxData { Msg = "您发送验证码过于频繁,请等待" };
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
            NeedTime.text = _timeCtrl + "s";
        }
        public void OnFindProvince()
        {
            ProvinceBg.SetActive(!ProvinceBg.activeSelf);
            CityBg.SetActive(false);
            RegionBg.SetActive(false);

            MahCity.text = "";
            MahRegion.text = "";
            if (ProvinceBg.activeSelf == false) return;

            Facade.Instance<TwManager>().SendAction("mahjongwm.provincesAddress", new Dictionary<string, object>(),
                data =>
                {
                    if (data == null) return;
                    var dataInfo = data as Dictionary<string, object>;
                    var prv = dataInfo != null && dataInfo.ContainsKey("prv") ? dataInfo["prv"].ToString() : "";
                    if (prv.Equals("")) return;
                    var provinces = prv.Split(',');
                    Clear(MahProvinceGrid.gameObject);
                    Clear(MahCityGrid.gameObject);
                    Clear(MahRegionGrid.gameObject);

                    foreach (var province in provinces)
                    {
                        var item = YxWindowUtils.CreateItem(MahInfoItem, MahProvinceGrid.transform);
                        item.InitMahInfoData(province);
                        item.name = province;
                        NguiAddOnClick(item.gameObject, obj =>
                        {
                            MahProvin.text = obj.name;
                            _province = obj.name;
                            obj.GetComponent<UIDragScrollView>().scrollView = ProvinScrollView;
                            ProvinceBg.SetActive(false);
                        }, item.name);
                    }
                    MahProvinceGrid.repositionNow = true;
                    ProvinScrollView.ResetPosition();
                });
        }

        public void OnFindCity()
        {
            ProvinceBg.SetActive(false);
            CityBg.SetActive(!CityBg.activeSelf);
            RegionBg.SetActive(false);

            MahRegion.text = "";
            if (CityBg.activeSelf == false) return;
            var dic = new Dictionary<string, object>();
            dic["province"] = _province;
            Facade.Instance<TwManager>().SendAction("mahjongwm.provincesAddress", dic, data =>
            {
                if (data == null) return;
                var dataInfo = data as Dictionary<string, object>;
                var city = dataInfo != null && dataInfo.ContainsKey("city") ? dataInfo["city"].ToString() : "";
                if (city.Equals("")) return;
                var citys = city.Split(',');

                Clear(MahCityGrid.gameObject);
                Clear(MahRegionGrid.gameObject);

                foreach (var cityName in citys)
                {
                    var item = YxWindowUtils.CreateItem(MahInfoItem, MahCityGrid.transform);
                    item.InitMahInfoData(cityName);
                    item.name = cityName;
                    NguiAddOnClick(item.gameObject, obj =>
                    {
                        MahCity.text = obj.name;
                        _city = obj.name;
                        obj.GetComponent<UIDragScrollView>().scrollView = CityScrollView;
                        CityBg.SetActive(false);
                    }, item.name);
                }
                MahCityGrid.repositionNow = true;
                CityScrollView.ResetPosition();
            });
        }

        public void OnFindRegion()
        {
            ProvinceBg.SetActive(false);
            CityBg.SetActive(false);
            RegionBg.SetActive(!RegionBg.activeSelf);
            if (RegionBg.activeSelf == false) return;
            var dic = new Dictionary<string, object>();
            dic["province"] = _province;
            dic["city"] = _city;
            Facade.Instance<TwManager>().SendAction("mahjongwm.provincesAddress", dic, data =>
            {
                if (data == null) return;
                var dataInfo = data as Dictionary<string, object>;
                var region = dataInfo != null && dataInfo.ContainsKey("region") ? dataInfo["region"] : null;
                if (region == null) return;
                Clear(MahRegionGrid.gameObject);
                if (region is List<object>)
                {
                    var regionDatas = region as List<object>;
                    foreach (var regionData in regionDatas)
                    {
                        if (regionData is string)
                        {
                            var regionInfo = regionData as string;
                            var item = YxWindowUtils.CreateItem(MahInfoItem, MahRegionGrid.transform);
                            item.InitMahInfoData(regionInfo);
                            item.name = regionInfo;
                            NguiAddOnClick(item.gameObject, obj =>
                            {
                                MahRegion.text = obj.name;
                                _region = obj.name;
                                obj.GetComponent<UIDragScrollView>().scrollView = RegionScrollView;
                                RegionBg.SetActive(false);
                            }, item.name);
                        }
                    }
                    MahRegionGrid.repositionNow = true;
                    RegionScrollView.ResetPosition();
                }
            });
        }

        public void OnCtrlGameListBg()
        {
            GameListBg.SetActive(!GameListBg.activeSelf);
        }

        private void NguiAddOnClick(GameObject gob, UIEventListener.VoidDelegate callback, string id)
        {
            var uiel = UIEventListener.Get(gob);
            uiel.onClick = callback;
            uiel.parameter = id;
        }

        private void Clear(GameObject obj)
        {
            while (obj.transform.childCount > 0)
            {
                DestroyImmediate(obj.transform.GetChild(0).gameObject);
            }
        }
    }
}
