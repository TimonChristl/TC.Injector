using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TC.Injector
{

    /// <summary>
    /// Event data for the <see cref="Injector.CreateInstanceFailed"/> event.
    /// </summary>
    public class CreateInstanceFailedEventArgs : EventArgs
    {

        private CreateInstanceFailureReason multipleAttributedConstructors;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateInstanceFailedEventArgs"/> type.
        /// </summary>
        /// <param name="multipleAttributedConstructors"></param>
        public CreateInstanceFailedEventArgs(CreateInstanceFailureReason multipleAttributedConstructors)
        {
            this.multipleAttributedConstructors = multipleAttributedConstructors;
        }

        /// <summary>
        /// Reason why the instance creation failed.
        /// </summary>
        public CreateInstanceFailureReason MultipleAttributedConstructors
        {
            get { return multipleAttributedConstructors; }
        }

    }

}
