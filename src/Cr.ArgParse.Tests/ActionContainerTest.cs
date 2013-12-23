using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    [TestFixture] public class ActionContainerTest
    {
        [Test] public void CreateDefault()
        {
            var actionContainer = new ActionContainer();
            CollectionAssert.AreEqual(new[] {"--", "-", "/"}, actionContainer.Prefixes);
        }

        [Test] public void AddArgument()
        {
            var actionContainer = new ActionContainer();
            var res = actionContainer.AddArgument(new Argument {OptionStrings = new[] {"-e", "--example"}});
            Assert.That(res, Is.InstanceOf<StoreAction>());
        }
    }
}