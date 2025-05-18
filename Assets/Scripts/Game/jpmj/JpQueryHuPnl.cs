using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj
{
    public class JpQueryHuPnl : MonoBehaviour
    {
        [SerializeField] protected JpHuManjongGroupInfo JpHumjGroupInfo;

        [SerializeField]
        protected GameObject BgParent;

        [SerializeField]
        protected GameObject BgOrg;

        [SerializeField] private string _keyCards = "cards";
        [SerializeField]
        private string _keyTai = "tai";
        [SerializeField]
        private string _keyHua = "hua";
        [SerializeField]
        private string _keyCnt = "cnt";

        /// <summary>
        /// 每行限宽
        /// </summary>
        [SerializeField] private float _maxWigth = 500;

        /// <summary>
        /// 每行限宽
        /// </summary>
        [SerializeField]
        private int _hideLayer = -1000000;

        public static JpQueryHuPnl Instance {
            get { return _instance; }
        }
        private static JpQueryHuPnl _instance;

        void Awake()
        {
            _instance = this;
        }

        void OnDestory()
        {
            _instance = null;
        }

/*        // Use this for initialization
        void Start ()
        {

            ISFSObject data = new SFSObject();
            data.PutIntArray("cards", new int[] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 33, 34, 35, 36, 37, 38, 39, 40, 41, 49, 50, 51, 52, 53, 54, 55, 56, 57 });//,35,36,37,38,39,40,41,49,50,51,52,53,54,55,56,57
            data.PutInt("tai", 11);
            data.PutInt("hua", 12);
            data.PutInt("cnt", 13);

            ISFSObject data1 = new SFSObject();
            data1.PutIntArray("cards", new int[] { 17});
            data1.PutInt("tai", 5);
            data1.PutInt("hua", 55);
            data1.PutInt("cnt", 5);


            ISFSObject data2 = new SFSObject();
            data2.PutIntArray("cards", new int[] { 18, 18, 18, 18, 18, 18, 19, 20, 21, 22, 23, 24, });
            data2.PutInt("tai", 6);
            data2.PutInt("hua", 6);
            data2.PutInt("cnt", 666);
            SFSArray dd = new SFSArray();
            dd.AddSFSObject(data);
/*            dd.AddSFSObject(data1);
            dd.AddSFSObject(data2);
            dd.AddSFSObject(data4);
            dd.AddSFSObject(data5);
            dd.AddSFSObject(data6);
            dd.AddSFSObject(data7);
            dd.AddSFSObject(data8);
            dd.AddSFSObject(data9);#1#

            SetMahjongHulistInfo(dd, 15);
        }*/

        private readonly Dictionary<int, GameObject> _hupanelDic = new Dictionary<int, GameObject>();

        public void SetMahjongHulistInfo(ISFSArray dataArray,int cardKey)
        {
            //Reset();

            if (!_hupanelDic.ContainsKey(cardKey))
            {
                var bg = Instantiate(BgOrg);
                bg.transform.SetParent(BgParent.transform);
                bg.transform.localPosition = Vector3.zero;//new Vector3(bg.transform.localPosition.x,bg.transform.localPosition.y,0);
                bg.transform.localScale = Vector3.one;
                _hupanelDic.Add(cardKey,bg);

                SetTotalMjGrouInfo(dataArray, _hupanelDic[cardKey]);

         
            }
            else
            {
                var v3 = _hupanelDic[cardKey].transform.localPosition;
                _hupanelDic[cardKey].transform.localPosition = new Vector3(v3.x,v3.y,0);
            }



            //ShowJpqueryHuPnl();
        }

/*        private void SetMjCdsInfo(ISFSObject isfObj ,ref int with,ref int hight)
        {
            var sfsobj = isfObj;
            var cds = sfsobj.GetIntArray(_keyCards);
            var cdslen = cds.Length;
            if ((with + cdslen * Mjwith) > Screen.width)
            {
                var leftwigth = Screen.width - with;

                var num = (int)Math.Ceiling((double)leftwigth / Mjwith);

                var cdsgp1 = new int[num];
                for (var i = 0; i < num; i++)
                {
                    cdsgp1[i] = cds[i];
                    Debug.LogError(i);
                }
                sfsobj.PutIntArray(_keyCards, cdsgp1);

                var gob =  AddMjGroupInfo(sfsobj);
                gob.transform.localPosition = new Vector3(with,hight,0);

                with = 0;
                hight -= 80;

                var leftLen = cdslen - num;

                if (leftLen > 0)
                {
                    var cdsgp2 = new int[cdslen - num];
                    int j = 0;
                    for (var i = num; i < cdslen; i++)
                    {
                        cdsgp2[j] = cds[i];
                        j++;
                    }
                    sfsobj.PutIntArray(_keyCards, cdsgp2);
                    SetMjCdsInfo(sfsobj, ref with, ref hight);
                }


            }
            else
            {
                var gob = AddMjGroupInfo(sfsobj);
                gob.transform.localPosition = new Vector3(with, hight, 0);
                with += cdslen * Mjwith+50;
            }
        }*/


        private void SetTotalMjGrouInfo(ISFSArray dataArray,GameObject bg)
        {
            JpHuManjongGroupInfo groupInfo = null;
            foreach (var obj in dataArray)
            {
                AddMjGroupInfo((ISFSObject)obj,bg, ref groupInfo);
            }

            SortPnl(bg);
        }

        private void AddMjGroupInfo(ISFSObject data, GameObject bg,ref JpHuManjongGroupInfo groupInfo)
        {
            if (groupInfo == null)
            {
                groupInfo = Instantiate(JpHumjGroupInfo);
                groupInfo.gameObject.SetActive(true);
                groupInfo.gameObject.transform.SetParent(bg.transform);
                groupInfo.transform.localScale = Vector3.one;
                Vector3 gpTransform = groupInfo.GetComponent<RectTransform>().localPosition;
                groupInfo.GetComponent<RectTransform>().localPosition
                    = new Vector3(gpTransform.x, gpTransform.y, 0);
            }

            var cards = data.GetIntArray(_keyCards);
            var tai = data.GetInt(_keyTai);
            var hua = data.GetInt(_keyHua);
            var cnt = data.GetInt(_keyCnt);

            if(cards==null) return;
            var len = cards.Length;
            if (len < 1) return;

            for (int i = 0; i < len; i++)
            {
                groupInfo.AddMjGobCell(cards[i]);
                if (groupInfo.PosX > _maxWigth)// && i < len - 1
                {
                    if (len - (i + 1) > 0)
                    {
                        groupInfo = null;
                        var j = 0;
                        var cdsLeftList = new int[len - (i + 1)];
                        for (int k = i + 1; k < len; k++)
                        {
                            cdsLeftList[j] = cards[k];
                            j++;
                        }

                        data.PutIntArray(_keyCards, cdsLeftList);
                        AddMjGroupInfo(data, bg, ref groupInfo);
                    }
                    else
                    {

                        groupInfo.AddInfotextGob(tai, hua, cnt);
                        groupInfo = null;
                    }


                    return;
                }
            }

            groupInfo.AddInfotextGob(tai, hua, cnt);

        }

        [SerializeField]
        protected int Wightinterval = 65;
        [SerializeField]
        protected int Hightinterval = 98;


        [SerializeField]
        protected int DefaultMaxWigth = 860;
        /// <summary>
        /// 调整设置麻将和背景板元素布局
        /// </summary>
        private void SortPnl(GameObject bg)
        {
            var len = bg.transform.childCount;
            if (len <= 2)
            {
                for (int i = 0; i < len; i++)
                {
                    if (bg.transform.GetChild(i).name != "Button")
                    {
                        bg.GetComponent<RectTransform>().sizeDelta  =  new Vector2(bg.transform.GetChild(i).childCount*Wightinterval, Hightinterval);
                        return;
                    }
                }
            }
            else
            {
                bg.GetComponent<RectTransform>().sizeDelta = new Vector2(DefaultMaxWigth, Hightinterval * len);
            }

        }


        public void ClearMahJongGroup()
        {
            _hupanelDic.Clear();
            while (BgParent.transform.childCount > 0)
                {
                    DestroyImmediate(BgParent.transform.GetChild(0).gameObject);
                }
        }

        public void HideJpqueryHuPnl()
        {
            var childCount = BgParent.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var v3 = BgParent.transform.GetChild(i).localPosition;
                BgParent.transform.GetChild(i).localPosition = new Vector3(v3.x, v3.y, _hideLayer);
            }
        }

/*        public void ShowJpqueryHuPnl()
        {
            var v3 = gameObject.transform.localPosition;
            gameObject.transform.localPosition = new Vector3(v3.x, v3.y, 0);
        }*/
    }
}
