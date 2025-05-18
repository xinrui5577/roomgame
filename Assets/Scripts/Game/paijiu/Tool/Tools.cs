using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.paijiu.Tool
{
    public class Tools : MonoBehaviour
    {

        public static readonly string AssetBundlePath =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
 "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
            "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
            "file://"+Application.dataPath + "/Raw/";
#endif

        /// <summary>
        /// 为NGUI对象添加点击事件
        /// </summary>
        /// <param name="gob">点击对象</param>
        /// <param name="callback">监听事件</param>
        /// <param name="id">ID</param>
        public static void NguiAddOnClick(GameObject gob, UIEventListener.VoidDelegate callback, int id)
        {
            UIEventListener uiel = UIEventListener.Get(gob);
            uiel.onClick = callback;
            uiel.parameter = id;
        }

        /// <summary>
        /// 根据名字只显示唯一的gob
        /// </summary>
        public static GameObject GobShowOnlyOne(GameObject[] gobs, string name)
        {
            int index = -1;

            for (int i = 0; i < gobs.Length; i++)
            {
                GameObject gob = gobs[i];
                if (gob.name.Equals(name))
                {
                    gob.SetActive(true);
                    index = i;
                }
                else
                {
                    gob.SetActive(false);
                }
            }

            return index == -1 ? null : gobs[index];
        }

        public static GameObject GobGet(GameObject[] gobs, string name)
        {
            return gobs.FirstOrDefault(t => t.name.Equals(name));
        }


        /// <summary>
        /// 获取钱
        /// </summary>
        /// <param name="gold"></param>
        /// <param name="isPic"></param>
        /// <returns></returns>
        public static string GetShowGold(float gold, bool isPic = false)
        {
            string unit = "";
            string tempStr = string.Empty;
            float tempGold = gold >= 0 ? gold : -gold;

            if (tempGold < 100)
            {
                tempStr = tempGold.ToString("0.##");
            }
            else if (tempGold >= 100 && tempGold < 10000)
            {
                tempStr = tempGold.ToString("0");
            }
            else if (tempGold >= 10000 && tempGold < 100000)
            {
                tempStr = (tempGold / 10000).ToString("0.##");
                unit = isPic ? "w" : "万";
            }
            else if (tempGold >= 100000 && tempGold < 10000000)
            {
                tempStr = (tempGold / 10000).ToString("0.#");
                unit = isPic ? "w" : "万";
            }
            else if (tempGold >= 10000000 && tempGold < 100000000)
            {
                tempStr = (tempGold / 10000).ToString("0");
                unit = isPic ? "w" : "万";
            }
            else if (tempGold >= 100000000 && tempGold < 1000000000)
            {
                tempStr = (tempGold / 100000000).ToString("0.##");
                unit = isPic ? "y" : "亿";
            }
            else if (tempGold > 1000000000)
            {
                tempStr = (tempGold / 100000000).ToString("0");
                unit = isPic ? "y" : "亿";
            }

            return (gold >= 0 ? "" : "-") + tempStr + unit;
        }

        /// <summary>
        /// 通过Hex的值获取32位颜色
        /// </summary>
        /// <param name="hex">十六进制数0xFFFFFF</param>
        /// <returns></returns>
        public static Color ChangeToColor(int hex)
        {
            float r = (hex >> 16) & 0x0000FF;
            float g = (hex >> 8) & 0x0000FF;
            float b = hex & 0x0000FF;
            //Debug.Log("G == " + (G / 225));
            return new Color(r / 255, g / 255, b / 255, 1);
        }


        public static void TestDebug(int[] cards)
        {
            string temp = cards.Aggregate(string.Empty, (current, card) => current + (" , " + card.ToString()));
            Debug.Log(" ==== Length : " + cards.Length + " == " + temp + " === ");
        }
       



        public static string GetName(Group group)
        {
            int type = group.Type;
            int[] cards = group.Cards;

            return GetName(cards, type);
        }

        public static string GetName( int[] cards,int type)
        {
            int bigCard = cards[1];
            string name = string.Empty;
            if (type < 20)
            {
                switch ((OneCardType)bigCard)
                {
                    case OneCardType.Tianpai:
                        name += "天";
                        break;
                    case OneCardType.Dipai:
                        name += "地";
                        break;
                    case OneCardType.Renpai:
                        name += "人";
                        break;
                    case OneCardType.Epai:
                        name += "鹅";
                        break;
                    case OneCardType.Meipai:
                        name += "梅";
                        break;
                    case OneCardType.Changsan:
                        name += "长";
                        break;
                    case OneCardType.Bandeng:
                        name += "板";
                        break;
                    case OneCardType.Futou:
                        name += "斧";
                        break;
                    case OneCardType.Hongtoushi:
                        name += "红头";
                        break;
                    case OneCardType.Gaojiaoqi:
                        name += "高脚";
                        break;
                    case OneCardType.Tongchuiliu:
                        name += "铜锤";
                        break;
                    default:
                        Debug.Log(" ==== 没有特殊牌型 ==== ");
                        break;
                }
                int p = type % 10;
                switch (p)
                {
                    case 0:
                        name = "闭十";
                        break;
                    case 1:
                        name += "一";
                        break;
                    case 2:
                        name += "二";
                        break;
                    case 3:
                        name += "三";
                        break;
                    case 4:
                        name += "四";
                        break;
                    case 5:
                        name += "五";
                        break;
                    case 6:
                        name += "六";
                        break;
                    case 7:
                        name += "七";
                        break;
                    case 8:
                        name += "八";
                        break;
                    case 9:
                        name += "九";
                        break;
                   
                }
            }
            else
            {
                switch ((PaiJiuType)type)
                {
                    case PaiJiuType.None:
                        name = "";
                        Debug.Log(" ==== 名字有误 ==== ");
                        break;
                    case PaiJiuType.Digaojiu:
                        name = "地高九";
                        break;
                    case PaiJiuType.Tiangaojiu:
                        name = "天高九";
                        break;
                    case PaiJiuType.Digang:
                        name = "地杠";
                        break;
                    case PaiJiuType.Tiangang:
                        name = "天杠";
                        break;
                    case PaiJiuType.Za5:
                        name = "杂五";
                        break;
                    case PaiJiuType.Za7:
                        name = "杂七";
                        break;
                    case PaiJiuType.Za8:
                        name = "杂八";
                        break;
                    case PaiJiuType.Za9:
                        name = "杂九";
                        break;
                    case PaiJiuType.Shuang_Gao_Ling_Fu_Hong:
                        switch (bigCard)
                        {
                            case 0x70A:
                                name = "双红头";
                                break;
                            case 0x70B:
                                name = "双斧头";
                                break;
                            case 0x706:
                                name = "双零霖";
                                break;
                            case 0x707:
                                name = "双高脚";
                                break;
                            default:
                                 name = "";
                                 Debug.Log(" ==== shuang_gao_ling_fu_hong ,名字错误 ==== ");
                                break;
                        }
                        break;
                    case PaiJiuType.Shuang_Mei_San_Ban:
                        switch (bigCard)
                        {
                            case 0x804:
                                name = "双板凳";
                                break;
                            case 0x806:
                                name = "双长三";
                                break;
                            case 0x80A:
                                name = "双梅";
                                break;
                            default:
                                name = "";
                                Debug.Log(" ==== shuang_mei_san_ban , 名字错误 ==== ");
                                break;
                        }
                        break;
                    case PaiJiuType.ShuangEr:
                        name = "双鹅";
                        break;
                    case PaiJiuType.ShuangRen:
                        name = "双人";
                        break;
                    case PaiJiuType.ShuangDi:
                        name = "双地";
                        break;
                    case PaiJiuType.Tianwang9:
                        switch (cards[0])
                        {
                            case 0x609:
                                name = "天王九";
                                break;
                            case 0xc0c:
                                name = "双天";
                                break;
                            default:
                                name = "";
                                Debug.Log(" ==== tianwang_9 名字错误 ==== ");
                                break;
                        }
                        break;
                    case PaiJiuType.Zhizhunbao:
                        name = "至尊";
                        break;
                    case PaiJiuType.Guizi:
                        name = "鬼子";
                        break;
                    //case PaiJiuType.tian:
                    //    break;
                    //case PaiJiuType.di:
                    //    break;
                    //case PaiJiuType.ren:
                    //    break;
                    //case PaiJiuType.e:
                    //    break;
                    //case PaiJiuType.mei:
                    //    break;
                    //case PaiJiuType.changsan:
                    //    break;
                    //case PaiJiuType.bandeng:
                    //    break;
                    //case PaiJiuType.futou:
                    //    break;
                    //case PaiJiuType.hongtoushi:
                    //    break;
                    //case PaiJiuType.gaojiaoqi:
                    //    break;
                    //case PaiJiuType.tongcui6:
                    //    break;
                    //case PaiJiuType.cza9_45:
                    //    break;
                    //case PaiJiuType.cza9_36:
                    //    break;
                    //case PaiJiuType.cza8_26:
                    //    break;
                    //case PaiJiuType.cza8_35:
                    //    break;
                    //case PaiJiuType.cza7_25:
                    //    break;
                    //case PaiJiuType.cza7_34:
                    //    break;
                    //case PaiJiuType.ersi:
                    //    break;
                    //case PaiJiuType.cza5_14:
                    //    break;
                    //case PaiJiuType.cza5_32:
                    //    break;
                    //case PaiJiuType.dingsan:
                    //    break;
                    default:
                        name = "";
                        Debug.Log(" ==== type > 20 ,名字错误 ==== ");
                        break;
                }
            }
            return name;
        }
    }
}
