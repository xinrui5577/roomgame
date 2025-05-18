namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool
{
    /// <summary>
    /// 众多条件的设置，例如声音，音乐，画质三个设置的综合开关，可以获取其中某[一个/多个]开关是否同时开启。
    /// 尤其是判断多个条件同时满足的情况尤为有效。
    /// </summary>
    public class SwitchCombination
    {
        /// <summary>
        /// 设置一个由conditionNum个条件组合的开关，以及这些开关的默认值,defaultNum=0表示所有开关都关闭
        /// </summary>
        /// <param name="conditionNum">条件的个数</param>
        /// <param name="defaultNum">开关的默认值,例如：0x1101，表示【开，关，开，开】</param>
        public SwitchCombination(int conditionNum, int defaultNum) {
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

        private int _maxNum;
        private int _value;

        public int GetValue() {
            return _value;
        }

        private void Init(int typeNum, int defaultNum) {
            _maxNum = (2<<typeNum) - 1;
            _value = defaultNum > _maxNum?_maxNum:defaultNum;
        }

        /// <summary>
        /// 单独开启一个开关
        /// </summary>
        /// <param name="condition">开关的类型，一般为1,2,4,8...2^n</param>
        public void EnableCondition(int condition) {
            _value = _value | condition;
        }
        /// <summary>
        /// 单独关闭一个开关
        /// </summary>
        /// <param name="condition">开关的类型，一般为1,2,4,8...2^n</param>
        public void DisAbleCondition(int condition) {
            _value = _value | condition;
            _value = _value ^ condition;
        }

        /// <summary>
        /// 是否所有的开关都开启
        /// </summary>
        /// <returns></returns>
        public bool IsAllowAll() {
            return _maxNum == _value;
        }

        /// <summary>
        /// 是否所有开关都禁止了
        /// </summary>
        /// <returns></returns>
        public bool IsForbiddenAll() {
            return _value == 0;
        }

        /// <summary>
        /// 某个/某几个开关是否开启
        /// </summary>
        /// <param name="type">开关的类型，一般为1,2,4,8...2^n,如果需要第3个和第5个开关是否同时开启则传入：[0x10100]=20</param>
        /// <returns></returns>
        public bool IsAllowSubCondition(int type) {
            return (_value & type) == type;
        }

        public bool IsAllowAt(int index) {
            return IsAllowSubCondition(1 << index);
        }

        /// <summary>
        /// 所有开关都开启
        /// </summary>
        public void EnableAll() {
            _value = _maxNum;
        }

        /// <summary>
        /// 所有开关的设置为关闭
        /// </summary>
        public void DisableAll() {
            _value = 0;
        }
    }
}
