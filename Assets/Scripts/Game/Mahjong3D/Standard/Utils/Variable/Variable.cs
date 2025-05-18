using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class Variable<T> : Variable
    {
        private T mValue;

        protected Variable() { mValue = default(T); }

        protected Variable(T value) { mValue = value; }

        public Type Type
        {
            get { return typeof(T); }
        }

        public T Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        public void Reset()
        {
            mValue = default(T);
        }

        public TValue GetValue<TValue>() where TValue : class
        {
            TValue value = mValue as TValue;
            if (null != mValue)
            {
                return value;
            }
            return null;
        }

        public override string ToString()
        {
            return (mValue != null) ? mValue.ToString() : "<Null>";
        }
    }
}