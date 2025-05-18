using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhResultView : MonoBehaviour
    {

        public GameObject Result;

        public EventObject EventObj;

        public GameObject Grid;

        public GameObject Item;

        public GameObject Content;

        public UILabel Timer;

        public UISprite ResultIcon;

        public float TTime;

        protected float TimeSum;

        protected bool IsStartTimer;

        protected List<GameObject> Items = new List<GameObject>();

        protected bool InformTtResultOnHide;

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "Result":
                    OnResult(data.Data);
                    break;
                case "Ready":
                    OnGameReady(data.Data);
                    break;
                case "Show":
                    Show();
                    break;
                case "IsHide":
                    CheckIsHide(data.Data);
                    break;
            }
        }

        private void CheckIsHide(object data)
        {
            EventDelegate del = (EventDelegate) data;
            if (Result.activeSelf)
            {
                del.Execute();
                InformTtResultOnHide = true;
            }
        }

        private void OnGameReady(object data)
        {
            //Hide();
            
        }

        private void OnResult(object data)
        {
            foreach (var o in Items)
            {
                Destroy(o);
            }
            Items.Clear();

            Result.SetActive(true);

            IsStartTimer = true;

            Timer.text = "" + (int)(TTime - TimeSum);

            UIGrid gd = Grid.GetComponent<UIGrid>();
            gd.Reposition();
            gd.enabled = true;

            //TweenAlpha al = Content.GetComponent<TweenAlpha>();

            //if (al != null)
            //{
            //    al.ResetToBeginning();
            //    al.PlayForward();
            //}

            ISFSArray arr = (ISFSArray)data;
            for (int i = 0; i < arr.Count; i++)
            {
                ISFSObject obj = arr.GetSFSObject(i);
                int result = obj.GetInt("ResultType");
                string uname = obj.GetUtfString("Name");
                int gold = obj.GetInt("AddGold");
                int[] cards = new int[3];
                int chair = obj.GetInt("Chair");
                if (obj.ContainsKey("Cards"))
                {
                    cards = obj.GetIntArray("Cards");
                    Debug.LogError(" result cards " + uname + "  " + cards[0] + " " + cards[1] + " " + cards[2]);
                }
                if (obj.ContainsKey("IsWinner"))
                {
                    if (chair == 0)
                    {
                        ResultIcon.spriteName = obj.GetBool("IsWinner") ? "public_019" : "public_018";
                    }
                }

                GameObject newItem = Instantiate(Item);
                newItem.SetActive(true);
                newItem.transform.parent = Grid.transform;
                newItem.transform.localScale = Vector3.one;
                JhResultItem item = newItem.GetComponent<JhResultItem>();
                item.SetInfo(uname, gold, result, cards);
                Items.Add(newItem);
            }
        }

        public void Show()
        {
            if (Items.Count > 0)
            {
                Result.SetActive(true);
            }

        }

        public void Hide()
        {
            Result.SetActive(false);
            IsStartTimer = false;

            TimeSum = 0;

            if (InformTtResultOnHide)
            {
                EventObj.SendEvent("TtResultViewEvent","Show",null);
            }

        }


        void Update()
        {
            if (!IsStartTimer || TTime<=0)
            {
                return;
            }

            TimeSum += Time.deltaTime;
            if (TimeSum >= TTime)
            {
                Hide();
            }
            else
            {
                Timer.text = "" + (int)(TTime-TimeSum);
            }
        }
    }
}
