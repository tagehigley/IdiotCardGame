using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdiotGame.Shared.Models.Enums;

namespace IdiotGame.Shared.Models {

    /// <summary>
    /// Card class that holds an individual card for the game.
    /// </summary>
    public class Card {
        public CardSuit Suit { get; set; }
        public CardValue Value { get; set; }
        public string highlight { get; set; }

        public bool IsRed {
            get {
                return Suit == CardSuit.Diamonds || Suit == CardSuit.Hearts;
            }
        }

        public bool IsBlack {
            get {
                return !IsRed;
            }
        }

        public override string ToString() {
            return (int)Value + Suit.ToString() + ".png";
        }
    }
}
