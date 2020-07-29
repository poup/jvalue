using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Halak.Tests.Enumerators
{
    public class CharEnumeratorTest
    {
        private static IReadOnlyList<char> GetItems(string json, string propertyName)
        {
            JValue jvalue = JValue.Parse(json);
            return jvalue.GetValue(propertyName).GetCharEnumerator().ToList();
        }
        
        [Test]
        public void CharEnumeratorShouldReturnNothingWithEmpty()
        {
            string json = "{ \"test\": \"\" }";
            var result = GetItems(json, "test");
            Assert.IsTrue(result.Count == 0);
        }
        
        [Test]
        public void CharEnumeratorShouldReturnNothingWithNonExisting()
        {
            string json = "{}";
            var result = GetItems(json, "test");
            Assert.IsTrue(result.Count == 0);
        }
        
        [Test]
        public void CharEnumeratorShouldReturnItems()
        {
            string json = "{ \"test\": [1,2,3] }";
            var result = GetItems(json, "test").ToArray();
            Assert.Contains(new [] { '[', '1', ',', '2', ',', '3', ']'}, result);
        }

    }
}