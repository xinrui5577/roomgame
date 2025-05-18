using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.brnn3d
{
    public class BankerListUI : MonoBehaviour
    {
        public Transform BankerItem;

        //清理上下庄列表UI
        public void DeleteBankerListUI()
        {
            foreach (Transform t in BankerItem.parent)
            {
                if (t == BankerItem) continue;
                Destroy(t.gameObject);
            }
        }

        //设置庄家列表的UI
        public void SetBankerListUI(string bankerName, long money)
        {
            var item = Instantiate(BankerItem.gameObject);
            item.transform.FindChild("Name").GetComponent<Text>().text = bankerName;
            item.transform.FindChild("Money").GetComponent<Text>().text = YxUtiles.GetShowNumberForm(money);
            item.transform.parent = BankerItem.parent;
            item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y, 0);
            item.transform.localScale = Vector3.one;

            item.SetActive(true);
        }

    }
}

