using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class GameRulePnl : PopPnlBase
    {

        public GameObject Item;

        public GameObject ShowBtn;

        public GameObject ItemParent;
        public Vector2 EndPos;
        public Vector2 StartPos;
        public float time;

        protected bool IsMoving;

        /// <summary>
        /// 标记是否已经设置了玩法参数
        /// </summary>
        private bool _hasSetRoomInfo = false;


        public string DefalutRuleInfo;

        //消息格式: 局数:10局;玩法:xxx; 
        public Dictionary<string, string> TryGetDefaultRoomRuleForUi()
        {
            var dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(DefalutRuleInfo))
            {
                return dic;
            }
            string[] rules = Regex.Split(DefalutRuleInfo, ";");
            for (int i = 0; i < rules.Length; i++)
            {
                string[] kv = Regex.Split(rules[i], ":");
                if (kv.Length > 1)
                {
                    dic[kv[0]] = kv[1];
                }
            }
            return dic;
        }


        void Awake()
        {

            EventDispatch.Instance.RegisteEvent((int)UIEventId.RoomInfo, OnRoomInfo);
            IsMoving = false;
            var self = GetComponent<RectTransform>();
            
            self.localPosition = StartPos;
            //var testRoomInfo = new RoomInfo();
            //OnRoomInfo(1, new EventData(testRoomInfo));
        }

        protected virtual void OnRoomInfo(int id, EventData data)
        {
            if(_hasSetRoomInfo) return;

            var roomInfo = (RoomInfo)data.data1;
            Dictionary<string, string> gameExplain = roomInfo.GetRoomRule();
            if (gameExplain == null || gameExplain.Count==0)
            {
                gameExplain = TryGetDefaultRoomRuleForUi();
                if(gameExplain==null)return;
            }
            float height = 0;
            foreach (KeyValuePair<string, string> keyValuePair in gameExplain)
            {
                var newObj = Instantiate(Item);
                newObj.transform.SetParent(ItemParent.transform);
                newObj.transform.localScale = Vector3.one;
                newObj.transform.localPosition = new Vector3(0, height,0);
                newObj.SetActive(true);


                var itemSpr = newObj.GetComponent<RuleItem>();
                itemSpr.SetTital(keyValuePair.Key);
                itemSpr.SetContent(keyValuePair.Value);

                height -= itemSpr.Size.y;
            }

            _hasSetRoomInfo = true;
        }

        public override void Show()
        {
            if (IsMoving)
            {
                return;
            }
            base.Show();
            PlayShowAction();
            if (ShowBtn)
            {
                ShowBtn.SetActive(false);
            }
        }

        public override void Hide()
        {
            if (IsMoving)
            {
                return;
            }
            PlayHideAction();
        }

        protected void PlayShowAction()
        {
            IsMoving = true;
            var self = GetComponent<RectTransform>();
            self.localPosition = StartPos;
            var move = self.DOLocalMove(EndPos, time);
            move.SetEase(Ease.OutCubic);
            move.OnComplete(() =>
                {
                    IsMoving = false;
                });
        }

        protected void PlayHideAction()
        {
            IsMoving = true;
            var self = GetComponent<RectTransform>();
            self.localPosition = EndPos;
            var move = self.DOLocalMove(StartPos, time);
            move.SetEase(Ease.OutCubic);
            move.OnComplete(() =>
            {
                IsMoving = false;
                base.Hide();
                if (ShowBtn)
                {
                    ShowBtn.SetActive(true);
                }
            });
        }
    }
}
