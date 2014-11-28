namespace Cr.ArgParse.Actions
{
    public class StoreTrueAction : StoreConstAction
    {
        public StoreTrueAction(Argument argument) : base(argument)
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