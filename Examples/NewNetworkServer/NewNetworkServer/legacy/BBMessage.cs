// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: BBMessage.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from BBMessage.proto</summary>
public static partial class BBMessageReflection {

  #region Descriptor
  /// <summary>File descriptor for BBMessage.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static BBMessageReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Cg9CQk1lc3NhZ2UucHJvdG8i7gIKCUJCTWVzc2FnZRIrCgttZXNzYWdlVHlw",
          "ZRgBIAEoDjIWLkJCTWVzc2FnZS5NZXNzYWdlVHlwZRIpCgpwbGF5ZXJJbmZv",
          "GAIgASgLMhUuQkJNZXNzYWdlLlBsYXllckluZm8SKgoLcGxheWVySW5mb3MY",
          "AyADKAsyFS5CQk1lc3NhZ2UuUGxheWVySW5mbxITCgttZXNzYWdlVGV4dBgE",
          "IAEoCRoqCgdWZWN0b3IzEgkKAXgYASABKAISCQoBeRgCIAEoAhIJCgF6GAMg",
          "ASgCGmYKClBsYXllckluZm8SDAoEbmFtZRgBIAEoCRIkCghwb3NpdGlvbhgC",
          "IAEoCzISLkJCTWVzc2FnZS5WZWN0b3IzEiQKCHJvdGF0aW9uGAMgASgLMhIu",
          "QkJNZXNzYWdlLlZlY3RvcjMiNAoLTWVzc2FnZVR5cGUSCwoHQ29ubmVjdBAA",
          "EggKBFRleHQQARIOCgpQbGF5ZXJEYXRhEAJiBnByb3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::BBMessage), global::BBMessage.Parser, new[]{ "MessageType", "PlayerInfo", "PlayerInfos", "MessageText" }, null, new[]{ typeof(global::BBMessage.Types.MessageType) }, null, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::BBMessage.Types.Vector3), global::BBMessage.Types.Vector3.Parser, new[]{ "X", "Y", "Z" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::BBMessage.Types.PlayerInfo), global::BBMessage.Types.PlayerInfo.Parser, new[]{ "Name", "Position", "Rotation" }, null, null, null, null)})
        }));
  }
  #endregion

}
#region Messages
public sealed partial class BBMessage : pb::IMessage<BBMessage>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<BBMessage> _parser = new pb::MessageParser<BBMessage>(() => new BBMessage());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<BBMessage> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::BBMessageReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public BBMessage() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public BBMessage(BBMessage other) : this() {
    messageType_ = other.messageType_;
    playerInfo_ = other.playerInfo_ != null ? other.playerInfo_.Clone() : null;
    playerInfos_ = other.playerInfos_.Clone();
    messageText_ = other.messageText_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public BBMessage Clone() {
    return new BBMessage(this);
  }

  /// <summary>Field number for the "messageType" field.</summary>
  public const int MessageTypeFieldNumber = 1;
  private global::BBMessage.Types.MessageType messageType_ = global::BBMessage.Types.MessageType.Connect;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::BBMessage.Types.MessageType MessageType {
    get { return messageType_; }
    set {
      messageType_ = value;
    }
  }

  /// <summary>Field number for the "playerInfo" field.</summary>
  public const int PlayerInfoFieldNumber = 2;
  private global::BBMessage.Types.PlayerInfo playerInfo_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::BBMessage.Types.PlayerInfo PlayerInfo {
    get { return playerInfo_; }
    set {
      playerInfo_ = value;
    }
  }

  /// <summary>Field number for the "playerInfos" field.</summary>
  public const int PlayerInfosFieldNumber = 3;
  private static readonly pb::FieldCodec<global::BBMessage.Types.PlayerInfo> _repeated_playerInfos_codec
      = pb::FieldCodec.ForMessage(26, global::BBMessage.Types.PlayerInfo.Parser);
  private readonly pbc::RepeatedField<global::BBMessage.Types.PlayerInfo> playerInfos_ = new pbc::RepeatedField<global::BBMessage.Types.PlayerInfo>();
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public pbc::RepeatedField<global::BBMessage.Types.PlayerInfo> PlayerInfos {
    get { return playerInfos_; }
  }

