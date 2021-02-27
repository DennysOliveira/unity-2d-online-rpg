using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Mirror;

public class NetworkAuthenticatorMMO : NetworkAuthenticator
{
    [Header("Components")]
    public MyNetworkManager manager;

    // login info for the local player
    // we don't just name it 'account' to avoid collisions in handshake
    [Header("Login")]
    public string loginAccount = "";
    public string loginPassword = "";

    [Header("Security")]
    public string passwordSalt = "at_least_16_byte";
    public int accountMaxLength = 16;

    // CLIENT ~
    public override void OnStartClient()
    {
        // register login success message, allowed before authenticated
        NetworkClient.RegisterHandler<LoginSuccessMsg>(OnClientLoginSuccess, false);
    }

    public override void OnClientAuthenticate(NetworkConnection conn)
    {
        // we send login information with hashed password
        //
        // it is recommended to use a different salt for each hash.
        // ideally, we would store each user salt in the db.
        // this model will use account name as salt
        // 
        string hash = Utils.PBKDF2Hash(loginPassword, passwordSalt + loginAccount);
        LoginMsg message = new LoginMsg{account=loginAccount, password=hash, version=Application.version};
        conn.Send(message);
        Debug.Log("Login message was sent");

        // set state    
        manager.state = NetworkState.Handshake;
    }

    void OnClientLoginSuccess(NetworkConnection conn, LoginSuccessMsg msg)
    {
        // authenticated successfully. OnClientConnected will be called.
        OnClientAuthenticated.Invoke(conn);
    }

    // SERVER ~~~~~~~~~
    public override void OnStartServer()
    {
        // register login message, allowed before authenticated
        NetworkServer.RegisterHandler<LoginMsg>(OnServerLogin, false);
    }

    public override void OnServerAuthenticate(NetworkConnection conn)
    {
        // wait for LoginMsg from client
    }

    public bool IsAllowedAccountName(string account)
    {
        return account.Length <= accountMaxLength &&
               Regex.IsMatch(account, @"^[a-zA-Z0-9_]+$");
    }

    bool AccountLoggedIn(string account)
    {
        // in lobby or in world:
        return manager.lobby.ContainsValue(account) || Player.onlinePlayers.Values.Any(p => p.account == account);
    }

    void OnServerLogin(NetworkConnection conn, LoginMsg message)
    {
        // check version
        if(message.version == Application.version)
        {
            // allowed account name?
            if (IsAllowedAccountName(message.account))
            {
                // validate account info
                if(Database.singleton.TryLogin(message.account, message.password))
                {
                    // not in lobby and not in world yet?
                    if (!AccountLoggedIn(message.account))
                    {
                        // add to logged in accounts
                        manager.lobby[conn] = message.account;

                        // notify the client about the successful login
                        conn.Send(new LoginSuccessMsg());

                        // authenticate on server
                        OnServerAuthenticated.Invoke(conn);
                    }
                    else
                    {
                        Debug.Log("NetworkAuthenticatorMMO => AccountLoggedIn: " + message.account);
                        manager.ServerSendError(conn, "already logged in", true);

                        // note: we should disconnect the client here, but we can't as
                        // long as unity has no "SendAllAndThenDisconnect" function,
                        // because then the error message would never be sent.
                        //conn.Disconnect();
                    }
                }
                else
                {
                    Debug.Log("Invalid account or password for acc " + message.account);
                    manager.ServerSendError(conn, "invalid account", true);
                }
            }
            else
            {
                manager.ServerSendError(conn, "account name not allowed", true);
            }
        }
        else
        {
            manager.ServerSendError(conn, "outdated version", true);
        }
    }

}
