using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TC.Injector
{

    /// <summary>
    /// Helper for defining bindings from binding contract types to instances.
    /// </summary>
    /// <typeparam name="TContract"></typeparam>
    public class FluentBinder<TContract>
    {

        private Injector injector;
        private Func<InjectorRequest, bool> condition = null;

        internal FluentBinder(Injector injector)
        {
            this.injector = injector;
        }

        /// <summary>
        /// Creates a binding of the contract type <typeparamref name="TContract"/> to the given instance <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public void To(TContract instance)
        {
            injector.AddBinding(typeof(TContract), condition, new InstanceBinding<TContract>(injector, instance));
        }

        /// <summary>
        /// Creates a binding of the contract type <typeparamref name="TContract"/> to instances created by <paramref name="factory"/>.
        /// Each time an instance is requested for this binding, a new instance will be created.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public void To(Func<TContract> factory)
        {
            injector.AddBinding(typeof(TContract), condition, new FactoryBinding<TContract>(injector, factory, false));
        }

        /// <summary>
        /// Creates a binding of the contract type <typeparamref name="TContract"/> to a singleton instance created by <paramref name="factory"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public void ToSingleton(Func<TContract> factory)
        {
            injector.AddBinding(typeof(TContract), condition, new FactoryBinding<TContract>(injector, factory, true));
        }

        /// <summary>
        /// Creates a binding of the contract type <typeparamref name="TContract"/> to instances of the implementation type
        /// <typeparamref name="TImplementation"/> (which must derive from the contract type).
        /// </summary>
        /// <remarks>
        /// If the implementation type has only one constructor, its arguments are attempted to be resolved when the instance is created.
        /// If the implementation type has more than one constructor, the constructor chosen is the one that is marked with <see cref="InjectAttribute"/>.
        /// If there is no such constructor, or if there are multiple such constructors, no instance of the implementation can be created.
        /// </remarks>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public void To<TImplementation>()
            where TImplementation : class, TContract
        {
            injector.AddBinding(typeof(TContract), condition, new FactoryBinding<TContract>(injector, () => injector.CreateInstance<TImplementation>(), false));
        }

        /// <summary>
        /// Creates a binding of the contract type <typeparamref name="TContract"/> to a singletone instance of the implementation type
        /// <typeparamref name="TImplementation"/> (which must derive from the contract type).
        /// </summary>
        /// <remarks>
        /// If the implementation type has only one constructor, its arguments are attempted to be resolved when the instance is created.
        /// If the implementation type has more than one constructor, the constructor chosen is the one that is marked with <see cref="InjectAttribute"/>.
        /// If there is no such constructor, or if there are multiple such constructors, no instance of the implementation can be created.
        /// </remarks>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public void ToSingleton<TImplementation>()
            where TImplementation : class, TContract
        {
            injector.AddBinding(typeof(TContract), condition, new FactoryBinding<TContract>(injector, () => injector.CreateInstance<TImplementation>(), true));
        }

        /// <summary>
        /// Defines a condition for the binding being constructed, using the non-strongly-typed version
        /// of <see cref="InjectorRequest"/>. If multiple calls to If() are made for one
        /// <see cref="FluentBinder{TContract}"/>, only the last defined condition will actually be applied.
        /// </summary>
        /// <remarks>
        /// Other this overload when the expected type of the enclosing object for the binding is not known in advance.
        /// </remarks>
        /// <param name="condition"></param>
        /// <returns></returns>
        public FluentBinder<TContract> If(Func<InjectorRequest, bool> condition)
        {
            this.condition = condition;
            return this;
        }

        /// <summary>
        /// Defines a condition for the binding being constructed, using the strongly-typed version of
        /// <see cref="InjectorRequest{TEnclosingObject}"/>. If multiple calls to If() are made for one
        /// <see cref="FluentBinder{TContract}"/>, only the last defined condition will actually be applied.
        /// </summary>
        /// <remarks>
        /// Use this overload when the expected type of the enclosing object for the binding is known in advance, as this
        /// overload can save you an ugly cast when accessing the enclosing object.
        /// If the actual type does not match the expected type, an <see cref="InvalidCastException"/> will be thrown.
        /// </remarks>
        /// <param name="condition"></param>
        /// <returns></returns>
        public FluentBinder<TContract> If<TEnclosingObject>(Func<InjectorRequest<TEnclosingObject>, bool> condition)
        {
            this.condition = (request) => condition(new InjectorRequest<TEnclosingObject>(request.ContractType, (TEnclosingObject)request.EnclosingObject, request.Attribute));
            return this;
        }

    }

}
