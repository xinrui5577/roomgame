using Assets.Scripts.Game.jlgame.EventII;
using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameRuleView : MonoBehaviour
    {
        public EventObject EventObj;

        public JlGameRuleViewItem ItemPrefab;

        public UITable Table;

        public GameObject HelpBtn;

        public UIWidget ViewWidget;

        public float BgBottomSpace = 10;

        private Vector2 _initialSize;

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "show":
                    SetRuleViewInfo(data.Data);
                    break;
            }
        }

        public void SetRuleViewInfo(object data)
        {
            if (ItemPrefab == null) return;
            var ruleData = (ISFSObject) data;
            var ruleInfo = ruleData.GetUtfString("rule");

            if (string.IsNullOrEmpty(ruleInfo))
            {
                return;
            }
            HelpBtn.SetActive(true);

            _initialSize = ViewWidget.localSize;
            float shiftY = 0;

            gameObject.SetActive(true);
            var ruleSplit = ruleInfo.Split(';');
            int len = ruleSplit.Length;
            if (len == Table.transform.childCount)
            {
                return;
            }
            for (int i = 0; i < len; i++)
            {
                var info = ruleSplit[i].Split(':');
                var go = YxWindowUtils.CreateItem(ItemPrefab, Table.transform);
                go.SetRuleItem(info[0], info[1]);
                shiftY += go.ItemHeight;
            }
            shiftY += BgBottomSpace;
            ViewWidget.height = (int) (Mathf.Max(_initialSize.y, shiftY));
        }

        public void OnPressRuleBtn()
        {
            Table.repositionNow = true;
            ViewWidget.gameObject.SetActive(true);
        }

        public void OnReleaseBtn()
        {
            ViewWidget.gameObject.SetActive(false);
        }
    }
}
