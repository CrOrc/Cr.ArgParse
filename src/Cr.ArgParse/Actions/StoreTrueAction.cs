namespace Cr.ArgParse.Actions
{
    public class StoreTrueAction : StoreConstAction
    {
        public StoreTrueAction(Argument argument, IActionContainer container) : base(argument,container)
        {
        }

        public override object ConstValue
        {
            get { return true; }
        }

        public override object DefaultValue
        {
            get { return Argument.DefaultValue ?? false; }
        }
    }
}