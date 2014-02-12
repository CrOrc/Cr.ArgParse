namespace Cr.ArgParse.Actions
{
    public class StoreTrueAction : StoreConstAction
    {
        public override object ConstValue
        {
            get { return true; }
        }

        public override object DefaultValue
        {
            get { return false; }
        }

        public StoreTrueAction(Argument argument) : base(argument)
        {
        }
    }
}