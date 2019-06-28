/*
Modified from https://github.com/jamesqo/Be.IO

Copyright (c) 2015, James Ko
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections.Generic;
using System.IO.Helpers;
using System.Text;

namespace System.IO
{
    public unsafe class BinReader : IDisposable
    {
        private static readonly Encoding UTF8NoBom = new UTF8Encoding();

        protected Stream InStream;
        protected readonly byte[] buffer;
        private byte[] charBuffer;
        private int charBufferLength;

        private const int CharBufferSize = 256;

        private Encoding encoding;
        private Encoder encoder;

        public bool IsEndOfFile { get { return InStream.Position == InStream.Length; } }

        public BinReader(Stream s)
            : this(s, UTF8NoBom)
        {
        }

        public BinReader(Stream s, Encoding e)
        {
            if (s==null)
                throw new ArgumentNullException("input");
            if (e==null)
                throw new ArgumentNullException("encoding");
            if (!s.CanRead)
                throw new ArgumentException("StreamNotReadable");

            InStream = s;

            encoding = e;
            encoder = e.GetEncoder();

            buffer = new byte[16];
        }
        
        public void Close()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing) 
            {
                InStream.Close();
            }
        }
    
        public long Seek(int offset, SeekOrigin origin)
        {
            return InStream.Seek(offset, origin);
        }

        public decimal ReadDecimal()
        {
            FillBuffer(16);
            fixed (byte* p = buffer)
                return BigEndian.ReadDecimal(p);
        }

        public double ReadDouble()
        {
            FillBuffer(8);
            fixed (byte* p = buffer)
                return Reinterpret.CastToDouble(BigEndian.ReadInt64(p));
        }

        public byte ReadByte()
        {
            return (byte)InStream.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return (sbyte)InStream.ReadByte();
        }

        public short ReadInt16()
        {
            FillBuffer(2);
            fixed (byte* p = buffer)
                return BigEndian.ReadInt16(p);
        }

        public int ReadInt32()
        {
            FillBuffer(4);
            fixed (byte* p = buffer)
                return BigEndian.ReadInt32(p);
        }

        public long ReadInt64()
        {
            FillBuffer(8);
            fixed (byte* p = buffer)
                return BigEndian.ReadInt64(p);
        }

        public float ReadSingle()
        {
            FillBuffer(4);
            fixed (byte* p = buffer)
                return Reinterpret.CastToFloat(BigEndian.ReadInt32(p));
        }

        public ushort ReadUInt16()
        {
            FillBuffer(2);
            fixed (byte* p = buffer)
                return (ushort)BigEndian.ReadInt16(p);
        }

        public uint ReadUInt32()
        {
            FillBuffer(4);
            fixed (byte* p = buffer)
                return (uint)BigEndian.ReadInt32(p);
        }

        public ulong ReadUInt64()
        {
            FillBuffer(8);
            fixed (byte* p = buffer)
                return (ulong)BigEndian.ReadInt64(p);
        }

        protected void FillBuffer(int numBytes)
        {
            if ((uint)numBytes > buffer.Length)
                Error.Range("numBytes", "Expected a non-negative value.");
            var s = InStream;
            if (s == null)
                Error.Disposed();
            int n, read = 0;
            do
            {
                n = s.Read(buffer, read, numBytes - read);
                if (n == 0)
                    Error.EndOfStream();
                read += n;
            } while (read < numBytes);
        }

        // TODO(local): better implementation here plzzzz
        public string ReadString()
        {
            if (charBuffer == null) {
                charBuffer = new byte[CharBufferSize];
                charBufferLength = charBuffer.Length / encoding.GetMaxByteCount(1);
            }

            // TODO(local): use the char buffer before writing to mem
            using (var mem = new MemoryStream(64))
            {
                byte value;
                while ((value = ReadByte()) != (byte)0)
                    mem.WriteByte(value);

                mem.Flush();
                return encoding.GetString(mem.ToArray());
            }
        }

        public List<string> ReadStringList()
        {
            int count = ReadInt32();
            var result = new List<string>(count);

            for (int i = 0; i < count; i++)
                result.Add(ReadString());

            return result;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
    }
}
