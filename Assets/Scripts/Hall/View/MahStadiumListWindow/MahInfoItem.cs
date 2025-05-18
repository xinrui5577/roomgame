using UnityEngine;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahInfoItem : MonoBehaviour
    {
        public void InitMahInfoData(string mahName)
        {
            GetComponent<UILabel>().text = mahName;
        }
    }
}
