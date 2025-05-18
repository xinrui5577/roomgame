using System.Globalization;
using UnityEngine;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahStadiumItem : MonoBehaviour
    {
        public UILabel SerialNumber;
        public UILabel MahStadiumName;
        public UILabel GoodCommend;
        public UISprite EnterBtn;
        public int IsUsepwd;


        public void InitMahStadiumInfo(int index, string mahStadiumName, string goodCommend, int isUsepwd)
        {
            SerialNumber.text = index.ToString(CultureInfo.InvariantCulture);
            MahStadiumName.text = mahStadiumName;
            GoodCommend.text = goodCommend;
            IsUsepwd = isUsepwd;
        }
    }
}
