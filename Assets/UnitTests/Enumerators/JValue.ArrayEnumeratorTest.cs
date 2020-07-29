using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Halak.Tests.Enumerators
{
    public class ArrayEnumeratorTest
    {
        private static IReadOnlyList<JValue> GetItems(string json, string propertyName)
        {
            JValue jvalue = JValue.Parse(json);
            return jvalue.GetValue(propertyName).GetArrayItems().ToList();
        }
        
        [Test]
        public void ArrayEnumeratorShouldReturnNothingWithEmpty()
        {
            string json = "{ \"test\": [] }";
            var result = GetItems(json, "test");
            Assert.IsTrue(result.Count == 0);
        }
        
        [Test]
        public void ArrayEnumeratorShouldReturnNothingWithNonExisting()
        {
            string json = "{}";
            var result = GetItems(json, "test");
            Assert.IsTrue(result.Count == 0);
        }
        
        [Test]
        public void ArrayEnumeratorShouldReturnItems()
        {
            string json = "{ \"test\": [1,2,3] }";
            var result = GetItems(json, "test").ToArray();
            Assert.Contains(new [] {new JValue(1), new JValue(2), new JValue(3)}, result);
        }
        
        [Test]
        public void ArrayEnumeratorShouldReturnItems2()
        {
            string json = "{ \"test\": [1,2,3] }";
            var result = GetItems(json, "test")
                .Select(v => v.ToInt32())
                .ToArray();
            
            Assert.Contains(new [] {1, 2, 3}, result);
        }

        
    }
}