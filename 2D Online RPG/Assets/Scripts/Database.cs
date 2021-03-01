using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using UnityEngine;     
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System;

public partial class Database : MonoBehaviour
{
    // Singleton
    public static Database singleton;

    [SerializeField] private string accountsDatabaseName     = "mmo-accounts";    
    [SerializeField] private string statisticsDatabaseName   = "mmo-serverstatistics"; 
    [SerializeField] private string serverDatabaseName       = "mmo-nyzoren"; 
    IMongoDatabase accountsDatabase;
    IMongoDatabase serverDatabase;
    IMongoDatabase statisticsDatabase;
    MongoClient connection;
    IMongoCollection<character> characters;
    IMongoCollection<account> accounts; 
    
    public class account {
        public string id { get; set; }
        [BsonRequired] public string name { get; set; }
        [BsonRequired] public string password { get; set; }
        public DateTime created { get; set; }
        public DateTime lastlogin { get; set; }
        public bool banned { get; set; }
    }

    public class character {
        public string id { get; set; }
        [BsonRequired] public string server { get; set; }
        [BsonRequired] public string name { get; set; }
        [BsonRequired] public string account { get; set; }
        public string classname { get; set; }
        public int level { get; set; }
        public int experience { get; set; }

        public float x;
        public float y;
        public float z;
        
        public int health { get; set; }
        public int mana { get; set; }
        public int stamina { get; set; }
        public int strength { get; set; }
        public int intelligence { get; set; }
        public int agility { get; set; }

        public bool isGameMaster {get; set; }
        public bool online { get; set; } 
        public DateTime lastSaved { get; set; }
        public bool deleted { get; set; } = false;
    }

    [Header("Events")]
    // use onConnected to create an extra table for your addon
    public UnityEvent onConnected;
    public UnityEventPlayer onCharacterLoad;
    public UnityEventPlayer onCharacterSave;

    void Awake()
    {
        if(singleton == null) singleton = this;
    }

