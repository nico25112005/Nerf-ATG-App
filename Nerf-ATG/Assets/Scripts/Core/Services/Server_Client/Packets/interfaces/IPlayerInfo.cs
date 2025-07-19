public interface IPlayerInfo
{
    string PlayerId { get; }
    string Name { get; }
    byte Index { get; }
    PacketAction Action { get; set; }
}
