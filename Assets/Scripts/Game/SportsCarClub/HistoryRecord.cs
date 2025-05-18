using System.Linq;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.SportsCarClub
{
    /// <summary>
    /// 历史记录 单例
    /// </summary>
    public class HistoryRecord : MonoBehaviour
    {

        private static HistoryRecord _instance;

        public static HistoryRecord GetInstance()
        {
            if (_instance == null)
            {
                _instance = new HistoryRecord();
            }
            return _instance;
        }

        void Awake()
        {
            _instance = this;
        }

        /// <summary>
        /// 分数记录
        /// </summary>
        public GameObject[] Scores;
        /// <summary>
        /// 分数记录数据
        /// </summary>
        public int[] ScoresDatas;
        /// <summary>
        /// 开奖记录
        /// </summary>
        public UISprite[] Records;
        /// <summary>
        /// 记录数据
        /// </summary>
        public int[] RecordsDatas;
        /// <summary>
        /// 记录下标
        /// </summary>
        public int RecordsIndex;
        /// <summary>
        /// 小开奖记录
        /// </summary>
        public UISprite[] SmallRecords;
        /// <summary>
        /// 小开奖记录页码
        /// </summary>
        public int PageIndex;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public GameObject BG;

        public void OpenPanel()
        {
            BG.SetActive(true);
            //RefreshHistory();
        }

        public void ClosePanel()
        {
            BG.SetActive(false);
        }
        /// <summary>
        /// 初始化记录面板
        /// </summary>
        /// <param name="recordsD"></param>
        /// <param name="recordsI"></param>
        /// <param name="scoresD"></param>
        public void InitHistory(int[] recordsD, int recordsI, int[] scoresD)
        {
            //Debug.LogError("recordsD.Length:" + recordsD.Length);
            //目前服務器上返的值为40，前端只有24个记录位置,ScoresD依然是8个
            recordsD = recordsD.Skip(0).Take(24).ToArray();  //截取24个=>数组第一位对应的是最后出的奖
            if (recordsI > 23) recordsI = 23;

            ScoresDatas = scoresD;
            RecordsDatas = recordsD;
            RecordsIndex = recordsI;
            PageIndex = 0;

            for (int i = 0; i < Wheel.GetInstance().ItemsValue.Length; i++)
            {
                Scores[i].transform.FindChild("Sprite").transform.GetComponent<UISprite>().spriteName = "car_" + (int)Wheel.GetInstance().ItemsValue[i];
                Scores[i].transform.FindChild("Sprite").transform.GetComponent<UISprite>().MakePixelPerfect();
                Scores[i].transform.FindChild("Sprite").transform.localScale = new Vector3(0.4f, 0.4f, 1f);
                Scores[i].transform.FindChild("Label").transform.GetComponent<UILabel>().text = ScoresDatas[i].ToString();
            }

            for (int i = 0; i < RecordsDatas.Length; i++)
            {
                int index = i > RecordsIndex ? RecordsDatas.Length + RecordsIndex - i : RecordsIndex - i;
                Records[index].spriteName = "car_" + (RecordsDatas[i] % ScoresDatas.Length);
                Records[index].MakePixelPerfect();
                Records[index].transform.localScale = new Vector3(0.4f, 0.4f, 1f);
            }

            RefreshPage();
        }
        /// <summary>
        /// 刷新历史记录数据
        /// </summary>
        /// <param name="addRecords"></param>
        /// <param name="addScores"></param>
        /// <param name="luckIndex"></param>
        public void RefreshData(int addRecords, int addScores)
        {
            RecordsIndex = (RecordsIndex + 1) % RecordsDatas.Length;
            RecordsDatas[RecordsIndex] = addRecords;
            ScoresDatas[addRecords % ScoresDatas.Length] += addScores;

            RefreshHistory();
        }
        /// <summary>
        /// 刷新界面
        /// </summary>
        public void RefreshHistory()
        {
            for (int i = 0; i < Wheel.GetInstance().ItemsValue.Length; i++)
            {
                Scores[i].transform.FindChild("Label").transform.GetComponent<UILabel>().text = ScoresDatas[i].ToString();
            }

            for (int i = 0; i < RecordsDatas.Length; i++)
            {
                int index = i > RecordsIndex ? RecordsDatas.Length + RecordsIndex - i : RecordsIndex - i;
                //YxDebug.Log("I == " + i + " Index == " + index);
                Records[index].spriteName = "car_" + (RecordsDatas[i] % ScoresDatas.Length);
                Records[index].MakePixelPerfect();
                Records[index].transform.localScale = new Vector3(0.4f, 0.4f, 1f);
            }

            RefreshPage();
        }

        #region 小历史记录

        public GameObject smallSelected;

        public void UpPage()
        {
            PageIndex = PageIndex - 1 >= 0 ? PageIndex - 1 : 0;
            RefreshPage();

            smallSelected.SetActive(true);
        }

        public void DownPage()
        {
            PageIndex = PageIndex + 1 <= 1 ? PageIndex + 1 : 1;
            RefreshPage();

            smallSelected.SetActive(false);
        }

        public void RefreshPage()
        {
            for (int i = 0; i < RecordsDatas.Length; i++)
            {
                int index = i > RecordsIndex ? RecordsDatas.Length + RecordsIndex - i : RecordsIndex - i;
                //YxDebug.Log("I == " + i + " Index == " + index);
                if (index < 20 && PageIndex == (int)(index / 10))
                {
                    index = index % 10;
                    SmallRecords[index].spriteName = "car_" + (RecordsDatas[i] % ScoresDatas.Length);
                    SmallRecords[index].MakePixelPerfect();
                    SmallRecords[index].transform.localScale = new Vector3(0.4f, 0.4f, 1f);
                }
            }
        }

        #endregion
    }
}
