using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.fillpit.Mgr
{
    public class RuleViewMgr : MonoBehaviour
    {

        public RuleViewItem ItemPrefab;

        public UITable Table;

        public GameObject ViewBtn;

        public GameObject CloseBtn;


        public virtual void SetRuleViewInfo(ISFSObject gameInfo)
        {
            if (ItemPrefab == null) return;
            Table.transform.DestroyChildren();
            if (!gameInfo.ContainsKey("rule"))
            {
                gameObject.SetActive(false);
                return;
            }

            string ruleInfo = gameInfo.GetUtfString("rule");
            if (string.IsNullOrEmpty(ruleInfo))
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            var ruleSplit = ruleInfo.Split(';');
            int len = ruleSplit.Length;
            for (int i = 0; i < len; i++)
            {
                var info = ruleSplit[i].Split(':');
                var go = CreateItem();
                go.SetRuleItem(info[0], info[1]);
            }

            Table.repositionNow = true;
            Table.Reposition();
        }

        protected RuleViewItem CreateItem()
        {
            var go = Instantiate(ItemPrefab);
            go.transform.parent = Table.transform;
            go.transform.localScale = Vector3.one;
            go.gameObject.SetActive(true);
            return go;
        }

        
        public void OnClickViewBtn()
        {
            ViewBtn.GetComponent<BoxCollider>().enabled = false;
            ViewBtn.GetComponent<UIButton>().SetState(UIButtonColor.State.Disabled,true);
            CloseBtn.SetActive(true);
        }

        public void OnClickCloseBtn()
        {
            ViewBtn.GetComponent<BoxCollider>().enabled = true;
            CloseBtn.SetActive(false);
        }

    }
}
