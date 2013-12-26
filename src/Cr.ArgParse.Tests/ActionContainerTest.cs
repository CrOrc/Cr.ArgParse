using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    [TestFixture] public class ActionContainerTest:BaseTest
    {
        [Test] public void CreateDefault()
        {
            var actionContainer = new ActionContainer();
            CollectionAssert.AreEqual(new[] {"--", "-", "/"}, actionContainer.Prefixes);
        }

        [Test] public void AddOneOptionalArgument()
        {
            var actionContainer = new ActionContainer();
            var res = actionContainer.AddArgument(new Argument {OptionStrings = new[] {"-e", "--example"}});

            CollectionAssert.AreEquivalent(new[] {"-e", "--example"}, actionContainer.OptionStringActions.Keys);

            Assert.That(res, Is.InstanceOf<StoreAction>());
            Asserter.AreEqual(
                new StoreAction(new Argument {OptionStrings = new[] {"-e", "--example"}, Destination = "example"})
                {
                    Container = actionContainer,
                },
                res);
        }

        [Test] public void AddOneOptionalArgumentWithActionName()
        {
            var actionContainer = new ActionContainer();
            var argument = new Argument
            {
                OptionStrings = new[] {"-e", "--example"},
                ActionName = "count"
            };
            var argumentCopy = new Argument(argument);
            argumentCopy.Destination = "example";
            var res =
                actionContainer.AddArgument(argument);

            CollectionAssert.AreEquivalent(new[] {"-e", "--example"}, argument.OptionStrings);

            Assert.That(res, Is.InstanceOf<CountAction>());
            Asserter.AreEqual(
                new CountAction(argumentCopy)
                {
                    Container = actionContainer,
                },
                res);
        }
    }
}