using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    [TestFixture] public class ActionContainerTest
    {
        private ArgumentActionAsserter ArgumentActionAsserter
        {
            get { return new ArgumentActionAsserter(); }
        }

        [Test] public void CreateDefault()
        {
            var actionContainer = new ActionContainer();
            CollectionAssert.AreEqual(new[] {"--", "-", "/"}, actionContainer.Prefixes);
        }

        [Test] public void AddOneOptionalArgument()
        {
            var actionContainer = new ActionContainer();
            var res = actionContainer.AddArgument(new Argument {OptionStrings = new[] {"-e", "--example"}});
            
            CollectionAssert.AreEquivalent(new[] { "-e", "--example" },actionContainer.OptionStringActions.Keys);

            Assert.That(res, Is.InstanceOf<StoreAction>());
            ArgumentActionAsserter.AreEqual(
                new StoreAction(new Argument {OptionStrings = new[] {"-e", "--example"},Destination = "example"})
                {
                    Container = actionContainer,
                },
                res);
        }
    }
}