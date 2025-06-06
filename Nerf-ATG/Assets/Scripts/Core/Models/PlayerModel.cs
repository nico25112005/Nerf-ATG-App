using System;

public class PlayerModel : IPlayerModel
{
    public string Name { get; set; }
    public Guid Id { get;} = Guid.NewGuid();

}
