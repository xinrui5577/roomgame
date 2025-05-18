using System.Collections;
using Assets.Scripts.Game.biji.EventII;
using Assets.Scripts.Game.biji.Modle;
using Assets.Scripts.Game.biji.network;
using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjHupUpView : MonoBehaviour
    {
        public EventObject EventObj;
        public GameObject View;
        public BjHupUpItem BjHupUpItem;
        public UIGrid BjHupUpGrid;
        public UIButton AgreeBtn;
        public UIButton RefuseBtn;
        public UILabel CuntDownLabel;


        private BjGameTable _gdata
        {
            get { return App.GetGameData<BjGameTable>(); }
        }

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Rejoin":
                    OnRejoin(data.Data);
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

            for (int i = 0; i < _gdata.PlayerList.Length; i++)
            {
                if (_gdata.PlayerList[i].Info == null) continue;
                var item = YxWindowUtils.CreateItem(BjHupUpItem, BjHupUpGrid.transform);
                item.SetItemView(_gdata.PlayerList[i].Info.AvatarX, _gdata.PlayerList[i].Info.SexI, _gdata.PlayerList[i].Info.NickM);
                for (int j = 0; j < ids.Length; j++)
                {
                    if (_gdata.OwnerId == int.Parse(ids[j]))
                    {
                        item.ShowRoomOwner();
                    }

                    if (selfId.Equals(ids[j]))
                    {
                        BtnState(false);
                    }

                    if (_gdata.PlayerList[i].Info.Id == int.Parse(ids[j]))
                    {
                        item.ChangeStae();
                    }
                }
            }


            StartCoroutine(CuntDownTime((int)time));
        }
        public void OnHandUp(object data)
        {
            View.SetActive(true);
            var hupData = (HupData)data;

            if (hupData.Type == 2)
            {
                if (hupData.IsSelf)
                {
                    BtnState(!hupData.IsSelf);
                }
                else
                {
                    BtnState(!hupData.IsSelf);
                }

                StopAllCoroutines();
                while (BjHupUpGrid.transform.childCount > 0)
                {
                    DestroyImmediate(BjHupUpGrid.transform.GetChild(0).gameObject);
                }

                for (int i = 0; i < _gdata.PlayerList.Length; i++)
                {
                    if (_gdata.PlayerList[i].Info == null) continue;
                    var item = YxWindowUtils.CreateItem(BjHupUpItem, BjHupUpGrid.transform);

                    if (_gdata.OwnerId == _gdata.PlayerList[i].Info.Id)
                    {
                        item.ShowRoomOwner();
                    }

                    if (_gdata.PlayerList[i].Info.Id == hupData.UserId)
                    {
                        item.SetItemView(_gdata.PlayerList[i].Info.AvatarX, _gdata.PlayerList[i].Info.SexI, hupData.UserName, true);
                    }
                    else
                    {
                        item.SetItemView(_gdata.PlayerList[i].Info.AvatarX, _gdata.PlayerList[i].Info.SexI, _gdata.PlayerList[i].Info.NickM);
                    }
                }
                BjHupUpGrid.repositionNow = true;
                StartCoroutine(CuntDownTime(hupData.CdTime));
            }
            else if (hupData.Type == 3)
            {
                if (hupData.IsSelf)
                {
                    BtnState(!hupData.IsSelf);
                }
                for (int i = 0; i < BjHupUpGrid.transform.childCount; i++)
                {
                    if (BjHupUpGrid.transform.GetChild(i).name.Equals(hupData.UserName))
                    {
                        BjHupUpGrid.transform.GetChild(i).GetComponent<BjHupUpItem>().ChangeStae();
                    }
                }

            }
            else if (hupData.Type == -1)
            {
                OnClose();
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

        private void BtnState(bool isCanClick)
        {
            if (isCanClick)
            {
                AgreeBtn.normalSprite = "greenBtn";
                AgreeBtn.enabled = true;
                RefuseBtn.normalSprite = "yellowBtn";
                RefuseBtn.enabled = true;
            }
            else
            {
                AgreeBtn.normalSprite = "green_grayBtn";
                AgreeBtn.enabled = false;
                RefuseBtn.normalSprite = "yellow_grayBtn";
                RefuseBtn.enabled = false;
            }
        }

        public void OnClose()
        {
            View.SetActive(false);
            StopAllCoroutines();
            while (BjHupUpGrid.transform.childCount > 0)
            {
                DestroyImmediate(BjHupUpGrid.transform.GetChild(0).gameObject);
            }
        }
    }
}
