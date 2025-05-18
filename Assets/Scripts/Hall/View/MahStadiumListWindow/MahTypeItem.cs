using UnityEngine;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahTypeItem : MonoBehaviour
    {
        public UILabel MahName;
        public UIButton DeleButton;

        public void InitData(string mahName)
        {
            MahName.text = mahName;
        }
    }
}
