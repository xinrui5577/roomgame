using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Texas
{
    public class RuleView : MonoBehaviour {

        public RuleViewItem ItemPrefab;

        public UITable Table;

        public GameObject HelpBtn;

        public UIWidget ViewWidget;

        public float BgBottomSpace = 10;

        private Vector2 _initialSize;


        public void SetRuleViewInfo(ISFSObject gameInfo)
        {
            if (ItemPrefab == null) return;

            if (!gameInfo.ContainsKey("rule") || !App.GetGameData<TexasGameData>().IsRoomGame)
            {
                gameObject.SetActive(false);
                return;
            }

            _initialSize = ViewWidget.localSize;
            float shiftY = 0;

            gameObject.SetActive(true);
            string ruleInfo = gameInfo.GetUtfString("rule");
            var ruleSplit = ruleInfo.Split(';');
            int len = ruleSplit.Length;
            for (int i = 0; i < len; i++)
            {
                var info = ruleSplit[i].Split(':');
                var go = CreateItem();
                go.SetRuleItem(info[0], info[1]);
                shiftY += go.ItemHeight;
            }
            shiftY += BgBottomSpace;
            ViewWidget.height = (int) (Mathf.Max(_initialSize.y, shiftY));
            Table.repositionNow = true;
            Table.Reposition();
        }

        RuleViewItem CreateItem()
        {
            var go = Instantiate(ItemPrefab);
            go.transform.parent = Table.transform;
            go.transform.localScale = Vector3.one;
            go.gameObject.SetActive(true);
            return go;
        }


        public void OnPressRuleBtn()
        {
            ViewWidget.gameObject.SetActive(true);
        }

        public void OnReleaseBtn()
        {
            ViewWidget.gameObject.SetActive(false);
        }
    }
}
