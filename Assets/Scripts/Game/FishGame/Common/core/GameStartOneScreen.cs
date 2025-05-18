using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class GameStartOneScreen : MonoBehaviour {
        public Renderer Rd_SystemStart;
        public GameObject Go_SystemStartIcon;
        public GameObject Go_SystemStartTile;
 
        public GameObject GO_SystemInfo;

        public tk2dTextMesh Text_LineIdx;
        public tk2dTextMesh Text_TableIdx;
        public tk2dTextMesh Text_Date;
        public tk2dTextMesh Text_Version;

        public Renderer Rdr_WarnningCN;
        public Renderer Rdr_WarnningEN;


        public int Go_SystemStartIconCount;//1:系统启动中的开机时为 17 。 计算机检测的开机时为95
        public float Go_SystemStartIconTime;//20.4794F 或14.298F
        [System.NonSerialized]
        public int IdxScreen;//屏幕索引(开始:0,有GameStart赋值)

        public delegate void Event_GameLogoStart(int screenIdx, float time);

        public Event_GameLogoStart EvtGameLogoStart;//游戏logo开始
        public Event_Generic EvtGameInitFinish;//游戏初始化完成
 
        public float TimeLoadTexture = 5.2F;//加载鱼图片资源使用时间

    

        void Awake()
        {

            Go_SystemStartTile.GetComponent<Renderer>().enabled = false;

            GO_SystemInfo.SetActive(false);


        }
 


   
        // Use this for initialization
        IEnumerator Start()
        { 
            //两张警告图片
            //Rdr_WarnningCN.enabled = true;
            //yield return new WaitForSeconds(2F);
            //Rdr_WarnningCN.enabled = false;
            //Rdr_WarnningEN.enabled = true;
            //yield return new WaitForSeconds(2F);
            //Rdr_WarnningEN.enabled = false;
            yield return new WaitForSeconds(0.5F);
            //系统启动进度条
            Rd_SystemStart.enabled = true;
            if(Go_SystemStartIcon != null)
                Go_SystemStartIcon.GetComponent<Renderer>().enabled = true;
            Transform tsSystemStart = Go_SystemStartTile.transform;
            int numTile = Go_SystemStartIconCount;
            int curTile = 0;
            List<GameObject> goSystemTileLst = new List<GameObject>();

            while (curTile < numTile)
            {
                GameObject goTile = Instantiate(Go_SystemStartTile) as GameObject;
                goTile.GetComponent<Renderer>().enabled = true;
                goTile.transform.parent = transform;
                goTile.transform.localPosition = new Vector3( tsSystemStart.localPosition.x + Go_SystemStartIconTime * curTile, tsSystemStart.localPosition.y, tsSystemStart.localPosition.z );
                goSystemTileLst.Add(goTile);

                ++curTile;
                yield return new WaitForSeconds(TimeLoadTexture / numTile);//实际为0.6*17 = 10.2秒
            }

            foreach (GameObject go in goSystemTileLst)
            {
                Destroy(go);
            }
            if(Go_SystemStartIcon != null)
                Destroy(Go_SystemStartIcon);
            Destroy(Rd_SystemStart.gameObject);
            Destroy(Go_SystemStartTile);
 

            if (EvtGameLogoStart != null)
                EvtGameLogoStart(IdxScreen,5F);
            yield return new WaitForSeconds(5F); 


            //前台信息
            GO_SystemInfo.SetActive(true);
            Text_Version.text = "V" + GameMain.MainVersion + " - " + GameMain.SubVersion;
            Text_Version.Commit();
            Text_LineIdx.text = string.Format("{0:d3}", GameMain.Singleton.BSSetting.Dat_IdLine.Val);// GameMain.Singleton.BSSetting.Dat_IdLine.Val.ToString();
            Text_LineIdx.Commit();
            Text_TableIdx.text = string.Format("{0:d8}", GameMain.Singleton.BSSetting.Dat_IdTable.Val);
            Text_TableIdx.Commit();
            System.DateTime dtNow = System.DateTime.Now;

            if (GameMain.Singleton.BSSetting.LaguageUsing.Val == Scripts.Game.FishGame.Common.core.Language.Cn)
            {
                Text_Date.text = string.Format("{0:d}年{1:d2}月{2:d2}日{3:d2}时{4:d2}分      星期", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute);
                string dayOfWeekCn = "";
                switch (dtNow.DayOfWeek)
                {
                    case System.DayOfWeek.Sunday: dayOfWeekCn = "日"; break;
                    case System.DayOfWeek.Monday: dayOfWeekCn = "一"; break;
                    case System.DayOfWeek.Tuesday: dayOfWeekCn = "二"; break;
                    case System.DayOfWeek.Wednesday: dayOfWeekCn = "三"; break;
                    case System.DayOfWeek.Thursday: dayOfWeekCn = "四"; break;
                    case System.DayOfWeek.Friday: dayOfWeekCn = "五"; break;
                    case System.DayOfWeek.Saturday: dayOfWeekCn = "六"; break;

                }
                Text_Date.text += dayOfWeekCn;
            }
            else
            {
                //dtNow.
                Text_Date.text = dtNow.ToString("dd MMMM,yyyy H:mm dddd");// string.Format("{0:d}年{1:d2}月{2:d2}日{3:d2}时{4:d2}分      星期", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute);
             
            }

            Text_Date.Commit();

            yield return new WaitForSeconds(5F);

  
            Destroy(gameObject);


            SendMessageUpwards("Msg_GameInitFinish", SendMessageOptions.DontRequireReceiver);
        }
     
    }
}
