namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    public class RestoreExceptionTest
    {
        [Test]
        public void SerializationRoundtrip()
        {
            var exception = new RestoreException(new Exception("Save failed"), new Exception("Restore failed"));
            var binaryFormatter = new BinaryFormatter();
            using var stream = PooledMemoryStream.Borrow();
            binaryFormatter.Serialize(stream, exception);
            stream.Position = 0;
#pragma warning disable CA2300, CA2301 // Do not use insecure deserializer BinaryFormatter
            var roundtripped = (RestoreException)binaryFormatter.Deserialize(stream);
#pragma warning restore CA2300, CA2301 // Do not use insecure deserializer BinaryFormatter
            Assert.AreEqual("Save failed", roundtripped.SaveException.Message);
            Assert.NotNull(roundtripped.InnerException);
            Assert.AreEqual("Restore failed", roundtripped.InnerException?.Message);
        }
    }
}