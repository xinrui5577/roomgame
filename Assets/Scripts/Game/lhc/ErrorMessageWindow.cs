using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lhc
{
    public class ErrorMessageWindow : MonoBehaviour
    {
        public GameObject Bg;

        public UILabel ShowLabel;

        public GameObject CurrentWinNum;

        public UILabel CurrntLotteryNum;

        public UILabel WaitShow;

        public GameObject BackHallBtn;

        public List<RecordItem> RecordItems = new List<RecordItem>();

        public string[] DescriblStrings = { "主", "持", "人", "介", "绍", "中", "..." };

        protected void Start()
        {
            HideWindow();
            CurrntLotteryNum.text = "";
            CurrentWinNum.SetActive(false);
            WaitShow.gameObject.SetActive(false);
        }

        public void ShowMsg(string msg)
        {
            ShowLabel.text = msg;
            Bg.SetActive(true);
            CurrntLotteryNum.text = "";
            BackHallBtn.SetActive(false);
            CurrentWinNum.SetActive(false);
        }

        public void ShowBtnAndMsg(string msg)
        {
           
            ShowLabel.text = msg;
            Bg.SetActive(true);
            CurrntLotteryNum.text = "";
            BackHallBtn.SetActive(true);
            CurrentWinNum.SetActive(false);
        }

        public void SendResultState()
        {
            App.GetRServer<LhcGameServer>().RequestResultState();
        }

        public void OnBackHall()
        {
            App.QuitGameWithMsgBox();
        }

        public void ShowResultState(Dictionary<string, object> dic)
        {
            var gdata = App.GetGameData<LhcGameData>();
            CurrntLotteryNum.text = string.Format("第{0}期", gdata.LotteryNum);
            ShowLabel.text = "";
            CurrentWinNum.SetActive(true);
            Bg.SetActive(true);
            BackHallBtn.SetActive(false);

            var wait = dic.ContainsKey("wait") && bool.Parse(dic["wait"].ToString());
            if (wait)
            {
                for (int i = 0; i < RecordItems.Count; i++)
                {
                    RecordItems[i].ItemNum.text = DescriblStrings[i];
                    RecordItems[i].ItemAnimal.text = "";
                }
            }
            else
            {
                var winNums = dic.ContainsKey("winNums") ? dic["winNums"] : null;

                var winAnimals = dic.ContainsKey("winAnimals") ? dic["winAnimals"] : null;
                if (winNums is List<object> && winAnimals is List<object>)
                {
                    var curShowNums = winNums as List<object>;
                    var curShowAnimals = winAnimals as List<object>;

//                    if (curShowNums.Count == 1)
//                    {
                        foreach (var t in RecordItems)
                        {
                            t.ItemBg.spriteName = "Bg_black";
                            t.ItemNum.text = "";
                            t.ItemAnimal.text = "";
                        }
//                    }


                    for (int i = 0; i < curShowNums.Count; i++)
                    {
                        foreach (var value in gdata.BetPosColors)
                        {
                            if (value.Value == int.Parse(curShowNums[i].ToString()))
                            {
                                RecordItems[i].ItemBg.spriteName = value.Color;
                                RecordItems[i].ItemNum.text = curShowNums[i].ToString();

                                RecordItems[i].ItemAnimal.text = curShowAnimals[i].ToString();
                                RecordItems[i].ItemAnimal.color = FontColor(value.Color);
                            }
                        }

                    }

                    if (curShowNums.Count == 7)
                    {
                        var winning = dic.ContainsKey("winning") && bool.Parse(dic["winning"].ToString());
                        if (winning)
                        {
                            if (!WaitShow.gameObject.activeSelf)
                            {
                                Invoke("OpenWaitShow", 5);
                            }
                        }
                        else
                        {
                            WaitShow.gameObject.SetActive(false);
                            Invoke("DeleayTime", 5);
                            return;
                        }
                    }
                    else
                    {
                        WaitShow.gameObject.SetActive(false);
                    }
                }

            }
            Invoke("SendResultState", 2);
        }

        private void OpenWaitShow()
        {
            WaitShow.gameObject.SetActive(true);
        }

        private void DeleayTime()
        {
            HideWindow();
            App.GetGameManager<LhcGameManager>().OnSendResult();
        }

        private Color FontColor(string bgColor)
        {
            var color = new Color();
            switch (bgColor)
            {
                case "Bg_blue":
                    color = Color.blue;
                    break;
                case "Bg_green":
                    color = Color.green;
                    break;
                case "Bg_red":
                    color = Color.red;
                    break;
            }
            return color;
        }

        public void HideWindow()
        {
            Bg.SetActive(false);
        }
    }
}
