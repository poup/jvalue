using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Halak.Tests.Enumerators
{
    public class ObjectKeyValueEnumeratorTest
    {
        private static Dictionary<string, JValue> GetItems(string json)
        {
            JValue jvalue = JValue.Parse(json);
            return jvalue.GetObjectKeyValues().ToDictionary();
        }
        
        [Test]
        public void ObjectKeyValueEnumeratorShouldReturnNothingWithEmpty()
        {
            string json = "{}";
            var result = GetItems(json);
            Assert.IsTrue(result.Count == 0);
        }
        
        [Test]
        public void ObjectKeyValueEnumeratorShouldReturnItems()
        {
            string json = "{ " +
                          "\"test1\": 1," +
                          "\"test2\": 2" +
                          " }";
            var result = GetItems(json);
            
            Assert.IsTrue(result.Count == 2);
            Assert.AreEqual(1, result["test1"].ToInt32());
            Assert.AreEqual(2, result["test2"].ToInt32());
        }  
        
        [Test]
        public void ObjectKeyValueEnumeratorShouldReturnItems2()
        {
            string json = "{ " +
                          "\"test1\": 1,\"tr\",12," +
                          "\"test2\": 2" +
                          " }";
            var result = GetItems(json);
            
            Assert.IsTrue(result.Count == 2);
            Assert.AreEqual(1, result["test1"].ToInt32());
            Assert.AreEqual(2, result["test2"].ToInt32());
        }

    }
}