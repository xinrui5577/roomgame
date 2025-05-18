using UnityEngine;

namespace Assets.Scripts.Hall.View.AlllShowRecordWindow
{
    /// <summary>
    /// 战绩上方的标签
    /// </summary>
    public class TabItemSpecial : MonoBehaviour
    {
        public UILabel UpName;
        public UILabel DownName;
        public UIToggle Toggle;
        public UIToggledObjects ToggledObjects;
        public string NameShow;

        public void InitData(int round,bool isActive)
        {
            if (UpName != null) UpName.text = string.Format(NameShow, round);
            if (DownName != null) DownName.text = string.Format(NameShow, round);
            Toggle.startsActive = isActive;
        }
    }
}
