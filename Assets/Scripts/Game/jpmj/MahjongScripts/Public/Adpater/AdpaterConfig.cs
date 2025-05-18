using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.Public.Adpater
{
    [System.Serializable]
    public class AdpaterConfig
    {
        //屏幕分辨率类型
        public AdpaterType Type = AdpaterType.Normal;
        //UI摄像机
        public float UICamera_FieldOfView = 5;
        //麻将摄像机视野
        public float MahjongCamera_FieldOfView = 34.7f;
        //手牌摄像机视野
        public float HandCardCamera_Size = 1.55f;

        //麻将位置位置
        public Vector3 MahjongCamera_Pos = new Vector3(0, 8.35f, 6.4f);
        //手牌位置位置
        public Vector3 HandCardCamera_Pos = new Vector3(0, 2.18f, 7.12f);
        //聊天UI位置
        public Vector3 ChatBtn_Pos = new Vector3(-58.6f, -52.7f, 0);
        //ChooseCpPnl      
        public Vector3 ChooseCpPnl_Pos = new Vector3(-9, -192, 0);
        //cpg父节点位置
        public Vector3 Cpg0_Pos = new Vector3(-4.2f, 0.17f, 3.45f);
        public Vector3 Cpg1_Pos = new Vector3(-4.24f, -0.17f, -2.14f);
        public Vector3 Cpg3_Pos = new Vector3(-4.21f, -0.17f, 1.96f);
        //头像UI位置
        public Vector3 HeadBgImg0_Pos = new Vector3(0, 110, 0);
        public Vector3 HeadBgImg1_Pos = new Vector3(-16.2f, 62.7f, 0);
        public Vector3 HeadBgImg3_Pos = new Vector3(0, 62.7f, 0);

        //麻将牌适配
        public Vector3 HandMahjongSize = new Vector3(1.1f, 1.1f, 1.1f);
        //按钮特效适配
        public Vector3 EffextButtonSize = new Vector3(1, 1, 1);
    }

}