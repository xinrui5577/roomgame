using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using YxFramwork.Common;
using ResourceManager = YxFramwork.Manager.ResourceManager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{
    public class NewTable : MonoBehaviour
    {
        public GameTable GameTable;
        public DnxbCtl NewDnxb;
        public GameObject Bans;
        public GameObject MainDesk;
        Texture[] TableClothes; 
    
        Vector3[] WallsPos = { new Vector3(0, 0.175f, 2.61f), new Vector3(-2.87f, 0.175f, 0.12f), new Vector3(0, 0.175f, -2.71f), new Vector3(2.87f, 0.175f, 0.12f) };
        Vector3[] WallsAniPos = { new Vector3(0, -0.33f, 3.01f), new Vector3(-3.27f, -0.33f, 0.12f), new Vector3(0, -0.33f, -3.01f), new Vector3(3.27f, -0.33f, 0.12f) };
        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < WallsPos.Length; i++)
            {
                GameTable.WallMj[i].transform.position = WallsPos[i];
            }
            GameTable.HardMj[2].transform.position = new Vector3(0, 0.175f, -3.33f);
            GameTable.ThrowMj[2].transform.position=new Vector3(-0.85f,0.13f,-0.95f);
            ChangeWallState(false);
            if (GameTable.Dnxb!=null)
            {
                GameTable.Dnxb.gameObject.SetActive(false);
            }
            GameTable.Dnxb = NewDnxb;
            //EventDispatch.Instance.RegisteEvent((int)GameEventId.InitTable, InitTable);
            InitTable();
            EventDispatch.Instance.RegisteEvent((int)GameEventId.ChangeTableColor, ChangeTableColor);
        }

        public string[] TableClothesNames;

        private void InitTable()
        {
            Texture[] Clothes=new Texture[TableClothesNames.Length];
            for (int i = 0; i < TableClothesNames.Length; i++)
            {
                Texture obj = ResourceManager.LoadAsset<Texture>(App.GameKey, TableClothesNames[i], TableClothesNames[i]);
                Clothes[i] = obj;
            }
            TableClothes=Clothes;
            if (PlayerPrefs.HasKey("DeskColor"))
            {
                int index = PlayerPrefs.GetInt("DeskColor");
                ChangeTableColor(index);
            } 
        }

        public void ChangeWallState(bool state)
        {
            foreach (MahjongWall wall in GameTable.WallMj)
            {
                wall.gameObject.SetActive(state);
            }
        }

        Coroutine _newTable ;
        public void AniStart(DVoidEventData callback,EventData data)
        {
            if (_newTable!=null)
            {
                StopCoroutine(_newTable);
            }
            _newTable=StartCoroutine(NewTableAni(callback,data));
        }

        public void ChangeTableColor(int index)
        {
            if (index>TableClothes.Length-1)
            {
                return;
            }
            PlayerPrefs.SetInt("DeskColor",index);
            Material tableMaterial = MainDesk.GetComponent<MeshRenderer>().material;
            tableMaterial.mainTexture = TableClothes[index];
            foreach (Transform ban in Bans.transform)
            {
                Material banMaterial = ban.GetComponent<MeshRenderer>().material;
                banMaterial.mainTexture = TableClothes[index];
            }
        }

        public void ChangeTableColor(int eventid, EventData data)
        {
            int index = (int) data.data1;
            if (index > TableClothes.Length - 1)
            {
                return;
            }
            PlayerPrefs.SetInt("DeskColor", index);
            Material tableMaterial = MainDesk.GetComponent<MeshRenderer>().material;
            tableMaterial.mainTexture = TableClothes[index];
            foreach (Transform ban in Bans.transform)
            {
                Material banMaterial = ban.GetComponent<MeshRenderer>().material;
                banMaterial.mainTexture = TableClothes[index];
            }
        }


        private float MjSpeed = 0.1f;
        private float TimeLag = 0.03f;
        IEnumerator NewTableAni(DVoidEventData callback, EventData data)
        {
            Transform[] Walls = new Transform[UtilDef.GamePlayerCnt];
            for (int i = 0; i < Walls.Length; i++)
            {
                Walls[i] = GameTable.WallMj[i].transform;
                Walls[i].gameObject.SetActive(true);
                Walls[i].localPosition = WallsAniPos[i];
            }
            //落板
            while (Bans.transform.localPosition.y > -0.1f)
            {
                Bans.transform.position = new Vector3(0, Bans.transform.position.y - MjSpeed, 0);
            
                if (Bans.transform.localPosition.y <= -0.11f)
                {
                    Bans.transform.localPosition = new Vector3(0, -0.11f, 0);
                }
                yield return new WaitForSeconds(TimeLag);
            }
            yield return new WaitForSeconds(0.1f);
            //牌横移

            while (Walls[0].localPosition.z > WallsPos[0].z)
            {
                Walls[0].localPosition = new Vector3(Walls[0].localPosition.x, Walls[0].localPosition.y, Walls[0].localPosition.z - MjSpeed);
                Walls[1].localPosition = new Vector3(Walls[1].localPosition.x + MjSpeed, Walls[1].localPosition.y, Walls[1].localPosition.z);
                Walls[2].localPosition = new Vector3(Walls[2].localPosition.x, Walls[2].localPosition.y, Walls[2].localPosition.z + MjSpeed);
                Walls[3].localPosition = new Vector3(Walls[3].localPosition.x - MjSpeed, Walls[3].localPosition.y, Walls[3].localPosition.z);
            
                if (Walls[0].localPosition.z <= WallsPos[0].z)
                {
                    Walls[0].localPosition = new Vector3(Walls[0].localPosition.x, Walls[0].localPosition.y, WallsPos[0].z);
                    Walls[1].localPosition = new Vector3(WallsPos[1].x, Walls[1].localPosition.y, Walls[1].localPosition.z);
                    Walls[2].localPosition = new Vector3(Walls[2].localPosition.x, Walls[2].localPosition.y, WallsPos[2].z);
                    Walls[3].localPosition = new Vector3(WallsPos[3].x, Walls[3].localPosition.y, Walls[3].localPosition.z);
                }
                yield return new WaitForSeconds(TimeLag);
            }
        
            yield return new WaitForSeconds(0.1f); 

            //上升
            while (Walls[0].localPosition.y < WallsPos[0].y)
            {
                for (int i = 0; i < Walls.Length; i++)
                {
                    Walls[i].localPosition = new Vector3(Walls[i].localPosition.x, Walls[i].localPosition.y + MjSpeed, Walls[i].localPosition.z);
                }
                if (Walls[0].localPosition.y > -0.24f)
                {
                    Bans.transform.position = new Vector3(0, Bans.transform.position.y + MjSpeed, 0);
                }
                if (Walls[0].localPosition.y >= WallsPos[0].y)
                {
                    for (int i = 0; i < Walls.Length; i++)
                    {
                        Walls[i].localPosition = WallsPos[i];
                    }
                    Bans.transform.localPosition = new Vector3(0,-0.004f,0);
                }
                yield return new WaitForSeconds(TimeLag);
            }
            yield return new WaitForSeconds(0.1f);
            callback(data);
        }
    }
}
