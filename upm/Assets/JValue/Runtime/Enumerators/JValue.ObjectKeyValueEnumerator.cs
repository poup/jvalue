using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Assertions;

namespace Halak
{
    public readonly partial struct JValue
    {
        public struct ObjectKeyValueEnumerator : IEnumerator<KeyValuePair<JValue, JValue>>
        {
            private readonly JValue m_source;
            private readonly int m_endIndex;

            private int m_nextIndex;
            private KeyValuePair<JValue, JValue> m_current;

            public KeyValuePair<JValue, JValue> Current => m_current;
            object IEnumerator.Current => m_current;

            internal ObjectKeyValueEnumerator(JValue value)
            {
                Assert.IsTrue(value.Type == TypeCode.Object);

                m_source = value;
                m_endIndex = value.startIndex + value.length - 1;

                m_nextIndex = value.SkipWhitespaces(value.startIndex);
                m_current = default;
            }


            [UsedImplicitly]
            public ObjectKeyValueEnumerator GetEnumerator()
            {
                return this;
            }

            [UsedImplicitly]
            public bool MoveNext()
            {
                if (m_nextIndex >= m_endIndex)
                {
                    return false;
                }

                var source = m_source;
                var keyStart = m_nextIndex;
                var keyEnd = source.SkipString(keyStart);

                var valueStart = source.SkipWhitespaces(keyEnd + 1);
                var valueEnd = source.SkipValue(valueStart);

                m_current = new KeyValuePair<JValue, JValue>(
                    new JValue(source.source, keyStart, keyEnd - keyStart),
                    new JValue(source.source, valueStart, valueEnd - valueStart)
                );

                m_nextIndex = source.SkipWhitespaces(valueEnd + 1);
                return true;
            }

            public void Reset()
            {
                m_nextIndex = m_source.SkipWhitespaces(m_source.startIndex);
                m_current = default;
            }

            public void Dispose()
            {
            }
        }
    }
}
