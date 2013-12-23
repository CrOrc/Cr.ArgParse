using System.Collections.Generic;

namespace Cr.ArgParse
{
    public class ArgumentGroup : ActionContainer
    {
        public ArgumentGroup(ActionContainer container, string title = null, string description = null)
        {
            Container = container;
            GroupActions = new List<ArgumentAction>();
        }

        public IList<ArgumentAction> GroupActions { get; private set; }

        public ActionContainer Container { get; set; }

        public override IList<ArgumentAction> Actions
        {
            get { return Container.Actions; }
        }

        public override IList<MutuallyExclusiveGroup> MutuallyExclusiveGroups
        {
            get { return Container.MutuallyExclusiveGroups; }
        }


        public override IDictionary<string, ArgumentAction> OptionStringActions
        {
            get { return Container.OptionStringActions; }
        }

        public override ArgumentAction AddAction(ArgumentAction argumentAction)
        {
            var res = base.AddAction(argumentAction);
            GroupActions.Add(res);
            return res;
        }

        public override void RemoveAction(ArgumentAction argumentAction)
        {
            base.RemoveAction(argumentAction);
            GroupActions.Remove(argumentAction);
        }

    }
}