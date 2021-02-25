using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;     
using System.Collections.Generic;

public partial class Database : MonoBehaviour
{
    // 
    public static Database singleton;

    private string databaseIp    = "";
    private string databasePort  = "";
    private string dbName  = "onlinerpg";    
    IMongoDatabase database;

    MongoClient connection;

    void Awake()
    {
        if(singleton == null) singleton = this;
    }

    public void Connect()
    {
        // open connection and create the db
        connection = new MongoClient("mongodb://127.0.0.1:27017/?retryWrites=true&w=majority");
        database = connection.GetDatabase(dbName);
        

        // create documents
        IMongoCollection<BsonDocument> accounts = database.GetCollection<BsonDocument>("accounts");
        IMongoCollection<BsonDocument> characters = database.GetCollection<BsonDocument>("characters");
        

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
        return true;
    }

    public bool CharacterExists(string characterName)
    {
        return true;
    }

    public void CharacterDelete(string characterName)
    {

    }

    public List<string> CharactersForAccount(string account)
    {

    }

    void LoadInventory(Player player)
    {

    }

    void LoadEquipment(Player player)
    {

    }

    // Load Skills

    // Load buffs

    // Load Quests

    public GameObject CharacterLoad(string characterName, List<Player> prefabs, bool isPreview)
    {

    }

    // Save Inventory

    // Save Equipment

    // Save Skills

    // Save Buffs

    // Save Quests

    // Character Save

    // Character Save Many

    // public bool guidExists(string guild)


}