using System;

public class PlayerModel : IPlayerModel
{
    private static PlayerModel _instance;
    private static readonly object _lock = new object();

    private PlayerModel() { }

    public static PlayerModel Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new PlayerModel();
                }
                return _instance;
            }
        }
    }
    public string Name { get; set; }
    public Guid Id { get;} = Guid.NewGuid();

}
