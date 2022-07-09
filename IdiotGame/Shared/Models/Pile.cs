using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdiotGame.Shared.Models {

    /// <summary>
    /// Pile class that holds cards
    /// The pile is modeled after a deck of cards that are all face down
    /// Cards can be drawn from the top of the deck
    /// Cards can be inserted at the top of the deck
    /// Cards can be inserted at the bottom of the deck
    /// </summary>
    public class Pile /*:IEnumerator, IEnumerable*/ {

        //****************************************************************************
        //Had to remove the enumerator to allow for json deserialization
        //ReAdd it if you need to but you shouldn't have to i believe, at least at this time
        //****************************************************************************

        /// <summary>
        /// List that holds are the cards in the deck
        /// </summary>
        public List<Card> cards { get; set; }

        /// <summary>
        ///Position is used for iEnummerable and Ienumerator interfaces
        /// </summary>
       // public int position = -1;


        /// <summary>
        /// Constructor that builds an empty pile
        /// </summary>
        public Pile() {
            cards = new List<Card>();

        }

        /// <summary>
        /// Sets a list of passed in cards to a pile; Removes any exists cards in the pile
        /// </summary>
        /// <param name="_cards"></param>
        public void setCards(List<Card> _cards) {
            this.cards = _cards;
        }
        
        /// <summary>
        /// Returns the list of cards in the pile
        /// </summary>
        /// <returns></returns>
        public List<Card> GetCards() {
            return this.cards;
        }

        /// <summary>
        /// Returns a card of a random suit that has the same value as the passed in int value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<Card> DrawSpecificValue(int value) {

            List<Card> c = new();

            while (c.Count != 4) {

                if (c.Count > 0) {
                    c.Clear();
                }

                for (int i = 0; i < cards.Count(); ++i) {

                    if ((int)cards[i].Value == value) {
                        c.Add(cards[i]);
                    }
                }

                ++value;
            }

            RemoveCards(c);
            return c;

        }

        /// <summary>
        /// Draws the top card off the pile and returns it
        /// Card is removed from the deck
        /// </summary>
        /// <returns></returns>
        public Card DrawTopCard() {

            if (cards.Count() > 0) {
                var card = cards[0];
                cards.RemoveAt(0);
                return card;
            }

            return null;
        }

        /// <summary>
        /// Removes card from the end of the card list, more optimized then drawing from the top of the list
        /// </summary>
        /// <returns></returns>
        public Card DrawBottomCard() {
            if (cards.Count() > 0) {
                var card = cards.Last();
                cards.Remove(card);
                return card;
            }

            return null;
        }

        /// <summary>
        /// Draw the top three cards from a pile
        /// </summary>
        /// <returns></returns>
        public List<Card> DrawThreeCardsFromTop() {
            List<Card> threeCards = new();

            for(int i = 0; i < 3; ++i) {
                threeCards.Add(DrawTopCard());
            }

            return threeCards;
        }

        /// <summary>
        /// Inserts card on top of the deck
        /// </summary>
        /// <param name="card"></param>
       public void InsertCardTop(Card card) {
            if (card is not null) {
                cards.Insert(0, card);
            }

        }

        /// <summary>
        /// Inserts a card at the bottom of the pile
        /// </summary>
        /// <param name="card"></param>
        public void InsertCardBottom(Card card) {
            if (card is not null) {
                cards.Add(card);
            }
        }

        /// <summary>
        /// Inserts a list of cards to the top of current pile
        /// </summary>
        /// <param name="_cards"></param>
        public void InsertCardListTop(List<Card> _cards) {
            if (_cards is not null && _cards.Count() > 0) {
                cards = _cards.Concat(cards).ToList();
            }
        }

        /// <summary>
        /// Inserts a list of cards to the bottom of current pile
        /// </summary>
        /// <param name="_cards"></param>
        public void InsertCardListBottom(List<Card> _cards) {
            if (_cards is not null /*&& _cards.Count() > 0*/) {
                cards = cards.Concat(_cards).ToList();
            }
        }

        /// <summary>
        /// Returns the number of cards in the current pile
        /// </summary>
        /// <returns></returns>
        public int CardCount() {
            return cards.Count;
        }

        /// <summary>
        /// Shuffles the current pile in a random order
        /// </summary>
        public void Shuffle() {
            Random rng = new Random();
            int size = cards.Count();

            while (size > 1) {
                size--;
                int swapSpot = rng.Next(size + 1);
                Card a = cards[swapSpot];
                cards[swapSpot] = cards[size];
                cards[size] = a;
            }
        }

        /// <summary>
        /// Removes cards from the pile based on a passed in list of cards
        /// Had to be done with two lists; Janky, but it gets the job done
        /// Refactor later 
        /// </summary>
        /// <param name="cardsToRemove"></param>
        public void RemoveCards(List<Card> cardsToRemove) {

            List<Card> cardRemoval = new();

            foreach (Card card in cards) {
                foreach (var crd in cardsToRemove) {
                    if (crd is not null) {
                        if (crd.Value == card.Value && crd.Suit == card.Suit) {
                            cardRemoval.Add(card);
                        }
                    }
                }
            }

            foreach (Card card in cardRemoval) {
                cards.Remove(card);
            }

        }

        public void Clear() {
            cards.Clear();
        }


        //public IEnumerator GetEnumerator() {
        //    return (IEnumerator) this;
        //}

        //public bool MoveNext() {
        //    position++;
        //    return (position < cards.Count);
        //}

        //public void Reset() {
        //    position = -1;
        //}

        //public object Current {
        //    get { return cards[position]; }
        //}


    }
}
