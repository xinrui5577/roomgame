/**
 * 通知回包的处理 type=2 收到服务端通知的获奖广播字符串
 * 例：(utf_string) data: 16-05-17 18:05:55,两对,500,0,zhangxinli
 * 结构化字符串
 * 
 */
namespace Assets.Scripts.Game.mx97
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
            int pos = prizeData.IndexOf(",", System.StringComparison.Ordinal);
            PrizeTime = prizeData.Substring(0, pos);
            prizeData = prizeData.Substring(pos + 1, prizeData.Length - pos - 1);

            pos = prizeData.IndexOf(",", System.StringComparison.Ordinal);
            TypeName = prizeData.Substring(0, pos);
            prizeData = prizeData.Substring(pos + 1, prizeData.Length - pos - 1);

            pos = prizeData.IndexOf(",", System.StringComparison.Ordinal);
            ScoreNum = prizeData.Substring(0, pos);
            prizeData = prizeData.Substring(pos + 1, prizeData.Length - pos - 1);

            pos = prizeData.IndexOf(",", System.StringComparison.Ordinal);
            JackpotNum = prizeData.Substring(0, pos);
            prizeData = prizeData.Substring(pos + 1, prizeData.Length - pos - 1);

            UserName = prizeData;

            NoticeWords = string.Format("恭喜玩家{0}以【{1}】获得{2}银两奖池奖金", UserName, TypeName, JackpotNum);
        }
    }
}