  /// <summary>Field number for the "messageText" field.</summary>
  public const int MessageTextFieldNumber = 4;
  private string messageText_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string MessageText {
    get { return messageText_; }
    set {
      messageText_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as BBMessage);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(BBMessage other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (MessageType != other.MessageType) return false;
    if (!object.Equals(PlayerInfo, other.PlayerInfo)) return false;
    if(!playerInfos_.Equals(other.playerInfos_)) return false;
    if (MessageText != other.MessageText) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (MessageType != global::BBMessage.Types.MessageType.Connect) hash ^= MessageType.GetHashCode();
    if (playerInfo_ != null) hash ^= PlayerInfo.GetHashCode();
    hash ^= playerInfos_.GetHashCode();
    if (MessageText.Length != 0) hash ^= MessageText.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (MessageType != global::BBMessage.Types.MessageType.Connect) {
      output.WriteRawTag(8);
      output.WriteEnum((int) MessageType);
    }
    if (playerInfo_ != null) {
      output.WriteRawTag(18);
      output.WriteMessage(PlayerInfo);
    }
    playerInfos_.WriteTo(output, _repeated_playerInfos_codec);
    if (MessageText.Length != 0) {
      output.WriteRawTag(34);
      output.WriteString(MessageText);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (MessageType != global::BBMessage.Types.MessageType.Connect) {
      output.WriteRawTag(8);
      output.WriteEnum((int) MessageType);
    }
    if (playerInfo_ != null) {
      output.WriteRawTag(18);
      output.WriteMessage(PlayerInfo);
    }
    playerInfos_.WriteTo(ref output, _repeated_playerInfos_codec);
    if (MessageText.Length != 0) {
      output.WriteRawTag(34);
      output.WriteString(MessageText);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (MessageType != global::BBMessage.Types.MessageType.Connect) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) MessageType);
    }
    if (playerInfo_ != null) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(PlayerInfo);
    }
    size += playerInfos_.CalculateSize(_repeated_playerInfos_codec);
    if (MessageText.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(MessageText);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(BBMessage other) {
    if (other == null) {
      return;
    }
    if (other.MessageType != global::BBMessage.Types.MessageType.Connect) {
      MessageType = other.MessageType;
    }
    if (other.playerInfo_ != null) {
      if (playerInfo_ == null) {
        PlayerInfo = new global::BBMessage.Types.PlayerInfo();
      }
      PlayerInfo.MergeFrom(other.PlayerInfo);
    }
    playerInfos_.Add(other.playerInfos_);
    if (other.MessageText.Length != 0) {
      MessageText = other.MessageText;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          MessageType = (global::BBMessage.Types.MessageType) input.ReadEnum();
          break;
        }
        case 18: {
          if (playerInfo_ == null) {
            PlayerInfo = new global::BBMessage.Types.PlayerInfo();
          }
          input.ReadMessage(PlayerInfo);
          break;
        }
        case 26: {
          playerInfos_.AddEntriesFrom(input, _repeated_playerInfos_codec);
          break;
        }
        case 34: {
          MessageText = input.ReadString();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 8: {
          MessageType = (global::BBMessage.Types.MessageType) input.ReadEnum();
          break;
        }
        case 18: {
          if (playerInfo_ == null) {
            PlayerInfo = new global::BBMessage.Types.PlayerInfo();
          }
          input.ReadMessage(PlayerInfo);
          break;
        }
        case 26: {
          playerInfos_.AddEntriesFrom(ref input, _repeated_playerInfos_codec);
          break;
        }
        case 34: {
          MessageText = input.ReadString();
          break;
        }
      }
    }
  }
  #endif

  #region Nested types
  /// <summary>Container for nested types declared in the BBMessage message type.</summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static partial class Types {
    public enum MessageType {
      [pbr::OriginalName("Connect")] Connect = 0,
      [pbr::OriginalName("Text")] Text = 1,
      [pbr::OriginalName("PlayerData")] PlayerData = 2,
    }

