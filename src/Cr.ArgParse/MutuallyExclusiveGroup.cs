using System;
using Cr.ArgParse.Actions;

namespace Cr.ArgParse
{
    public class MutuallyExclusiveGroup: ArgumentGroup
    {
        public MutuallyExclusiveGroup(ActionContainer container, bool isRequired=false)
            : base(container, null,null)
        {
            IsRequired = isRequired;
        }

        public bool IsRequired { get; private set; }

        public override ArgumentAction AddAction(ArgumentAction argumentAction)
        {
            if (argumentAction.IsRequired)
                throw new Exception("Mutually exclusive arguments must be optional");
            return base.AddAction(argumentAction);
        }
    }
}