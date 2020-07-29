using JetBrains.Annotations;
using System.IO;
using System.Runtime.CompilerServices;

namespace Halak
{
    public partial class JsonWriter
    {
        [PublicAPI]
        public class Formatter
        {
            public static readonly Formatter compact = new Formatter();
            public static readonly Formatter compactLineByLine = new CompactLineByLineFormatter();
            public static Formatter pretty => new PrettyFormatter();

            protected internal Formatter()
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public virtual void WriteStartArrayToken(TextWriter underlyingWriter)
            {
                underlyingWriter.Write('[');
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public virtual void WriteEndArrayToken(TextWriter underlyingWriter)
            {
                underlyingWriter.Write(']');
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public virtual void WriteStartObjectToken(TextWriter underlyingWriter)
            {
                underlyingWriter.Write('{');
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public virtual void WriteEndObjectToken(TextWriter underlyingWriter)
            {
                underlyingWriter.Write('}');
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public virtual void WriteValueSeparator(TextWriter underlyingWriter)
            {
                underlyingWriter.Write(',');
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public virtual void WritePropertySeparator(TextWriter underlyingWriter)
            {
                underlyingWriter.Write(':');
            }
        }

        private sealed class CompactLineByLineFormatter : Formatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WriteStartArrayToken(TextWriter underlyingWriter)
            {
                base.WriteStartArrayToken(underlyingWriter);
                underlyingWriter.Write('\n');
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WriteEndArrayToken(TextWriter underlyingWriter)
            {
                underlyingWriter.Write('\n');
                base.WriteEndArrayToken(underlyingWriter);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WriteStartObjectToken(TextWriter underlyingWriter)
            {
                base.WriteStartObjectToken(underlyingWriter);
                underlyingWriter.Write('\n');
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WriteEndObjectToken(TextWriter underlyingWriter)
            {
                underlyingWriter.Write('\n');
                base.WriteEndObjectToken(underlyingWriter);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WriteValueSeparator(TextWriter underlyingWriter)
            {
                base.WriteValueSeparator(underlyingWriter);
                underlyingWriter.Write('\n');
            }
        }

        private sealed class PrettyFormatter : Formatter
        {
            private int depth;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void WriteIndentation(TextWriter underlyingWriter)
            {
                for (int i = 0, spaces = depth * 2; i < spaces; ++i)
                {
                    underlyingWriter.Write(' ');
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WriteStartArrayToken(TextWriter underlyingWriter)
            {
                depth++;
                base.WriteStartArrayToken(underlyingWriter);
                underlyingWriter.Write('\n');
                WriteIndentation(underlyingWriter);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WriteEndArrayToken(TextWriter underlyingWriter)
            {
                depth--;
                underlyingWriter.Write('\n');
                WriteIndentation(underlyingWriter);
                base.WriteEndArrayToken(underlyingWriter);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WriteStartObjectToken(TextWriter underlyingWriter)
            {
                depth++;
                base.WriteStartObjectToken(underlyingWriter);
                underlyingWriter.Write('\n');
                WriteIndentation(underlyingWriter);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WriteEndObjectToken(TextWriter underlyingWriter)
            {
                depth--;
                underlyingWriter.Write('\n');
                WriteIndentation(underlyingWriter);
                base.WriteEndObjectToken(underlyingWriter);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WriteValueSeparator(TextWriter underlyingWriter)
            {
                base.WriteValueSeparator(underlyingWriter);
                underlyingWriter.Write('\n');
                WriteIndentation(underlyingWriter);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void WritePropertySeparator(TextWriter underlyingWriter)
            {
                base.WritePropertySeparator(underlyingWriter);
                underlyingWriter.Write(' ');
            }
        }
    }
}