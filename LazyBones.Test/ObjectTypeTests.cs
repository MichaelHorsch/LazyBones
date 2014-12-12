using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace LazyBones.Test
{
    [TestFixture]
    public class ObjectTypeTests
    {
        BodyBuilder _builder;

        [SetUp]
        public void Init()
        {
            _builder = new BodyBuilder();
        }
        
        [Test]
        public void TestFooBarObject()
        {
            var result = _builder.Create(new
            {
                foo = "bar"
            });
            Assert.AreEqual(result, "foo: bar\r\n");
        }

        [Test]
        public void TestFooBarNestedObject()
        {
            var result = _builder.Create(new
            {
                foo = "bar",
                baz = new
                {
                    hello = "world",
                },
            });
            Assert.AreEqual(result, "foo: bar\r\nbaz: \r\n- hello: world\r\n");
        }

        [Test]
        public void TestObjectWithObjectArray()
        {
            var result = _builder.Create(new 
            {
                hello = "world",
                foo = new object[] 
                {
                    new
                    {
                        bar = "baz",
                    }
                },
            });

            Assert.AreEqual(result, "hello: world\r\nfoo: \r\n- *item 1*\r\n- bar: baz\r\n");
        }

        [Test]
        public void TestSimpleArray()
        {
            var result = _builder.Create(new string[] 
            {
                "foo",
                "bar",
            });

            Assert.AreEqual(result, "*item 1*\r\nfoo\r\n*item 2*\r\nbar\r\n");
        }

        [Test]
        public void TestComplexArray()
        {
            var result = _builder.Create(new object[] 
            {
                new 
                {
                    foo = "bar",
                },
                new 
                {
                    hello = "world",
                },
            });

            Assert.AreEqual(result, "*item 1*\r\nfoo: bar\r\n*item 2*\r\nhello: world\r\n");
        }

        [Test]
        public void TestNestedObjectArray()
        {
            var result = _builder.Create(new object[] 
            {
                new 
                {
                    foo = new object[] 
                    {
                        new
                        {
                            bar = "baz",
                        }
                    },
                },
                new 
                {
                    hello = "world",
                },
            });

            Assert.AreEqual(result, "*item 1*\r\nfoo: \r\n- *item 1*\r\n- bar: baz\r\n*item 2*\r\nhello: world\r\n");
        }

        [Test]
        public void PlainString()
        {
            var result = _builder.Create("test");
            Assert.AreEqual(result, "test\r\n");
        }

        [Test]
        public void PlainInt()
        {
            var result = _builder.Create(5);
            Assert.AreEqual(result, "5\r\n");
        }

        [Test]
        public void PlainBool()
        {
            var result = _builder.Create(true);
            Assert.AreEqual(result, "True\r\n");
        }

        [Test]
        public void PlainDateTime()
        {
            var result = _builder.Create(new DateTime(2014, 12, 25));
            Assert.AreEqual(result, "12/25/2014 12:00:00 AM\r\n");
        }
    }
}
