namespace Cr.ArgParse
{
    public class ArgumentActionContainer
    {
        private readonly IList<IArgumentAction> actions = new List<IArgumentAction>();
        private IList<IArgumentAction> Actions{get { return actions; }}

        private void AddArgumentAction(IArgumentAction argumentAction)
        {
            if(ReferenceEquals(argumentAction,null))
                throw new ArgumentNullException("argumentAction");
            Actions.Add(argumentAction);
            var optionStrings = argumentAction.Argument.OptionStrings ?? new string[] {};
            foreach (var optionString in optionStrings)
                OptionStringActions[optionString] = argumentAction;
        }
    }
}