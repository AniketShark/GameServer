using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netcode.Common
{
    public enum EventType
    {
        None = 0,
        RegisterPlayer = 1,
        Input = 3,
        Custom = 4,
        UpdateLobby = 5
    }
}
