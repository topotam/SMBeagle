﻿using SMBeagle.ShareDiscovery;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace SMBeagle.HostDiscovery
{
    class Host
    {
        public const int PORT_MAX_WAIT_MS = 500;
        public string Address { get; set; }
        public bool SMBAvailable { get { return _SMBAvailable; } }
        private bool _SMBAvailable { get; set; }
        public List<Share> Shares { get; set; } = new();

        public List<string> UNCPaths { 
            get 
            {
                return Shares
                        .Select(share => $@"\\{Address}\{share.Name}\")
                        .ToList();
            }
        }

        public int ShareCount { get { return Shares.Count; } }
        public Host(string address)
        {
            Address = address;
        }

        public void TestSMB()
        {
            _SMBAvailable = HostRespondsToTCP445();
        }

        bool HostRespondsToTCP445()
        {
            using TcpClient t = new();

            try
            {
                if (t.ConnectAsync(Address, 445).Wait(PORT_MAX_WAIT_MS))
                    return true; // We connected

                return false; // We timedout
            }
            catch
            {
                return false; // We hit an error
            }
        }

        public override string ToString()
        {
            return Address;
        }
    }
}
