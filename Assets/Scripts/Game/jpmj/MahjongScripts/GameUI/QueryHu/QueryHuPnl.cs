using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI.QueryHu
{
    public class QueryHuPnl : MonoBehaviour
    {
        public RectTransform HupaiInfoGroup;
        public GridLayoutGroup Grid;
        public HupaiItem ItemPrefab;

        public int RowConstraintCount;//行元素约束
        public int AmendOffsetX; //pnl超出屏幕修正 x轴
        public int MoveRangeX; //pnl活动范围

        public Camera HandCamera { get { return handCamera; } }
        public Vector2 CanvasSize { get { return canvasSize; } }

        private GameObject Bg;
        private Camera handCamera;
        private Vector2 canvasSize = Vector2.zero;
        private List<HupaiItem> itemCache = new List<HupaiItem>();      

        private int _sgin;

        /// <summary>
        /// 居中显示
        /// </summary>
        public bool JuZhong = false;

        public bool IsOverStepScreen
        {
            get
            {
                float pos3D = Mathf.Abs(HupaiInfoGroup.anchoredPosition3D.x);
                int pos = (int)(HupaiInfoGroup.sizeDelta.x * 0.5 + pos3D);
                return pos >= canvasSize.x * 0.5f - MoveRangeX;
            }
        }

        void Awake()
        {
            Grid.constraintCount = RowConstraintCount;
            CanvasScaler CScaler = transform.GetComponentInParent<CanvasScaler>();
            if (null == CScaler) return;
            canvasSize = CScaler.referenceResolution;
            handCamera = GameObject.Find("GameTable/handCardCamera").GetComponent<Camera>();
            Bg = transform.FindChild("Bg").gameObject;
            Bg.SetActive(false);
        }

        private void Push(int value, Transform parent)
        {
            if (!itemCache.Exists((a) => { return a.Value == value; }))
            {
                HupaiItem item = Instantiate<HupaiItem>(ItemPrefab);

                //0是显示任意牌
                if (value == 0)
                    item.AnyCard(parent);
                else
                {
                    GameObject obj = D2MahjongMng.Instance.GetMj(value, EnD2MjType.Me);
                    item.NormalCard(obj, parent, value);
                }

                itemCache.Add(item);
            }
        }

        private void Push(List<int> arr, Transform parent)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                Push(arr[i], parent);
            }
        }

        private HupaiItem Pop(int value, int laizi, Transform perant)
        {
            HupaiItem item;

            item = itemCache.Find((a) => { return a.Value == value; });

            if (null != item)
            {
                item.transform.SetParent(perant);             
                item.transform.localScale = Vector3.one;

                if (laizi == value)
                    item.SetLaizi();

                if (laizi >= 96)
                {
                    if ((value >= 96) && (value < 100))
                        item.SetLaizi();
                    if (value >= 100 && value < 104)
                        item.SetLaizi();
                }

                item.gameObject.SetActive(true);
            }

            return item;
        }

        public void OnShowHulist(EventData evn)
        {
            QueryHulistData data = (QueryHulistData)evn.data1;
            Transform card = (Transform)evn.data2;

            if (data == null || card == null || data.Cards.Count == 0) return;           

            Bg.SetActive(true);
            _sgin = data.Laizi;          

            //显示任意牌
            if (data.Flag == (int)MahjongIcon.Flag.Youjin || null == data.Cards)
            {
                //自适应 窗体大小
                SetPnlSize(1);     
                Push(0, transform); 
                HupaiItem item = Pop(0, -1, Grid.transform);
                item.SetTextNum(data.LeaveMahjongCnt);
            }
            else
            {
                //自适应 窗体大小
                SetPnlSize(data.Cards.Count);  
                //生成牌
                Push(data.Cards, transform);
                //牌值s
                List<int> values = data.Cards;
                //牌数量
                List<int> numArr = data.CardsNum;
                ShowHupaiInfo(values, numArr, _sgin);
            }

            //窗体位置
            SetPnlPosition(card.position);
        }
     
        /// <summary>
        /// 显示胡牌panel 入口
        /// </summary>
        /// <param name="cards">可胡的牌</param>
        /// <param name="cardsNum">可胡的牌的数量</param> 
        public void OnShowHulist(List<int> cards, List<int> cardsNum)
        {
            transform.localPosition = Vector3.zero;
            if (cards == null || cardsNum == null || cards.Count == 0) return;

            Bg.SetActive(true);          
            //自适应 窗体大小
            SetPnlSize(cards.Count);        

            //生成牌
            Push(cards, transform);

            ShowHupaiInfo(cards, cardsNum, -1);  

            HupaiInfoGroup.transform.localPosition = Vector3.zero;            
        }

        public void ShowHupaiInfo(List<int> values, List<int> numlist, int sign)
        {
            for (int i = 0; i < values.Count; i++)
            {
                HupaiItem item = Pop(values[i], sign, Grid.transform);
                if (item != null)
                {
                    item.SetTextNum(numlist[i]);
                }
            }
        }

        public void HidePnl()
        {
            for (int i = 0; i < itemCache.Count; i++)
            {
                itemCache[i].HideLaizi();
                itemCache[i].transform.SetParent(transform);
                itemCache[i].gameObject.SetActive(false);
            }

            Bg.SetActive(false);
        }

        private void SetPnlSize(int itemNum)
        {
            int num = itemNum >= RowConstraintCount ? RowConstraintCount : itemNum;
            float width = Grid.cellSize.x * num + 150;

            //行数
            float RowNum = Mathf.Ceil((float)itemNum / RowConstraintCount);
            float high = (Grid.cellSize.y + Grid.spacing.y) * RowNum + 25;
            HupaiInfoGroup.sizeDelta = new Vector2(width, high);
        }

        private void SetPnlPosition(Vector3 worldPos)
        {
            if (null == HandCamera || null == CanvasSize) return;

            Vector3 screenPoint = HandCamera.WorldToScreenPoint(worldPos);
            float offsetX = CanvasSize.x * screenPoint.x / Screen.width;
            float x = -CanvasSize.x * 0.5f + offsetX;
            Vector3 uiScreenPoint = new Vector3(x, -220f, 0);
            
            HupaiInfoGroup.anchoredPosition3D = uiScreenPoint;

            //修正位置
            if (IsOverStepScreen)
            {
                AmendOffsetX = Screen.width * AmendOffsetX / (int)canvasSize.x;
                float pos = Screen.width * 0.5f - AmendOffsetX - HupaiInfoGroup.sizeDelta.x * 0.5f;

                uiScreenPoint.x = canvasSize.x * 0.5f > screenPoint.x ? -pos : pos;
                HupaiInfoGroup.anchoredPosition3D = uiScreenPoint;
            }
            if (JuZhong)
            {
                HupaiInfoGroup.anchoredPosition3D=new Vector3(0,-90f,0);
            }
        }

        //麻将坐标转换为屏幕坐标
        //private Vector3 ItemToScreenPos(Vector3 pos)
        //{
        //    if (null == HandCamera || null == CanvasSize)
        //        return Vector3.zero;

        //    Vector3 screenPoint = HandCamera.WorldToScreenPoint(pos);

        //    float offsetX = CanvasSize.x * screenPoint.x / Screen.width;
        //    float x = -CanvasSize.x * 0.5f + offsetX;
        //    return new Vector3(x, -220f, 0);
        //}

    }
}