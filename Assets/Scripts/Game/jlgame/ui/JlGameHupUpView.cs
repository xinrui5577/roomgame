using System.Collections;
using Assets.Scripts.Game.jlgame.EventII;
using Assets.Scripts.Game.jlgame.network;
using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameHupUpView : MonoBehaviour
    {
        public EventObject EventObj;
        public GameObject View;
        public JlGameHupUpItem JlGameHupUpItem;
        public UIGrid JlGameHupUpGrid;
        public GameObject AgreeBtn;
        public GameObject RefuseBtn;
        public UILabel CuntDownLabel;

        private YxBaseGamePlayer[] _players;

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Rejoin":
                    OnRejoin(data.Data);
                    break;
                case "PlayerList":
                    _players = (YxBaseGamePlayer[])data.Data;
                    break;
                case "HupUpResponse":
                    OnHandUp(data.Data);
                    break;
                case "Close":
                    OnClose();
                    break;
            }
        }

        public void OnClickBtn(string objName)
        {
            EventObj.SendEvent("ServerEvent", "HupReq", int.Parse(objName));
        }

        public void OnRejoin(object data)
        {
            View.SetActive(true);
            var hupData = (ISFSObject)data;
            var time = hupData.GetLong("time");
            var hup = hupData.GetUtfString("hup");
            var selfId = hupData.GetUtfString("userId");
            string[] ids = hup.Split(',');

            for (int i = 0; i < _players.Length; i++)
            {
                var item = YxWindowUtils.CreateItem(JlGameHupUpItem, JlGameHupUpGrid.transform);
                item.SetItemView(_players[i].Info.AvatarX, _players[i].Info.SexI, false, _players[i].Info.NickM);
                for (int j = 0; j < ids.Length; j++)
                {
                    if (selfId.Equals(ids[j]))
                    {
                        AgreeBtn.SetActive(false);
                        RefuseBtn.SetActive(false);
                    }
                    if (_players[i].Info.UserId.Equals(ids[j]))
                    {
                        item.ChangeIcon();
                    }
                }
            }

            JlGameHupUpGrid.repositionNow = true;

            StartCoroutine(CuntDownTime((int)time));
        }
        public void OnHandUp(object data)
        {
            View.SetActive(true);
            var hupData = (HupData)data;

            if (hupData.Type == 2)
            {
                StopAllCoroutines();
                while (JlGameHupUpGrid.transform.childCount > 0)
                {
                    DestroyImmediate(JlGameHupUpGrid.transform.GetChild(0).gameObject);
                }

                for (int i = 0; i < _players.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(JlGameHupUpItem, JlGameHupUpGrid.transform);
                    if (int.Parse(_players[i].Info.UserId) == hupData.UserId)
                    {
                        item.SetItemView(_players[i].Info.AvatarX, _players[i].Info.SexI, true, hupData.UserName);
                    }
                    else
                    {
                        item.SetItemView(_players[i].Info.AvatarX, _players[i].Info.SexI, false, _players[i].Info.NickM);
                    }
                }
                JlGameHupUpGrid.repositionNow = true;

                StartCoroutine(CuntDownTime(hupData.CdTime));
            }
            else if (hupData.Type == 3)
            {
                for (int i = 0; i < JlGameHupUpGrid.transform.childCount; i++)
                {
                    if (JlGameHupUpGrid.transform.GetChild(i).name.Equals(hupData.UserName))
                    {
                        JlGameHupUpGrid.transform.GetChild(i).GetComponent<JlGameHupUpItem>().ChangeIcon();
                    }
                }

            }
            else if (hupData.Type == -1)
            {
                OnClose();
            }
            if (hupData.IsSelf)
            {
                AgreeBtn.SetActive(!hupData.IsSelf);
                RefuseBtn.SetActive(!hupData.IsSelf);
            }
            else
            {
                AgreeBtn.SetActive(!hupData.IsSelf);
                RefuseBtn.SetActive(!hupData.IsSelf);
            }

        }

        void SetTime(int cdTime)
        {
            if (CuntDownLabel == null) return;
            CuntDownLabel.text = string.Format("{0}", cdTime);
            CuntDownLabel.gameObject.SetActive(true);
        }

        private IEnumerator CuntDownTime(int cdTime)
        {
            var curTime = cdTime;
            while (curTime >= 0)
            {
                if (curTime < 0) curTime = 0;
                SetTime(curTime);
                yield return new WaitForSeconds(1f);
                curTime--;
                if (curTime <= 0) yield break;
            }
        }

        public void OnClose()
        {

            View.SetActive(false);
            StopAllCoroutines();
            while (JlGameHupUpGrid.transform.childCount > 0)
            {
                DestroyImmediate(JlGameHupUpGrid.transform.GetChild(0).gameObject);
            }
        }
    }
}
