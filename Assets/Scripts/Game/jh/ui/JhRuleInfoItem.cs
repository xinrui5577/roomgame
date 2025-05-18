using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhRuleInfoItem : MonoBehaviour
    {

        public UILabel Name;

        public UILabel Value;

        public void SetInfo(string name,string value)
        {
            Name.text = name + ":";

            Value.text = value;
        }

    }
}
