using System.Collections.Generic;
using System.Linq;
using Mirror;
using System;

public partial class ErrorMsg : NetworkMessage
{
    public uint netId;
    public string text;
    public bool causesDisconnect;
}