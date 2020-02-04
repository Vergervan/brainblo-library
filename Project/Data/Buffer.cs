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
                if (buffer[i] == splitter)
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

                if(i == buffer.Length-1 && buffer[i] != splitter)
                {
                    arrays.Add(new ByteArray(buffer));
                }
            }

            return arrays;
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

        public static byte[] CombineBuffers(byte[] sourceBuffer, byte[] addedBuffer)
        {
            int newbufferSize = sourceBuffer.Length + addedBuffer.Length;
            byte[] newbuffer = new byte[newbufferSize];
            for(int i = 0; i < sourceBuffer.Length; i++)
            {
                newbuffer[i] = sourceBuffer[i];
            }
            for(int i = sourceBuffer.Length; i < newbufferSize; i++)
            {
                newbuffer[i] = addedBuffer[i - sourceBuffer.Length];
            }
            return newbuffer;
        }
    }
}
