using System;

namespace Assets.Scripts.Game.jh.Public
{
    public class JhFunc
    {
        public static int MaxInt = 0x7fffffff;

        public static void AddDepthForWidget(UIWidget widget,int add)
        {
            UIWidget[] widgets = widget.transform.GetComponentsInChildren<UIWidget>(true);
            foreach (UIWidget wgt in widgets)
            {
                wgt.depth += add;
            }
        }

        public static void SetDepthForWidget(UIWidget widget, int dd)
        {
            UIWidget[] widgets = widget.transform.GetComponentsInChildren<UIWidget>(true);
            foreach (UIWidget wgt in widgets)
            {
                wgt.depth = dd;
            }
        }

        /// <summary>

        /// 获取当前时间戳
        /// </summary>
        /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>
        /// <returns></returns>
        public static long GetTimeStamp(bool bflag = true)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long ret;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds);
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds);
            return ret;
        }
    }
}
