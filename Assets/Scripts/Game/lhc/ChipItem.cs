using UnityEngine;

namespace Assets.Scripts.Game.lhc
{
    public class ChipItem : MonoBehaviour
    {
        public UILabel ChipValueLabel;
//        public int Pos;

        public void FreshDep(int dep)
        {
            gameObject.GetComponent<UISprite>().depth += dep;
            ChipValueLabel.depth += dep;
        }
    }
}
