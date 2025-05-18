using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{

    public class ShowGameExplain : MonoBehaviour
    {
       
        [SerializeField]
        public float uiSpace = 10f;
        [SerializeField]
        public GameObject showObjectParent;
        [SerializeField]
        public GameObject showObjectChild;
        //explainItemsName 与 explainItemsTexture 应该一一对应
        [SerializeField] public List< string> explainItemsNames;
        [SerializeField] public Sprite[] explainItemsTextures;

        //显示玩法
        public void ShowExplains(IDictionary<string, string> explainItems)
        {
            try
            {
                TryShowExplains(explainItems);
            }
            catch (ArgumentOutOfRangeException e)
            {
                YxDebug.LogError("数组的索引不正确，两个数组的大小是否对应或者传入消息字典key是否在客端存在 ：" + e);
            }
        }
        
       
        public virtual void TryShowExplains(IDictionary<string,string> explainItems)
        {
            float startPositionY = 0;
            
            foreach (var explainItem in explainItems)
            {
                int idx = explainItemsNames.IndexOf(explainItem.Key);
                if(idx>-1){
                    Sprite explainItemSprite = explainItemsTextures[idx];              
                    GameObject gameExplainItem = InstanceAndInitExplainItem(explainItemSprite, explainItem.Value);
                    ExplainItemLayout(ref startPositionY, gameExplainItem);
                }
            }
            
        }
        //实例化并初始游戏玩法展示的内容
        public GameObject InstanceAndInitExplainItem( Sprite titleTexture,string contextString)
        {
            var showStringItem = Instantiate(showObjectChild);
            showStringItem.SetActive(true);
            Image tileImage = showStringItem.transform.FindChild("Image").GetComponent<Image>();
            tileImage.sprite = titleTexture;
            Text contenText = showStringItem.transform.FindChild("Text").GetComponent<Text>();
            contenText.text = contextString;
            showStringItem.transform.parent = showObjectParent.transform;
            return showStringItem;
        }
        //排序玩法展示的内容
        public void ExplainItemLayout(ref float startPositionY,GameObject layoutObj)
        {
            RectTransform layoutObjTransform = layoutObj.GetComponent<RectTransform>();
            layoutObjTransform.localScale = Vector3.one;
            layoutObjTransform.localPosition = Vector3.zero;
            Image tileImage = layoutObj.transform.FindChild("Image").GetComponent<Image>();
            Text contentText = layoutObj.transform.FindChild("Text").GetComponent<Text>();
            float layoutObjHeight = (tileImage.preferredHeight > contentText.preferredHeight) ? tileImage.preferredHeight + uiSpace : contentText.preferredHeight + uiSpace;
            float layoutObjWidth = layoutObjTransform.sizeDelta.x;
            layoutObjTransform.sizeDelta = new Vector2(layoutObjWidth, layoutObjHeight);
            layoutObjTransform.localPosition = new Vector3(uiSpace, startPositionY, layoutObjTransform.localPosition.z);
            startPositionY -= layoutObjHeight;
        }




       

        
    }
}