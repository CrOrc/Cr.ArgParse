using System.Collections.Generic;
using Cr.ArgParse.Actions;

namespace Cr.ArgParse
{
    public interface IActionContainer
    {
        void RemoveAction(Action action);
        Action AddAction(Action action);

        IList<Action> Actions { get; }
        IList<bool> HasNegativeNumberOptionals { get; }
        IList<MutuallyExclusiveGroup> MutuallyExclusiveGroups { get;}
        IDictionary<string, Action> OptionStringActions { get; }
        IList<string> Prefixes { get; }
    }
}