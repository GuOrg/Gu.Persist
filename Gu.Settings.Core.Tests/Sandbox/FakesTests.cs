//namespace Gu.Settings.Tests.Sandbox
//{
//    using System;
//    using System.Fakes;

//    using Microsoft.QualityTools.Testing.Fakes;

//    using NUnit.Framework;

//    [Explicit]
//    public class FakesTests
//    {
//        [Test]
//        public void FakeDateTimeNow()
//        {
//            using (ShimsContext.Create())
//            {
//                ShimDateTime.NowGet = () => new DateTime(2000, 1, 1);

//                Assert.AreEqual(new DateTime(2000, 1, 1), DateTime.Now);
//            }
//        }
//    }
//}
