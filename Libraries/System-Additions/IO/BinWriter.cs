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
    public unsafe class BinWriter : IDisposable
    {
        private static readonly Encoding UTF8NoBomThrows = new UTF8Encoding(false, true);

        protected Stream OutStream;
        protected readonly byte[] buffer;
        private byte[] charBuffer;
        private int charBufferLength;

        private const int CharBufferSize = 256;

        private Encoding encoding;
        private Encoder encoder;

        public BinWriter(Stream s)
            : this(s, UTF8NoBomThrows)
        {
        }

        public BinWriter(Stream s, Encoding e)
        {
            if (s==null)
                throw new ArgumentNullException("output");
            if (e==null)
                throw new ArgumentNullException("encoding");
            if (!s.CanWrite)
                throw new ArgumentException("StreamNotWritable");

            OutStream = s;

            encoding = e;
            encoder = encoding.GetEncoder();

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
                OutStream.Close();
            }
        }
    
        public void Flush() 
        {
            OutStream.Flush();
        }
    
        public long Seek(int offset, SeekOrigin origin)
        {
            return OutStream.Seek(offset, origin);
        }
        
        public void Write(bool value) {
            buffer[0] = (byte) (value ? 1 : 0);
            OutStream.Write(buffer, 0, 1);
        }
        
        public void Write(byte value) 
        {
            OutStream.WriteByte(value);
        }
        
        public virtual void Write(sbyte value) 
        {
            OutStream.WriteByte((byte) value);
        }
        
        public virtual void Write(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            OutStream.Write(buffer, 0, buffer.Length);
        }
        
        public virtual void Write(byte[] buffer, int index, int count)
        {
            OutStream.Write(buffer, index, count);
        }

        public void Write(decimal value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteDecimal(p, value);
            OutStream.Write(buffer, 0, 16);
        }

        public void Write(double value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt64(p, Reinterpret.CastToLong(value));
            OutStream.Write(buffer, 0, 8);
        }

        public void Write(float value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteDecimal(p, Reinterpret.CastToInt(value));
            OutStream.Write(buffer, 0, 4);
        }

        public void Write(int value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt32(p, value);
            OutStream.Write(buffer, 0, 4);
        }

        public void Write(long value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt64(p, value);
            OutStream.Write(buffer, 0, 8);
        }

        public void Write(short value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt16(p, value);
            OutStream.Write(buffer, 0, 2);
        }

        public void Write(uint value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt32(p, (int)value);
            OutStream.Write(buffer, 0, 4);
        }

        public void Write(ulong value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt64(p, (long)value);
            OutStream.Write(buffer, 0, 8);
        }

        public void Write(ushort value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt16(p, (short)value);
            OutStream.Write(buffer, 0, 2);
        }

        public void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Write((byte)0);
                return;
            }

            int len = encoding.GetByteCount(value);

            if (charBuffer == null) {
                charBuffer = new byte[CharBufferSize];
                charBufferLength = charBuffer.Length / encoding.GetMaxByteCount(1);
            }
 
            if (len <= charBuffer.Length) {
                //Contract.Assert(len == _encoding.GetBytes(chars, 0, chars.Length, _largeByteBuffer, 0), "encoding's GetByteCount & GetBytes gave different answers!  encoding type: "+_encoding.GetType().Name);
                encoding.GetBytes(value, 0, value.Length, charBuffer, 0);
                OutStream.Write(charBuffer, 0, len);
            }
            else {
                // Aggressively try to not allocate memory in this loop for
                // runtime performance reasons.  Use an Encoder to write out 
                // the string correctly (handling surrogates crossing buffer
                // boundaries properly).  
                int charStart = 0;
                int numLeft = value.Length;
                while (numLeft > 0) {
                    // Figure out how many chars to process this round.
                    int charCount = (numLeft > charBufferLength) ? charBufferLength : numLeft;
                    int byteLen;
 
                    checked {
                        if (charStart < 0 || charCount < 0 || charStart + charCount > value.Length) {
                            throw new ArgumentOutOfRangeException("charCount");
                        }
 
                        fixed(char* pChars = value) {
                            fixed(byte* pBytes = charBuffer) {
                                byteLen = encoder.GetBytes(pChars + charStart, charCount, pBytes, charBuffer.Length, charCount == numLeft);
                            }
                        }
                    }
                    OutStream.Write(charBuffer, 0, byteLen);
                    charStart += charCount;
                    numLeft -= charCount;
                }
            }

            // null term
            Write((byte)0);
        }

        public void Write(List<string> values)
        {
            Write(values.Count);
            foreach (string value in values)
                Write(value);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
    }
}
