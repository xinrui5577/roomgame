using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelGameRule), UIPanelhierarchy.EffectAndTip)]
    public class PanelGameRule : UIPanelBase
    {
        public RectTransform Target;
        public GameObject Item;
        public GameObject ItemParent;
        public Vector2 EndPos;
        public Vector2 StartPos;
        public float time;
        private bool IsMoving;
        private bool IsHasRule;

        void Awake()
        {
            IsMoving = false;
            Target.localPosition = StartPos;
        }

        public override void OnInit()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.ShowGameRule, Show);
        }

        protected bool SetRoomInfo(Dictionary<string, string> rule)
        {
            if (rule == null || rule.Count == 0)
            {
                gameObject.SetActive(DataCenter.Room.RoomType == MahRoomType.FanKa);
                return false;
            }
            float height = 0;
            foreach (KeyValuePair<string, string> keyValuePair in rule)
            {
                var newObj = Instantiate(Item);
                newObj.transform.SetParent(ItemParent.transform);
                newObj.transform.localScale = Vector3.one;
                newObj.transform.localPosition = new Vector3(0, height, 0);
                newObj.SetActive(true);
                var itemSpr = newObj.GetComponent<RuleItem>();
                itemSpr.SetTital(keyValuePair.Key);
                itemSpr.SetContent(keyValuePair.Value);
                height -= itemSpr.Size.y;
            }
            return true;
        }

        public void Show(EvtHandlerArgs args)
        {
            if (IsMoving) { return; }
            if (!IsHasRule)
            {
                string temp = GameCenter.DataCenter.Config.DefaultGameRule;
                Dictionary<string, string> rules = ParseRoomRule(temp);
                if (SetRoomInfo(rules))
                {
                    IsHasRule = true;
                }
            }
            PlayShowAction();
        }

        /// <summary>
        /// 消息格式: 局数:10局;玩法:xxx; 
        /// </summary>
        public Dictionary<string, string> ParseRoomRule(string rule)
        {
            var dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(rule)) return dic;

            var rules = Regex.Split(rule, ";");
            for (int i = 0; i < rules.Length; i++)
            {
                var kv = Regex.Split(rules[i], ":");
                if (kv.Length > 1)
                {
                    dic[kv[0]] = kv[1];
                }
            }
            return dic;
        }

        public void Hide()
        {
            if (IsMoving) { return; }
            PlayHideAction();
        }

        protected void PlayShowAction()
        {
            IsMoving = true;
            Target.localPosition = StartPos;
            var move = Target.DOLocalMove(EndPos, time);
            move.SetEase(Ease.OutCubic);
            move.OnComplete(() => { IsMoving = false; });
        }

        protected void PlayHideAction()
        {
            IsMoving = true;
            Target.localPosition = EndPos;
            var move = Target.DOLocalMove(StartPos, time);
            move.SetEase(Ease.OutCubic);
            move.OnComplete(() =>
            {
                IsMoving = false;
            });
        }
    }
}