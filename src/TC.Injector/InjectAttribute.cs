using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TC.Injector
{

    /// <summary>
    /// Specifies that a property attributed with this attribute should be injected
    /// by <see cref="Injector"/> when obtaining an instance (<see cref="Injector.Get"/>) or resolving (<see cref="Injector.Resolve{TInstance}(TInstance)"/>.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class InjectAttribute : Attribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttribute"/>.
        /// </summary>
        public InjectAttribute()
        {
        }

        /// <summary>
        /// A value that can be used to carry user-defined information to a conditional injector binding.
        /// This value can be accessed in binding conditions via the <see cref="InjectorRequest.Attribute"/> property of the <see cref="InjectorRequest"/>
        /// that is passed to a binding condition function.
        /// </summary>
        /// <remarks>
        /// Binding conditions can be specified using <see cref="FluentBinder{TContract}.If(Func{InjectorRequest, bool})"/>
        /// and <see cref="FluentBinder{TContract}.If{TEnclosingObject}(Func{InjectorRequest{TEnclosingObject}, bool})"/>.
        /// </remarks>
        public string Tag { get; set; }

    }

}
