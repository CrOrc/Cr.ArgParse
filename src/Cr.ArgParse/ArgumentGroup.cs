using System.Collections.Generic;
using Cr.ArgParse.Actions;

namespace Cr.ArgParse
{
    public class ArgumentGroup : ActionContainer
    {
        public ArgumentGroup(ActionContainer container, string title = null, string description = null)
        {
            Container = container;
            GroupActions = new List<Action>();
        }

        public IList<Action> GroupActions { get; private set; }

        public override IList<string> Prefixes
        {
            get { return Container.Prefixes; }
        }

        public ActionContainer Container { get; set; }

        public override IList<Action> Actions
        {
            get { return Container.Actions; }
        }

        public override IList<MutuallyExclusiveGroup> MutuallyExclusiveGroups
        {
            get { return Container.MutuallyExclusiveGroups; }
        }


        public override IDictionary<string, Action> OptionStringActions
        {
            get { return Container.OptionStringActions; }
        }

        public override IList<bool> HasNegativeNumberOptionals
        {
            get { return Container.HasNegativeNumberOptionals; }
        }
        public override Action AddAction(Action action)
        {
            var res = base.AddAction(action);
            GroupActions.Add(res);
            return res;
        }

        public override void RemoveAction(Action action)
        {
            base.RemoveAction(action);
            GroupActions.Remove(action);
        }

    }
}