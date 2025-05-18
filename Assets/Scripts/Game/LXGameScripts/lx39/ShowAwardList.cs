using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.LXGameScripts.lx39
{
    /// <summary>
    /// 显示奖励列表
    /// </summary>
    public class ShowAwardList : MonoBehaviour
    {
        public UIGrid ThreeGrid;
        public UIGrid NineGrid;
        public GameObject[] ThreeItems;
        public GameObject NineItem;
        public Transform ShowPos;

        private int[] ThreeAward = { 80, 70, 60, 50, 30, 30, 30, 20, 20, 10, 5, 2 };
        private int[] NineAward = { 100, 80, 70, 60, 45, 45, 45, 20, 15 };
        private string _sName = "show_0";
        private string[] spriteNames = { "icon_01", "icon_02", "icon_03" };
        private bool isFirst = true;
        private Vector3 StartPos;
        
        // Use this for initialization
        void Start()
        {
            StartPos = transform.localPosition;
        }
        

        protected virtual void CreateThreeAwardList()
        {
            for (int i = 0; i < ThreeAward.Length; i++)
            {
                GameObject go = null;
                if (i < ThreeAward.Length - 2)
                    go = Instantiate(ThreeItems[0]);
                else if (i == ThreeAward.Length - 2)
                    go = Instantiate(ThreeItems[1]);
                else if (i == ThreeAward.Length - 1)
                    go = Instantiate(ThreeItems[2]);
                if (go != null)
                {
                    go.transform.parent = ThreeGrid.transform;
                    InitGameObject(go);
                    UILabel award = go.transform.FindChild("Label").GetComponent<UILabel>();
                    award.text = ThreeAward[i].ToString();
                    if (i < ThreeAward.Length - 2)
                    {
                        UISprite[] icons = GetUiSpriteByName(go);
                        foreach (var icon in icons)
                        {
                            if (icon != null)
                                icon.spriteName = _sName + i;
                        }
                    }
                    else
                    {
                        UISprite[] icons = GetUiSpriteByName(go);
                        foreach (var icon in icons)
                        {
                            if (icon != null)
                                icon.spriteName = _sName + 9;
                        }
                    }
                }
            }
        }

        protected virtual void CreateNineAwardList()
        {
            for (int i = 0; i < NineAward.Length; i++)
            {
                GameObject go = Instantiate(NineItem);
                if (go != null)
                {
                    go.transform.parent = NineGrid.transform;
                    InitGameObject(go);
                    go.transform.FindChild(spriteNames[0]).GetComponent<UISprite>().spriteName = _sName + i;
                    go.transform.FindChild("Label").GetComponent<UILabel>().text = NineAward[i].ToString();
                }
            }
        }

        protected virtual UISprite[] GetUiSpriteByName(GameObject go)
        {
            UISprite[] target = new UISprite[spriteNames.Length];
            UISprite[] temp = go.GetComponentsInChildren<UISprite>();
            for (int i = 0; i < target.Length; i++)
            {
                foreach (var value in temp)
                {
                    if (value.name == spriteNames[i])
                    {
                        target[i] = value;
                    }
                }
            }
            return target;
        }

        public virtual void Show()
        {
            if (isFirst)
            {
                CreateNineAwardList();
                CreateThreeAwardList();
                isFirst = false;
            }
            transform.parent.gameObject.SetActive(true);
            iTween.MoveTo(gameObject, ShowPos.position, 1.5f);
        }

        public virtual void Hide()
        {
            Hashtable hash = new Hashtable();
            hash.Add("position", StartPos);
            hash.Add("time", 1);
            hash.Add("oncomplete", "HideParent");
            hash.Add("islocal", true);
            iTween.MoveTo(gameObject, hash);
        }

        protected void InitGameObject(GameObject temp)
        {
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localScale = Vector3.one;
        }

        public void HideParent()
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}