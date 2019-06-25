using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Tea
{
    public class DlmjHelpwindow : YxNguiWindow
    {
        public GameObject DangQianUp;
        public GameObject DangQianDown;
        public GameObject YiJingUp;
        public GameObject YiJingDown;
        public GameObject DlmjInfo;
        public GameObject TdhInfo;
      

        public void DangQianClick()
        {
            DlmjInfo.SetActive(true);
            TdhInfo.SetActive(false);
            DangQianUp.SetActive(true);
            DangQianDown.SetActive(false);
            YiJingUp.SetActive(false);
            YiJingDown.SetActive(true);
        }

        public void YiJingClick()
        {
            DlmjInfo.SetActive(false);
            TdhInfo.SetActive(true) ;
            DangQianUp.SetActive(false);
            DangQianDown.SetActive(true);
            YiJingUp.SetActive(true);
            YiJingDown.SetActive(false);
        }


    }
}
