#region Copyright notice and license

// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd

#endregion

using System.Security;

namespace LightProto
{
    /// <summary>
    /// Reading and skipping messages / groups
    /// </summary>
    [SecuritySafeCritical]
    internal static class ParsingPrimitivesMessages
    {
        private static readonly byte[] ZeroLengthMessageStreamData = new byte[] { 0 };

        public static void SkipLastField(
            ref ReadOnlySpan<byte> buffer,
            ref ParserInternalState state
        )
        {
            if (state.lastTag == 0)
            {
                throw new InvalidOperationException(
                    "SkipLastField cannot be called at the end of a stream"
                );
            }

            switch (WireFormat.GetTagWireType(state.lastTag))
            {
                case WireFormat.WireType.StartGroup:
                    SkipGroup(ref buffer, ref state, state.lastTag);
                    break;
                case WireFormat.WireType.EndGroup:
                    throw new InvalidProtocolBufferException(
                        "SkipLastField called on an end-group tag, indicating that the corresponding start-group was missing"
                    );
                case WireFormat.WireType.Fixed32:
                    ParsingPrimitives.ParseRawLittleEndian32(ref buffer, ref state);
                    break;
                case WireFormat.WireType.Fixed64:
                    ParsingPrimitives.ParseRawLittleEndian64(ref buffer, ref state);
                    break;
                case WireFormat.WireType.LengthDelimited:
                    var length = ParsingPrimitives.ParseLength(ref buffer, ref state);
                    ParsingPrimitives.SkipRawBytes(ref buffer, ref state, length);
                    break;
                case WireFormat.WireType.Varint:
                    ParsingPrimitives.ParseRawVarint32(ref buffer, ref state);
                    break;
            }
        }

        /// <summary>
        /// Skip a group.
        /// </summary>
        public static void SkipGroup(
            ref ReadOnlySpan<byte> buffer,
            ref ParserInternalState state,
            uint startGroupTag
        )
        {
            // Note: Currently we expect this to be the way that groups are read. We could put the recursion
            // depth changes into the ReadTag method instead, potentially...
            state.recursionDepth++;
            if (state.recursionDepth >= state.recursionLimit)
            {
                throw InvalidProtocolBufferException.RecursionLimitExceeded();
            }

            uint tag;
            while (true)
            {
                tag = ParsingPrimitives.ParseTag(ref buffer, ref state);
                if (tag == 0)
                {
                    throw InvalidProtocolBufferException.TruncatedMessage();
                }

                // Can't call SkipLastField for this case- that would throw.
                if (WireFormat.GetTagWireType(tag) == WireFormat.WireType.EndGroup)
                {
                    break;
                }

                // This recursion will allow us to handle nested groups.
                SkipLastField(ref buffer, ref state);
            }

            int startField = WireFormat.GetTagFieldNumber(startGroupTag);
            int endField = WireFormat.GetTagFieldNumber(tag);
            if (startField != endField)
            {
                throw new InvalidProtocolBufferException(
                    $"Mismatched end-group tag. Started with field {startField}; ended with field {endField}"
                );
            }

            state.recursionDepth--;
        }

        /// <summary>
        /// Verifies that the last call to ReadTag() returned tag 0 - in other words,
        /// we've reached the end of the stream when we expected to.
        /// </summary>
        /// <exception cref="InvalidProtocolBufferException">The
        /// tag read was not the one specified</exception>
        public static void CheckReadEndOfStreamTag(ref ParserInternalState state)
        {
            if (state.lastTag != 0)
            {
                throw InvalidProtocolBufferException.MoreDataAvailable();
            }
        }

        private static void CheckLastTagWas(ref ParserInternalState state, uint expectedTag)
        {
            if (state.lastTag != expectedTag)
            {
                throw InvalidProtocolBufferException.InvalidEndTag();
            }
        }
    }
}
