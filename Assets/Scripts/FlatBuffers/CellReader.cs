using System.IO;
using World;

namespace FlatBuffers
{
    public class CellReader
    {
        public static Cell Read(string cellPath)
        {
            var readAllBytes = File.ReadAllBytes(cellPath);

            var byteBuffer = new ByteBuffer(readAllBytes);

            return Cell.GetRootAsCell(byteBuffer);
        }
    }
}