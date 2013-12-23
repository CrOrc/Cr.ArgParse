using System;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    public class ArgumentActionAsserter
    {
        public void AreEqual(ArgumentAction action1, ArgumentAction action2)
        {
            CollectionAssert.AreEquivalent(action1.OptionStrings,action2.OptionStrings);
            Assert.AreSame(action1.Container,action2.Container);
            AreEqual(action1.Argument, action2.Argument);
        }

        public void AreEqual(Argument argument1, Argument argument2)
        {
            //Assert.AreEqual(argument1.Action,argument2.Action);
            Assert.AreEqual(argument1.ActionName, argument2.ActionName);
            Assert.AreEqual(argument1.ConstValue, argument2.ConstValue);
            Assert.AreEqual(argument1.DefaultValue, argument2.DefaultValue);
            Assert.AreEqual(argument1.Destination, argument2.Destination);
            Assert.AreEqual(argument1.HelpText, argument2.HelpText);
            CollectionAssert.AreEqual(argument1.OptionStrings, argument2.OptionStrings);
            Assert.AreEqual(argument1.ValueCount, argument2.ValueCount);
        }
    }
}