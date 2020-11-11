using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBlo
{
    public struct ByteArray
    {
        public byte[] bytes;

        public ByteArray(byte[] bytes)
        {
            this.bytes = bytes;
        }
    }

    public static class Buffer
    {
        public static List<ByteArray> SplitBuffer(byte[] buffer, byte splitter)
        {
            List<ByteArray> arrays = new List<ByteArray>();
            int lastIndex = 0;
            int inc;
            int x;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (i == buffer.Length - 1 && lastIndex != 0)
                {
                    byte[] newbuffer = new byte[buffer.Length - 1 - lastIndex];
                    for (int j = lastIndex + 1, k = 0; j < buffer.Length; j++, k++)
                    {
                        newbuffer[k] = buffer[j];
                    }
                    arrays.Add(new ByteArray(newbuffer));

                }
                else if (i == buffer.Length - 1 && lastIndex == 0)
                {
                    arrays.Add(new ByteArray(buffer));
                }
                else if (buffer[i] == splitter)
                {
                    inc = lastIndex == 0 ? 0 : 1;
                    x = i - inc - lastIndex;

                    byte[] newbuffer = new byte[x];
                    for (int j = 0, y = lastIndex == 0 ? lastIndex : lastIndex + 1; j < newbuffer.Length; j++, y++)
                    {
                        newbuffer[j] = buffer[y];
                    }

                    arrays.Add(new ByteArray(newbuffer));

                    lastIndex = i;
                }
            }

            return arrays;
        }

        public static bool HasSplitter(byte[] sourceBuffer, byte splitter)
        {
            for(int i = 0; i < sourceBuffer.Length; i++)
            {
                if (sourceBuffer[i] == splitter) return true;
            }
            return false;
        }

        public static byte[] AddStartCode(byte[] sourceBuffer, byte code)
        {
            byte[] newbuffer = new byte[sourceBuffer.Length + 1];
            newbuffer[0] = code;
            for(int i = 0, j = 1; i < sourceBuffer.Length; i++, j++)
            {
                newbuffer[j] = sourceBuffer[i];
            }
            return newbuffer;
        }

        public static byte[] ChangeBufferSize(byte[] sourceBuffer, int startIndex, int length)
        {
            byte[] newbuffer = new byte[length];
            for(int i = startIndex, j = 0; j < length; i++, j++)
            {
                newbuffer[j] = sourceBuffer[i];
            }
            return newbuffer;
        }

        public static byte[] AddSplitter(byte[] buffer, byte splitter)
        {
            byte[] newbuffer = new byte[buffer.Length + 1];

            for (int i = 0; i < buffer.Length; i++)
            {
                newbuffer[i] = buffer[i];
            }

            newbuffer[newbuffer.Length - 1] = splitter;

            return newbuffer;
        }

        public static byte[] RemoveEndSplitter(byte[] buffer)
        {

            byte[] newbuffer = new byte[buffer.Length - 1];
            for(int i = 0; i < newbuffer.Length; i++)
            {
                newbuffer[i] = buffer[i];
            }
            return newbuffer;
        }

        public static byte[] CombineBuffers(byte[] sourceBuffer, byte[] addedBuffer) { return CombineBuffers(sourceBuffer, 0, addedBuffer, 0, addedBuffer.Length); }
        public static byte[] CombineBuffers(byte[] sourceBuffer, byte[] addedBuffer, int addedBufferSize) { return CombineBuffers(sourceBuffer, 0, addedBuffer, 0, addedBufferSize); }
        public static byte[] CombineBuffers(byte[] sourceBuffer, int sourceBufferStartPos, byte[] addedBuffer, int addedBufferStartPos, int addedBufferSize)
        {
            int newbufferSize = sourceBuffer.Length + addedBufferSize - sourceBufferStartPos - addedBufferStartPos;
            byte[] newbuffer = new byte[newbufferSize];
            for (int i = 0, j = sourceBufferStartPos; j < sourceBuffer.Length; i++, j++)
            {
                newbuffer[i] = sourceBuffer[j];
            }
            for (int i = sourceBuffer.Length == 0 ? sourceBuffer.Length : sourceBuffer.Length - sourceBufferStartPos, j = addedBufferStartPos; j < addedBufferSize; i++, j++)
            {
                newbuffer[i] = addedBuffer[j];
            }
            return newbuffer;
        }

        public static void CombineBuffers(ref byte[] sourceBuffer, byte[] addedBuffer) { CombineBuffers(ref sourceBuffer, addedBuffer, addedBuffer.Length); }

        public static void CombineBuffers(ref byte[] sourceBuffer, byte[] addedBuffer, int addedBufferSize)
        {
            int newbufferSize = sourceBuffer.Length + addedBufferSize;
            byte[] newbuffer = new byte[newbufferSize];
            for (int i = 0; i < sourceBuffer.Length; i++)
            {
                newbuffer[i] = sourceBuffer[i];
            }
            for (int i = sourceBuffer.Length; i < newbufferSize; i++)
            {
                newbuffer[i] = addedBuffer[i - sourceBuffer.Length];
            }
            sourceBuffer = newbuffer;
        }
    }
}
