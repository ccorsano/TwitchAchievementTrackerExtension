// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace TwitchAchievementTracker
{

using global::System;
using global::Google.FlatBuffers;

public struct Configuration : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static Configuration GetRootAsConfiguration(ByteBuffer _bb) { return GetRootAsConfiguration(_bb, new Configuration()); }
  public static Configuration GetRootAsConfiguration(ByteBuffer _bb, Configuration obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public Configuration __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string XApiKey { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
  public ArraySegment<byte>? GetXApiKeyBytes() { return __p.__vector_as_arraysegment(4); }
  public ulong StreamerXuid { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
  public uint TitleId { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
  public string Locale { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
  public ArraySegment<byte>? GetLocaleBytes() { return __p.__vector_as_arraysegment(10); }

  public static Offset<Configuration> CreateConfiguration(FlatBufferBuilder builder,
      StringOffset xApiKeyOffset = default(StringOffset),
      ulong streamerXuid = 0,
      uint titleId = 0,
      StringOffset localeOffset = default(StringOffset)) {
    builder.StartObject(4);
    Configuration.AddStreamerXuid(builder, streamerXuid);
    Configuration.AddLocale(builder, localeOffset);
    Configuration.AddTitleId(builder, titleId);
    Configuration.AddXApiKey(builder, xApiKeyOffset);
    return Configuration.EndConfiguration(builder);
  }

  public static void StartConfiguration(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddXApiKey(FlatBufferBuilder builder, StringOffset xApiKeyOffset) { builder.AddOffset(0, xApiKeyOffset.Value, 0); }
  public static void AddStreamerXuid(FlatBufferBuilder builder, ulong streamerXuid) { builder.AddUlong(1, streamerXuid, 0); }
  public static void AddTitleId(FlatBufferBuilder builder, uint titleId) { builder.AddUint(2, titleId, 0); }
  public static void AddLocale(FlatBufferBuilder builder, StringOffset localeOffset) { builder.AddOffset(3, localeOffset.Value, 0); }
  public static Offset<Configuration> EndConfiguration(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Configuration>(o);
  }
  public static void FinishConfigurationBuffer(FlatBufferBuilder builder, Offset<Configuration> offset) { builder.Finish(offset.Value); }
  public static void FinishSizePrefixedConfigurationBuffer(FlatBufferBuilder builder, Offset<Configuration> offset) { builder.FinishSizePrefixed(offset.Value); }
};


}