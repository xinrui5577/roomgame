using UnityEngine;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine.EventSystems;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class Plan : MonoBehaviour
    {
        public int TableId;

        public Dictionary<int, int> CoinDic = new Dictionary<int, int>();


        public void XiaZhuChouMaXianShi(int zhuAreaId, int areaId)
        {
            if (zhuAreaId < 0 || zhuAreaId > 7) return;
            var obj = Instantiate(App.GetGameManager<Bjl3DGameManager>().TheGameScene.ZhuMoveDemos[zhuAreaId - 1]);

            if (obj == null) return;
            var tf = transform.FindChild("Bet_" + zhuAreaId);
            if (tf == null) return;
            var ani = obj.GetComponent<Animator>();
            if (ani == null)
                YxDebug.LogError("No Such Animator");
            if (ani != null) ani.enabled = false;
            obj.gameObject.SetActive(true);
            obj.parent = tf.parent;
            obj.localEulerAngles = new Vector3(0, 0, 0);
            if (areaId == 7)
                obj.localScale = new Vector3(0.35f, 0.4f, 0.5f);
            else if (areaId == 6)
                obj.localScale = new Vector3(0.4f, 0.4f, 0.5f);
            else if (areaId == 1)
                obj.localScale = new Vector3(0.3f, 0.5f, 0.8f);
            else
                obj.localScale = new Vector3(0.3f, 0.4f, 0.6f);
            if (CoinDic.ContainsKey(zhuAreaId))
            {
                obj.localPosition = tf.localPosition + new Vector3(0f, 0.2f * CoinDic[zhuAreaId], 0f);
                CoinDic[zhuAreaId] += 1;
            }
            else
            {
                CoinDic.Add(zhuAreaId, 1);
                obj.localPosition = tf.localPosition + new Vector3(0f, 0f, 0f);
            }
        }

        public void OnMouseDown()
        {
            Facade.Instance<MusicManager>().Play("Bet");
            var gdata = App.GetGameData<Bjl3DGameData>();
            if (IsPointerOverUIObject())
            {
                return;
            }
            var gameUI = App.GetGameManager<Bjl3DGameManager>().TheGameUI;
            var gameCfg = App.GetGameData<Bjl3DGameData>().GameConfig;
            var allow = gdata.Allow[TableId];
            if (gameCfg.GameState != 5)
            {
                gameUI.NoteText_Show("此时不能下注！！！");
                return;
            }
            var goldNum = gdata.AnteRate[gameCfg.CoinType];
            var self = gdata.GetPlayer();
            if (self.Coin < goldNum)
            {
                gameUI.NoteText_Show("金币不足！！！");
                return;
            }
            if (allow != 0 && goldNum > allow)
            {
                gameUI.NoteText_Show("下注已经达到上限！！！");
                return;
            }
            if (self.Info.Seat == gdata.B)
            {
                gameUI.NoteText_Show("自己是庄家，不能下注！！！");
                return;
            }
            App.GetRServer<Bjl3DGameServer>().UserBet(TableId, goldNum);
            self.Coin -= goldNum;
            gdata.GStatus = YxEGameStatus.PlayAndConfine;
        }

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

    }
}
