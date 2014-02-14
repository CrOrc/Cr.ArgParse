namespace Cr.ArgParse.Actions
{
    public class StoreFalseAction : StoreConstAction
    {
        public override object ConstValue
        {
            get { return false; }
        }

        public override object DefaultValue
        {
            get { return Argument.DefaultValue ?? true; }
        }


        public StoreFalseAction(Argument argument)
            : base(argument)
        {
        }
    }
}