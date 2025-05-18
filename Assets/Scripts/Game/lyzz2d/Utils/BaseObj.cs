using System;

//using LitJson;

namespace Assets.Scripts.Game.lyzz2d.Utils
{
    public class BaseObj
    {
        private readonly int _keyA;
        private readonly int _keyB;

        public BaseObj()
        {
            var rnd = new Random();
            _keyA = rnd.Next(9999);
            _keyB = rnd.Next(999);
        }

        /// <summary>
        ///     加密int型数据，保护关键数据，不被内存修改
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected long encript(long value)
        {
            value = value ^ _keyA;
            return value - _keyB;
        }

        /// <summary>
        ///     解密int型数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected long decript(long value)
        {
            value += _keyB;
            return value ^ _keyA;
        }

        //}
        //{
        //public virtual void InitData(JsonData data)
    }
}