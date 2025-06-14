using System.Net.Sockets;

namespace Server
{
    internal interface IGame
    {
        #region Properties

        /// <summary>
        /// Name of the game
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// How many players are needed to start
        /// </summary>
        public int RequiredPlayers { get; }

        #endregion

        #region Functions

        /// <summary>
        /// Adds a player to the game
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool AddPlayer(TcpClient client);

        /// <summary>
        /// Tells the server to disconnect a player
        /// </summary>
        /// <param name="client"></param>
        public void DisconnectClient(TcpClient client);

        /// <summary>
        /// The main game loop
        /// </summary>
        public void Run();

        #endregion
    }
}
