using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Halak
{
    public readonly partial struct JValue
    {
        public struct ArrayEnumerator : IEnumerator<JValue>
        {
            private readonly JValue m_source;
            private readonly int m_endIndex;

            private int m_nextIndex;
            private JValue m_current;

            public JValue Current => m_current;
            object IEnumerator.Current => m_current;

            internal ArrayEnumerator(JValue value)
            {
                Assert.IsTrue(value.typeCode == TypeCode.Array);
                m_source = value;
                m_endIndex = value.startIndex + value.length-1;

                m_nextIndex = value.SkipWhitespaces(value.startIndex);
                m_current = default;
            }


            [UsedImplicitly]
            public ArrayEnumerator GetEnumerator()
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

                int currentEnd = source.SkipValue(m_nextIndex);
                m_current = new JValue(source.source, m_nextIndex, currentEnd - m_nextIndex);

                m_nextIndex = source.SkipWhitespaces(currentEnd + 1);
                return true;
            }

            public void Reset()
            {
                m_nextIndex = m_source.SkipWhitespaces(m_source.startIndex+1);
                m_current = default;
            }

            public void Dispose()
            {
            }
        }
    }
}
