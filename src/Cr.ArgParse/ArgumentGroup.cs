using System.Collections.Generic;
using Cr.ArgParse.Actions;

namespace Cr.ArgParse
{
    public class ArgumentGroup : ActionContainer
    {
        public ArgumentGroup(IActionContainer container) : base(container: container)
        {
            GroupActions = new List<Action>();
        }

        public override IList<Action> Actions
        {
            get { return Container.Actions; }
        }

        public IList<Action> GroupActions { get; private set; }

        public override IList<bool> HasNegativeNumberOptionals
        {
            get { return Container.HasNegativeNumberOptionals; }
        }

        protected override IList<MutuallyExclusiveGroup> MutuallyExclusiveGroups
        {
            get { return Container.MutuallyExclusiveGroups; }
        }

        protected override IDictionary<string, Action> OptionStringActions
        {
            get { return Container.OptionStringActions; }
        }

        public override IList<string> Prefixes
        {
            get { return Container.Prefixes; }
        }

        public override Action AddAction(Action action)
        {
            Action res = base.AddAction(action);
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