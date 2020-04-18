using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace exiii.Unity
{
    public class NetworkChecker : MonoBehaviour
    {
        [SerializeField]
        private List<string> IPAddress = new List<string>();

        // Use this for initialization
        void Start()
        {
            foreach (NetworkInterface network in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (network.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 && network.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
                {
                    continue;
                }

                foreach (UnicastIPAddressInformation ip in network.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        IPAddress.Add(ip.Address.ToString());
                    }
                }
            }
        }
    }
}