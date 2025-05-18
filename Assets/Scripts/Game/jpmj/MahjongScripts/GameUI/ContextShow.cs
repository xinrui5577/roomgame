using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{

    public class ContextShow : MonoBehaviour
    {
        private string gameRules ;
        private string splitFlag  ;        
        public float uiSpace = 10f;
        public GameObject showObjectParent;
        public GameObject showObjectChild;
        [SerializeField]
        public Sprite[] gameExplainItemTexture;

        void Awake()
        {
            splitFlag = "-";
            gameRules = "一码全中 碰碰胡 七对 抢杠胡";
            EventDispatch.Instance.RegisteEvent((int)UIEventId.RoomInfo, ParseGameRulesInfo);           
        }
       
        private void ParseGameRulesInfo(int eventId, EventData data)
        {
            var roomInfo = (RoomInfo)data.data1;
            gameRules = roomInfo.MaxRound + "局-" + gameRules;
            ShowGameExplain(gameRules);
        }

        // Use this for initialization
       
        ///<summary>
        ///传入的string中请携带三个分割符 ，分隔符为split 变量；
        /// </summary>
        public void ShowGameExplain(string showString)
        {
            float HeightNum = 0;
            string[] showStringAry = Regex.Split(showString, splitFlag);                      
            for (int i = 0; i < showStringAry.Length; i++)
            {
                if ( string.IsNullOrEmpty(showStringAry[i]) )continue;
                GameObject gameExplainItem = InstanceAndInitGameExplainItem(gameExplainItemTexture[i], showStringAry[i]);
                GameExplainItemLayout(ref HeightNum,gameExplainItem);

            }            
        }
        //实例化并初始游戏玩法展示的内容
        public GameObject InstanceAndInitGameExplainItem( Sprite titleTexture,string contextString)
        {
            var showStringItem = Instantiate(showObjectChild);
            Image tileImage = showStringItem.transform.FindChild("Image").GetComponent<Image>();
            tileImage.sprite = titleTexture;
            Text contenText = showStringItem.transform.FindChild("Text").GetComponent<Text>();
            contenText.text = contextString;
            showStringItem.transform.parent = showObjectParent.transform;
            return showStringItem;
        }
        //排序游戏玩法展示的内容
        public void GameExplainItemLayout(ref float positionY,GameObject layoutObj)
        {
            RectTransform showStringItemRectTransform = layoutObj.GetComponent<RectTransform>();
            showStringItemRectTransform.localScale = Vector3.one;
            showStringItemRectTransform.localPosition = Vector3.zero;
            Image tileImage = layoutObj.transform.FindChild("Image").GetComponent<Image>();
            Text contenText = layoutObj.transform.FindChild("Text").GetComponent<Text>();
            float showStringItemRectTransformHeight = (tileImage.preferredHeight > contenText.preferredHeight) ? tileImage.preferredHeight + uiSpace : contenText.preferredHeight + uiSpace;
            float showStringItemRectTransformWidth = showStringItemRectTransform.sizeDelta.x;
            showStringItemRectTransform.sizeDelta = new Vector2(showStringItemRectTransformWidth, showStringItemRectTransformHeight);
            showStringItemRectTransform.localPosition = new Vector3(uiSpace, positionY, showStringItemRectTransform.localPosition.z);
            positionY -= showStringItemRectTransformHeight;
        }




       

        
    }
}