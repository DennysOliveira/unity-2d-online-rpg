using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization;
using UnityEngine;     
using System.Collections.Generic;
using System.Collections;
using System;

public partial class Database : MonoBehaviour
{
    // Singleton
    public static Database singleton;

    private string dbName  = "onlinerpg1";    
    IMongoDatabase database;

    MongoClient connection;
    
    public class account : BsonDocument {
        [BsonId]
        public string name { get; set; }
        public string password { get; set; }
        public DateTime created { get; set; }
        public DateTime lastlogin { get; set; }
        public bool banned { get; set; }
    }

    public class character : BsonDocument {
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
        database.GetCollection<character>("characters");
        database.GetCollection<account>("accounts");

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

    public bool TryLogin(string account, string password)
    {
        if (!string.IsNullOrWhiteSpace(account) && !string.IsNullOrWhiteSpace(password))
        {
            var collection = database.GetCollection<account>("accounts");
            var builder = Builders<account>.Filter;

            // Filter: Name = Account && Banned = False
            var filter = (builder.Eq("name", account) & builder.Eq("banned", false));

            // If the account doesn't exist, create it upon Login
            Debug.Log(collection.Find(filter));
            if(collection.Find(filter) == null)
            {
                Debug.Log("passing throuhg acc definition");
                var acc = new account { { "name", account }, 
                                            { "password", password },
                                            { "created", DateTime.UtcNow },
                                            { "lastlogin", DateTime.Now },
                                            { "banned", false } };

                collection.UpdateOne(filter, acc, new UpdateOptions{ IsUpsert = true });
                Debug.Log(acc.name);
            }

            // check account name, password and banned status
            var existsFilter = (builder.Eq("name", account) & builder.Eq("password", password) & builder.Eq("banned", false));
            if(collection.Find(existsFilter) != null)
            {
                
                // save last login date and return true
                var updateFilter = builder.Eq("name", account);
                var updateValue = Builders<account>.Update.Set("lastlogin", DateTime.UtcNow);
                collection.UpdateOne(updateFilter, updateValue);
                return true;
            }
        }
        return false;
    }

    public bool CharacterExists(string characterName)
    {
        var collection = database.GetCollection<character>("characters");

        var filter = Builders<character>.Filter.Eq("name", characterName);

        var result = collection.Find(filter);
        Debug.Log(result);

        if(result != null) return true;
        else return false;

    }

    // TO-DO
    public void CharacterDelete(string characterName){}

    public List<string> CharactersForAccount(string account)
    {        
        List<string> result = new List<string>();

        // Select which collection to use
        var collection = database.GetCollection<character>("characters");
        
        // Define our filterBuilder for character
        var filterBuilder = Builders<character>.Filter;
        
        // Define our filters
        var filter = filterBuilder.Eq("account", account) & filterBuilder.Eq("deleted", 0);
  
        // Walk through the cursor so we can read our data and pass it into our List<string>
        var cursor = collection.Find(filter).ToCursor();
        foreach ( var document in cursor.ToEnumerable())
        {
            result.Add(document.ToString());
        }

        return result;
    }
    
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
        var filter = filterBuilder.Eq("name", characterName) & filterBuilder.Eq("deleted", 0);
        
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
    public void CharacterSave(Player player, bool online, bool useTransaction = true){}

    //TO-DO
    public void CharacterSaveMany(IEnumerable<Player> players, bool online = true){}

    //TO-DO
    // public bool guidExists(string guild)


}