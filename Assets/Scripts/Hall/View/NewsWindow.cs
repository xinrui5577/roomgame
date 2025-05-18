using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows; 
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View
{
    public class NewsWindow : YxNguiWindow
    {
        public UILabel NewsLabel;
        protected override void OnAwake()
        {
            Facade.Instance<TwManager>().SendAction("simpleMessage", new Dictionary<string, object>(), UpdateView);
        }

        protected override void OnFreshView()
        {
            if (Data == null) return;
            NewsLabel.text = Data.ToString();
            NewsLabel.gameObject.SetActive(false);
            StartCoroutine(UpdateLabel());
        }

        private IEnumerator UpdateLabel()
        {
            yield return null;
            NewsLabel.gameObject.SetActive(true);
        }
    }
}
