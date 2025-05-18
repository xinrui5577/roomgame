using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using YxFramwork.Common;

namespace Assets.Scripts.Game.bjl3d
{

    public class PlanScene : MonoBehaviour
    {
        public TextMesh[] SelfNoteTexts;
        public TextMesh[] QuyuNoteTexts;

        public Transform[] Planes;
       
        public bool IsPointerOverUIObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
      
        public void UserNoteDataFun(int serverSeat,int coin)
        {
            var gdata = App.GetGameData<Bjl3DGameData>();
            if (serverSeat == gdata.SelfSeat)
            {
                ShowSelfNoteInfo(gdata.P, coin);
            }
            ShowquyuNoteInfo(gdata.P, coin);
            //筹码显示
            ShowChouMaZhu(gdata.P, coin);
        }

        public void ShowChouMaZhu(int iArea, long money)
        {
            var list = App.GameData.AnteRate;
            var count = list.Count;
            var index = 0;
            for (var i = 0; i < count; i++)
            {
                var value = list[i];
                if (value != money) continue;
                index = i;
                break;
            }
            var plan = Planes[iArea].GetComponent<Plan>();
            if (plan == null) return;
            plan.XiaZhuChouMaXianShi(index + 1, iArea);
        }

        /// <summary>
        /// 显示注意信息
        /// </summary>
        public void ShowSelfNoteInfo(int area, int gold)
        {
            App.GetGameManager<Bjl3DGameManager>().TheBetMoneyUI.BetMoneySelfNoteInfo(area, gold);
        }

        /// <summary>
        /// 显示
        /// </summary>
        void ShowquyuNoteInfo(int area, int gold)
        {
            App.GetGameManager<Bjl3DGameManager>().TheBetMoneyUI.BetMoneyquyuNoteInfo(area, gold);
        }

        /// <summary>
        /// 清空下注筹码
        /// </summary>
        public void QingKongChouma()
        {
            for (int i = 0; i < Planes.Length; i++)
            {
                foreach (Transform t in Planes[i])
                {
                    if (t.name.Contains("coin"))
                        Destroy(t.gameObject);
                    Plan plan = Planes[i].GetComponent<Plan>();
                    if (plan == null) return;
                    plan.CoinDic.Clear();
                }
            }
        }

    }
}
