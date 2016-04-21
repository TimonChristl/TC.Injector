using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TC.Injector
{

    /// <summary>
    /// Possible reasons for why creating an instance failed.
    /// </summary>
    public enum CreateInstanceFailureReason
    {
        /// <summary>
        /// There were no public constructors at all.
        /// </summary>
        NoConstructors,

        /// <summary>
        /// There were multiple constructors but none of them were attributed with <see cref="InjectAttribute"/>.
        /// </summary>
        MultipleConstructorsButNoAttributedOne,

        /// <summary>
        /// There were multiple constructors but multiple of them were attributed with <see cref="InjectAttribute"/>.
        /// </summary>
        MultipleAttributedConstructors
    }

}
