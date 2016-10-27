namespace Gu.Settings.Core.Tests.Repositories
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    public class RestoreExceptionTest
    {
        [Test]
        public void SerializationRoundtrip()
        {
            var exception = new RestoreException(new Exception("Save failed"), new Exception("Restore failed"));
            var binaryFormatter = new BinaryFormatter();
            using (var steram = new MemoryStream())
            {
                binaryFormatter.Serialize(steram, exception);
                steram.Position = 0;
                var roundtripped = (RestoreException)binaryFormatter.Deserialize(steram);
                Assert.AreEqual("Save failed", roundtripped.SaveException.Message);
                Assert.NotNull(roundtripped.InnerException);
                Assert.AreEqual("Restore failed", roundtripped.InnerException.Message);
            }
        }
    }
}