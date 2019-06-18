using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using FlatBuffers;
using UnityEngine;
using World;
using Vector3 = World.Vector3;

namespace Utils
{
    public static class CellUtils
    {
        private static readonly string Suffix = "rrtd";
        private static readonly Func<string, string> Path = name => $"Assets/Resources/landscape/{name}.{Suffix}";
        private static readonly Regex Reg = new Regex(@"x(\-*\d+)y(\-*\d+)z(\-*\d+)");
        
        public static Vector3Int GetWorldCellCenter(string cellName)
        {
            var groups = Reg.Matches(cellName)[0].Groups;

            var cellSideSize = Configs.CellSideSize;
            var halfSize = cellSideSize / 2;
            return new Vector3Int(
                int.Parse(groups[1].Value) * cellSideSize + halfSize,
                int.Parse(groups[2].Value) * cellSideSize + halfSize,
                int.Parse(groups[3].Value) * cellSideSize + halfSize
            );
        }
        
        public static Vector3Int GetWorldCellPosition(string cellName)
        {
            var groups = Reg.Matches(cellName)[0].Groups;

            var cellSideSize = Configs.CellSideSize;

            return new Vector3Int(
                int.Parse(groups[1].Value) * cellSideSize,
                int.Parse(groups[2].Value) * cellSideSize,
                int.Parse(groups[3].Value) * cellSideSize
            );
        }

        public static string CreateCellPath(Vector3Int position)
        {
            return Path(CreateCellName(position));
        }
        
        public static string CreateCellPath(string cellName)
        {
            return Path(cellName);
        }

        public static string CreateCellName(Vector3Int position)
        {
            return $"x{position.x}y{position.y}z{position.z}";
        }

        public static bool CellExists(string path)
        {
            return File.Exists(path);
        }

        public static CellWrapper LoadCellByName(string cellName)
        {
            var cellPath = CreateCellPath(cellName);

            if (!CellExists(cellPath)) return null;

            var cell = CellReader.Read(cellPath);
            cell.Name = cellName;
            cell.Path = cellPath;

            return cell;
        }
    }
}