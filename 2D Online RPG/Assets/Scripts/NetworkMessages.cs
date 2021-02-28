using System.Collections.Generic;
using System.Linq;
using Mirror;
using System;

public partial struct LoginMsg : NetworkMessage
{
    public string account;
    public string password;
    public string server;
    public string version;
}

public partial struct LoginSuccessMsg : NetworkMessage
{
    
}

public partial struct CharacterCreateMsg : NetworkMessage
{
    public string name;
    public string server;
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


public partial struct CharactersAvailableMsg : NetworkMessage
{
    public partial struct CharacterPreview
    {
        public string name;
        public string className; // prefab name
        public bool isGameMaster;
        //public ItemSlot[] equipment;
    }
    public CharacterPreview[] characters;

    // load method in this class so we can still modify the characters structs
    // in the addon hooks

    public void Load(List<Player> players)
    {
        characters = new CharacterPreview[players.Count];
        for ( int i = 0; i < players.Count; ++i )
        {
            Player player = players[i];
            characters[i] = new CharacterPreview
            {
                name = player.name,
                className = player.className,
                isGameMaster = false // = player.isGameMaster (to-do)
                // equipment = player.equipment.slots.ToArray()
            };

        }
        Utils.InvokeMany(typeof(CharactersAvailableMsg), this, "Load_", players);
    }


}