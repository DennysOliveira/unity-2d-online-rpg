using System.Collections.Generic;
using System.Linq;
using Mirror;
using System;

public partial struct LoginMsg : NetworkMessage
{
    public string account;
    public string password;
    public string version;
}

public partial struct CharacterCreateMsg : NetworkMessage
{
    public string name;
    public int classIndex;
    public bool gameMaster;
}

public partial struct CharacterSelectMsg : NetworkMessage
{
    public int index;
}

public partial struct CharacterDeleteMsg : NetworkMessage
{
    public int index;
}

public partial struct ErrorMsg : NetworkMessage
{
    public string text;
    public bool causesDisconnect;
}

