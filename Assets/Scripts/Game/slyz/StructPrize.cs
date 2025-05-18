



using YxFramwork.Common;
/**
* 通知回包的处理 type=2 收到服务端通知的获奖广播字符串
* 例：(utf_string) data: 16-05-17 18:05:55,两对,500,0,zhangxinli
* 结构化字符串
* 
*/
using YxFramwork.Tool;

namespace Assets.Scripts.Game.slyz
{
    public class StructPrize
    {
        public string PrizeTime = "";            // 获得奖励时间

        public string TypeName = "";             // 奖励名称

        public string ScoreNum = "";             // 获得金币数

        public string JackpotNum = "";           // 获得彩池的金额（正式包只有大于0才显示 测试包0的时候才显示）

        public string UserName = "";             // 获奖用户昵称

        public string NoticeWords = "";          // 将获奖信息拼接成一句话 显示于游戏顶部

        public bool NeedDisplay = true;          // 是否需要滚动显示 因为启动的时候后台发来的中奖列表是显示在点击中奖列表中的


        // 字符串解析出结构化数据
        public void ParseDataFromString(string prizeData)
        {
            //18-01-08 16:50:56,奖励名称,2000,235,游客_225891 
            var infos = prizeData.Split(',');
            if (infos.Length < 5) return;
            PrizeTime = infos[0];
            TypeName = infos[1];
            ScoreNum = infos[2];
            long jackpotNum;
            long.TryParse(infos[3], out jackpotNum);
            JackpotNum = GetShowNumberForm(jackpotNum, 0, "#");
            UserName = infos[4];
            NoticeWords = string.Format("恭喜玩家{0}以【{1}】获得{2}银两奖池奖金", UserName, TypeName, JackpotNum);
        }

        /// <summary>
        /// 显示的数字格式化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valid"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetShowNumberForm(long value, int valid = 2, string format = "N")
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "N";
            }
            return App.ShowGoldRate < 2 ? value.ToString(format) : YxUtiles.GetShowNumber(value, valid).ToString(format);
        }

    }
}
