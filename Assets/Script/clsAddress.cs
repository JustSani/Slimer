using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class clsAddress : MonoBehaviour
    {

        public static IPAddress[] ipVett;

        /***************/
        /* Costruttore */
        /***************/
        static clsAddress()
        {
            ipVett = new IPAddress[10];
            for (int I = 0; I < 10; I++)
                ipVett[I] = null;

        }

        /* Cerca gli Indirizzi IP presenti e li carica nel Vettore ipVett */
        public static void cercaIP()
        {
            int cont = 0;
            /*
             * GetHostName  = restituisce il nome del Host locale;
             * GetHostEntry = restituisce tutte le Info di Rete dell'Host locale;
             *                può anche essere usato per recuperare le Info 
             *                di un Host di Rete;
             */
            IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());

            /* 
             * Scorro tutta la Lista delle Info legate al mio Host
             * e recupero solo gli Indirizzi IP v4
            */
            foreach(IPAddress ip in hostInfo.AddressList)
            {
                /* Considero SOLO gli Indirizzi IP V4*/
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    ipVett[cont++] = ip;
            }

            /* Aggiungo anche il 127.0.0.1 */
            ipVett[cont++] = cercaIP("127.0.0.1");

        }

        /* 
         * Cerca l' Indirizzo IP dell' Host ricevuto in input 
         * (Nome DNS o IP alfaNumerico)
         */
        public static IPAddress cercaIP(string hostInput)
        {
            IPAddress ip = null;

            if (!(IPAddress.TryParse(hostInput, out ip)))
            {
                IPHostEntry hostInfo = Dns.GetHostEntry(hostInput);
                foreach (IPAddress ipInput in hostInfo.AddressList)
                {
                    /* Considero SOLO gli Indirizzi IP V4*/
                    if (ipInput.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ip = ipInput;
                        break;
                    }
                }
            }
            return ip;

        }

    }
