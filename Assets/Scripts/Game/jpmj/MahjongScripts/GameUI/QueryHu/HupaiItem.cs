using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI.QueryHu
{
    public class HupaiItem : MonoBehaviour
    {
        public int Value;
        public Text Num;
        private GameObject Obj;

        public void NormalCard(GameObject obj, Transform parent, int value)
        {           
            gameObject.SetActive(true);
            Value = value;
            Obj = obj;

            transform.SetParent(parent); 
            Obj.transform.SetParent(transform);
            
            Obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-50, 0, 0);
            transform.FindChild("AnyCard").gameObject.SetActive(false);
        }

        public void AnyCard(Transform parent)
        {
            Value = 0;           
            transform.SetParent(parent);
            transform.FindChild("AnyCard").gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        public void SetTextNum(int num)
        {
            Num.text = num.ToString();
        }

        public void SetLaizi()
        {
            if (Obj == null) return;
            Transform obj = Obj.transform.Find("Laizi");
            if (obj != null) obj.gameObject.SetActive(true);
        }

        public void HideLaizi()
        {
            if (Obj == null) return;
            Transform obj = Obj.transform.Find("Laizi");
            if (obj != null) obj.gameObject.SetActive(false);
        }

    }
}
