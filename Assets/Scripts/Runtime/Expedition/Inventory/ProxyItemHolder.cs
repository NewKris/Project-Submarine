using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Inventory {
    public class ProxyItemHolder : NetworkBehaviourExtended {
        public ProxyItem[] thirdPersonProxyItems;
        public ProxyItem[] firstPersonProxyItems;

        [Rpc(SendTo.Everyone)]
        public void ShowProxyRpc(int id) {
            HideAllItems();

            if (IsOwner) {
                firstPersonProxyItems.First(x => x.id == id).gameObject.SetActive(true);
            }
            else {
                thirdPersonProxyItems.First(x => x.id == id).gameObject.SetActive(true);
            }
        }

        [Rpc(SendTo.Everyone)]
        public void HideProxyRpc() {
            HideAllItems();
        }

        private void Start() {
            HideAllItems();
        }

        private void HideAllItems() {
            foreach (ProxyItem proxyItem in thirdPersonProxyItems) {
                proxyItem.gameObject.SetActive(false);
            }

            foreach (ProxyItem proxyItem in firstPersonProxyItems) {
                proxyItem.gameObject.SetActive(false);
            }
        }
    }
}