namespace Cr.ArgParse.Actions
{
    public class StoreFalseAction : StoreConstAction
    {
        public StoreFalseAction(Argument argument, IActionContainer container)
            : base(argument, container)
        {
        }

        public override object ConstValue
        {
            get { return false; }
        }

        public override object DefaultValue
        {
            get { return Argument.DefaultValue ?? true; }
        }
    }
}