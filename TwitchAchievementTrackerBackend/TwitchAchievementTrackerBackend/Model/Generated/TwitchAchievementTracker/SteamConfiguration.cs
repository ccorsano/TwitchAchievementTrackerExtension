// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace TwitchAchievementTracker
{

using global::System;
using global::Google.FlatBuffers;

public struct SteamConfiguration : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static SteamConfiguration GetRootAsSteamConfiguration(ByteBuffer _bb) { return GetRootAsSteamConfiguration(_bb, new SteamConfiguration()); }
  public static SteamConfiguration GetRootAsSteamConfiguration(ByteBuffer _bb, SteamConfiguration obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public SteamConfiguration __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string WebApiKey { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
  public ArraySegment<byte>? GetWebApiKeyBytes() { return __p.__vector_as_arraysegment(4); }
  public uint AppId { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
  public ulong SteamId { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetUlong(o + __p.bb_pos) : (ulong)0; } }
  public string Locale { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
  public ArraySegment<byte>? GetLocaleBytes() { return __p.__vector_as_arraysegment(10); }

  public static Offset<SteamConfiguration> CreateSteamConfiguration(FlatBufferBuilder builder,
      StringOffset webApiKeyOffset = default(StringOffset),
      uint appId = 0,
      ulong steamId = 0,
      StringOffset localeOffset = default(StringOffset)) {
    builder.StartObject(4);
    SteamConfiguration.AddSteamId(builder, steamId);
    SteamConfiguration.AddLocale(builder, localeOffset);
    SteamConfiguration.AddAppId(builder, appId);
    SteamConfiguration.AddWebApiKey(builder, webApiKeyOffset);
    return SteamConfiguration.EndSteamConfiguration(builder);
  }

  public static void StartSteamConfiguration(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddWebApiKey(FlatBufferBuilder builder, StringOffset webApiKeyOffset) { builder.AddOffset(0, webApiKeyOffset.Value, 0); }
  public static void AddAppId(FlatBufferBuilder builder, uint appId) { builder.AddUint(1, appId, 0); }
  public static void AddSteamId(FlatBufferBuilder builder, ulong steamId) { builder.AddUlong(2, steamId, 0); }
  public static void AddLocale(FlatBufferBuilder builder, StringOffset localeOffset) { builder.AddOffset(3, localeOffset.Value, 0); }
  public static Offset<SteamConfiguration> EndSteamConfiguration(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SteamConfiguration>(o);
  }
};


}