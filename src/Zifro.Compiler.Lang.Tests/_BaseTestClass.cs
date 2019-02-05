﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zifro.Compiler.Core.Exceptions;
using Zifro.Compiler.Core.Interfaces;
using Zifro.Compiler.Lang.Entities;

namespace Zifro.Compiler.Lang.Tests
{
    public class BaseTestClass
    {
        protected Mock<IProcessor> processorMock;
        protected Mock<IScriptTypeFactory> factoryMock;

        [TestInitialize]
        public void TestInitialize()
        {
            processorMock = new Mock<IProcessor>();
            factoryMock = new Mock<IScriptTypeFactory>();

            processorMock.SetupGet(o => o.Factory).Returns(factoryMock.Object);
            factoryMock.Setup(o => o.Create(It.IsAny<int>()))
                .Returns<int>(GetInteger);
            factoryMock.Setup(o => o.Create(It.IsAny<double>()))
                .Returns<double>(GetDouble);
            factoryMock.Setup(o => o.Create(It.IsAny<string>()))
                .Returns<string>(GetString);
            factoryMock.Setup(o => o.Create(It.IsAny<char>()))
                .Returns<char>(c => GetString(c.ToString()));
        }

        protected IntegerBase GetInteger(int value)
        {
            var mock = new Mock<IntegerBase> {CallBase = true};
            IntegerBase integerBase = mock.Object;
            integerBase.Value = value;
            integerBase.Processor = processorMock.Object;
            return integerBase;
        }

        protected DoubleBase GetDouble(double value)
        {
            var mock = new Mock<DoubleBase> {CallBase = true};
            DoubleBase doubleBase = mock.Object;
            doubleBase.Value = value;
            doubleBase.Processor = processorMock.Object;
            return doubleBase;
        }

        protected StringBase GetString(string value)
        {
            var mock = new Mock<StringBase> {CallBase = true};
            StringBase stringBase = mock.Object;
            stringBase.Value = value;
            stringBase.Processor = processorMock.Object;
            return stringBase;
        }

        protected void AssertArithmeticResult<T>(IScriptType resultBase, IScriptType lhs, IScriptType rhs,
            object expected)
            where T : IScriptType
        {
            Assert.IsNotNull(resultBase);
            Assert.IsInstanceOfType(resultBase, typeof(T));
            var result = (T)resultBase;
            Assert.IsNotNull(result);
            Assert.AreNotSame(lhs, result);
            Assert.AreNotSame(rhs, result);

            AssertAreEqual(expected, result);

            processorMock.VerifyGet(o => o.Factory, Times.Once);
            processorMock.VerifyNoOtherCalls();

            if (expected is int i)
            {
                factoryMock.Verify(o => o.Create(i),
                    Times.Once);
            }

            if (expected is double d)
            {
                factoryMock.Verify(o => o.Create(It.Is<double>(v => Math.Abs(v - d) < 1e-10)),
                    Times.Once);
            }

            if (expected is string s)
            {
                factoryMock.Verify(o => o.Create(It.Is<string>(v => v == s)),
                    Times.Exactly(s.Length == 1 ? 0 : 1));
                factoryMock.Verify(o => o.Create(It.Is<char>(v => v.ToString() == s)),
                    Times.Exactly(s.Length == 1 ? 1 : 0));
            }

            factoryMock.VerifyNoOtherCalls();
        }

        protected void AssertThrow(Action action, string localizationKey, object[] expectedFormatArgs)
        {
            var ex = Assert.ThrowsException<RuntimeException>(action);
            Assert.IsNotNull(ex);
            Assert.AreEqual(ex.LocalizeKey, localizationKey);
            CollectionAssert.AreEqual(ex.FormatArgs, expectedFormatArgs);
            processorMock.VerifyNoOtherCalls();
            factoryMock.VerifyNoOtherCalls();
        }

        protected void AssertAreEqual(object expected, IScriptType actual)
        {
            switch (actual)
            {
                case DoubleBase d when expected is double e:
                    Assert.AreEqual(e, d.Value, 1e-10);
                    break;
                case IntegerBase i:
                    Assert.AreEqual(expected, i.Value);
                    break;
                case StringBase s:
                    Assert.AreEqual(expected, s.Value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}