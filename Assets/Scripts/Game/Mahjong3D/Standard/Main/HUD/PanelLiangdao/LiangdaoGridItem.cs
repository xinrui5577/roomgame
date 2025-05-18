using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class LiangdaoGridItem : MonoBehaviour
    {
        public UIItemsManager ItemStore;
        public GameObject Tick;

        private bool mChoose;

        public int Value { get; set; }

        public bool ChooseFlag
        {
            get
            {
                return mChoose;
            }
            set
            {
                mChoose = value;
                Tick.SetActive(value);
            }
        }

        public void SetCard(int value)
        {
            Value = value;
            gameObject.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                var item = ItemStore.GetItem<UIBigCardItem>(i);
                if (item != null)
                {
                    item.SetCard(value);
                }
            }
        }

        public void OnItemClick()
        {
            ChooseFlag = !ChooseFlag;
        }

        private void OnDisable()
        {
            gameObject.SetActive(false);
        }
    }
}
