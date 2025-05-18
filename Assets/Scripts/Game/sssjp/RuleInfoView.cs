using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class RuleInfoView : RoomRuleView
    {
        [SerializeField]
        private GameObject _ruleView;

        [SerializeField]
        private GameObject _itemPrefab;

        [SerializeField]
        private Transform _parent;

        public GameObject ShowRuleInfoBtn;


        private string _ruleInfo = "失败:未能成功获得规则信息";


        private Dictionary<string, string> _ruleDic;


        public void InitRuleInfo(string ruleInfo)
        {
            _ruleInfo = ruleInfo;
            _ruleDic = StringToDic();
            InitView();
        }



        void InitView()
        {
            int count = 0;
            foreach (var item in _ruleDic)
            {
                GameObject go = Instantiate(_itemPrefab);
                go.SetActive(true);
                go.transform.parent = _parent;
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(0, -45 * count, 0);
                var itemInfo = go.GetComponent<RuleInfoItem>();
                itemInfo.SetTitel(item.Key);
                itemInfo.SetContent(item.Value);
                count++;
            }
        }

        public Dictionary<string, string> StringToDic()
        {
            try
            {
                return TryGetRuleInfo();
            }
            catch (Exception e)
            {
                YxDebug.LogError("规则信息格式转换失败,请查验信息格式:" + e);
                return new Dictionary<string, string>();
            }
        }

        Dictionary<string, string> TryGetRuleInfo()
        {
            var dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(_ruleInfo))
            {
                return dic;
            }
            string[] rules = Regex.Split(_ruleInfo, ";");
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


        public void OnClickShowRuleViewBtn()
        {
            _ruleView.SetActive(true);
            _ruleView.transform.parent.gameObject.SetActive(true);
            var tweener = _ruleView.GetComponents<UITweener>();
            foreach (var item in tweener)
            {
                item.ResetToBeginning();
                item.PlayForward();
            }
        }

        public override void InitRoomRuleInfo(ISFSObject gameInfo)
        {
            string ruleInfo = gameInfo.GetUtfString("rule");
            InitRuleInfo(ruleInfo);
        }
    }
}