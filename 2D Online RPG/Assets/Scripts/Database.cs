using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;     
using System.Collections.Generic;
using System;

public partial class Database : MonoBehaviour
{
    // Singleton
    public static Database singleton;

    private string dbName  = "onlinerpg";    
    IMongoDatabase database;

    MongoClient connection;
    
    class account {
        [BsonId]
        public string name { get; set; }
        public string password { get; set; }
        public DateTime created { get; set; }
        public DateTime lastlogin { get; set; }
        public bool banned { get; set; }
    }

    class character {
        [BsonId] public string name { get; set; }
        public string account { get; set; }
        public string classname { get; set; }
        public int level { get; set; }
        public int experience { get; set; }
        
        public int health { get; set; }
        public int mana { get; set; }
        public int stamina { get; set; }
        public int strength { get; set; }
        public int intelligence { get; set; }
        public int agility { get; set; }

        public bool online { get; set; } 
        public DateTime lastSaved { get; set; }
        public bool deleted { get; set; }
    }
    

    void Awake()
    {
        if(singleton == null) singleton = this;
    }

    public void Connect()
    {
        // open connection and create the db
        connection = new MongoClient("mongodb://127.0.0.1:27017/?retryWrites=true&w=majority");
        database = connection.GetDatabase(dbName);
        

        // create document Collections
        database.CreateCollection("accounts");
        database.CreateCollection("characters");

        //accounts = database.GetCollection<BsonDocument>("accounts");
        //characters = database.GetCollection<BsonDocument>("characters");
        

        // addon system hooks
        Utils.InvokeMany(typeof(Database), this, "Initialize_");
        Utils.InvokeMany(typeof(Database), this, "Connect_");

        Debug.Log("Connected to the database.");
    }

    void OnApplicationQuit()
    {
        Debug.Log("Database should disconnect.");
        //database.Close();
    }

    // TO-DO
    public bool TryLogin(string account, string password){return true;}

    // TO-DO
    public bool CharacterExists(string characterName){return true;}

    // TO-DO
    public void CharacterDelete(string characterName){}

    // TO-DO
    //public List<string> CharactersForAccount(string account){}
    
    public void TestSaveChar(string charName, string accName)
    {
        var collection = database.GetCollection<character>("characters");

        var document = new character {
            name = charName,
            account = accName,
            classname = "none",
            level = 1,
            experience = 0,
            health = 100,
            mana = 100,
            strength = 1,
            intelligence = 1,
            agility = 1,
            online = false,
            lastSaved = DateTime.Now,
            deleted = false
        };

        
        

        // var character = new BsonDocument
        // {
        //     { "name", charName },
        //     { "account", accName },
        //     { "level", 1 },
        //     { "experience", 0 },
        //     { "stats", 
        //         new BsonDocument {
        //             { "strength", 1 },
        //             { "intelligence", 1 },
        //             { "health", 100 },
        //             { "mana", 100 },
        //         }
        //     },
        //     { "online", false },
        //     { "lastSaved", DateTime.Now },
        //     { "deleted", false }
        // };

       collection.InsertOne(document);
       Debug.Log("Character " + charName + " was saved on the DB.");
    }

    // TO-DO
    void LoadInventory(PlayerInventory inventory){}

    // TO-DO
    void LoadEquipment(PlayerEquipment equipment){}

    // TO-DO
    void LoadItemCooldowns(Player player){}

    // TO-DO
    void LoadSkills(PlayerSkills player){}

    // TO-DO
    void LoadBuffs(PlayerSkills player){}

    // TO-DO
    void LoadQuests(PlayerQuests quests){}

    // Half-done
    public GameObject CharacterLoad(string characterName, List<Player> prefabs, bool isPreview)
    {
        var collection = database.GetCollection<character>("characters");
        var filterBuilder = Builders<character>.Filter;
        var filter = filterBuilder.Gt("name", characterName) & filterBuilder.Gt("deleted", 0);
        var document = collection.Find(filter).First();

        if( document != null )
        {

            // instantiate based on class name
            Player prefab = prefabs.Find(p => p.name == document.classname);
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab.gameObject);
                Player player= go.GetComponent<Player>();

                player.name                 = document.name;
                player.account              = document.account;
                player.className            = document.classname;
                player.entity.level         = document.level;
                player.entity.strength      = document.strength;
                player.entity.agility       = document.agility;
                player.entity.intelligence  = document.intelligence;

                // if SAVED player position 'isValidToSpawn'
                // { player spawns on saved position }
                // else
                // { player spawns on nearest spawn point position }

                // for now, i'll just spawn the player on spawn point 0,0
                player.transform.position = new Vector3(0, 0, 0);

                // Load player assets
                    // LoadInventory(player.inventory);
                    // LoadEquipment(player.equipment)
                    // LoadItemCooldowns(player);
                    // LoadSkills
                    // LoadBuffs
                    // LoadQUests
                    // LoadGuildOnDemand

                // Assign health/mana after max values were fully loaded
                // (max values depend on equipment, stats, etc)
                player.entity.curHealth     = document.health;
                player.entity.curMana       = document.mana;
                player.entity.curStamina    = document.stamina;

                if(!isPreview)
                {
                    var updateValues = Builders<character>.Update.Set("online", true);
                    collection.UpdateOne(filter, updateValues);
                }

                // addon system hooks
                //onCharacterLoad.Invoke(player);

                return go;
            }
            else Debug.LogError("no prefab was found for class: " + document.classname);
        }
        return null;
    
    }

    //TO-DO
    void SaveInventory(Player player){}
    
    //TO-DO
    void SaveEquipment(Player player){}

    //TO-DO
    void SaveSkills(Player player){}

    //TO-DO
    void SaveBuffs(Player player){}

    //TO-DO
    void SaveQuests(Player player){}

    //TO-DO   
    // public void CharacterSave(Player player, bool online, bool useTransaction = true)

    //TO-DO
    // public void CharacterSaveMany(IEnumerable<Player> players, bool online = true)

    //TO-DO
    // public bool guidExists(string guild)


}