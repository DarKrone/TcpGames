using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class TcpGamesServer
    {
        private TcpListener _listener;

        private List<TcpClient> _clients = new List<TcpClient>();
        private List<TcpClient> _waitingLobby = new List<TcpClient>();

        private Dictionary<TcpClient, IGame> _gameClientIsIN = new Dictionary<TcpClient, IGame>();
        private List<IGame> _games = new List<IGame>();
        private List<Thread> _gameThreads = new List<Thread>();
        private IGame _nextGame;

        public readonly string Name;
        public readonly int Port;
        public bool Running { get; private set; }

        public TcpGamesServer(string name, int port)
        {
            Name = name;
            Port = port;
            Running = true;

            _listener = new TcpListener(IPAddress.Any, Port);
        }

        public void Shutdown()
        {
            if (Running)
            {
                Running = false;
                Console.WriteLine("Shutting down the Game(s) server...");
            }
        }

        public void Run()
        {
            Console.WriteLine("Starting the \"{0}\" Game(s) server on port {1}.", Name, Port);
            Console.WriteLine("Press CTRL-C to shutdown the server at any time");

            _nextGame = new GuessMyNumberGame(this);

            _listener.Start();
            Running = true;
            List<Task> newConnectionTasks = new List<Task>();
            Console.WriteLine("Waiting for incomming connections...");

            while (Running)
            {
                if (_listener.Pending())
                {
                    newConnectionTasks.Add(HandleNewConnection());
                }

                if (_waitingLobby.Count >= _nextGame.RequiredPlayers)
                {
                    int numPlayers = 0;
                    
                    while (numPlayers < _nextGame.RequiredPlayers)
                    {
                        TcpClient player = _waitingLobby[0];
                        _waitingLobby.RemoveAt(0);

                        if (_nextGame.AddPlayer(player))
                        {
                            numPlayers++;
                        }
                        else
                        {
                            _waitingLobby.Add(player);
                        }
                    }

                    Console.WriteLine("Starting a \"{0}\" game.", _nextGame.Name);

                }
            }
        }
    }
}
