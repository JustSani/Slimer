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

    // Definisco la firma della Procedura di Evento datiRicevutiEvent
    public delegate void datiRicevutiEventHandler(clsMessaggio Msg);

    public class clsSocket : MonoBehaviour
    {

        private bool server;
        public static int maxETH = 1460;
        private Socket socketID;
        private EndPoint binary;
        private Thread threadAscolta;
        // (1) Aggiunta TCP
        private Socket ConnID;

        // Definisco il Puntatore a Funzione legato all' Evento datiRicevutiEvent
        public event datiRicevutiEventHandler datiRicevutiEvent;

        /***************/
        /* Costruttore */
        /***************/
        public clsSocket(bool inTSocket, int porta, IPAddress ip)
        {

            /*
            * inTSocket = true ==> SEVER
            * inTSocket = fasle ==> CLIENT
            */
            server = inTSocket;

            // (2) - Aggiunta TCP
            socketID = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Unspecified);
            /*
            * 1° Parametro = indica l'insieme dei Protocolli a cui il Socket fa riferimento (IP V4);
            * 
            * 2° Parametro = indica il Tipo di Protocollo DGram (non Connesso) o Stream (Connesso);
            *
            * 3° Parametro = indica il Singolo protocollo di Connessione (non deve essere specificato per Protocollo DGram)
            */

            binary = new IPEndPoint(ip, porta);
            /*
            * Per il SERVER rappresenta l' endPoint di Ascolto;
            * Per il CLIENT rappresenta l' endPoint del Server al quale si deve collegare;
            */

            /* Nel caso del SERVER lo metto in ascolto sul Binary */
            if (server)
            {
                socketID.Bind(binary);
                // (3) - Aggiunta TCP
                // Gestisco il Numero MASSIMO di Connessione Contemporanee
                socketID.Listen(25);
            }
                

        }

        /***********************************************/
        /* Procedura di Ascolto per ricezione dei Dati */
        /***********************************************/
        private void serverRicevi()
        {
            int nByteRicevuti;
            string strMsg;
            byte[] bufferRX;
            bufferRX = new byte[maxETH];

            while(true)
            {
                // (4) - Aggiunta TCP

                // Il TCP Invia/Riceve i Dati sulla Connessione restituita da Accept();
                ConnID = socketID.Accept();

                // Ricevo i Dati nel Buffer dalla Connessione recuperata dall'Accept();
                nByteRicevuti = ConnID.Receive(bufferRX, maxETH, 0);
                                               
                // Converto il Vettore di Byte (BufferRX) in una Stringa 
                strMsg = Encoding.ASCII.GetString(bufferRX, 0, nByteRicevuti);

                // Preparo il Messaggio ricevuto 
                clsMessaggio Messaggio = new clsMessaggio();
                Messaggio.messaggio = strMsg;
                // (5) - Aggiunta TCP
                Messaggio.ip = ((IPEndPoint)ConnID.RemoteEndPoint).Address.ToString();
                Messaggio.porta = Convert.ToUInt16(((IPEndPoint)ConnID.RemoteEndPoint).Port);

                /* Genero l' Evento per Visuaizzare i Dati Ricevuti */
                datiRicevutiEvent(Messaggio);

            }

        }

        /**********************************************************************/
        /* Avvia la Procedura di Ascolto (serverRicevi) su un Thread separato */
        /**********************************************************************/
        public void avviaServer()
        {
            // Instazio il mio Nuovo Thread
            threadAscolta = new Thread(serverRicevi);

            // Avvio il Thread
            threadAscolta.Start();

            // Aspetto finchè il Theard nonè avviato
            while (!(threadAscolta.IsAlive));

            if (threadAscolta == null)
            {
                // Avvio il Thread
                threadAscolta.Start();

                // Aspetto finchè il Theard nonè avviato
                while (!(threadAscolta.IsAlive)) ;
            }
            else if (threadAscolta.ThreadState == ThreadState.SuspendRequested)
                threadAscolta.Resume();
        }

        /***************************************************************/
        /* Arresta il Thread con la procedura di Asolto (serverRicevi) */
        /***************************************************************/
        public void arrestaServer()
        {
            if (threadAscolta.ThreadState == ThreadState.Running)
            {
                threadAscolta.Suspend();
            }
        }

        /************************************************/
        /* Restituisce il Messaggio ricevuto sul Client */
        /************************************************/
        public clsMessaggio clientRicevi()
        {
            int nByteRicevuti;
            string strMsg;
            byte[] bufferRX;
            bufferRX = new byte[maxETH];

            /* Ricevo i Dati nel Buffer e salvo le Coordinate del Server in Binary */
            nByteRicevuti = socketID.ReceiveFrom(bufferRX, maxETH, 0, ref binary);

            /* Converto il Vettore di Byte (BufferRX) in una Stringa */
            strMsg = Encoding.ASCII.GetString(bufferRX, 0, nByteRicevuti);

            /* Preparo il Messaggio ricevuto */
            clsMessaggio Messaggio = new clsMessaggio();
            Messaggio.messaggio = strMsg;
            Messaggio.ip = ((IPEndPoint)binary).Address.ToString();
            Messaggio.porta = Convert.ToUInt16(((IPEndPoint)binary).Port);

            return Messaggio;
        }

        // (6) - Aggiunta TCP

        /******************************************/
        /* Invia una Stringa dal SERVER al CLIENT */
        /******************************************/
        public void inviaMsgSERVER(string strMsg)
        {
            byte[] bufferTX;

            // Serializzo la Stringa ricevuta in input
            bufferTX = Encoding.ASCII.GetBytes(strMsg);

            // Invio della Stringa
            ConnID.Send(bufferTX, bufferTX.Length, 0);

            // Chiudo la Connessione
            ConnID.Shutdown(SocketShutdown.Both);
            ConnID.Close();

        }

        /******************************************/
        /* Invia una Stringa dal CLIENT al SERVER */
        /******************************************/
        public void inviaMsgCLIENT(string strMsg)
        {
            byte[] bufferTX;

            // Serializzo la Stringa ricevuta in input
            bufferTX = Encoding.ASCII.GetBytes(strMsg);

            /*
             *  Binary contiene l' IP e la Porta (IPEndPont) del SERVER
             *  assegnato nel momento in cui ho istanziato 
             *  il Socket sul CLIENT
             */

            // Connetto al Server Remoto attraverso Binary
            socketID.Connect(binary);

            // Invio della Stringa
            socketID.Send(bufferTX, bufferTX.Length, 0);

        }

        /*********************************************************/
        /* Procedura che implementa il Metodo astratto Dispose() */
        /*********************************************************/
        public void Dispose()
        {
            
            // (7) Aggiunta TCP

            // (1) - Ripulisco i Buffer di Comunazione (SOLO per il Client)
            if (!server)
                socketID.Shutdown(SocketShutdown.Both);
           
            // (2) - Chiudo il Socket
            socketID.Close();

            // (3) - Arresto il Thread (SOLO per il Server)
            if (server && threadAscolta != null)
            {
                // Se il Thread è Sospeso => lo "Riesumo"
                if (threadAscolta.ThreadState == ThreadState.SuspendRequested)
                    threadAscolta.Resume();

                // Arresto il Thread
                threadAscolta.Abort();
            }

        }


    }