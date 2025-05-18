using UnityEngine;

namespace Assets.Scripts.Game.toubao
{
    public class HistoryPanel : MonoBehaviour
    {
        public HistoryItem PreItem;
        public UIGrid grid;
        private int sortNum = 99999;
        public void CreateHistoryItem(int[] points)
        {
            HistoryItem item=Instantiate(PreItem);
            item.SetValue(points);
            item.transform.parent = grid.transform;
            item.transform.localPosition=Vector3.zero;
            item.transform.localScale=Vector3.one;
            item.gameObject.SetActive(true);
            item.name = "item" + sortNum;
            sortNum--;
            DeleteMore();
            grid.Reposition();
        }

        private int maxCount = 10;
        void DeleteMore()
        {
            if (grid.transform.childCount > maxCount)
            {
                Transform firstChild=grid.transform.GetChild(0);
                Destroy(firstChild.gameObject);
            }
        }
	
    }
}
