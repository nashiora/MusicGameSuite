using System.Text;

namespace System.IO
{
    public static class BigEndian
    {
        private static byte[] ToLittleEndianOrder(this byte[] array)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(array);
            return array;
        }

        private static byte[] ToBigEndianOrder(this byte[] array)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(array);
            return array;
        }

        public static byte[] ReadBytesRequired(this BinaryReader reader, int count)
        {
            var result = reader.ReadBytes(count);
            if (result.Length != count)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", count, result.Length));
            return result;
        }

        /// https://stackoverflow.com/questions/10439242/count-leading-zeroes-in-an-int32
        private static int RtlFindMostSignificantBit(int x)
        {
            //do the smearing
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            //count the ones
            x -= x >> 1 & 0x55555555;
            x = (x >> 2 & 0x33333333) + (x & 0x33333333);
            x = (x >> 4) + x & 0x0f0f0f0f;
            x += x >> 8;
            x += x >> 16;
            return x & 0x0000003f; //subtract # of 1s from 32
        }

        #region Read

        public static sbyte ReadInt8(this BinaryReader reader) => reader.ReadSByte();
        public static byte ReadUInt8(this BinaryReader reader) => reader.ReadByte();

        public static string ReadStringUTF8(this BinaryReader reader)
        {
            int byteCount = reader.Read7BitEncodedInt();
            byte[] bytes = reader.ReadBytesRequired(byteCount);
            return Encoding.UTF8.GetString(bytes);
        }

        public static int Read7BitEncodedInt(this BinaryReader br)
        {
            sbyte b;
            int r = -7, v = 0;
            do v |= ((b = br.ReadSByte()) & 0x7F) << (r += 7);
            while (b < 0);
            return v;
        }

        public static short ReadInt16BE(this BinaryReader reader) => BitConverter.ToInt16(reader.ReadBytesRequired(sizeof(short)).ToBigEndianOrder(), 0);
        public static int ReadInt32BE(this BinaryReader reader) => BitConverter.ToInt32(reader.ReadBytesRequired(sizeof(int)).ToBigEndianOrder(), 0);
        public static long ReadInt64BE(this BinaryReader reader) => BitConverter.ToInt64(reader.ReadBytesRequired(sizeof(long)).ToBigEndianOrder(), 0);

        public static ushort ReadUInt16BE(this BinaryReader reader) => BitConverter.ToUInt16(reader.ReadBytesRequired(sizeof(ushort)).ToBigEndianOrder(), 0);
        public static uint ReadUInt32BE(this BinaryReader reader) => BitConverter.ToUInt32(reader.ReadBytesRequired(sizeof(uint)).ToBigEndianOrder(), 0);
        public static ulong ReadUInt64BE(this BinaryReader reader) => BitConverter.ToUInt64(reader.ReadBytesRequired(sizeof(ulong)).ToBigEndianOrder(), 0);

        public static float ReadSingleBE(this BinaryReader reader) => BitConverter.ToSingle(reader.ReadBytesRequired(sizeof(float)).ToBigEndianOrder(), 0);
        public static double ReadDoubleBE(this BinaryReader reader) => BitConverter.ToDouble(reader.ReadBytesRequired(sizeof(double)).ToBigEndianOrder(), 0);

        public static short ReadInt16LE(this BinaryReader reader) => BitConverter.ToInt16(reader.ReadBytesRequired(sizeof(short)).ToLittleEndianOrder(), 0);
        public static int ReadInt32LE(this BinaryReader reader) => BitConverter.ToInt32(reader.ReadBytesRequired(sizeof(int)).ToLittleEndianOrder(), 0);
        public static long ReadInt64LE(this BinaryReader reader) => BitConverter.ToInt64(reader.ReadBytesRequired(sizeof(long)).ToLittleEndianOrder(), 0);

        public static ushort ReadUInt16LE(this BinaryReader reader) => BitConverter.ToUInt16(reader.ReadBytesRequired(sizeof(ushort)).ToLittleEndianOrder(), 0);
        public static uint ReadUInt32LE(this BinaryReader reader) => BitConverter.ToUInt32(reader.ReadBytesRequired(sizeof(uint)).ToLittleEndianOrder(), 0);
        public static ulong ReadUInt64LE(this BinaryReader reader) => BitConverter.ToUInt64(reader.ReadBytesRequired(sizeof(ulong)).ToLittleEndianOrder(), 0);

        public static float ReadSingleLE(this BinaryReader reader) => BitConverter.ToSingle(reader.ReadBytesRequired(sizeof(float)).ToLittleEndianOrder(), 0);
        public static double ReadDoubleLE(this BinaryReader reader) => BitConverter.ToDouble(reader.ReadBytesRequired(sizeof(double)).ToLittleEndianOrder(), 0);

        #endregion

        #region Write

        public static void WriteInt8(this BinaryWriter writer, sbyte value) => writer.Write(value);
        public static void WriteUInt8(this BinaryWriter writer, byte value) => writer.Write(value);

        public static void WriteStringUTF8(this BinaryWriter writer, string value)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(value);
            writer.Write7BitEncodedInt(utf8.Length);
            writer.Write(utf8, 0, utf8.Length);
        }

        public static void Write7BitEncodedInt(this BinaryWriter bw, int i)
        {
            var str = bw.BaseStream;
            switch (RtlFindMostSignificantBit(i) / 7)
            {
                case 0:
                    str.WriteByte((byte)i);
                    break;
                case 1:
                    str.WriteByte((byte)(i | 0x80));
                    str.WriteByte((byte)(i >> 7));
                    break;
                case 2:
                    str.WriteByte((byte)(i /***/ | 0x80));
                    str.WriteByte((byte)(i >> 07 | 0x80));
                    str.WriteByte((byte)(i >> 14));
                    break;
                case 3:
                    str.WriteByte((byte)(i /***/ | 0x80));
                    str.WriteByte((byte)(i >> 07 | 0x80));
                    str.WriteByte((byte)(i >> 14 | 0x80));
                    str.WriteByte((byte)(i >> 21));
                    break;
                case 4:
                    str.WriteByte((byte)(i /***/ | 0x80));
                    str.WriteByte((byte)(i >> 07 | 0x80));
                    str.WriteByte((byte)(i >> 14 | 0x80));
                    str.WriteByte((byte)(i >> 21 | 0x80));
                    str.WriteByte((byte)((uint)i >> 28));
                    break;
            }
        }

        public static void WriteInt16BE(this BinaryWriter writer, short value) => writer.Write(BitConverter.GetBytes(value).ToBigEndianOrder());
        public static void WriteInt32BE(this BinaryWriter writer, int value) => writer.Write(BitConverter.GetBytes(value).ToBigEndianOrder());
        public static void WriteInt64BE(this BinaryWriter writer, long value) => writer.Write(BitConverter.GetBytes(value).ToBigEndianOrder());

        public static void WriteUInt16BE(this BinaryWriter writer, ushort value) => writer.Write(BitConverter.GetBytes(value).ToBigEndianOrder());
        public static void WriteUInt32BE(this BinaryWriter writer, uint value) => writer.Write(BitConverter.GetBytes(value).ToBigEndianOrder());
        public static void WriteUInt64BE(this BinaryWriter writer, ulong value) => writer.Write(BitConverter.GetBytes(value).ToBigEndianOrder());

        public static void WriteSingleBE(this BinaryWriter writer, float value) => writer.Write(BitConverter.GetBytes(value).ToBigEndianOrder());
        public static void WriteDoubleBE(this BinaryWriter writer, double value) => writer.Write(BitConverter.GetBytes(value).ToBigEndianOrder());


        public static void WriteInt16LE(this BinaryWriter writer, short value) => writer.Write(BitConverter.GetBytes(value).ToLittleEndianOrder());
        public static void WriteInt32LE(this BinaryWriter writer, int value) => writer.Write(BitConverter.GetBytes(value).ToLittleEndianOrder());
        public static void WriteInt64LE(this BinaryWriter writer, long value) => writer.Write(BitConverter.GetBytes(value).ToLittleEndianOrder());

        public static void WriteUInt16LE(this BinaryWriter writer, ushort value) => writer.Write(BitConverter.GetBytes(value).ToLittleEndianOrder());
        public static void WriteUInt32LE(this BinaryWriter writer, uint value) => writer.Write(BitConverter.GetBytes(value).ToLittleEndianOrder());
        public static void WriteUInt64LE(this BinaryWriter writer, ulong value) => writer.Write(BitConverter.GetBytes(value).ToLittleEndianOrder());

        public static void WriteSingleLE(this BinaryWriter writer, float value) => writer.Write(BitConverter.GetBytes(value).ToLittleEndianOrder());
        public static void WriteDoubleLE(this BinaryWriter writer, double value) => writer.Write(BitConverter.GetBytes(value).ToLittleEndianOrder());

        #endregion
    }
}
