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

  public string Version { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
  public ArraySegment<byte>? GetVersionBytes() { return __p.__vector_as_arraysegment(4); }
  public ActiveConfiguration Active { get { int o = __p.__offset(6); return o != 0 ? (ActiveConfiguration)__p.bb.GetSbyte(o + __p.bb_pos) : ActiveConfiguration.XApiConfiguration; } }
  public XApiConfiguration? XBoxLiveConfig { get { int o = __p.__offset(8); return o != 0 ? (XApiConfiguration?)(new XApiConfiguration()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
  public SteamConfiguration? SteamConfig { get { int o = __p.__offset(10); return o != 0 ? (SteamConfiguration?)(new SteamConfiguration()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

  public static Offset<Configuration> CreateConfiguration(FlatBufferBuilder builder,
      StringOffset versionOffset = default(StringOffset),
      ActiveConfiguration active = ActiveConfiguration.XApiConfiguration,
      Offset<XApiConfiguration> xBoxLiveConfigOffset = default(Offset<XApiConfiguration>),
      Offset<SteamConfiguration> steamConfigOffset = default(Offset<SteamConfiguration>)) {
    builder.StartObject(4);
    Configuration.AddSteamConfig(builder, steamConfigOffset);
    Configuration.AddXBoxLiveConfig(builder, xBoxLiveConfigOffset);
    Configuration.AddVersion(builder, versionOffset);
    Configuration.AddActive(builder, active);
    return Configuration.EndConfiguration(builder);
  }

  public static void StartConfiguration(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddVersion(FlatBufferBuilder builder, StringOffset versionOffset) { builder.AddOffset(0, versionOffset.Value, 0); }
  public static void AddActive(FlatBufferBuilder builder, ActiveConfiguration active) { builder.AddSbyte(1, (sbyte)active, 0); }
  public static void AddXBoxLiveConfig(FlatBufferBuilder builder, Offset<XApiConfiguration> xBoxLiveConfigOffset) { builder.AddOffset(2, xBoxLiveConfigOffset.Value, 0); }
  public static void AddSteamConfig(FlatBufferBuilder builder, Offset<SteamConfiguration> steamConfigOffset) { builder.AddOffset(3, steamConfigOffset.Value, 0); }
  public static Offset<Configuration> EndConfiguration(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Configuration>(o);
  }
  public static void FinishConfigurationBuffer(FlatBufferBuilder builder, Offset<Configuration> offset) { builder.Finish(offset.Value); }
  public static void FinishSizePrefixedConfigurationBuffer(FlatBufferBuilder builder, Offset<Configuration> offset) { builder.FinishSizePrefixed(offset.Value); }
};


}