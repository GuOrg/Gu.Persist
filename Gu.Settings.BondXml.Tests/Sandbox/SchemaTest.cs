namespace Gu.Settings.BondXml.Tests.Helpers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Bond;
    using Bond.Protocols;
    using Core.Tests;
    using NUnit.Framework;

    public class SchemaTest
    {
        [Test]
        public void GetRuntimeSchema()
        {
            var runtimeSchema = Schema.GetRuntimeSchema(typeof(WithSchema));
        }

        [Test]
        public void RuntimeSchema()
        {
            var schema = Schema<WithSchema>.RuntimeSchema;
            // System.TypeInitializationException : The type initializer for 'Bond.Schema`1' threw an exception.
            // ----> System.Collections.Generic.KeyNotFoundException : The given key was not present in the dictionary.
        }

        [Test]
        public void RountripWithSchema()
        {
            var sb = new StringBuilder();
            var foo = new WithSchema { Value = 1 };

            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                var writer = new SimpleXmlWriter(xmlWriter);
                Serialize.To(writer, foo);
                writer.Flush();
            }
            var xml = sb.ToString();
            Console.Write(xml);
            using (var xmlReader = XmlReader.Create(new StringReader(xml)))
            {
                var reader = new SimpleXmlReader(xmlReader);
                var roundtripped = Deserialize<WithSchema>.From(reader);
                Assert.AreEqual(foo.Value, roundtripped.Value);
            }
        }

        [Test]
        public void RountripFoo()
        {
            var sb = new StringBuilder();
            var foo = new Foo { Value = 1 };

            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                var writer = new SimpleXmlWriter(xmlWriter);
                Serialize.To(writer, foo);
                writer.Flush();
            }
            var xml = sb.ToString();
            Console.Write(xml);
            using (var xmlReader = XmlReader.Create(new StringReader(xml)))
            {
                var reader = new SimpleXmlReader(xmlReader);
                var roundtripped = Deserialize<Foo>.From(reader);
                Assert.AreEqual(foo.Value, roundtripped.Value);
            }
        }
    }

    [Bond.Schema]
    public class WithSchema
    {
        [Bond.Id(0)]
        public int Value { get; set; }
    }

    public class Foo
    {
        public int Value { get; set; }
    }
}