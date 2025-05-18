using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using com.yxixia.utile.Utiles;
using UnityEngine;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahStadiumCreatGameWindow : YxNguiWindow
    {
        public UILabel MahStadiumName;
        public UILabel MahStadiumId;
        public UILabel OnlinePeople;
        public UILabel MahProvince;
        public UILabel CurrentGameName;

        public GameObject GameBg;
        public MahInfoItem MahInfoItem;

        public UIGrid GameGrid;

        public UIScrollView GameScrollView;

        private string _currentGameKey;
        private string _mahId;
        private int _freshIndex;
        protected override void OnFreshView()
        {
            var mahInfo = Data as Dictionary<string, object>;
            if (mahInfo == null) return;

            _freshIndex++;
            var mahDatas = mahInfo.ContainsKey("data") ? mahInfo["data"] : null;
            var mahData = mahDatas as Dictionary<string, object>;
            if (mahData == null) return;


            var provinceName = mahData.ContainsKey("provinceName")
                                   ? mahData["provinceName"].ToString()
                                   : "";
            var onlineNum = mahData.ContainsKey("onlineNum") ? mahData["onlineNum"].ToString() : "";

            _mahId = mahData.ContainsKey("mah_id") ? mahData["mah_id"].ToString() : "";
            var mahName = mahData.ContainsKey("name_m") ? mahData["name_m"].ToString() : "";
            MahStadiumName.text = mahName;
            MahStadiumId.text =_mahId;
            OnlinePeople.text = string.Format("[BA8B6FFF]即时在线[-] [FD4F03FF]{0}[-][BA8B6FFF] 人[-]", onlineNum);
            MahProvince.text = provinceName;

            var gameNameData = mahData.ContainsKey("gameName_s") ? mahData["gameName_s"] : null;
            var gameKeyData = mahData.ContainsKey("gameKey_s") ? mahData["gameKey_s"] : null;
            if (gameNameData is List<object> && gameKeyData is List<object>)
            {
                var gameNames = gameNameData as List<object>;
                var gameKeys = gameKeyData as List<object>;
                while (GameGrid.transform.childCount > 0)
                {
                    DestroyImmediate(GameGrid.transform.GetChild(0).gameObject);
                }
                for (int i = 0; i < gameNames.Count; i++)
                {
                    var s = gameNames[i] as string;
                    if (s != null && gameKeys[i] is string)
                    {
                        var selectGame = s;
                        var gameKey = (string)gameKeys[i];
                        var item = YxWindowUtils.CreateItem(MahInfoItem, GameGrid.transform);
                        item.InitMahInfoData(selectGame);
                        item.name = gameKey;
                        if (PlayerPrefs.HasKey("selectGame"))
                        {
                            if (PlayerPrefs.GetString("selectGame").Equals(selectGame))
                            {
                                CurrentGameName.text = selectGame;
                                _currentGameKey = item.name;
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                CurrentGameName.text = selectGame;
                                _currentGameKey = item.name;
                            }
                        }
                        
                        NguiAddOnClick(item.gameObject, obj =>
                        {
                            GameBg.SetActive(false);
                            CurrentGameName.text = selectGame;
                            _currentGameKey = item.name;
                            obj.GetComponent<UIDragScrollView>().scrollView = GameScrollView;
                        }, item.name);
                    }
                }

                if (_freshIndex == 3)
                {
                    if (PlayerPrefs.HasKey("selectGame"))
                    {
                        PlayerPrefs.DeleteKey("selectGame");
                    }
                }
                GameGrid.repositionNow = true;
                GameScrollView.ResetPosition();
            }
        
        }

        public void ShowGameName()
        {
            GameBg.SetActive(!GameBg.activeSelf);
        }

        public void OnCreatRoom()
        {
            var win = (CreateRoomWindow) CreateChildWindow("CreateRoomWindow");
            win.GameKey = _currentGameKey;
            win.IsDesignated = true;
            win.FromInfo = _mahId;
        }

        private void NguiAddOnClick(GameObject gob, UIEventListener.VoidDelegate callback, string id)
        {
            var uiel = UIEventListener.Get(gob);
            uiel.onClick = callback;
            uiel.parameter = id;
        }
    }
}
