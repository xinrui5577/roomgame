using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon
{
    public class ChatManager : MonoBehaviour
    {
        public Sprite[] SpriteArray;

        public GameObject ExpressModle;
        public GameObject ExpressTemp;
        public GameObject SortTalkTemp;
        //郑谭 左边对话框的黑色字体预制体
        public GameObject LeftSortTalkTemp;
        public static ChatManager Instance;

        private Dictionary<int, List<Sprite>> _dicExpress = new Dictionary<int, List<Sprite>>();
        public string[] ManSortTalk;
        public string[] WomanSortTalk;

        public int FontSize = 20;
        private int _sex;

        void Awake()
        {
            Init();

            Instance = this;
        }

        private void Init()
        {
            foreach (Sprite spr in SpriteArray)
            {
                string name = spr.name;
                int findIndex = name.IndexOf("_");
                if (findIndex == 0 || findIndex == name.Length)
                    continue;

                int key = int.Parse(name.Substring(0, findIndex));

                if (!_dicExpress.ContainsKey(key))
                    _dicExpress.Add(key, new List<Sprite>());

                _dicExpress[key].Add(spr);
            }

            foreach (KeyValuePair<int, List<Sprite>> keyValuePair in _dicExpress)
            {
                keyValuePair.Value.Sort((a, b) =>
                {
                    int findIndexA = a.name.IndexOf("_");
                    string aa = a.name.Substring(findIndexA + 1, a.name.Length - (findIndexA + 1));
                    int indexA = int.Parse(aa);
                    int findIndexB = b.name.IndexOf("_");
                    string bb = b.name.Substring(findIndexB + 1, b.name.Length - (findIndexB + 1));
                    int indexB = int.Parse(bb);

                    if (indexA > indexB)
                        return 1;

                    return -1;
                });
            }
        }

        public GameObject GetExpress(int key)
        {
            GameObject ret = Instantiate(ExpressModle);
            ret.GetComponent<Image>().sprite = _dicExpress[key][0];
            ret.GetComponent<Image>().SetNativeSize();
            List<Sprite> frames = new List<Sprite>();
            for (int i = 0; i < _dicExpress[key].Count; i++)
            {
                frames.Add(Instantiate(_dicExpress[key][i]));
            }
            ret.GetComponent<UGUISpriteAnimation>().SpriteFrames = frames;

            return ret;
        }

        public GameObject GetSortTalk(int key)
        {
            
            string[] talkValue = _sex == 0 ? WomanSortTalk : ManSortTalk;
            //郑谭 实例化黑色字体的预制体
            GameObject text = Instantiate(LeftSortTalkTemp);
            var textScp = text.GetComponent<Text>();
            textScp.text = talkValue[key];
            textScp.fontSize = GameConfig.ChatTxtFontSize;
            textScp.color = Color.black;            
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(text.GetComponent<Text>().preferredWidth, text.GetComponent<Text>().preferredHeight);
            return text;
        }

        public GameObject GetText(string content)
        {

            //郑谭 实例化黑色字体的预制体
            GameObject text = Instantiate(LeftSortTalkTemp);
            var textScp = text.GetComponent<Text>();
            textScp.text = content;
            textScp.fontSize = GameConfig.ChatTxtFontSize;
            textScp.color = Color.black;

            text.GetComponent<RectTransform>().sizeDelta = new Vector2(text.GetComponent<Text>().preferredWidth, text.GetComponent<Text>().preferredHeight);
            return text;
        }

        public List<GameObject> GetExpressIcon(DVoidInt CallBack)
        {
            List<GameObject> lis = new List<GameObject>();

            foreach (KeyValuePair<int, List<Sprite>> item in _dicExpress)
            {
                GameObject itemExp = Instantiate(ExpressTemp);
                itemExp.name = item.Key + "";

                Sprite sprExp = Instantiate(item.Value[0]);

                Image itemExpImg = itemExp.GetComponent<Image>();
                itemExpImg.sprite = sprExp;
                itemExpImg.SetNativeSize();

                Button itemExpBtn = itemExp.GetComponent<Button>();
                itemExpBtn.onClick.AddListener(() =>
                {
                    CallBack(int.Parse(itemExp.name));
                });

                lis.Add(itemExp);
            }

            lis.Sort((a, b) =>
            {
                return a.name.CompareTo(b.name);
            });

            return lis;
        }

        public List<GameObject> GetSortTalkList(int sex, DVoidInt CallBack)
        {
            _sex = sex;
            List<GameObject> lis = new List<GameObject>();
            string[] talkValue = sex == 0 ? WomanSortTalk : ManSortTalk;
            for (int i = 0; i < talkValue.Length; i++)
            {
                GameObject text = Instantiate(SortTalkTemp);
                text.name = i + "";
                var textScp = text.GetComponent<Text>();
                textScp.text = talkValue[i];
                textScp.alignment = TextAnchor.MiddleLeft;
                textScp.fontSize = FontSize;
                textScp.color = Color.white;
               
                text.GetComponent<Button>().onClick.AddListener(() =>
                {
                    CallBack(int.Parse(text.name));
                });
                lis.Add(text);
            }

            return lis;
        }

        void OnDestroy()
        {
            Instance = null;
        }
    }
}
