using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdiotGame.Shared.Models {
    public class Player {

        /// <summary>
        /// Username for the player; should be set with constructor
        /// </summary>
        public string? Username { get; set; }

        public string? ConnectionId { get; set; }

        /// <summary>
        /// Lets the game know if this players turn is currently active or not
        /// </summary>
        public bool TurnActive { get; set; }

        /// <summary>
        /// Hand pile of cards for the player
        /// </summary>
        public Pile? Hand { get; set; }

        /// <summary>
        /// Face up card pile on table, max 3 cards
        /// </summary>
        public Pile? FaceUpCardPile { get; set; }

        /// <summary>
        /// Face down card pile on table, max 3 cards
        /// </summary>
        public Pile? FaceDownCardPile { get; set; }

        public bool HasWon { get; set; }

        /// <summary>
        /// Contructor that takes in a username and builds the player object
        /// </summary>
        /// <param name="_username"></param>
        /// <param name="_connectionId"></param>
        public Player(string _connectionId, string _username) {
            Username = _username;
            ConnectionId = _connectionId;
            TurnActive = false;
            Hand = new Pile();
            FaceUpCardPile = new Pile();
            FaceDownCardPile = new Pile();
            HasWon = false;
        }

        /// <summary>
        /// Empty contructor for json deserialization
        /// </summary>
        public Player() { }

        /// <summary>
        /// Returns the current hand of the user
        /// </summary>
        /// <returns></returns>
        public Pile GetHand() {
            if (Hand is not null) {
                return Hand;
            }

            return null;
        }

        /// <summary>
        /// Returns the current face up card pile of the user
        /// </summary>
        /// <returns></returns>
        public Pile GetFaceUpCardPile() {
            if (FaceUpCardPile is not null) {
                return FaceUpCardPile;
            }

            return null;
        }

        /// <summary>
        /// Returns the cvurrent face down card pile of hte user
        /// </summary>
        /// <returns></returns>
        public Pile GetFaceDownCardPile() {
            if (FaceDownCardPile is not null) {
                return FaceDownCardPile;
            }

            return null;
        }

        /// <summary>
        /// Returns the current number of cards in the users hand
        /// </summary>
        /// <returns></returns>
        public int GetHandCount() {
            if (Hand is not null) {
                return Hand.CardCount();
            }

            return -1;
        }


        /// <summary>
        /// Returns the total number of cards within all of the players piles
        /// </summary>
        /// <returns></returns>
        public int GetTotalCardCount() {
            return Hand.CardCount() + FaceDownCardPile.CardCount() + FaceUpCardPile.CardCount();
        }

        /// <summary>
        /// Overrides the to string method for printing out a player
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "Username " + Username + ", ConnectionId " + ConnectionId;

        }
        
        



    }
}
