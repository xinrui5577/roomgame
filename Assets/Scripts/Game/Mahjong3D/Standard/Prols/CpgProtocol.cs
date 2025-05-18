namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CpgProtocol
    {
        public static int Chi = 1;
        public static int Peng = 2;
        public static int WZhuaGang = 3;
        /// <summary>
        /// 先碰，后杠
        /// </summary>
        public static int ZhuaGang = 4;
        /// <summary>
        /// 直接杠别人的
        /// </summary>
        public static int PengGang = 5;
        /// <summary>
        /// 明杠
        /// </summary>
        public static int MingGang = 6;
        /// <summary>
        /// 暗杠
        /// </summary>
        public static int AnGang = 7;
        /// <summary>
        /// 旋风杠
        /// </summary>
        public static int XFGang = 10;
        /// <summary>
        /// 绝杠 -暗杠
        /// </summary>
        public static int AnJueGang = 0xb;
        /// <summary>
        /// 旋风杠，以下四条长春麻将专用
        /// </summary>
        public static int XiaoJi = 100;
        public static int YaoDan = 101;
        public static int JiuDan = 102;
        public static int ZFBDan = 103;
        public static int XFDan = 104;
        public static int None;
    }
}
