using System.Collections;
using System.Text;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    public class CreateHelpWindow : YxWindow
    {
        public UILabel ContentLabel;

        protected override void OnAwake()
        {
            base.OnAwake();
            InitStateTotal = 2; 
        }

        protected override void OnFreshView()
        { 
            if (!(Data is string)) return;
            var url = Data as string;
            if (string.IsNullOrEmpty(url)) return;
            Facade.Instance<DownLoadTool>().LoadFile(url,CallBack);
        }

        private void CallBack(byte[] obj)
        {
            var content = Encoding.UTF8.GetString(obj);
            ContentLabel.text = content;//todo 真奇怪
            StartCoroutine(UpdaateLabel());
        }

        private IEnumerator UpdaateLabel()
        {
            ContentLabel.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
            ContentLabel.gameObject.SetActive(true);
        }
    }
}
