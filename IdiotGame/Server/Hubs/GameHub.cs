using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using IdiotGame.Server.Engine;
using IdiotGame.Shared.Models;

namespace IdiotGame.Server.Hubs {
    public class GameHub : Hub {

        /// <summary>
        /// Holds a dict of connection ids and usernames
        /// </summary>
        private static Dictionary<string, string> users = new Dictionary<string, string>();

        private GameEngine engine;

        public GameHub(GameEngine engine) {
            this.engine = engine;
        }

        #region connections
        public override async Task OnConnectedAsync() {
            string username = Context.GetHttpContext().Request.Query["username"];
            users.Add(Context.ConnectionId, username);
            //engine.SetPlayerOnConnect(username, Context.ConnectionId);
            await SendMessage(string.Empty, $"{username} Connected!");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception) {
            string username = users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value;
            users.Remove(Context.ConnectionId);
            //engine.RemovePlayerOnDisconnect(username);
            await SendMessage(string.Empty, $"{username} left!");
        }

        #endregion

        public async Task SendMessage(string user, string message) {

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }


        public async Task RequestPlayers() {
            await engine.ListOfPlayers(this);
        }

        public async Task StartGame() {

            //if (users.Count < 3) {
            //    await Clients.Caller.SendAsync("StartCheck", false);
            //} else {
            //    await engine.StartGame(this);
            //}

            await engine.StartGame(this, users);
        }

        public async Task PlayCard(List<Card> card) {
            await engine.PlayCard(this, card);
        }

        public async Task DrawCard(int handCount) {
            await engine.Draw(this, handCount);
        }

        public async Task PlayerPickUp(Card card) {
            await engine.PlayerPickUp(this, card);
        }

        public async Task ServerHand() {
            await engine.PrintServerHand(this);
        }

    }
}
