using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.BankWindows
{
    public class BankLogView : YxView
    {

        public BankLogItemView ItemPerfab;
        public UIGrid ItemGridPerfab;

        private UIGrid _curItemParent;

        protected override void OnStart()
        {
            base.OnStart();
            Facade.Instance<TwManager>().SendAction("bankLog", new Dictionary<string, object>(), UpdateView);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Facade.Instance<TwManager>().SendAction("bankLog", new Dictionary<string, object>(), UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var list = Data as List<object>;
            if (list == null) return;
            CreateItemParent();
            var count = list.Count;
            for (var i = 0; i < count; i++)
            {
                var item = Instantiate(ItemPerfab); 
                var ts = item.transform;
                ts.parent = _curItemParent.transform;
                ts.localPosition = Vector3.zero;
                ts.localScale = Vector3.one;
                ts.localRotation = Quaternion.identity;
                item.gameObject.SetActive(true);
                item.UpdateView(list[i]); 
            }
            _curItemParent.repositionNow = true;
            _curItemParent.Reposition();
        }

        private void CreateItemParent()
        {
            if (_curItemParent != null)
            {
                Destroy(_curItemParent.gameObject);
            }
            var perfabTs = ItemGridPerfab.transform;
            _curItemParent = Instantiate(ItemGridPerfab);
            var ts = _curItemParent.transform;
            ts.parent = perfabTs.parent;
            ts.gameObject.SetActive(true);
            ts.localPosition = perfabTs.localPosition;
            ts.localScale = perfabTs.localScale;
            ts.localRotation = perfabTs.localRotation;
        }
    }
}
