using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sssjp.skin1
{
    public class RuleInfoViewSk1 : RoomRuleView {

        public RuleViewItem ItemPrefab;

        public UITable Table;

        public GameObject HelpBtn;

        public UIWidget ViewWidget;

        public float BgBottomSpace = 10;

        /// <summary>
        /// 娱乐房是否显示规则按钮
        /// </summary>
        public bool ShowInRecreationRoom;

        private Vector2 _initialSize;



        public override void InitRoomRuleInfo(ISFSObject gameInfo)
        {
            if (ItemPrefab == null) return;

            if (!gameInfo.ContainsKey("rule") || (!App.GetGameData<SssGameData>().IsRoomGame && !ShowInRecreationRoom))
            {
                return;
            }
            _initialSize = ViewWidget.localSize;
            float shiftY = 0;
            HelpBtn.SetActive(true);
            gameObject.SetActive(true);
            string ruleInfo = gameInfo.GetUtfString("rule");
            var ruleSplit = ruleInfo.Split(';');
            int len = ruleSplit.Length;

            if (Table.transform.childCount >= len)
                return;

            for (int i = len - 1; i >= 0; i--)
            {
                var info = ruleSplit[i].Split(':');
                var go = CreateItem();
                go.SetRuleItem(info[0], info[1]);
                shiftY += go.ItemHeight;
            }
            shiftY += BgBottomSpace;
            ViewWidget.height = (int) (Mathf.Max(_initialSize.y, shiftY));
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
            Table.repositionNow = true;
            Table.Reposition();
        }

        public void OnReleaseBtn()
        {
            ViewWidget.gameObject.SetActive(false);
        }
   
    }
}
