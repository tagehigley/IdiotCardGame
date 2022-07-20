using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdiotGame.Shared.Models.Enums;

namespace IdiotGame.Shared.Models {

    /// <summary>
    /// Main game deck that holds the initial 52 cards for the game. 
    /// </summary>
    public class GameDeck : Pile {


        /// <summary>
        /// Constructor that builds the 52 card deck
        /// </summary>
        public GameDeck() {
            foreach (CardSuit suit in (CardSuit[])Enum.GetValues(typeof(CardSuit))) {

                foreach (CardValue value in (CardValue[])Enum.GetValues(typeof(CardValue))) {

                    //For each suit and value, create and insert a new Card object.
                    Card newCard = new Card() {
                        Suit = suit,
                        Value = value,
                    };

                    cards.Add(newCard);
                }

            }
        }

        /// <summary>
        /// Creates a game deck using passed in cards
        /// Used for creating game decks on the connected clients
        /// </summary>
        /// <param name="_cards"></param>
        public GameDeck(List<Card> _cards) {
            setCards(_cards);
        }

        /// <summary>
        /// Prints out a list of the cards in the deck starting from the 
        /// top of the deck
        /// </summary>
        public void Print() {
            foreach (Card card in cards) {
                Console.WriteLine(card.ToString());
            }
        }


    }
}
