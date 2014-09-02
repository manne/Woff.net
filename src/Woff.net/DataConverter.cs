//
// Authors:
//   Miguel de Icaza (miguel@novell.com)
//
// See the following url for documentation:
//     http://www.mono-project.com/Mono_DataConvert
//
// Compilation Options:
//     MONO_DATACONVERTER_PUBLIC:
//         Makes the class public instead of the default internal.
//
//     MONO_DATACONVERTER_STATIC_METHODS:     
//         Exposes the public static methods.
//
// TODO:
//   Support for "DoubleWordsAreSwapped" for ARM devices
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

using WoffDotNet.Properties;

#pragma warning disable 3021

// ReSharper disable CheckNamespace
namespace Mono
// ReSharper restore CheckNamespace
{

#if MONO_DATACONVERTER_PUBLIC
    unsafe public abstract class DataConverter {
#else
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    unsafe internal abstract class DataConverter
    {

        // Disables the warning: CLS compliance checking will not be performed on
        //  `XXXX' because it is not visible from outside this assembly
#pragma warning disable  3019
#endif
        static readonly DataConverter SwapConv = new SwapConverter();
        static readonly DataConverter CopyConv = new CopyConverter();

        public static readonly bool IsLittleEndian = BitConverter.IsLittleEndian;

        public abstract double GetDouble(byte[] data, int index);
        public abstract float GetFloat(byte[] data, int index);
        public abstract long GetInt64(byte[] data, int index);
        public abstract int GetInt32(byte[] data, int index);

        public abstract short GetInt16(byte[] data, int index);

        [CLSCompliant(false)]
        public abstract uint GetUInt32(byte[] data, int index);
        [CLSCompliant(false)]
        public abstract ushort GetUInt16(byte[] data, int index);
        [CLSCompliant(false)]
        public abstract ulong GetUInt64(byte[] data, int index);

        public abstract void PutBytes(byte[] dest, int destIdx, double value);
        public abstract void PutBytes(byte[] dest, int destIdx, float value);
        public abstract void PutBytes(byte[] dest, int destIdx, int value);
        public abstract void PutBytes(byte[] dest, int destIdx, long value);
        public abstract void PutBytes(byte[] dest, int destIdx, short value);

        [CLSCompliant(false)]
        public abstract void PutBytes(byte[] dest, int destIdx, ushort value);
        [CLSCompliant(false)]
        public abstract void PutBytes(byte[] dest, int destIdx, uint value);
        [CLSCompliant(false)]
        public abstract void PutBytes(byte[] dest, int destIdx, ulong value);

        public byte[] GetBytes(double value)
        {
            byte[] ret = new byte[8];
            PutBytes(ret, 0, value);
            return ret;
        }

        public byte[] GetBytes(float value)
        {
            byte[] ret = new byte[4];
            PutBytes(ret, 0, value);
            return ret;
        }

        public byte[] GetBytes(int value)
        {
            byte[] ret = new byte[4];
            PutBytes(ret, 0, value);
            return ret;
        }

        public byte[] GetBytes(long value)
        {
            byte[] ret = new byte[8];
            PutBytes(ret, 0, value);
            return ret;
        }

        public byte[] GetBytes(short value)
        {
            byte[] ret = new byte[2];
            PutBytes(ret, 0, value);
            return ret;
        }

        [CLSCompliant(false)]
        public byte[] GetBytes(ushort value)
        {
            byte[] ret = new byte[2];
            PutBytes(ret, 0, value);
            return ret;
        }

        [CLSCompliant(false)]
        public byte[] GetBytes(uint value)
        {
            byte[] ret = new byte[4];
            PutBytes(ret, 0, value);
            return ret;
        }

        [CLSCompliant(false)]
        public byte[] GetBytes(ulong value)
        {
            byte[] ret = new byte[8];
            PutBytes(ret, 0, value);
            return ret;
        }

        static public DataConverter LittleEndian
        {
            get
            {
                return BitConverter.IsLittleEndian ? CopyConv : SwapConv;
            }
        }

        static public DataConverter BigEndian
        {
            get
            {
                return BitConverter.IsLittleEndian ? SwapConv : CopyConv;
            }
        }

        static public DataConverter Native
        {
            get
            {
                return CopyConv;
            }
        }

        static int Align(int current, int align)
        {
            return ((current + align - 1) / align) * align;
        }

        class PackContext
        {
            // Buffer
            // ReSharper disable MemberCanBePrivate.Local check later if not needed
            public byte[] Buffer;
            // ReSharper restore MemberCanBePrivate.Local
            int _next;

            public string Description;
            public int I; // position in the description
            public DataConverter Conv;
            public int Repeat;

            //
            // if AlignValue == -1, auto AlignValue to the size of the byte array
            // if AlignValue == 0, do not do alignment
            // Any other values aligns to that particular size
            //
            public int AlignValue;

            public void Add(byte[] group)
            {
                //Console.WriteLine ("Adding {0} bytes to {1} (next={2}", group.Length,
                // buffer == null ? "null" : buffer.Length.ToString (), next);

                if (Buffer == null)
                {
                    Buffer = group;
                    _next = group.Length;
                    return;
                }

                if (AlignValue != 0)
                {
                    _next = Align(_next, AlignValue == -1 ? group.Length : AlignValue);
                    AlignValue = 0;
                }

                if (_next + group.Length > Buffer.Length)
                {
                    byte[] nb = new byte[Math.Max(_next, 16) * 2 + group.Length];
                    Array.Copy(Buffer, nb, Buffer.Length);
                    Array.Copy(group, 0, nb, _next, group.Length);
                    _next = _next + group.Length;
                    Buffer = nb;
                }
                else
                {
                    Array.Copy(group, 0, Buffer, _next, group.Length);
                    _next += group.Length;
                }
            }

            public byte[] Get()
            {
                if (Buffer == null)
                    return new byte[0];

                if (Buffer.Length != _next)
                {
                    byte[] b = new byte[_next];
                    Array.Copy(Buffer, b, _next);
                    return b;
                }
                return Buffer;
            }
        }

        //
        // Format includes:
        // Control:
        //   ^    Switch to big endian encoding
        //   _    Switch to little endian encoding
        //   %    Switch to host (native) encoding
        //   !    aligns the next data type to its natural boundary (for strings this is 4).
        //
        // Types:
        //   s    Int16
        //   S    UInt16
        //   i    Int32
        //   I    UInt32
        //   l    Int64
        //   L    UInt64
        //   f    float
        //   d    double
        //   b    byte
        //   c    1-byte signed character
        //   C    1-byte unsigned character
        //   z8   string encoded as UTF8 with 1-byte null terminator
        //   z6   string encoded as UTF16 with 2-byte null terminator
        //   z7   string encoded as UTF7 with 1-byte null terminator
        //   zb   string encoded as BigEndianUnicode with 2-byte null terminator
        //   z3   string encoded as UTF32 with 4-byte null terminator
        //   z4   string encoded as UTF32 big endian with 4-byte null terminator
        //   $8   string encoded as UTF8
        //   $6   string encoded as UTF16
        //   $7   string encoded as UTF7
        //   $b   string encoded as BigEndianUnicode
        //   $3   string encoded as UTF32
        //   $4   string encoded as UTF-32 big endian encoding
        //   x    null byte
        //
        // Repeats, these are prefixes:
        //   N    a number between 1 and 9, indicates a repeat count (process N items
        //        with the following datatype
        //   [N]  For numbers larger than 9, use brackets, for example [20]
        //   *    Repeat the next data type until the arguments are exhausted
        //
        static public byte[] Pack(string description, params object[] args)
        {
            int argn = 0;
            PackContext b = new PackContext { Conv = CopyConv, Description = description };

            for (b.I = 0; b.I < description.Length; )
            {
                object oarg;

                if (argn < args.Length)
                    oarg = args[argn];
                else
                {
                    if (b.Repeat != 0)
                        break;

                    oarg = null;
                }

                int save = b.I;

                if (PackOne(b, oarg))
                {
                    argn++;
                    if (b.Repeat > 0)
                    {
                        if (--b.Repeat > 0)
                            b.I = save;
                        else
                            b.I++;
                    }
                    else
                        b.I++;
                }
                else
                    b.I++;
            }
            return b.Get();
        }

        static public byte[] PackEnumerable(string description, IEnumerable args)
        {
            PackContext b = new PackContext
            {
                Conv = CopyConv,
                Description = description
            };

            IEnumerator enumerator = args.GetEnumerator();
            bool ok = enumerator.MoveNext();

            for (b.I = 0; b.I < description.Length; )
            {
                object oarg;

                if (ok)
                    oarg = enumerator.Current;
                else
                {
                    if (b.Repeat != 0)
                        break;
                    oarg = null;
                }

                int save = b.I;

                if (PackOne(b, oarg))
                {
                    ok = enumerator.MoveNext();
                    if (b.Repeat > 0)
                    {
                        if (--b.Repeat > 0)
                            b.I = save;
                        else
                            b.I++;
                    }
                    else
                        b.I++;
                }
                else
                    b.I++;
            }
            return b.Get();
        }

        //
        // Packs one datum `oarg' into the buffer `b', using the string format
        // in `description' at position `i'
        //
        // Returns: true if we must pick the next object from the list
        //
        static bool PackOne(PackContext b, object oarg)
        {
            int n;

            switch (b.Description[b.I])
            {
                case '^':
                    b.Conv = BigEndian;
                    return false;
                case '_':
                    b.Conv = LittleEndian;
                    return false;
                case '%':
                    b.Conv = Native;
                    return false;

                case '!':
                    b.AlignValue = -1;
                    return false;

                case 'x':
                    b.Add(new byte[] { 0 });
                    return false;

                // Type Conversions
                case 'i':
                    b.Add(b.Conv.GetBytes(Convert.ToInt32(oarg, CultureInfo.InvariantCulture)));
                    break;

                case 'I':
                    b.Add(b.Conv.GetBytes(Convert.ToUInt32(oarg, CultureInfo.InvariantCulture)));
                    break;

                case 's':
                    b.Add(b.Conv.GetBytes(Convert.ToInt16(oarg, CultureInfo.InvariantCulture)));
                    break;

                case 'S':
                    b.Add(b.Conv.GetBytes(Convert.ToUInt16(oarg, CultureInfo.InvariantCulture)));
                    break;

                case 'l':
                    b.Add(b.Conv.GetBytes(Convert.ToInt64(oarg, CultureInfo.InvariantCulture)));
                    break;

                case 'L':
                    b.Add(b.Conv.GetBytes(Convert.ToUInt64(oarg, CultureInfo.InvariantCulture)));
                    break;

                case 'f':
                    b.Add(b.Conv.GetBytes(Convert.ToSingle(oarg, CultureInfo.InvariantCulture)));
                    break;

                case 'd':
                    b.Add(b.Conv.GetBytes(Convert.ToDouble(oarg, CultureInfo.InvariantCulture)));
                    break;

                case 'b':
                    b.Add(new[] { Convert.ToByte(oarg, CultureInfo.InvariantCulture) });
                    break;

                case 'c':
                    b.Add(new[] { (byte)(Convert.ToSByte(oarg, CultureInfo.InvariantCulture)) });
                    break;

                case 'C':
                    b.Add(new[] { Convert.ToByte(oarg, CultureInfo.InvariantCulture) });
                    break;

                // Repeat acount;
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    b.Repeat = ((short)b.Description[b.I]) - ((short)'0');
                    return false;

                case '*':
                    b.Repeat = Int32.MaxValue;
                    return false;

                case '[':
                    int count = -1, j;

                    for (j = b.I + 1; j < b.Description.Length; j++)
                    {
                        if (b.Description[j] == ']')
                            break;
                        n = ((short)b.Description[j]) - ((short)'0');
                        if (n >= 0 && n <= 9)
                        {
                            if (count == -1)
                                count = n;
                            else
                                count = count * 10 + n;
                        }
                    }
                    if (count == -1)
                        throw new ArgumentException("invalid size specification");
                    b.I = j;
                    b.Repeat = count;
                    return false;

                case '$':
                case 'z':
                    bool addNull = b.Description[b.I] == 'z';
                    b.I++;
                    if (b.I >= b.Description.Length)
                        throw new ArgumentException(Resources.Dollar_description_needs_a_type_specified, "b");
                    char d = b.Description[b.I];
                    Encoding e;

                    switch (d)
                    {
                        case '8':
                            e = Encoding.UTF8;
                            n = 1;
                            break;
                        case '6':
                            e = Encoding.Unicode;
                            n = 2;
                            break;
                        case '7':
                            e = Encoding.UTF7;
                            n = 1;
                            break;
                        case 'b':
                            e = Encoding.BigEndianUnicode;
                            n = 2;
                            break;
                        case '3':
                            e = Encoding.GetEncoding(12000);
                            n = 4;
                            break;
                        case '4':
                            e = Encoding.GetEncoding(12001);
                            n = 4;
                            break;

                        default:
                            throw new ArgumentException(Resources.Invalid_format_for_dollar_specifier, "b");
                    }
                    if (b.AlignValue == -1)
                        b.AlignValue = 4;
                    b.Add(e.GetBytes(Convert.ToString(oarg, CultureInfo.InvariantCulture)));
                    if (addNull)
                        b.Add(new byte[n]);
                    break;
                default:
                    throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "invalid format specified `{0}'", b.Description[b.I]));
            }
            return true;
        }

        static bool Prepare(byte[] buffer, ref int idx, int size, ref bool align)
        {
            if (align)
            {
                idx = Align(idx, size);
                align = false;
            }
            if (idx + size > buffer.Length)
            {
                idx = buffer.Length;
                return false;
            }
            return true;
        }

        static public IList Unpack(string description, byte[] buffer, int startIndex)
        {
            DataConverter conv = CopyConv;
            ArrayList result = new ArrayList();
            int idx = startIndex;
            bool align = false;
            int repeat = 0;

            for (int i = 0; i < description.Length && idx < buffer.Length; )
            {
                int save = i;

                int n;
                switch (description[i])
                {
                    case '^':
                        conv = BigEndian;
                        break;
                    case '_':
                        conv = LittleEndian;
                        break;
                    case '%':
                        conv = Native;
                        break;
                    case 'x':
                        idx++;
                        break;

                    case '!':
                        align = true;
                        break;

                    // Type Conversions
                    case 'i':
                        if (Prepare(buffer, ref idx, 4, ref align))
                        {
                            result.Add(conv.GetInt32(buffer, idx));
                            idx += 4;
                        }
                        break;

                    case 'I':
                        if (Prepare(buffer, ref idx, 4, ref align))
                        {
                            result.Add(conv.GetUInt32(buffer, idx));
                            idx += 4;
                        }
                        break;

                    case 's':
                        if (Prepare(buffer, ref idx, 2, ref align))
                        {
                            result.Add(conv.GetInt16(buffer, idx));
                            idx += 2;
                        }
                        break;

                    case 'S':
                        if (Prepare(buffer, ref idx, 2, ref align))
                        {
                            result.Add(conv.GetUInt16(buffer, idx));
                            idx += 2;
                        }
                        break;

                    case 'l':
                        if (Prepare(buffer, ref idx, 8, ref align))
                        {
                            result.Add(conv.GetInt64(buffer, idx));
                            idx += 8;
                        }
                        break;

                    case 'L':
                        if (Prepare(buffer, ref idx, 8, ref align))
                        {
                            result.Add(conv.GetUInt64(buffer, idx));
                            idx += 8;
                        }
                        break;

                    case 'f':
                        if (Prepare(buffer, ref idx, 4, ref align))
                        {
                            result.Add(conv.GetFloat(buffer, idx));
                            idx += 4;
                        }
                        break;

                    case 'd':
                        if (Prepare(buffer, ref idx, 8, ref align))
                        {
                            result.Add(conv.GetDouble(buffer, idx));
                            idx += 8;
                        }
                        break;

                    case 'b':
                        if (Prepare(buffer, ref idx, 1, ref align))
                        {
                            result.Add(buffer[idx]);
                            idx++;
                        }
                        break;

                    case 'c':
                    case 'C':
                        if (Prepare(buffer, ref idx, 1, ref align))
                        {
                            char c;

                            if (description[i] == 'c')
                                c = ((char)((sbyte)buffer[idx]));
                            else
                                c = ((char)buffer[idx]);

                            result.Add(c);
                            idx++;
                        }
                        break;

                    // Repeat acount;
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        repeat = ((short)description[i]) - ((short)'0');
                        save = i + 1;
                        break;

                    case '*':
                        repeat = Int32.MaxValue;
                        break;

                    case '[':
                        int count = -1, j;

                        for (j = i + 1; j < description.Length; j++)
                        {
                            if (description[j] == ']')
                                break;
                            n = ((short)description[j]) - ((short)'0');
                            if (n >= 0 && n <= 9)
                            {
                                if (count == -1)
                                    count = n;
                                else
                                    count = count * 10 + n;
                            }
                        }
                        if (count == -1)
                            throw new ArgumentException("invalid size specification");
                        i = j;
                        save = i + 1;
                        repeat = count;
                        break;

                    case '$':
                    case 'z':
                        // bool with_null = description [i] == 'z';
                        i++;
                        if (i >= description.Length)
                            throw new ArgumentException(Resources.Dollar_description_needs_a_type_specified, "description");
                        char d = description[i];
                        Encoding e;
                        if (align)
                        {
                            idx = Align(idx, 4);
                            align = false;
                        }
                        if (idx >= buffer.Length)
                            break;

                        switch (d)
                        {
                            case '8':
                                e = Encoding.UTF8;
                                n = 1;
                                break;
                            case '6':
                                e = Encoding.Unicode;
                                n = 2;
                                break;
                            case '7':
                                e = Encoding.UTF7;
                                n = 1;
                                break;
                            case 'b':
                                e = Encoding.BigEndianUnicode;
                                n = 2;
                                break;
                            case '3':
                                e = Encoding.GetEncoding(12000);
                                n = 4;
                                break;
                            case '4':
                                e = Encoding.GetEncoding(12001);
                                n = 4;
                                break;

                            default:
                                throw new ArgumentException(Resources.Invalid_format_for_dollar_specifier, "description");
                        }
                        int k = idx;
                        switch (n)
                        {
                            case 1:
                                for (; k < buffer.Length && buffer[k] != 0; k++)
#pragma warning disable 642 check if empty loop is really needed
                                    // ReSharper disable once EmptyEmbeddedStatement
                                    ;
#pragma warning restore 642
                                result.Add(e.GetChars(buffer, idx, k - idx));
                                if (k == buffer.Length)
                                    idx = k;
                                else
                                    idx = k + 1;
                                break;

                            case 2:
                                for (; k < buffer.Length; k++)
                                {
                                    if (k + 1 == buffer.Length)
                                    {
                                        k++;
                                        break;
                                    }
                                    if (buffer[k] == 0 && buffer[k + 1] == 0)
                                        break;
                                }
                                result.Add(e.GetChars(buffer, idx, k - idx));
                                if (k == buffer.Length)
                                    idx = k;
                                else
                                    idx = k + 2;
                                break;

                            case 4:
                                for (; k < buffer.Length; k++)
                                {
                                    if (k + 3 >= buffer.Length)
                                    {
                                        k = buffer.Length;
                                        break;
                                    }
                                    if (buffer[k] == 0 && buffer[k + 1] == 0 && buffer[k + 2] == 0 && buffer[k + 3] == 0)
                                        break;
                                }
                                result.Add(e.GetChars(buffer, idx, k - idx));
                                if (k == buffer.Length)
                                    idx = k;
                                else
                                    idx = k + 4;
                                break;
                        }
                        break;
                    default:
                        throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "invalid format specified `{0}'",
                                                description[i]));
                }

                if (repeat > 0)
                {
                    if (--repeat > 0)
                        i = save;
                }
                else
                    i++;
            }
            return result;
        }

        internal static void Check(byte[] dest, int destIdx, int size)
        {
            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            if (destIdx < 0 || destIdx > dest.Length - size)
            {
                throw new ArgumentException("destIdx");
            }
        }

        class CopyConverter : DataConverter
        {
            public override double GetDouble(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 8)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException("index");
                Contract.EndContractBlock();

                double ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 8; i++)
                    b[i] = data[index + i];

                return ret;
            }

            public override ulong GetUInt64(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 8)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException("index");
                Contract.EndContractBlock();

                ulong ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 8; i++)
                    b[i] = data[index + i];

                return ret;
            }

            public override long GetInt64(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 8)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException("index");
                Contract.EndContractBlock();

                long ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 8; i++)
                    b[i] = data[index + i];

                return ret;
            }

            public override float GetFloat(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 4)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException("index");
                Contract.EndContractBlock();

                float ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 4; i++)
                    b[i] = data[index + i];

                return ret;
            }

            public override int GetInt32(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 4)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException("index");
                Contract.EndContractBlock();

                int ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 4; i++)
                    b[i] = data[index + i];

                return ret;
            }

            public override uint GetUInt32(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 4)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException("index");
                Contract.EndContractBlock();

                uint ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 4; i++)
                    b[i] = data[index + i];

                return ret;
            }

            public override short GetInt16(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 2)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException("index");
                Contract.EndContractBlock();

                short ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 2; i++)
                    b[i] = data[index + i];

                return ret;
            }

            public override ushort GetUInt16(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 2)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException("index");
                Contract.EndContractBlock();

                ushort ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 2; i++)
                    b[i] = data[index + i];

                return ret;
            }

            public override void PutBytes(byte[] dest, int destIdx, double value)
            {
                Check(dest, destIdx, 8);
                fixed (byte* target = &dest[destIdx])
                {
                    long* source = (long*)&value;

                    *((long*)target) = *source;
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, float value)
            {
                Check(dest, destIdx, 4);
                fixed (byte* target = &dest[destIdx])
                {
                    uint* source = (uint*)&value;

                    *((uint*)target) = *source;
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, int value)
            {
                Check(dest, destIdx, 4);
                fixed (byte* target = &dest[destIdx])
                {
                    uint* source = (uint*)&value;

                    *((uint*)target) = *source;
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, uint value)
            {
                Check(dest, destIdx, 4);
                fixed (byte* target = &dest[destIdx])
                {
                    uint* source = &value;

                    *((uint*)target) = *source;
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, long value)
            {
                Check(dest, destIdx, 8);
                fixed (byte* target = &dest[destIdx])
                {
                    long* source = &value;

                    *((long*)target) = *source;
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, ulong value)
            {
                Check(dest, destIdx, 8);
                fixed (byte* target = &dest[destIdx])
                {
                    ulong* source = &value;

                    *((ulong*)target) = *source;
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, short value)
            {
                Check(dest, destIdx, 2);
                fixed (byte* target = &dest[destIdx])
                {
                    ushort* source = (ushort*)&value;

                    *((ushort*)target) = *source;
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, ushort value)
            {
                Check(dest, destIdx, 2);
                fixed (byte* target = &dest[destIdx])
                {
                    ushort* source = &value;

                    *((ushort*)target) = *source;
                }
            }
        }

        class SwapConverter : DataConverter
        {
            public override double GetDouble(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 8)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException(Resources.Index_must_be_at_least_zero, "index");
                Contract.EndContractBlock();

                double ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 8; i++)
                    b[7 - i] = data[index + i];

                return ret;
            }

            public override ulong GetUInt64(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 8)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException(Resources.Index_must_be_at_least_zero, "index");
                Contract.EndContractBlock();

                ulong ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 8; i++)
                    b[7 - i] = data[index + i];

                return ret;
            }

            public override long GetInt64(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 8)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException(Resources.Index_must_be_at_least_zero, "index");
                Contract.EndContractBlock();

                long ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 8; i++)
                    b[7 - i] = data[index + i];

                return ret;
            }

            public override float GetFloat(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 4)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException(Resources.Index_must_be_at_least_zero, "index");
                Contract.EndContractBlock();

                float ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 4; i++)
                    b[3 - i] = data[index + i];

                return ret;
            }

            public override int GetInt32(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 4)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException(Resources.Index_must_be_at_least_zero, "index");
                Contract.EndContractBlock();

                int ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 4; i++)
                    b[3 - i] = data[index + i];

                return ret;
            }

            public override uint GetUInt32(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 4)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException(Resources.Index_must_be_at_least_zero, "index");
                Contract.EndContractBlock();

                uint ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 4; i++)
                    b[3 - i] = data[index + i];

                return ret;
            }

            public override short GetInt16(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 2)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException(Resources.Index_must_be_at_least_zero, "index");
                Contract.EndContractBlock();

                short ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 2; i++)
                    b[1 - i] = data[index + i];

                return ret;
            }

            public override ushort GetUInt16(byte[] data, int index)
            {
                if (data == null)
                    throw new ArgumentNullException("data");
                if (data.Length - index < 2)
                    throw new ArgumentException("index");
                if (index < 0)
                    throw new ArgumentException(Resources.Index_must_be_at_least_zero, "index");
                Contract.EndContractBlock();

                ushort ret;
                byte* b = (byte*)&ret;

                for (int i = 0; i < 2; i++)
                    b[1 - i] = data[index + i];

                return ret;
            }

            public override void PutBytes(byte[] dest, int destIdx, double value)
            {
                Check(dest, destIdx, 8);

                fixed (byte* target = &dest[destIdx])
                {
                    byte* source = (byte*)&value;

                    for (int i = 0; i < 8; i++)
                        target[i] = source[7 - i];
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, float value)
            {
                Check(dest, destIdx, 4);

                fixed (byte* target = &dest[destIdx])
                {
                    byte* source = (byte*)&value;

                    for (int i = 0; i < 4; i++)
                        target[i] = source[3 - i];
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, int value)
            {
                Check(dest, destIdx, 4);

                fixed (byte* target = &dest[destIdx])
                {
                    byte* source = (byte*)&value;

                    for (int i = 0; i < 4; i++)
                        target[i] = source[3 - i];
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, uint value)
            {
                Check(dest, destIdx, 4);

                fixed (byte* target = &dest[destIdx])
                {
                    byte* source = (byte*)&value;

                    for (int i = 0; i < 4; i++)
                        target[i] = source[3 - i];
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, long value)
            {
                Check(dest, destIdx, 8);

                fixed (byte* target = &dest[destIdx])
                {
                    byte* source = (byte*)&value;

                    for (int i = 0; i < 8; i++)
                        target[i] = source[7 - i];
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, ulong value)
            {
                Check(dest, destIdx, 8);

                fixed (byte* target = &dest[destIdx])
                {
                    byte* source = (byte*)&value;

                    for (int i = 0; i < 8; i++)
                        target[i] = source[7 - i];
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, short value)
            {
                Check(dest, destIdx, 2);

                fixed (byte* target = &dest[destIdx])
                {
                    byte* source = (byte*)&value;

                    for (int i = 0; i < 2; i++)
                        target[i] = source[1 - i];
                }
            }

            public override void PutBytes(byte[] dest, int destIdx, ushort value)
            {
                Check(dest, destIdx, 2);

                fixed (byte* target = &dest[destIdx])
                {
                    byte* source = (byte*)&value;

                    for (int i = 0; i < 2; i++)
                        target[i] = source[1 - i];
                }
            }
        }

#if MONO_DATACONVERTER_STATIC_METHODS
        static unsafe void PutBytesLE (byte *dest, byte *src, int count)
        {
            int i = 0;
            
            if (BitConverter.IsLittleEndian){
                for (; i < count; i++)
                    *dest++ = *src++;
            } else {
                dest += count;
                for (; i < count; i++)
                    *(--dest) = *src++;
            }
        }

        static unsafe void PutBytesBE (byte *dest, byte *src, int count)
        {
            int i = 0;
            
            if (BitConverter.IsLittleEndian){
                dest += count;
                for (; i < count; i++)
                    *(--dest) = *src++;
            } else {
                for (; i < count; i++)
                    *dest++ = *src++;
            }
        }

        static unsafe void PutBytesNative (byte *dest, byte *src, int count)
        {
            int i = 0;
            
            for (; i < count; i++)
                dest [i-count] = *src++;
        }
        
        static public unsafe double DoubleFromLE (byte[] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 8)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            double ret;
            fixed (byte *src = &data[index]){
                PutBytesLE ((byte *) &ret, src, 8);
            }
            return ret;
        }

        static public unsafe float FloatFromLE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 4)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            float ret;
            fixed (byte *src = &data[index]){
                PutBytesLE ((byte *) &ret, src, 4);
            }
            return ret;
        }

        static public unsafe long Int64FromLE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 8)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            long ret;
            fixed (byte *src = &data[index]){
                PutBytesLE ((byte *) &ret, src, 8);
            }
            return ret;
        }
        
        static public unsafe ulong UInt64FromLE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 8)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            ulong ret;
            fixed (byte *src = &data[index]){
                PutBytesLE ((byte *) &ret, src, 8);
            }
            return ret;
        }

        static public unsafe int Int32FromLE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 4)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            int ret;
            fixed (byte *src = &data[index]){
                PutBytesLE ((byte *) &ret, src, 4);
            }
            return ret;
        }
        
        static public unsafe uint UInt32FromLE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 4)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            uint ret;
            fixed (byte *src = &data[index]){
                PutBytesLE ((byte *) &ret, src, 4);
            }
            return ret;
        }

        static public unsafe short Int16FromLE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 2)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");

            short ret;
            fixed (byte *src = &data[index]){
                PutBytesLE ((byte *) &ret, src, 2);
            }
            return ret;
        }
        
        static public unsafe ushort UInt16FromLE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 2)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            ushort ret;
            fixed (byte *src = &data[index]){
                PutBytesLE ((byte *) &ret, src, 2);
            }
            return ret;
        }

        static public unsafe double DoubleFromBE (byte[] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 8)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            double ret;
            fixed (byte *src = &data[index]){
                PutBytesBE ((byte *) &ret, src, 8);
            }
            return ret;
        }

        static public unsafe float FloatFromBE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 4)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            float ret;
            fixed (byte *src = &data[index]){
                PutBytesBE ((byte *) &ret, src, 4);
            }
            return ret;
        }

        static public unsafe long Int64FromBE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 8)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            long ret;
            fixed (byte *src = &data[index]){
                PutBytesBE ((byte *) &ret, src, 8);
            }
            return ret;
        }
        
        static public unsafe ulong UInt64FromBE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 8)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            ulong ret;
            fixed (byte *src = &data[index]){
                PutBytesBE ((byte *) &ret, src, 8);
            }
            return ret;
        }

        static public unsafe int Int32FromBE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 4)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            int ret;
            fixed (byte *src = &data[index]){
                PutBytesBE ((byte *) &ret, src, 4);
            }
            return ret;
        }
        
        static public unsafe uint UInt32FromBE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 4)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            uint ret;
            fixed (byte *src = &data[index]){
                PutBytesBE ((byte *) &ret, src, 4);
            }
            return ret;
        }

        static public unsafe short Int16FromBE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 2)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");

            short ret;
            fixed (byte *src = &data[index]){
                PutBytesBE ((byte *) &ret, src, 2);
            }
            return ret;
        }
        
        static public unsafe ushort UInt16FromBE (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 2)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            ushort ret;
            fixed (byte *src = &data[index]){
                PutBytesBE ((byte *) &ret, src, 2);
            }
            return ret;
        }

        static public unsafe double DoubleFromNative (byte[] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 8)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            double ret;
            fixed (byte *src = &data[index]){
                PutBytesNative ((byte *) &ret, src, 8);
            }
            return ret;
        }

        static public unsafe float FloatFromNative (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 4)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            float ret;
            fixed (byte *src = &data[index]){
                PutBytesNative ((byte *) &ret, src, 4);
            }
            return ret;
        }

        static public unsafe long Int64FromNative (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 8)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            long ret;
            fixed (byte *src = &data[index]){
                PutBytesNative ((byte *) &ret, src, 8);
            }
            return ret;
        }
        
        static public unsafe ulong UInt64FromNative (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 8)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            ulong ret;
            fixed (byte *src = &data[index]){
                PutBytesNative ((byte *) &ret, src, 8);
            }
            return ret;
        }

        static public unsafe int Int32FromNative (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 4)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            int ret;
            fixed (byte *src = &data[index]){
                PutBytesNative ((byte *) &ret, src, 4);
            }
            return ret;
        }
        
        static public unsafe uint UInt32FromNative (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 4)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            uint ret;
            fixed (byte *src = &data[index]){
                PutBytesNative ((byte *) &ret, src, 4);
            }
            return ret;
        }

        static public unsafe short Int16FromNative (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 2)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");

            short ret;
            fixed (byte *src = &data[index]){
                PutBytesNative ((byte *) &ret, src, 2);
            }
            return ret;
        }
        
        static public unsafe ushort UInt16FromNative (byte [] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException ("data");
            if (data.Length - index < 2)
                throw new ArgumentException ("index");
            if (index < 0)
                throw new ArgumentException ("index");
            
            ushort ret;
            fixed (byte *src = &data[index]){
                PutBytesNative ((byte *) &ret, src, 2);
            }
            return ret;
        }

                unsafe static byte[] GetBytesPtr (byte *ptr, int count)
                {
                        byte [] ret = new byte [count];

                        for (int i = 0; i < count; i++) {
                                ret [i] = ptr [i];
                        }

                        return ret;
                }

                unsafe static byte[] GetBytesSwap (bool swap, byte *ptr, int count)
                {
                        byte [] ret = new byte [count];

            if (swap){
                int t = count-1;
                for (int i = 0; i < count; i++) {
                    ret [t-i] = ptr [i];
                }
            } else {
                for (int i = 0; i < count; i++) {
                    ret [i] = ptr [i];
                }
            }
                        return ret;
                }
        
                unsafe public static byte[] GetBytesNative (bool value)
                {
                        return GetBytesPtr ((byte *) &value, 1);
                }

                unsafe public static byte[] GetBytesNative (char value)
                {
                        return GetBytesPtr ((byte *) &value, 2);
                }

                unsafe public static byte[] GetBytesNative (short value)
                {
                        return GetBytesPtr ((byte *) &value, 2);
                }

                unsafe public static byte[] GetBytesNative (int value)
                {
                        return GetBytesPtr ((byte *) &value, 4);
                }

                unsafe public static byte[] GetBytesNative (long value)
                {
                        return GetBytesPtr ((byte *) &value, 8);
                }

                [CLSCompliant (false)]
                unsafe public static byte[] GetBytesNative (ushort value)
                {
                        return GetBytesPtr ((byte *) &value, 2);
                }

                [CLSCompliant (false)]
                unsafe public static byte[] GetBytesNative (uint value)
                {
                        return GetBytesPtr ((byte *) &value, 4);
                }

                [CLSCompliant (false)]
                unsafe public static byte[] GetBytesNative (ulong value)
                {
                        return GetBytesPtr ((byte *) &value, 8);
                }

                unsafe public static byte[] GetBytesNative (float value)
                {
                        return GetBytesPtr ((byte *) &value, 4);
                }

                unsafe public static byte[] GetBytesNative (double value)
                {
            return GetBytesPtr ((byte *) &value, 8);
                }

                unsafe public static byte[] GetBytesLE (bool value)
                {
                        return GetBytesSwap (!BitConverter.IsLittleEndian, (byte *) &value, 1);
                }

                unsafe public static byte[] GetBytesLE (char value)
                {
                        return GetBytesSwap (!BitConverter.IsLittleEndian, (byte *) &value, 2);
                }

                unsafe public static byte[] GetBytesLE (short value)
                {
                        return GetBytesSwap (!BitConverter.IsLittleEndian, (byte *) &value, 2);
                }

                unsafe public static byte[] GetBytesLE (int value)
                {
                        return GetBytesSwap (!BitConverter.IsLittleEndian, (byte *) &value, 4);
                }

                unsafe public static byte[] GetBytesLE (long value)
                {
                        return GetBytesSwap (!BitConverter.IsLittleEndian, (byte *) &value, 8);
                }

                [CLSCompliant (false)]
                unsafe public static byte[] GetBytesLE (ushort value)
                {
                        return GetBytesSwap (!BitConverter.IsLittleEndian, (byte *) &value, 2);
                }

                [CLSCompliant (false)]
                unsafe public static byte[] GetBytesLE (uint value)
                {
                        return GetBytesSwap (!BitConverter.IsLittleEndian, (byte *) &value, 4);
                }

                [CLSCompliant (false)]
                unsafe public static byte[] GetBytesLE (ulong value)
                {
                        return GetBytesSwap (!BitConverter.IsLittleEndian, (byte *) &value, 8);
                }

                unsafe public static byte[] GetBytesLE (float value)
                {
                        return GetBytesSwap (!BitConverter.IsLittleEndian, (byte *) &value, 4);
                }

                unsafe public static byte[] GetBytesLE (double value)
                {
            return GetBytesSwap (!BitConverter.IsLittleEndian, (byte *) &value, 8);
                }
        
                unsafe public static byte[] GetBytesBE (bool value)
                {
                        return GetBytesSwap (BitConverter.IsLittleEndian, (byte *) &value, 1);
                }

                unsafe public static byte[] GetBytesBE (char value)
                {
                        return GetBytesSwap (BitConverter.IsLittleEndian, (byte *) &value, 2);
                }

                unsafe public static byte[] GetBytesBE (short value)
                {
                        return GetBytesSwap (BitConverter.IsLittleEndian, (byte *) &value, 2);
                }

                unsafe public static byte[] GetBytesBE (int value)
                {
                        return GetBytesSwap (BitConverter.IsLittleEndian, (byte *) &value, 4);
                }

                unsafe public static byte[] GetBytesBE (long value)
                {
                        return GetBytesSwap (BitConverter.IsLittleEndian, (byte *) &value, 8);
                }

                [CLSCompliant (false)]
                unsafe public static byte[] GetBytesBE (ushort value)
                {
                        return GetBytesSwap (BitConverter.IsLittleEndian, (byte *) &value, 2);
                }

                [CLSCompliant (false)]
                unsafe public static byte[] GetBytesBE (uint value)
                {
                        return GetBytesSwap (BitConverter.IsLittleEndian, (byte *) &value, 4);
                }

                [CLSCompliant (false)]
                unsafe public static byte[] GetBytesBE (ulong value)
                {
                        return GetBytesSwap (BitConverter.IsLittleEndian, (byte *) &value, 8);
                }

                unsafe public static byte[] GetBytesBE (float value)
                {
                        return GetBytesSwap (BitConverter.IsLittleEndian, (byte *) &value, 4);
                }

                unsafe public static byte[] GetBytesBE (double value)
                {
            return GetBytesSwap (BitConverter.IsLittleEndian, (byte *) &value, 8);
                }
#endif

    }
}