    public void Connect()
    {
        // Open a connection to the databases server
        connection = new MongoClient("mongodb://127.0.0.1:27017/?retryWrites=true&w=majority");
        
        // Get specific references to the 
        accountsDatabase    = connection.GetDatabase(accountsDatabaseName);;
        statisticsDatabase  = connection.GetDatabase(statisticsDatabaseName);;
        serverDatabase      = connection.GetDatabase(serverDatabaseName);;
        

        // Get a reference to specific collections and store them on easier to use variables:
        characters = serverDatabase.GetCollection<character>("characters");
        accounts = accountsDatabase.GetCollection<account>("accounts");

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
    
    public void TestLogin(string account)
    {
        var builder = Builders<account>.Filter;
        var filter = builder.Eq("name", account);
        
        
        //var acc = new account { name = account, created = DateTime.UtcNow, lastlogin = DateTime.Now, banned = false };
                
        //_accounts.InsertOne(acc);

        var result = accounts.Find(filter).ToList();
        Debug.Log("TestLogin has found matching: [" + result.Count + "]");
    }

    public bool TryLogin(string account, string password)
    {
        if (!string.IsNullOrWhiteSpace(account) && !string.IsNullOrWhiteSpace(password))
        {
            
            var filterBuilder = Builders<account>.Filter;

            // Filter: (Account Name & Not Banned)
            var filter = (filterBuilder.Eq("name", account) & filterBuilder.Eq("banned", false));

            // If the account doesn't exist, create it upon Login
            bool exists = (accounts.Find(filter).ToList().Count >= 1) ? true : false;
            Debug.Log("[TryLogin]> Does the account exists? [" + exists + "].");
            if(exists == false)
            {
                Debug.Log("[TryLogin]> ACCOUNT not found. Creating one.");
                var newAccount = new account { id = account,
                                               name = account,
                                               password = password,
                                               created = DateTime.UtcNow,
                                               lastlogin = DateTime.Now,
                                               banned = false };
                
                accounts.InsertOne(newAccount);
                
                Debug.Log("[TryLogin]> InsertOne: account named " + newAccount.name + ".");
                 
            }
            if(exists == true) Debug.Log("[TryLogin]> Account found -> authenticating...");

            // The query for existing accounts should also check if the password is correct.
            // Filter: (Account Name & Password & Not Banned)
            var existsFilter = (filterBuilder.Eq("name", account) & filterBuilder.Eq("password", password) & filterBuilder.Eq("banned", false));
            bool shouldAuth = (accounts.Find(filter).ToList().Count >= 1) ? true : false;

            if(!shouldAuth) Debug.Log("[TryLogin]> Information provided is incorrect. Should not authenticate.");
            if(shouldAuth)
            {
                Debug.Log("[TryLogin]> Information provided is correct, authenticated.");
                // If user information is correct and should be authenticated, we should
                // uptade it's lastLogin information to now.
                var updateFilter = filterBuilder.Eq("name", account);
                var updateValue = Builders<account>.Update.Set("lastlogin", DateTime.UtcNow);
                accounts.UpdateOne(updateFilter, updateValue);
                Debug.Log("[TryLogin]> Last Login time updated.");
                return true;
            }
        }
        return false;
    }

    public bool CharacterExists(string characterName)
    {
        var filter = Builders<character>.Filter.Eq("name", characterName);

        var result = characters.Find(filter).ToList();
        
        Debug.Log("Database -> CharacterExists: [" + result.Count + "]");

        if(result.Count >= 1) 
            return true;
        else return false;

    }

    // TO-DO
    public void CharacterDelete(string characterName){}

    public List<string> CharactersForAccount(string account)
    {        
        List<string> result = new List<string>();
               
        // Define our filterBuilder for character
        var filterBuilder = Builders<character>.Filter;
        
        // Define our filters
        var filter = filterBuilder.Eq("account", account) & filterBuilder.Eq("deleted", 0);

        // Get the result as a cursor so we can use ToEnumerable if many found:
        var cursor = characters.Find(filter).ToCursor();

        // Walk through the cursor so we can read our data and pass it into our List<string>
        foreach ( var document in cursor.ToEnumerable())
        {
            result.Add(document.name);
            Debug.Log("CharactersForAccount> Loading character: " + document.name);
        }

        return result;
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
        var filterBuilder = Builders<character>.Filter;
        var filter = filterBuilder.Eq("name", characterName) & filterBuilder.Eq("deleted", 0);
        
        var result = characters.Find(filter).ToList();

        if( result.Count >= 1 )
        {
            Debug.Log("CharacterLoad> Finding prefabs for character class");
            // instantiate based on class name
            Player prefab = prefabs.Find(p => p.name == result[0].classname);
            if (prefab != null)
            {
                Debug.Log("CharacterLoad> Loading db info into the character: "+ characterName);
                GameObject go = Instantiate(prefab.gameObject);
                Player player = go.GetComponent<Player>();

                player.name                 = characterName;
                player.entity.name          = characterName;
                player.account              = result[0].account;
                player.className            = result[0].classname;
                player.entity.level         = result[0].level;
                player.entity.strength      = result[0].strength;
                player.entity.agility       = result[0].agility;
                player.entity.intelligence  = result[0].intelligence;

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
                player.entity.curHealth     = result[0].health;
                player.entity.curMana       = result[0].mana;
                player.entity.curStamina    = result[0].stamina;

                Debug.Log("CharacterLoad> If isnt preview");
                if(!isPreview)
                {
                    Debug.Log("updating online value");
                    var updateValues = Builders<character>.Update.Set("online", true);
                    characters.UpdateOne(filter, updateValues);
                }

                // addon system hooks
                //onCharacterLoad.Invoke(player);

                return go;
            }
            else Debug.LogError("no prefab was found for class: " + result[0].classname);
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
    public void CharacterSave(Player player, bool online, bool useTransaction = true){
        Debug.Log("Database> CharacterSave");
        var newChar = new character { id = player.name,
                                      name = player.name,
                                      classname = player.className,
                                      account = player.account,
                                      x = player.transform.position.x,
                                      y = player.transform.position.y,
                                      z = player.transform.position.z,
                                      level = player.entity.level,
                                      health = player.entity.curHealth,
                                      mana = player.entity.curMana,
                                      stamina = player.entity.curStamina,
                                      strength = player.entity.strength,
                                      intelligence = player.entity.intelligence,
                                      //experience = player.experience.current,
                                      online = online,
                                      lastSaved = DateTime.UtcNow,
                                      isGameMaster = player.isGameMaster
                                      };
        
        var filter = Builders<character>.Filter.Eq("id", player.name);
        var options = new UpdateOptions { IsUpsert = true };
        
        Debug.Log("Database> Trying to replace.");
        var result = characters.ReplaceOne(filter, newChar, options);

        Debug.Log(result.ToString());
        // SaveInventory(player.inventory);
        // SaveEquipment((PlayerEquipment)player.equipment);
        // SaveItemCooldowns(player);
        // SaveSkills((PlayerSkills)player.skills);
        // SaveBuffs((PlayerSkills)player.skills);
        // SaveQuests(player.quests);
        // if (player.guild.InGuild())
        //     SaveGuild(player.guild.guild, false);

        // addon system hooks
        onCharacterSave.Invoke(player);
    }

    //TO-DO
    public void CharacterSaveMany(IEnumerable<Player> players, bool online = true){}

    //TO-DO
    // public bool guidExists(string guild)


}