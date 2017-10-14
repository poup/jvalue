using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Halak
{
    [TestClass]
    public class LiteralParseTest
    {
        [TestMethod]
        public void TestIntParsing()
        {
            int Parse(string s) => JsonHelper.Parse(s, 0, s.Length, 0);

            Assert.AreEqual(10000, Parse("10000"));
            Assert.AreEqual(0, Parse("4294967295"));  // overflow
            Assert.AreEqual(0, Parse("2147483648"));  // overflow
            Assert.AreEqual(2147483647, Parse("2147483647"));  // max
            Assert.AreEqual(0, Parse("12387cs831"));  // invalid
            Assert.AreEqual(0, Parse("0"));
            Assert.AreEqual(-12938723, Parse("-12938723"));
            Assert.AreEqual(3948222, Parse("3948222"));

            var random = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var value = random.Next(int.MinValue, int.MaxValue);
                Assert.AreEqual(Parse(value.ToString()), value);
            }
        }

        [TestMethod]
        public void TestDoubleParsing()
        {
            double Parse(string s) => JsonHelper.Parse(s, 0, s.Length, 0.0);

            Assert.AreEqual(10000.0, Parse("10000"));
            Assert.AreEqual(2147483647.0, Parse("2147483647"));
            Assert.AreEqual(0.0, Parse("0"));
            Assert.AreEqual(-1293.8723, Parse("-1293.8723"));
            Assert.AreEqual(3948.222, Parse("3948.222"));

            var min = -10000000.0;
            var max = +10000000.0;

            var specifiers = new string[] { "G", "E", "e" };
            var random = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var value = Math.Round(random.NextDouble() * (max - min) + min, 6);

                Assert.AreEqual(value, Parse(value.ToString()));
                var valueString = value.ToString(specifiers[random.Next(specifiers.Length)]);
                Assert.AreEqual(
                    double.Parse(valueString),
                    JsonHelper.Parse(valueString, 0, valueString.Length, 0.0),
                    0.0000001);
            }
        }
    }
}
