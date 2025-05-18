using UnityEngine;

namespace Assets.Scripts.Game.toubao
{
    public class BankerItem : MonoBehaviour
    {
        public UILabel Name;
        public UILabel Gold;

        public void SetValue(string Name,string Gold)
        {
            this.Name.text = Name;
            this.Gold.text = Gold;
        }
    }
}
