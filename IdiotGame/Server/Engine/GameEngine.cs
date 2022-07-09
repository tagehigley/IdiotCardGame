
using IdiotGame.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace IdiotGame.Server.Engine {
    /// <summary>
    /// Holds the logic for the game and manages the game state
    /// </summary>
    public class GameEngine {


        private static List<Game> GamesList = new List<Game>();

        private Game CurGame;

        private static List<Player>? playerList;

        private bool gameStarted;
        private bool hasExiled;

        private Player curPlayerTurn;

        public GameEngine() {
            playerList = new List<Player>();
            hasExiled = false;
        }


        public async Task ListOfPlayers(Hub hub) {

            List<string> names = new();
            foreach(Player player in playerList) {
                names.Add(player.Username);
            }

            await hub.Clients.All.SendAsync("ReceivePlayerList", names);
        }



        //-------------------------------------------------------------------------------------
        // The above functions are currently for testing and may be removed or changed later

        #region connections

        /// <summary>
        /// Adds a player to the list of players for a game when the player connects
        /// </summary>
        /// <param name="username"></param>
        //public void SetPlayerOnConnect(string username, string connectionId) {
        //    if (playerList is not null) {
        //        playerList.Add(new Player(username, connectionId));
        //    }
        //}
        
        /// <summary>
        /// Removes a player from the list of players if they disconnect
        /// </summary>
        /// <param name="_username"></param>
        //public void RemovePlayerOnDisconnect(string _username) {

        //    foreach (Player player in playerList) {
        //        if (player.Username == _username) {
        //            playerList.Remove(player);
        //            break;
        //        }
        //    }
        //}

        #endregion 

        public async Task StartGame(Hub hub, Dictionary<string, string> users) {


            Game game = new Game(GenerateGameCode());

            foreach (var user in users) {
                game.PlayerList.Add(new Player(user.Key, user.Value));
            }

            game.GameDeck.Shuffle();

            foreach (Player player in game.PlayerList) {
                player.FaceDownCardPile.InsertCardListTop(game.GameDeck.DrawThreeCardsFromTop());
                player.FaceUpCardPile.InsertCardListTop(game.GameDeck.DrawThreeCardsFromTop());
                player.Hand.InsertCardListTop(game.GameDeck.DrawThreeCardsFromTop());
            }

            CurGame = game;

            GamesList.Add(game);


            FirstTurnOfGame();

            UpdatePlayers(hub);

            SetPlayerTurn(hub);
        }

        /// <summary>
        /// Generates a game code for a new game; if the code is already in use then generate a new code
        /// </summary>
        /// <returns></returns>
        private string GenerateGameCode() {

            Random random = new Random();

            bool accepted = false;

            string code = string.Empty;


            do {
                for (int i = 0; i < 5; ++i) {
                    char c = (char)random.Next(65, 91);

                    code += c;
                }

                accepted = true;

                foreach (Game g in GamesList) {
                    if (g.GameID == code) {
                        accepted = false;
                    }
                }

            } while (!accepted);

            return code;
        }

        /// <summary>
        /// Sets the first turn of the game in motion, picks the first player based off of who has the lowest card in hand
        /// starting at the card value 3 as cards with value of 2 are special cards
        /// </summary>
        private void FirstTurnOfGame() {

            for (int i = 3; i < 14; ++i) {
                foreach (Player player in CurGame.PlayerList) {
                    foreach (Card card in player.Hand.GetCards()) {
                        if ((int)card.Value == i) {
                            player.TurnActive = true;
                            CurGame.CurPlayer = player;
                            goto FirstTurn;
                        }
                    }

                }
            }

        FirstTurn:
            return;
        }

        /// <summary>
        /// Sends the player data to each connected player, also sends the game ID
        /// </summary>
        /// <param name="hub"></param>
        /// <returns></returns>
        private async Task UpdatePlayers(Hub hub) {
            foreach (Player player in CurGame.PlayerList) {
                await hub.Clients.Client(player.ConnectionId).SendAsync("SetPlayer", player, CurGame.GameID);
            }
        }

        /// <summary>
        /// Updates each client to whose turn it is
        /// </summary>
        /// <param name="hub"></param>
        /// <returns></returns>
        private async Task SetPlayerTurn(Hub hub) {
            foreach (Player player in CurGame.PlayerList) {

                await hub.Clients.Client(player.ConnectionId).SendAsync("SetPlayerTurn", CurGame.CurPlayer.Username.ToString());
            }
        }

        /// <summary>
        /// Adds any cards played on the pickup pile to the game pickup pile and returns the top most card of the deck
        /// for the pick up pile display
        /// </summary>
        /// <param name="hub"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        public async Task PlayCard(Hub hub, List<Card> cards) {

            Pile p = FindCardPile(cards[0]);

            if (ExileCheck(cards)) {
                p.RemoveCards(cards);
                await ExileCards(hub);
            } else {

                CurGame.PickUpPile.InsertCardListBottom(cards);
                
                p.RemoveCards(cards);

                await hub.Clients.All.SendAsync("RecievePickUpPileCard", CurGame.PickUpPile.cards.Last());
            }

            if (CurGame.CurPlayer.GetTotalCardCount() == 0) {
                CurGame.CurPlayer.HasWon = true;

                await hub.Clients.Caller.SendAsync("SetMessage", "You won!");

                await GameOverCheck(hub);
            }

            if (CurGame.GameStarted) {
                //if (handCount < 3) {
                //    await Draw(hub, handCount);
                //}

                if (!CurGame.HasExiled) {
                    ChangeTurn();
                    //RemoveWinners();
                } else {
                    CurGame.HasExiled = false;
                }


                await SetPlayerTurn(hub);
            }

        }

        /// <summary>
        /// Draws card from the main game deck, if the hand is less than 3 and the game
        /// deck is above 0 cards then draw the needed amount of cards to get to 3 cards
        /// </summary>
        /// <param name="hub"></param>
        /// <param name="handCount"></param>
        /// <returns></returns>
        public async Task Draw(Hub hub, int handCount) {

            List<Card> drawHand = new();

            //Draw the number of cards needed to get to a minumum of three cards
            //If there is less than 3 cards in the game deck draw remaining cards
            if (handCount < 3 && CurGame.GameDeck.CardCount() > 0) {
                for (int i = handCount; i < 3; ++i) {
                    Card card = CurGame.GameDeck.DrawBottomCard();
                    if (card is not null) {
                        drawHand.Add(card);
                    }
                }
            }
            ///Draw a single card
            else if (CurGame.GameDeck.CardCount() > 0) {

                drawHand.Add(CurGame.GameDeck.DrawBottomCard());
            }

            if (CurGame.GameDeck.CardCount() > 0) {
                CurGame.CurPlayer.Hand.cards = CurGame.CurPlayer.Hand.GetCards().Concat(drawHand).ToList();
            } else {
                await hub.Clients.All.SendAsync("DeckEmpty", true);
            }

            if (CurGame.GameDeck.CardCount() <= 2) {
                Print(CurGame.GameDeck.CardCount().ToString());
            }

            await hub.Clients.Caller.SendAsync("RecieveDraw", drawHand);
        
        
        } 

        /// <summary>
        /// Moves the current player to the next player
        /// If the player is the last in the list then move back to the front of the list
        /// Once a new player's turn is set break from the loop
        /// </summary>
        public void ChangeTurn() {

            Player temp = null;

           for (int i = 0; i < CurGame.PlayerList.Count; ++i) {

                if (CurGame.PlayerList[i].TurnActive) {
                    CurGame.PlayerList[i].TurnActive = false;

                    if (CurGame.PlayerList[i].HasWon) {
                        temp = CurGame.PlayerList[i];
                    }

                    if (CurGame.PlayerList[i] == CurGame.PlayerList.Last()) {
                        CurGame.PlayerList[0].TurnActive = true;
                        CurGame.CurPlayer = CurGame.PlayerList[0];
                        break;
                    } else {
                        CurGame.PlayerList[i + 1].TurnActive = true;
                        CurGame.CurPlayer = CurGame.PlayerList[i + 1];
                        break;
                    }


                }


            }

           if (temp is not null) {
                CurGame.PlayerList.Remove(temp);
            }


        }

        /// <summary>
        /// Checks if a 10 has been played or if four of the same value of cards has been played on the pick up pile
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private bool ExileCheck(List<Card> cards) {

            if ((int)cards.Last().Value == 10) {
                return true;
            }

            if (cards.Count() >= 4) {

                int value = (int)cards.Last().Value;

                for (int i = 0; i < cards.Count(); ++i) {
                    if ((int)cards[i].Value != value) {
                        break;
                    } else {
                        return true;
                    }
                }
            }

            return false;

        }

        public void RemoveWinners() {

            for (int i = 0; i < CurGame.PlayerList.Count; ++i) {
                if (CurGame.PlayerList[i].HasWon) {
                    CurGame.PlayerList.RemoveAt(i);
                }
            }

        }

        /// <summary>
        /// If a user plays a 10 or there is four of the same value on the pickup pile then clear the deck
        /// Remains the current players turn
        /// </summary>
        /// <param name="hub"></param>
        /// <returns></returns>
        public async Task ExileCards(Hub hub) {
            
            if (CurGame.ExilePile is null) {
                CurGame.ExilePile = new();
            }

            CurGame.ExilePile = CurGame.PickUpPile;
            CurGame.PickUpPile.Clear();
            CurGame.HasExiled = true;

            await hub.Clients.All.SendAsync("ClearPickUpPile", true);
        }


        /// <summary>
        /// Function fires if the user picks up the pickup pile at any time and clears the pickup pile on
        /// each clients screen
        /// </summary>
        /// <param name="hub"></param>
        /// <returns></returns>
        public async Task PlayerPickUp(Hub hub, Card cd) {
            CurGame.CurPlayer.Hand.InsertCardListBottom(CurGame.PickUpPile.GetCards());
            if (cd is not null) {
                CurGame.CurPlayer.Hand.InsertCardBottom(cd);


            }
            CurGame.PickUpPile.Clear();
            ChangeTurn();
            await hub.Clients.All.SendAsync("ClearPickUpPile", true);
            await SetPlayerTurn(hub);
        }

        /// <summary>
        /// Checks if there is only one player remaining and if so announce that the game is over and the remaining player
        /// has lost
        /// </summary>
        /// <param name="hub"></param>
        /// <returns></returns>
        public async Task GameOverCheck(Hub hub) {

            int count = 0;
            string name = string.Empty;
            foreach (Player player in CurGame.PlayerList) {
                if (!player.HasWon) {
                    ++count;
                    name = player.Username;
                }
            }

            if (count == 1) {
                await hub.Clients.All.SendAsync("GameOver", name);
                CurGame.GameStarted = false;

                GamesList.Remove(CurGame);

            }

        }


        /// <summary>
        /// Finds which pile a specific card belongs to
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private Pile FindCardPile(Card card) {

            foreach (Card crd in CurGame.CurPlayer.Hand.GetCards()) {
                if (crd.ToString() == card.ToString()) {
                    return CurGame.CurPlayer.Hand;
                }
            }

            foreach (Card crd in CurGame.CurPlayer.FaceUpCardPile.GetCards()) {
                if (crd.ToString() == card.ToString()) {
                    return CurGame.CurPlayer.FaceUpCardPile; 
                }
            }

            foreach (Card crd in CurGame.CurPlayer.FaceDownCardPile.GetCards()) {
                if (crd.ToString() == card.ToString()) {
                    return CurGame.CurPlayer.FaceDownCardPile;
                }
            }

            return null;

        }

        private void Print(string msg) {
            Console.WriteLine(msg);
        }

        private void printData() {
            foreach (Player player in playerList) {
                Print(player.Username + " , " + player.TurnActive.ToString() + "\n");
            }
        }

        public async Task PrintServerHand(Hub hub) {
            await hub.Clients.All.SendAsync("GetServerHand", CurGame.CurPlayer.FaceUpCardPile.GetCards());
        }

        

    }
}
