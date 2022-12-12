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

    public class clsMessaggio : MonoBehaviour
    {

        public string ip;
        public UInt16 porta;
        public string messaggio;
        public string esito;

        public override string ToString()
        {
            // Ritorno del Metodo ToString() base
            // return base.ToString();

            return this.ip + " : " +
                this.porta.ToString() + " - " +
                this.messaggio +
                " ==>  " +
                this.esito;
        }

    }
