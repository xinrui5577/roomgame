using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using YxFramwork.Common;
using YxFramwork.Tool;

// ReSharper disable UnusedMember.Local



namespace Assets.Scripts.Game.fillpit
{
    public class AddBetBtnItem : MonoBehaviour
    {

        public int AddBetValue;
      
       

        // Use this for initialization
        void Start()
        {
            UIButton btn = GetComponent<UIButton>();

            btn.onClick.Add(new EventDelegate(() =>
            {
                //发送下注消息
                Dictionary<string, int> data = new Dictionary<string, int>()
                {
                    {"gold", AddBetValue},
                    {"seat", App.GetGameData<FillpitGameData>().SelfSeat}
                };

                App.GetRServer<FillpitGameServer>().SendRequest(GameRequestType.Bet, data);
            }));
        }

        void OnEnable()
        {
            OnAddBetBtnShow();
        }

        public virtual void OnAddBetBtnShow()
        {
            //显示当前局此按钮添加筹码的值
            GetComponentInChildren<UILabel>().text = YxUtiles.ReduceNumber(AddBetValue);
        }
    }
}