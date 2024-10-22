// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace World
{

using global::System;
using global::FlatBuffers;

public struct TileReference : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static TileReference GetRootAsTileReference(ByteBuffer _bb) { return GetRootAsTileReference(_bb, new TileReference()); }
  public static TileReference GetRootAsTileReference(ByteBuffer _bb, TileReference obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public TileReference __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string TileNames(int j) { int o = __p.__offset(4); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
  public int TileNamesLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<TileReference> CreateTileReference(FlatBufferBuilder builder,
      VectorOffset tileNamesOffset = default(VectorOffset)) {
    builder.StartObject(1);
    TileReference.AddTileNames(builder, tileNamesOffset);
    return TileReference.EndTileReference(builder);
  }

  public static void StartTileReference(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddTileNames(FlatBufferBuilder builder, VectorOffset tileNamesOffset) { builder.AddOffset(0, tileNamesOffset.Value, 0); }
  public static VectorOffset CreateTileNamesVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateTileNamesVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static void StartTileNamesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<TileReference> EndTileReference(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<TileReference>(o);
  }
  public static void FinishTileReferenceBuffer(FlatBufferBuilder builder, Offset<TileReference> offset) { builder.Finish(offset.Value); }
  public static void FinishSizePrefixedTileReferenceBuffer(FlatBufferBuilder builder, Offset<TileReference> offset) { builder.FinishSizePrefixed(offset.Value); }
};


}
