//#if UNITY_EDITOR1//编辑模式下打开写数据库
//#define ENABLE_WRITE_DB//不写入数据库(只写入FARM,提高效率)
//#endif

namespace Assets.Scripts.Game.FishGame.Common.external.NemoFileIO
{
    /// <summary>
    /// 持久数据封装
    /// </summary>
    /// <remarks>主要作用是第一次读取时读取硬盘,之后就读取内存,而不用每次都读取硬盘</remarks>
    /// <typeparam name="TValType">输出值类型</typeparam>
    /// <typeparam name="TStoreType">存储的类型</typeparam>
    /// <remark>
    /// 注意:
    ///     1.保证文件操作在另外一条线程中进行,
    /// </remark>
    public class PersistentData<TValType, TStoreType>  
//where ValType : StoreType ,IEnumerable ,IEnumerator
    {
        private bool _mHaveReaded;//已经从硬盘读取了
        private TValType _mVal; 
        public PersistentData(string name)
        {  
            _mVal = Mask(Val);
        }
        public TValType Mask(TValType val)
        {
            if (typeof(TStoreType) == typeof(int))
            {
                return (TValType)(System.Object)((int)(System.Object)(val) ^ 0x5129A9AD);
            }
            return val;
        }

    
        public TValType Val
        {
            get
            {
                if (!_mHaveReaded)//第一次需要读文件
                {
                    _mVal = Mask(default(TValType));
                    _mHaveReaded = true;
                } 
                return Mask(_mVal);//UnMask
            }
            set
            { 
                _mVal = Mask(value); 
            }
        } 


        /// <summary>
        /// 设置数值并立即写入硬盘
        /// </summary>
        /// <param name="val"></param>
        public void SetImmdiately(TValType val)
        { 
            if ((System.Object)_mVal != (System.Object)Mask(val))
            {
                _mVal = Mask(val);  
            }
        }
    }
}