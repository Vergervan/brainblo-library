using System;
using System.Collections.Generic;
using System.Text;

namespace BrainBlo.NewNetwork
{
    public class FileReceiveEventArgs : EventArgs
    {
        public int FileSize { get; }
        public string FileName { get; }
        public FileReceiveEventArgs(int fileSize, string fileName)
        {
            FileSize = fileSize;
            FileName = fileName;
        }
    }
}
