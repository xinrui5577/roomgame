using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.fruit
{
    public class FruitSlotInfoCtrl : MonoBehaviour
    {

        private GameObject _btnBar;
        private GameObject _btnSeven;
        private GameObject _btnStar;
        private GameObject _btnXigua;
        private GameObject _btnLingdang;
        private GameObject _btnYezi;
        private GameObject _btnOrg;
        private GameObject _btnApple;

        public GameObject[] FruitSlotTextList;//现在的下标正好对应水果的枚举值，这点要注意，以后可能出现映射bug

        public LightItemCtrl LightItemCtrl;

        private readonly SlotInfo _slotInfo = SlotInfo.GetSlotInfo();//设置押注信息用

        // Use this for initialization
       protected void Start()
        {
            _btnBar = GameObject.Find("btn_bar");
            _btnSeven = GameObject.Find("btn_seven");
            _btnStar = GameObject.Find("btn_star");
            _btnXigua = GameObject.Find("btn_xigua");
            _btnLingdang = GameObject.Find("btn_lingdang");
            _btnYezi = GameObject.Find("btn_yezi");
            _btnOrg = GameObject.Find("btn_org");
            _btnApple = GameObject.Find("btn_apple");
            EventTriggerListener.Get(_btnBar).OnClick = OnbtnbarClick;
            EventTriggerListener.Get(_btnSeven).OnClick = OnbtnsevenClick;
            EventTriggerListener.Get(_btnStar).OnClick = OnbtnstarClick;
            EventTriggerListener.Get(_btnXigua).OnClick = OnbtnxiguaClick;
            EventTriggerListener.Get(_btnLingdang).OnClick = OnbtnlingdangClick;
            EventTriggerListener.Get(_btnYezi).OnClick = OnbtnyeziClick;
            EventTriggerListener.Get(_btnOrg).OnClick = OnbtnorgClick;
            EventTriggerListener.Get(_btnApple).OnClick = OnbtnappleClick;
            Facade.EventCenter.AddEventListeners<string, object>("AnimStateFresh", OnFreshSlot);
        }

        private void OnFreshSlot(object obj)
        {
            SlotInfo.ResetFruitSlotList();
            UpdateFruitSlotTxtInfo();
        }

        private void OnbtnbarClick(GameObject gob)
        {
            Facade.Instance<MusicManager>().Play("s1");
            SetFruitTextInfo(FruitType.Bar);
        }

        private void OnbtnsevenClick(GameObject gob)
        {
            Facade.Instance<MusicManager>().Play("s2");
            SetFruitTextInfo(FruitType.Seven);
        }

        private void OnbtnstarClick(GameObject gob)
        {
            Facade.Instance<MusicManager>().Play("s3");
            SetFruitTextInfo(FruitType.Star);
        }

        private void OnbtnxiguaClick(GameObject gob)
        {
            Facade.Instance<MusicManager>().Play("s4");
            SetFruitTextInfo(FruitType.Watermelon);
        }

        private void OnbtnlingdangClick(GameObject gob)
        {
            Facade.Instance<MusicManager>().Play("s5");
            SetFruitTextInfo(FruitType.Bell);
        }

        private void OnbtnyeziClick(GameObject gob)
        {
            Facade.Instance<MusicManager>().Play("s6");
            SetFruitTextInfo(FruitType.Coco);
        }

        private void OnbtnorgClick(GameObject gob)
        {
            Facade.Instance<MusicManager>().Play("s7");
            SetFruitTextInfo(FruitType.Orange);
        }

        private void OnbtnappleClick(GameObject gob)
        {
            Facade.Instance<MusicManager>().Play("s8");
            SetFruitTextInfo(FruitType.Apple);
        }


        private static bool _hasClearFruitList = true;
        public static bool HasClearFruitList
        {
            get { return _hasClearFruitList; }
            set { _hasClearFruitList = value; }
        }

        private void SetFruitTextInfo(FruitType fruitType)
        {
            if (LightItemCtrl != null && LightItemCtrl.AnimState != LightItemCtrl.ItemAnimState.Sleep)
            {
                return;
            }

            if (_hasClearFruitList == false)
            {
                //SlotInfo.ClearFruitSlotListToZero();
                //更新下各个对应点位的押注信息
              //  UpdateFruitSlotTxtInfo();
                _hasClearFruitList = true;
               // Debug.LogError("UpdateFruitSlotTxtInfo");
               // return;
            }
             
            var gdata = App.GetGameData<FruitGameData>();
            var player = gdata.GetPlayer();
            if (player.Coin > 0)
            {
                App.GetRServer<FruitGameServer>().SendRestart();
                //return;
            }
            if (_slotInfo.SetFruitSlotPoint(fruitType, 1))
            {
                var fruitList = _slotInfo.GetFruitSlotList();
                var basepart = "";
                var point = fruitList[fruitType];
                if (point < 10)
                {
                    basepart = "0";
                }
                FruitSlotTextList[(int)fruitType].GetComponent<Text>().text = basepart + point+"";
            }

            //更新下各个对应点位的押注信息
            UpdateFruitSlotTxtInfo();
        }
  
        public void UpdateFruitSlotTxtInfo()
        {
            Dictionary<FruitType, int> fruitListInof = _slotInfo.GetFruitSlotList();
            foreach (var i in fruitListInof)
            {
                var basepart = "";
                var point = fruitListInof[i.Key];
                if (point < 10)
                {
                    basepart = "0";
                }
                FruitSlotTextList[i.Key.GetHashCode()].GetComponent<Text>().text = basepart + point+"";
            }

        }

        void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<string, object>("AnimStateFresh", OnFreshSlot);
        }
    }
}
