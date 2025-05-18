/** 
 *文件名称:     MahJongData.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-02 
 *描述:         对象池
 *历史记录: 
*/

using System;
using System.Collections.Generic;

namespace Assets.Scripts.Game.lyzz2d.Utils.Pools
{
    public class PoolManager<T> where T : class, IPoolItem, new()
    {
        private readonly Action<T> _newAction;
        private readonly Stack<T> _poolStack;
        private Action<T> _restoreAction;
        private Action<T> _restoreAllAction;

        public PoolManager(int poolSize,
            Action<T> onNewItem = null,
            Action<T> OnRestoreItem = null,
            Action<T> OnRestoreAllItem = null)
        {
            _poolStack = new Stack<T>(poolSize);
            _newAction = onNewItem;
            _restoreAction = OnRestoreItem;
            _restoreAllAction = OnRestoreAllItem;
        }

        public T New()
        {
            T item;
            if (_poolStack.Count > 0)
            {
                item = _poolStack.Pop();
                item.Reset();
            }
            else
            {
                item = new T();
                if (_newAction != null)
                {
                    _newAction(item);
                }
            }
            return item;
        }

        public void Restore(T item)
        {
            _poolStack.Push(item);
        }
    }
}