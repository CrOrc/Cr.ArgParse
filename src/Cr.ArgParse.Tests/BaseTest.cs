namespace Cr.ArgParse.Tests
{
    public class BaseTest
    {
        protected Asserter Asserter
        {
            get{return new Asserter();}
        }
    }
}