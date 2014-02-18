using System;
using Action = Cr.ArgParse.Actions.Action;

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

        public override Action AddAction(Action action)
        {
            if (action.IsRequired)
                throw new Exception("Mutually exclusive arguments must be optional");
            return base.AddAction(action);
        }
    }
}