using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class SettingPnl : MonoBehaviour
    {
        public GameObject BgParent;
         
        public void Show(bool isShow)
        {
            BgParent.SetActive(isShow);
        }
        public void OnSureBtn()
        {
            Show(false);
        }
    }
}
