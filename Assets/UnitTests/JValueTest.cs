using NUnit.Framework;
using UnityEngine;

namespace Halak.Tests
{
    public class JValueTest
    {
        [Test]
        
        
        public void JValueShouldSerializeDeserialize()
        {
            string json = "{ \"a\": 1, " +
                          "\"b\": \"bb\", " +
                          "\"c\": [1,2], " +
                          "\"d\": { " +
                          "  \"da\": 11, " +
                          "  \"db\": \"dbb\"" +
                          "  } " +
                          "}";
            var jvalue = JValue.Parse(json);

            string result = jvalue.Serialize(JsonWriter.Formatter.compact);
            Debug.Log(result);

            Assert.AreEqual(json.Replace(" ", ""), result);
        }
    }
}