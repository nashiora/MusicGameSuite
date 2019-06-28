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

namespace System.IO.Helpers
{
    internal unsafe static class BigEndian
    {
        public static short ReadInt16(byte* p)
        {
            return (short)(p[0] << 8 | p[1]);
        }

        public static int ReadInt32(byte* p)
        {
            return p[0] << 24 | p[1] << 16 | p[2] << 8 | p[3];
        }

        public static long ReadInt64(byte* p)
        {
            int lo = ReadInt32(p);
            int hi = ReadInt32(p + 4);
            return (long)hi << 32 | (uint)lo;
        }

        public static decimal ReadDecimal(byte* p)
        {
            decimal result;
            int* d = (int*)&result;
            int lo = ReadInt32(p);
            int mid = ReadInt32(p + 4);
            int hi = ReadInt32(p + 8);
            int flags = ReadInt32(p + 12);
            d[0] = flags;
            d[1] = hi;
            d[2] = lo;
            d[3] = mid;
            return result;
        }

        public static void WriteInt16(byte* p, short s)
        {
            p[0] = (byte)(s >> 8);
            p[1] = (byte)s;
        }

        public static void WriteInt32(byte* p, int i)
        {
            p[0] = (byte)(i >> 24);
            p[1] = (byte)(i >> 16);
            p[2] = (byte)(i >> 8);
            p[3] = (byte)i;
        }

        public static void WriteInt64(byte* p, long l)
        {
            p[0] = (byte)(l >> 56);
            p[1] = (byte)(l >> 48);
            p[2] = (byte)(l >> 40);
            p[3] = (byte)(l >> 32);
            p[4] = (byte)(l >> 24);
            p[5] = (byte)(l >> 16);
            p[6] = (byte)(l >> 8);
            p[7] = (byte)l;
        }

        public static void WriteDecimal(byte* p, decimal d)
        {
            int* i = (int*)&d;
            int flags = i[0];
            int hi = i[1];
            int lo = i[2];
            int mid = i[3];
            WriteInt32(p, lo);
            WriteInt32(p + 4, mid);
            WriteInt32(p + 8, hi);
            WriteInt32(p + 12, flags);
        }
    }
}
