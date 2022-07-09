
using IdiotGame.Shared.Models;

namespace IdiotGame.Server.Engine {
    public class Game {

        /// <summary>
        /// Identification code for the game
        /// </summary>
        public string GameID;

        /// <summary>
        /// Holds the game deck of the current game
        /// </summary>
        public GameDeck GameDeck;

        /// <summary>
        /// Holds the game pick up pile
        /// </summary>
        public Pile PickUpPile;

        /// <summary>
        /// Pile that holds the exiled cards when a 10 or four of a same value is played
        /// </summary>
        public Pile ExilePile;

        /// <summary>
        /// Tracks if a game has started or not
        /// </summary>
        public bool GameStarted;

        /// <summary>
        /// A bool to keep track of if a player has caused cards to be exiled or not 
        /// </summary>
        public bool HasExiled;

        /// <summary>
        /// Keeps track of the players in the game
        /// </summary>
        public List<Player> PlayerList;

        /// <summary>
        /// Keeps track of the whose turn it currently is;
        /// </summary>
        public Player CurPlayer;

 
        /// <summary>
        /// Constructor for the game object
        /// </summary>
        public Game() {
            GameDeck = new GameDeck();
            PickUpPile = new Pile();
            ExilePile = new Pile();
            PlayerList = new List<Player>();
            CurPlayer = null;
            GameStarted = true;
            HasExiled = false;
        }

        public Game(string _gameId) {
            GameID = _gameId;
            GameDeck = new GameDeck();
            PickUpPile = new Pile();
            ExilePile = new Pile();
            PlayerList = new List<Player>();
            CurPlayer = null;
            GameStarted = true;
            HasExiled = false;
        }

        /// <summary>
        /// Returns the game deck
        /// </summary>
        /// <returns></returns>
        public GameDeck GetDeck() {
            if (GameDeck is not null) {
                return GameDeck;
            }
            return null;
        }

        /// <summary>
        /// Returns the pick up pile
        /// </summary>
        /// <returns></returns>
        public Pile GetPickUpPile() {

            if (PickUpPile is not null) {
                return PickUpPile;
            }

            return null;

        }


    }
}
