namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 众多条件的设置，例如声音，音乐，画质三个设置的综合开关，可以获取其中某[一个/多个]开关是否同时开启。
    /// 尤其是判断多个条件同时满足的情况尤为有效。
    /// </summary>
    public class SwitchCombination
    {
        private int mMaxNum;
        private int mValue;

        /// <summary>
        /// 设置一个由conditionNum个条件组合的开关，以及这些开关的默认值,defaultNum=0表示所有开关都关闭
        /// </summary>
        /// <param name="conditionNum">条件的个数</param>
        /// <param name="defaultNum">开关的默认值,例如：0x1101，表示【开，关，开，开】</param>
        public SwitchCombination(int conditionNum, int defaultNum)
        {
            Init(conditionNum, defaultNum);
        }

        /// <summary>
        /// 设置一个由conditionNum个条件组合的开关，这些开关默认都是开启的,
        /// </summary>
        /// <param name="conditionNum">条件的个数</param>
        public SwitchCombination(int conditionNum)
        {
            Init(conditionNum, int.MaxValue);
        }

        public int GetValue()
        {
            return mValue;
        }

        private void Init(int typeNum, int defaultNum)
        {
            mMaxNum = (2 << typeNum) - 1;
            mValue = defaultNum > mMaxNum ? mMaxNum : defaultNum;
        }

        /// <summary>
        /// 单独开启一个开关
        /// </summary>
        /// <param name="condition">开关的类型，一般为1,2,4,8...2^n</param>
        public void Open(int condition)
        {
            mValue = mValue | condition;
        }

        /// <summary>
        /// 尝试开启一个开关
        /// </summary>
        /// <param name="condition">开关的类型，一般为1,2,4,8...2^n</param>
        public void TryOpen(int condition)
        {
            if (!IsOpen(condition))
            {
                mValue = mValue | condition;
            }           
        }

        /// <summary>
        /// 单独关闭一个开关
        /// </summary>
        /// <param name="condition">开关的类型，一般为1,2,4,8...2^n</param>
        public void Close(int condition)
        {
            mValue = mValue | condition;
            mValue = mValue ^ condition;
        }

        /// <summary>
        /// 是否所有的开关都开启
        /// </summary>
        /// <returns></returns>
        public bool IsOpenAll()
        {
            return mMaxNum == mValue;
        }

        /// <summary>
        /// 是否所有开关都禁止了
        /// </summary>
        /// <returns></returns>
        public bool IsCloseAll()
        {
            return mValue == 0;
        }

        /// <summary>
        /// 某个/某几个开关是否开启
        /// </summary>
        /// <param name="type">开关的类型，一般为1,2,4,8...2^n,如果需要第3个和第5个开关是否同时开启则传入：[0x10100]=20</param>
        /// <returns></returns>
        public bool IsOpen(int type)
        {
            return (mValue & type) == type;
        }

        /// <summary>
        /// 某个开关是否开启
        /// </summary>
        /// <param name="index">编号</param>
        /// <returns></returns>
        public bool IsOpenAt(int index)
        {
            return IsOpen(1 << index);
        }

        /// <summary>
        /// 所有开关都开启
        /// </summary>
        public void OpenAll()
        {
            mValue = mMaxNum;
        }

        /// <summary>
        /// 所有开关的设置为关闭
        /// </summary>
        public void CloseAll()
        {
            mValue = 0;
        }
    }
}