    public sealed partial class Vector3 : pb::IMessage<Vector3>
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        , pb::IBufferMessage
    #endif
    {
      private static readonly pb::MessageParser<Vector3> _parser = new pb::MessageParser<Vector3>(() => new Vector3());
      private pb::UnknownFieldSet _unknownFields;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static pb::MessageParser<Vector3> Parser { get { return _parser; } }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static pbr::MessageDescriptor Descriptor {
        get { return global::BBMessage.Descriptor.NestedTypes[0]; }
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public Vector3() {
        OnConstruction();
      }

      partial void OnConstruction();

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public Vector3(Vector3 other) : this() {
        x_ = other.x_;
        y_ = other.y_;
        z_ = other.z_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public Vector3 Clone() {
        return new Vector3(this);
      }

      /// <summary>Field number for the "x" field.</summary>
      public const int XFieldNumber = 1;
      private float x_;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public float X {
        get { return x_; }
        set {
          x_ = value;
        }
      }

      /// <summary>Field number for the "y" field.</summary>
      public const int YFieldNumber = 2;
      private float y_;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public float Y {
        get { return y_; }
        set {
          y_ = value;
        }
      }

      /// <summary>Field number for the "z" field.</summary>
      public const int ZFieldNumber = 3;
      private float z_;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public float Z {
        get { return z_; }
        set {
          z_ = value;
        }
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override bool Equals(object other) {
        return Equals(other as Vector3);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public bool Equals(Vector3 other) {
        if (ReferenceEquals(other, null)) {
          return false;
        }
        if (ReferenceEquals(other, this)) {
          return true;
        }
        if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(X, other.X)) return false;
        if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Y, other.Y)) return false;
        if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Z, other.Z)) return false;
        return Equals(_unknownFields, other._unknownFields);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override int GetHashCode() {
        int hash = 1;
        if (X != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(X);
        if (Y != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Y);
        if (Z != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Z);
        if (_unknownFields != null) {
          hash ^= _unknownFields.GetHashCode();
        }
        return hash;
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override string ToString() {
        return pb::JsonFormatter.ToDiagnosticString(this);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public void WriteTo(pb::CodedOutputStream output) {
      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        output.WriteRawMessage(this);
      #else
        if (X != 0F) {
          output.WriteRawTag(13);
          output.WriteFloat(X);
        }
        if (Y != 0F) {
          output.WriteRawTag(21);
          output.WriteFloat(Y);
        }
        if (Z != 0F) {
          output.WriteRawTag(29);
          output.WriteFloat(Z);
        }
        if (_unknownFields != null) {
          _unknownFields.WriteTo(output);
        }
      #endif
      }

      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
        if (X != 0F) {
          output.WriteRawTag(13);
          output.WriteFloat(X);
        }
        if (Y != 0F) {
          output.WriteRawTag(21);
          output.WriteFloat(Y);
        }
        if (Z != 0F) {
          output.WriteRawTag(29);
          output.WriteFloat(Z);
        }
        if (_unknownFields != null) {
          _unknownFields.WriteTo(ref output);
        }
      }
      #endif

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public int CalculateSize() {
        int size = 0;
        if (X != 0F) {
          size += 1 + 4;
        }
        if (Y != 0F) {
          size += 1 + 4;
        }
        if (Z != 0F) {
          size += 1 + 4;
        }
        if (_unknownFields != null) {
          size += _unknownFields.CalculateSize();
        }
        return size;
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public void MergeFrom(Vector3 other) {
        if (other == null) {
          return;
        }
        if (other.X != 0F) {
          X = other.X;
        }
        if (other.Y != 0F) {
          Y = other.Y;
        }
        if (other.Z != 0F) {
          Z = other.Z;
        }
        _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public void MergeFrom(pb::CodedInputStream input) {
      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        input.ReadRawMessage(this);
      #else
        uint tag;
        while ((tag = input.ReadTag()) != 0) {
          switch(tag) {
            default:
              _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
              break;
            case 13: {
              X = input.ReadFloat();
              break;
            }
            case 21: {
              Y = input.ReadFloat();
              break;
            }
            case 29: {
              Z = input.ReadFloat();
              break;
            }
          }
        }
      #endif
      }

      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
        uint tag;
        while ((tag = input.ReadTag()) != 0) {
          switch(tag) {
            default:
              _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
              break;
            case 13: {
              X = input.ReadFloat();
              break;
            }
            case 21: {
              Y = input.ReadFloat();
              break;
            }
            case 29: {
              Z = input.ReadFloat();
              break;
            }
          }
        }
      }
      #endif

    }

    public sealed partial class PlayerInfo : pb::IMessage<PlayerInfo>
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        , pb::IBufferMessage
    #endif
    {
      private static readonly pb::MessageParser<PlayerInfo> _parser = new pb::MessageParser<PlayerInfo>(() => new PlayerInfo());
      private pb::UnknownFieldSet _unknownFields;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static pb::MessageParser<PlayerInfo> Parser { get { return _parser; } }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public static pbr::MessageDescriptor Descriptor {
        get { return global::BBMessage.Descriptor.NestedTypes[1]; }
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public PlayerInfo() {
        OnConstruction();
      }

      partial void OnConstruction();

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public PlayerInfo(PlayerInfo other) : this() {
        name_ = other.name_;
        position_ = other.position_ != null ? other.position_.Clone() : null;
        rotation_ = other.rotation_ != null ? other.rotation_.Clone() : null;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public PlayerInfo Clone() {
        return new PlayerInfo(this);
      }

      /// <summary>Field number for the "name" field.</summary>
      public const int NameFieldNumber = 1;
      private string name_ = "";
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public string Name {
        get { return name_; }
        set {
          name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
      }

      /// <summary>Field number for the "position" field.</summary>
      public const int PositionFieldNumber = 2;
      private global::BBMessage.Types.Vector3 position_;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public global::BBMessage.Types.Vector3 Position {
        get { return position_; }
        set {
          position_ = value;
        }
      }

      /// <summary>Field number for the "rotation" field.</summary>
      public const int RotationFieldNumber = 3;
      private global::BBMessage.Types.Vector3 rotation_;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public global::BBMessage.Types.Vector3 Rotation {
        get { return rotation_; }
        set {
          rotation_ = value;
        }
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override bool Equals(object other) {
        return Equals(other as PlayerInfo);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public bool Equals(PlayerInfo other) {
        if (ReferenceEquals(other, null)) {
          return false;
        }
        if (ReferenceEquals(other, this)) {
          return true;
        }
        if (Name != other.Name) return false;
        if (!object.Equals(Position, other.Position)) return false;
        if (!object.Equals(Rotation, other.Rotation)) return false;
        return Equals(_unknownFields, other._unknownFields);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override int GetHashCode() {
        int hash = 1;
        if (Name.Length != 0) hash ^= Name.GetHashCode();
        if (position_ != null) hash ^= Position.GetHashCode();
        if (rotation_ != null) hash ^= Rotation.GetHashCode();
        if (_unknownFields != null) {
          hash ^= _unknownFields.GetHashCode();
        }
        return hash;
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public override string ToString() {
        return pb::JsonFormatter.ToDiagnosticString(this);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public void WriteTo(pb::CodedOutputStream output) {
      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        output.WriteRawMessage(this);
      #else
        if (Name.Length != 0) {
          output.WriteRawTag(10);
          output.WriteString(Name);
        }
        if (position_ != null) {
          output.WriteRawTag(18);
          output.WriteMessage(Position);
        }
        if (rotation_ != null) {
          output.WriteRawTag(26);
          output.WriteMessage(Rotation);
        }
        if (_unknownFields != null) {
          _unknownFields.WriteTo(output);
        }
      #endif
      }

      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
        if (Name.Length != 0) {
          output.WriteRawTag(10);
          output.WriteString(Name);
        }
        if (position_ != null) {
          output.WriteRawTag(18);
          output.WriteMessage(Position);
        }
        if (rotation_ != null) {
          output.WriteRawTag(26);
          output.WriteMessage(Rotation);
        }
        if (_unknownFields != null) {
          _unknownFields.WriteTo(ref output);
        }
      }
      #endif

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public int CalculateSize() {
        int size = 0;
        if (Name.Length != 0) {
          size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
        }
        if (position_ != null) {
          size += 1 + pb::CodedOutputStream.ComputeMessageSize(Position);
        }
        if (rotation_ != null) {
          size += 1 + pb::CodedOutputStream.ComputeMessageSize(Rotation);
        }
        if (_unknownFields != null) {
          size += _unknownFields.CalculateSize();
        }
        return size;
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public void MergeFrom(PlayerInfo other) {
        if (other == null) {
          return;
        }
        if (other.Name.Length != 0) {
          Name = other.Name;
        }
        if (other.position_ != null) {
          if (position_ == null) {
            Position = new global::BBMessage.Types.Vector3();
          }
          Position.MergeFrom(other.Position);
        }
        if (other.rotation_ != null) {
          if (rotation_ == null) {
            Rotation = new global::BBMessage.Types.Vector3();
          }
          Rotation.MergeFrom(other.Rotation);
        }
        _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      public void MergeFrom(pb::CodedInputStream input) {
      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        input.ReadRawMessage(this);
      #else
        uint tag;
        while ((tag = input.ReadTag()) != 0) {
          switch(tag) {
            default:
              _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
              break;
            case 10: {
              Name = input.ReadString();
              break;
            }
            case 18: {
              if (position_ == null) {
                Position = new global::BBMessage.Types.Vector3();
              }
              input.ReadMessage(Position);
              break;
            }
            case 26: {
              if (rotation_ == null) {
                Rotation = new global::BBMessage.Types.Vector3();
              }
              input.ReadMessage(Rotation);
              break;
            }
          }
        }
      #endif
      }

      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
        uint tag;
        while ((tag = input.ReadTag()) != 0) {
          switch(tag) {
            default:
              _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
              break;
            case 10: {
              Name = input.ReadString();
              break;
            }
            case 18: {
              if (position_ == null) {
                Position = new global::BBMessage.Types.Vector3();
              }
              input.ReadMessage(Position);
              break;
            }
            case 26: {
              if (rotation_ == null) {
                Rotation = new global::BBMessage.Types.Vector3();
              }
              input.ReadMessage(Rotation);
              break;
            }
          }
        }
      }
      #endif

    }

  }
  #endregion

}

#endregion


#endregion Designer generated code
