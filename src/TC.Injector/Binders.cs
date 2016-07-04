using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TC.Injector
{

    internal abstract class BaseBinding
    {

        private Injector injector;

        public BaseBinding(Injector injector)
        {
            this.injector = injector;
        }

        public abstract bool GetInstance(out object instance);

        protected Injector Injector
        {
            get { return injector; }
        }

    }

    internal abstract class BaseBinding<T> : BaseBinding
    {

        public BaseBinding(Injector injector)
            : base(injector)
        {
        }

        public override bool GetInstance(out object instance)
        {
            T obj;
            var result = GetInstanceCore(out obj);
            instance = obj;
            return result;
        }

        public abstract bool GetInstanceCore(out T instance);

    }

    internal class InstanceBinding<T> : BaseBinding<T>
    {

        private T instance;

        public InstanceBinding(Injector injector, T instance)
            : base(injector)
        {
            this.instance = instance;
        }

        public override bool GetInstanceCore(out T instance)
        {
            instance = this.instance;
            return false;
        }

    }

    internal class FactoryBinding<T> : BaseBinding<T>
    {

        private Func<T> factory;
        private bool singleton;

        public FactoryBinding(Injector injector, Func<T> factory, bool singleton)
            : base(injector)
        {
            if(factory == null)
                throw new ArgumentNullException(nameof(factory));

            this.factory = factory;
            this.singleton = singleton;
        }

        public override bool GetInstanceCore(out T instance)
        {
            if(singleton)
            {
                bool singletonInstanceWasJustCreated = false;
                object singletonInstance;

                 Injector.GetOrCreateSingleton(typeof(T), out singletonInstance, out singletonInstanceWasJustCreated, () => factory());

                instance = (T)singletonInstance;
                return singletonInstanceWasJustCreated;
            }
            else
            {
                instance = factory();
                return true;
            }
        }

    }

}
