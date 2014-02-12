namespace Cr.ArgParse.Actions
{
    public class StoreFalseAction : StoreConstAction
    {
        public override object ConstValue
        {
            get { return false; }
        }

        public StoreFalseAction(Argument argument)
            : base(argument)
        {
        }
    }
}