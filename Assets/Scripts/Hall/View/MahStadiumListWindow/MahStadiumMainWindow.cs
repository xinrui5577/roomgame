using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Common.Windows.TabPages;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahStadiumMainWindow : YxNguiWindow
    {

        # region 界面的左边部分
        public UIGrid GoodsGrid;
        public GoodsItem GoodsItem;

        public void OnRecommend(UIToggle toggle)
        {
            if (!toggle.value) return;
            Facade.Instance<TwManager>().SendAction("mahjongwm.mahGoods", new Dictionary<string, object>(), OnFreshRecommendData);
        }
        public void OnLeague(UIToggle toggle)
        {
            if (!toggle.value) return;
            Facade.Instance<TwManager>().SendAction("mahjongwm.newJoin", new Dictionary<string, object>(), OnFreshLeagueData);
        }
        private void OnFreshRecommendData(object obj)
        {
            Clear(GoodsGrid.gameObject);
            var data = (IDictionary)obj;
            if (!data.Contains("goodsTop")) return;
            var record = 0;
            var info = data["goodsTop"];
            var goodsInfos = (List<object>)info;
            foreach (var goodsInfo in goodsInfos)
            {
                var goods = (Dictionary<string, object>)goodsInfo;
                var roomName = goods.ContainsKey("name_m") ? goods["name_m"].ToString() : "";
                var commend = goods.ContainsKey("good_s") ? goods["good_s"] : null;
                var mahId = goods.ContainsKey("mah_id") ? goods["mah_id"].ToString() : "";
                var item = YxWindowUtils.CreateItem(GoodsItem, GoodsGrid.transform);
                item.InitInfo(record++, roomName, mahId);
                NguiAddOnClick(item.gameObject, obj1 =>
                {
                    if (PlayerPrefs.HasKey("selectGame"))
                    {
                        PlayerPrefs.DeleteKey("selectGame");
                    }
                    var win = (FindMahStadiumWindow)CreateChildWindow("FindMahStadiumWindow");
                    win.MahNum = item.MahStadimId.text.ToString();
                    win.OnFindRoom();
                }, item.name);
            }
            GoodsGrid.repositionNow = true;
            GoodsGrid.Reposition();
        }

        private void OnFreshLeagueData(object obj)
        {
            Clear(GoodsGrid.gameObject);
            var data = (IDictionary)obj;
            if (!data.Contains("newTop")) return;
            var record = 0;
            var info = data["newTop"];
            var goodsInfos = (List<object>)info;
            foreach (var goodsInfo in goodsInfos)
            {
                var goods = (Dictionary<string, object>)goodsInfo;
                var roomName = goods.ContainsKey("name_m") ? goods["name_m"] : null;
                var commend = goods.ContainsKey("good_s") ? goods["good_s"] : null;
                var mahId = goods.ContainsKey("mah_id") ? goods["mah_id"] : null;
                var item = YxWindowUtils.CreateItem(GoodsItem, GoodsGrid.transform);
                item.InitInfo(record++, (string)roomName, (string)mahId);
                NguiAddOnClick(item.gameObject, obj1 =>
                {
                    var win = (FindMahStadiumWindow)CreateChildWindow("FindMahStadiumWindow");
                    Debug.LogError("打开查找界面");
                    win.MahNum = item.MahStadimId.text.ToString();
                    win.OnFindRoom();
                }, item.name);
            }
            GoodsGrid.repositionNow = true;
            GoodsGrid.Reposition();
        }

        # endregion
        #region 界面的右边部分

        public UIGrid MahInfoGrid;
        public MahInfoItem MahInfoItem;
        public UISprite MahInfoBg;
        public UILabel SelectMahName;


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

        public UILabel Address;

        public UIGrid MahStadiumListGrid;
        public MahStadiumItem MahStadiumItem;

        public GameObject ReturnHallMian;
        public GameObject ReturnLastBtn;
        public GameObject MySelectParent;
        public GameObject MahStadiumListParent;

        public UILabel InputPwdLabel;
        private  string _croomId = "";
        private  string _selectGame;
        private GameObject _currentObj;
        private bool _isPwd;
        private  string _province = "";
        private  string _city = "";
        private  string _region = "";

        public void OnMahType()
        {
            Facade.Instance<TwManager>().SendAction("mahjongwm.mahChoose", new Dictionary<string, object>(), OnFreshMahData);
        }

        public void OnReturnLast()
        {
            ReturnLastBtn.SetActive(false);
            ReturnHallMian.SetActive(true);
            MySelectParent.SetActive(true);
            MahStadiumListParent.SetActive(false);
        }



        public void OnSureBtn()
        {
            var dic = new Dictionary<string, object>();
            if (!_province.Equals(""))
            {
                dic["province"] = _province;
                dic["city"] = _city;
                dic["region"] = _region;
            }
            dic["game_id"] = _croomId;
            PlayerPrefs.SetString("selectGame", _selectGame);
            Facade.Instance<TwManager>().SendAction("mahjongwm.mahjongList", dic, OnFreshMahList);
        }

        public void CloseCurrentBg(GameObject obj)
        {
            if (obj.activeSelf) obj.SetActive(false);
        }
        public void OnFindProvince()
        {
            ProvinceBg.SetActive(true);
            CityBg.SetActive(false);
            RegionBg.SetActive(false);

            MahCity.text = "";
            _city = "";
            MahRegion.text = "";
            _region = "";
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
            CityBg.SetActive(true);
            RegionBg.SetActive(false);

            MahRegion.text = "";
            _region = "";
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
            RegionBg.SetActive(true);
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

        private void OnFreshMahData(object data)
        {
            Clear(MahInfoGrid.gameObject);
            var infos = (List<object>)data;
            foreach (var info in infos)
            {
                var mahInfo = (Dictionary<string, object>)info;
                var gameId = mahInfo.ContainsKey("game_id") ? mahInfo["game_id"].ToString() : "";
                var mahName = mahInfo.ContainsKey("game_m") ? mahInfo["game_m"].ToString() : "";
                var item = YxWindowUtils.CreateItem(MahInfoItem, MahInfoGrid.transform);
                item.InitMahInfoData(mahName);
                item.name = gameId;
                NguiAddOnClick(item.gameObject, obj =>
                {
                    _croomId = obj.name;
                    _selectGame = mahName;
                    SelectMahName.text = obj.GetComponent<UILabel>().text;
                    MahInfoBg.gameObject.SetActive(false);
                }, item.name);
            }
            MahInfoGrid.repositionNow = true;
            MahInfoGrid.Reposition();
            MahInfoBg.gameObject.SetActive(true);
        }

        private void OnFreshMahList(object data)
        {
            var dataIfo = data as IDictionary;
            if (dataIfo == null) return;

            MySelectParent.SetActive(false);
            if (!dataIfo.Contains("mahList")) return;
            var mahLists = dataIfo["mahList"];
            if (!(mahLists is List<object>)) return;
            Clear(MahStadiumListGrid.gameObject);
            var mahL = mahLists as List<object>;
            var index = 0;
            foreach (var mahInfo in mahL)
            {
                if (!(mahInfo is Dictionary<string, object>)) continue;
                var info = mahInfo as Dictionary<string, object>;
                var mahId = info["mah_id"];
                var mahStadiumName = info["name_m"];
                var goodCommend = info["mah_id"];
                var isUsepwd = info["is_usepwd"];
                var obj = Instantiate(MahStadiumItem);
                obj.gameObject.SetActive(true);
                obj.name = (string)mahId;
                obj.InitMahStadiumInfo(index++, (string)mahStadiumName, (string)goodCommend, int.Parse(isUsepwd.ToString()));
                obj.transform.parent = MahStadiumListGrid.transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.transform.localRotation = Quaternion.identity;
                NguiAddOnClick(obj.gameObject, obj1 =>
                {
                    var win = (FindMahStadiumWindow)CreateChildWindow("FindMahStadiumWindow");
                    win.MahNum = obj.name;
                    win.OnFindRoom();
                }, obj.name);
            }
            MahStadiumListGrid.repositionNow = true;
            MahStadiumListGrid.Reposition();
            MahStadiumListParent.SetActive(true);
            ReturnLastBtn.SetActive(true);
            ReturnHallMian.SetActive(false);
        }
        public void JoinMahStadium(GameObject obj)
        {
            if (obj.GetComponent<MahStadiumItem>().IsUsepwd == 1)
            {
                _isPwd = true;
                _currentObj = obj;
            }
            else
            {
                OpenCreatGameWindow(obj);
            }
        }

        public void RequestJoinMah()
        {
            OpenCreatGameWindow(_currentObj);
        }

        public void OpenCreatGameWindow(GameObject obj)
        {
            var dic = new Dictionary<string, object>();
            if (_isPwd)
            {
                dic["comm_id"] = InputPwdLabel.text;
            }
            dic["mah_id"] = obj.name;
            Facade.Instance<TwManager>().SendAction("mahjongwm.findWmRoom", dic, (info) =>
                {
                    var data = info as IDictionary<string, object>;
                    if (data == null)
                    {
                        YxMessageBox.Show("没有找到麻将馆！！");
                        return;
                    }
                    var str = data.ContainsKey(RequestKey.KeyMessage) ? data[RequestKey.KeyMessage] : null;
                    if (str != null)
                    {
                        YxMessageBox.Show(str.ToString());
                        return;
                    }
                    var child = CreateChildWindow("MahStadiumCreatGameWindow");
                    child.UpdateView(info);
                    MySelectParent.SetActive(true);
                    MahStadiumListParent.SetActive(false);
                    _currentObj = null;
                    _isPwd = false;
                });

        }

        #endregion
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
        public void OnQuickGame(GameObject obj)
        {
            RoomListController.Instance.QuickGame(obj.name);
        }
        # region 我的创建的请求

        public GameObject MyCreatBg;
        public YxTabItem MyCreatItem;
        public UIGrid MyCreatGrid;
        public string[] OpenWindowsNames;

        private List<object> _lableShows;
        public void OnMyCreatWindow()
        {
            Facade.Instance<TwManager>().SendAction("mahjongwm.myCreate", new Dictionary<string, object>(), OnFreshData);
        }

        private void OnFreshData(object msg)
        {
            MyCreatBg.SetActive(true);
            while (MyCreatGrid.transform.childCount > 0)
            {
                DestroyImmediate(MyCreatGrid.transform.GetChild(0).gameObject);
            }

            var datas = msg as IDictionary;
            if (datas == null) return;

            if (!datas.Contains("myCreateCfg")) return;
            var data = datas["myCreateCfg"];
            if (data is List<object>)
            {
                _lableShows = data as List<object>;
                var index = 0;
                foreach (var lableShow in _lableShows)
                {
                    if (!(lableShow is Dictionary<string, object>)) continue;
                    var obj = Instantiate(MyCreatItem);
                    obj.gameObject.SetActive(true);
                    obj.transform.parent = MyCreatGrid.transform;
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    var dictionary = lableShow as Dictionary<string, object>;
                    var text = dictionary.ContainsKey("text") ? dictionary["text"].ToString() : "";
                    var function = dictionary.ContainsKey("function") ? dictionary["function"].ToString() : "";
                    var clickEnable = dictionary.ContainsKey("clickEnable") ? dictionary["clickEnable"].ToString() : "";
                    var show = dictionary.ContainsKey("show") ? dictionary["show"].ToString() : "";
                    var showActive = show != "false";
                    var showEnable = clickEnable != "false";
                    obj.gameObject.SetActive(showActive);
                    obj.name = index.ToString(CultureInfo.InvariantCulture);
                    obj.GetComponent<BoxCollider>().enabled = showEnable;
                    obj.GetComponent<UIToggle>().startsActive = showEnable;
                    obj.DownNameLabel.text = text;
                    obj.NameLabel.text = text;
                    index++;
                }
            }
            MyCreatGrid.repositionNow = true;
        }
        public void OnRequestBtn(UIToggle toggle)
        {
            if (!toggle.value) return;
            var index = int.Parse(toggle.name);
            if (_lableShows[index] == null) return;
            var win = YxWindowManager.OpenWindow(OpenWindowsNames[index]);
            if (win == null) return;
            win.UpdateView(_lableShows[index]);
        }

        public void OnQuickJoin()
        {
            CreateOtherWindow("FindMahStadiumWindow");
        }
        # endregion
    }
}
