using System.IO;
using World;

namespace FlatBuffers
{
    public class CellReader
    {
        public static CellWrapper Read(string cellPath)
        {
            var readAllBytes = File.ReadAllBytes(cellPath);

            var byteBuffer = new ByteBuffer(readAllBytes);

            var rootAsCell = Cell.GetRootAsCell(byteBuffer);

            return new CellWrapper(ref rootAsCell);
        }
    }
}