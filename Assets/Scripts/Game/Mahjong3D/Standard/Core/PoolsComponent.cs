using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class PoolsComponent : BaseComponent
    {
        protected List<ObjectBase> mStroe = new List<ObjectBase>();

        public T Pop<T>(PoolObjectType type) where T : ObjectBase
        {
            return Pop(type) as T;
        }

        public ObjectBase Pop(PoolObjectType type)
        {
            var go = mStroe.Find((obj) =>
            {
                var temp = obj as EffectObject;
                if (temp != null)
                {
                    return temp.Type == type;
                }
                return false;
            });

            var effect = go as EffectObject;
            if (effect == null)
            {
                effect = Create<EffectObject>(type.ToString());
            }
            return effect;
        }

        public virtual void Push(ObjectBase obj)
        {
            if (obj == null) return;
            obj.ExSetParent(transform);
            mStroe.Add(obj);
        }

        protected T Create<T>(string assetsName) where T : ObjectBase
        {
            var temp = GameUtils.InstanceAssetsByPath<T>("effectmahjong-" + assetsName, "Effects");
            if (temp != null)
            {
                temp.ExCompShow();
                temp.name = assetsName;
            }
            return temp as T;
        }
    }
}