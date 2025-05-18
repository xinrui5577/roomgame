using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GameInfoSkin2 : AbsGameInfo
    {
        public GameInfoItem InfoItem;
        public UiLaiziLabel Laizi;
        public Transform ItemGroup;
        public Text RoomNum;

        private GameInfoItem mCurrRound;

        private void Start()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.SetGangdi, SetGangdi);
        }

        public void SetGangdi(EvtHandlerArgs args)
        {
            var data = args as PanelGameInfoArgs;
            Laizi.Value = data.Laizi;
        }

        public override void OnGetInfoRefresh()
        {
            var data = GameCenter.DataCenter.Room;
            RoomNum.text = "房间号：" + data.RoomID.ToString();
            //局数
            mCurrRound = CreateItem();
            SetCurrRound(); 
            //规则         
            StartCoroutine(SetGameRule());
        }

        private IEnumerator SetGameRule()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] rules = GameCenter.DataCenter.Config.DefaultGameRule.Split(';');
            for (int i = 0; i < rules.Length; i++)
            {
                string str = rules[i];
                if (str.Contains("玩法"))
                {
                    dic.Add("玩法", str);
                }
                if (str.Contains("分数"))
                {
                    dic.Add("分数", str);
                }
            }
            GameInfoItem item = null;
            //分数
            if (dic.ContainsKey("分数"))
            {
                item = CreateItem();
                item.Context = dic["分数"];
            }
            //玩法                     
            if (dic.ContainsKey("玩法"))
            {
                item = CreateItem();
                item.Context = dic["玩法"];
            }
            yield return new WaitForEndOfFrame();
            var rect = item.Txt.GetComponent<RectTransform>();
            item.MinHeight = rect.sizeDelta.y + 20;
        }

        public void SetCurrRound()
        {
            var data = GameCenter.DataCenter.Room;
            if (data.LoopType == MahGameLoopType.Round)
            {
                mCurrRound.Context = data.CurrRound + "/" + data.MaxRound + " " + "局";
            }
            else
            {
                mCurrRound.Context = data.CurrRound + "/" + data.MaxRound + " " + "圈";
            }
        }

        public override void OnReadyRefresh()
        {
            Laizi.gameObject.SetActive(false);
            SetCurrRound();
        }

        private GameInfoItem CreateItem()
        {
            var newObj = Instantiate(InfoItem);
            newObj.ExSetParent(ItemGroup.transform);
            newObj.gameObject.SetActive(true);
            return newObj;
        }

        public override void UpdateMahjongCount(GameInfoArgs args)
        {
            GameCenter.Scene.TableManager.MahjongCounter(GameCenter.DataCenter.LeaveMahjongCnt);
        }
    }
}