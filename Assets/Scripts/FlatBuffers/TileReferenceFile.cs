using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Boo.Lang.Runtime;
using World;

namespace FlatBuffers
{
    public class TileReferenceFile
    {
        private static readonly string path = "Assets/Resources/landscape/tileReference.rrtd";
        private readonly List<string> namesToAdd;
        private TileReference? tileReference;
        private Dictionary<string, ushort> stringToId;
        
        private static readonly Lazy<TileReferenceFile> ReferenceFile = new Lazy<TileReferenceFile>(() => new TileReferenceFile());

        public bool IsEditMode
        {
            get => stringToId != null;
            set
            {
                if (value)
                {
                    if (!IsEditMode)
                    {
                        stringToId = new Dictionary<string, ushort>();
                    }
                }
                else
                    stringToId = null;
            }
        }

        private TileReferenceFile()
        {
            namesToAdd = new List<string>();

            var bytes = File.Exists(path) ? File.ReadAllBytes(path) : new byte[200];
            
            var byteBuffer = new ByteBuffer(bytes);
            tileReference = TileReference.GetRootAsTileReference(byteBuffer);
        }

        public static TileReferenceFile Instance => ReferenceFile.Value;

        public string GetTileName(int refIndex)
        {
            if (!tileReference.HasValue) throw new RuntimeException("TileReferenceFileNotFound");

            return tileReference.Value.TileNames(refIndex);
        }

        public ushort GetRefOf(string tileName)
        {
            if (!tileReference.HasValue) throw new RuntimeException("TileReferenceFileNotFound");

            if (IsEditMode && stringToId.ContainsKey(tileName))
            {
                return stringToId[tileName];
            }
            
            var tileReferenceVal = tileReference.Value;
            for (ushort i = 0; i < tileReferenceVal.TileNamesLength; i++)
            {
                var name = tileReferenceVal.TileNames(i);

                if (tileName == name)
                {
                    return Cache(tileName, i);
                }
            }

            var findIndex = namesToAdd.FindIndex(name => name.Equals(tileName));

            if (findIndex < 0)
            {
                namesToAdd.Add(tileName);
                findIndex = namesToAdd.Count -1;

                if (namesToAdd.Count > 100)
                {
                    Save();
                }    
            }

            return Cache(tileName, (ushort) (tileReferenceVal.TileNamesLength + findIndex));
        }

        private ushort Cache(string tileName, ushort index)
        {
            if (IsEditMode)
            {
                stringToId.Add(tileName, index);
            }

            return index;
        }

        public void Save()
        {
            if (!tileReference.HasValue) throw new RuntimeException("TileReferenceFileNotFound");

            var newTilesCount = namesToAdd.Count;
            if (newTilesCount == 0) return;
            
            var builder = new FlatBufferBuilder(1024);
            
            var mergedData = MergeData(builder);

            var reference = TileReference.CreateTileReference(builder,
                TileReference.CreateTileNamesVector(builder, mergedData));
            
            builder.Finish(reference.Value);
            
            File.WriteAllBytes(path, builder.DataBuffer.ToSizedArray());
            tileReference = TileReference.GetRootAsTileReference(builder.DataBuffer);
            namesToAdd.Clear();
        }

        private StringOffset[] MergeData(FlatBufferBuilder builder)
        {
            var currentNames = new List<StringOffset>();

            var reference = tileReference.Value;
            
            for (var i = 0; i < reference.TileNamesLength; i++)
            {
                currentNames.Add(builder.CreateString(reference.TileNames(i)));
            }

            currentNames.AddRange(namesToAdd.Select(builder.CreateString));
            
            return currentNames.ToArray();
        }
    }
}