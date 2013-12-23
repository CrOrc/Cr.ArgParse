using System.Collections.Generic;

namespace Cr.ArgParse
{
    public class ArgumentGroup : ArgumentActionContainer
    {
        public ArgumentGroup(ArgumentActionContainer container)
        {
            Container = container;
        }

        public ArgumentActionContainer Container { get; set; }

        public override IList<IArgumentAction> Actions
        {
            get { return Container.Actions; }
        }

        public override IDictionary<string, IArgumentAction> OptionStringActions
        {
            get { return Container.OptionStringActions; }
        }
    }
}