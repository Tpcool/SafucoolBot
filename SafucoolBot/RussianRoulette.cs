using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace SafucoolBot
{
    class RussianRoulette
    {
        public static string rr_user1 = "", rr_user2 = "", rr_channel;
        public static int rr_id1 = -1, rr_id2 = -1;
        public static bool rr_isGameActive = false;
        public static long rr_channelID = -1;

        static Random rndm = new Random();
        static int bullet = -1, chamber = -1; //The bullet and chamber can only be 0-5 (to represent a revolver).
        static bool hasUsedSpin1 = false, hasUsedSpin2 = false, isTurnPlayer1;
        static string currentPlayer = "", otherPlayer = "";
        static int currentPlayerID = -1, otherPlayerID = -1;

        public static void Execute(MessageEventArgs e)
        {
            if (e.Message.Text.StartsWith(".endgame"))
            {
                EndGame();
                e.Channel.SendMessage(Format("Game ended by " + e.Message.User.Name + "."));
                return;
            }
            
            if (rr_isGameActive == false && !BotActions.isDirectMessage(e))
            {
                rr_user1 = e.Message.User.Name;
                rr_id1 = e.Message.User.Discriminator;
                rr_channel = e.Message.Channel.Name;
                rr_channelID = Convert.ToInt64(e.Message.Channel.Id);
                e.Channel.SendMessage(DisplayStartupMessage());
                rr_isGameActive = true;
                return;
            }
            else if (rr_isGameActive == false)
            {
                e.Channel.SendMessage("This game can only be played in a group.");
                return;
            }

            if (e.Message.Text.StartsWith(".join") && rr_id2 == -1 &&
                rr_id1 != e.Message.User.Discriminator && BotActions.hasSufficientFunds(0, e, Program.strPointsPath))
            {
                e.Channel.SendMessage(Format(e.Message.User.Name + " has joined the game."));
                rr_user2 = e.Message.User.Name;
                rr_id2 = e.Message.User.Discriminator;
                chamber = 0;
                bullet = rndm.Next(6);

                if (rndm.Next(2) == 0)
                {
                    isTurnPlayer1 = true;
                    e.Channel.SendMessage(Format(rr_user1 + " vs. " + rr_user2 + "\n\nIt's " + rr_user1 +
                        "'s turn first. Use .help for a list of commands."));
                    currentPlayer = rr_user1;
                    otherPlayer = rr_user2;
                    currentPlayerID = rr_id1;
                    otherPlayerID = rr_id2;
                }
                else
                {
                    isTurnPlayer1 = false;
                    e.Channel.SendMessage(Format(rr_user1 + " vs. " + rr_user2 + "\n\nIt's " + rr_user2 +
                        "'s turn first. Use .help for a list of commands."));
                    currentPlayer = rr_user2;
                    otherPlayer = rr_user1;
                    currentPlayerID = rr_id2;
                    otherPlayerID = rr_id1;
                }
                return;
            }
            else if (e.Message.Text.StartsWith(".join") && !BotActions.hasSufficientFunds(0, e, Program.strPointsPath))
            {
                e.Channel.SendMessage(Format("nigga u broke, pay off your shit then come back"));
                return;
            }
            else if (e.Message.Text.StartsWith(".join"))
            {
                e.Channel.SendMessage(Format("Bad idea."));
                return;
            }

            if (e.Message.Text.StartsWith(".help"))
            {
                e.Channel.SendMessage(Help());
                return;
            }

            if (e.Message.Text.StartsWith(".pull") && isUsersTurn(e))
            {
                if (bullet == chamber)
                {
                    e.Channel.SendMessage(Format("KABLAM! " + currentPlayer + "'s head blew up. " + otherPlayer +
                        " collects " + BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), currentPlayerID) +
                        " RAVEBUX for a total of " + (BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), currentPlayerID) +
                        BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), otherPlayerID))
                        + " bux."));
                    DistributePoints(!isTurnPlayer1, e);
                    EndGame();
                }
                else
                {
                    e.Channel.SendMessage(Format("*click*\nYou are safe... for now. It is " + otherPlayer + "'s turn."));
                    chamber += 1;
                    EndTurn();
                }
                return;
            }

            if (e.Message.Text.StartsWith(".spin") && isUsersTurn(e) &&
                ((hasUsedSpin1 == false && isTurnPlayer1 == true) || (hasUsedSpin2 == false && isTurnPlayer1 == false)))
            {
                bullet = rndm.Next(6);
                chamber = 0;
                e.Channel.SendMessage(Format(currentPlayer + " spun the chamber."));
                if (isTurnPlayer1)
                {
                    hasUsedSpin1 = true;
                }
                else
                {
                    hasUsedSpin2 = true;
                }
                EndTurn();
                return;
            }
            else if (e.Message.Text.StartsWith(".spin") && isUsersTurn(e))
            {
                e.Channel.SendMessage(Format("You already used your spin, bub."));
                return;
            }

            if (e.Message.Text.StartsWith(".shoot") && isUsersTurn(e))
            {
                if (bullet == chamber)
                {
                    e.Channel.SendMessage(Format("KAPOW! " + otherPlayer + "'s head blew up! " + currentPlayer +
                        " collects " + BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), otherPlayerID) +
                        " RAVEBUX for a total of " + (BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), otherPlayerID) +
                        BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), currentPlayerID))
                        + " bux."));
                    DistributePoints(isTurnPlayer1, e);
                    EndGame();
                }
                else if (bullet != chamber)
                {
                    e.Channel.SendMessage(Format("*click* ...Well then.\n\noverwatcher does not appreciate your gamble. " + 
                        "He pulls out his alien ray gun and disintegrates " + currentPlayer + ". overwatcher gives " + otherPlayer +
                        " " + BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), currentPlayerID) +
                        " RAVEBUX for a total of " + (BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), currentPlayerID) +
                        BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), otherPlayerID))
                        + " bux."));
                    DistributePoints(!isTurnPlayer1, e);
                    EndGame();
                }
            }
        }

        //Displays the message that starts up when the game is initiated.
        static string DisplayStartupMessage()
        {
            return Format("RUSSIAN ROULETTE\n\nTwo players face off in a game of luck, but only one will walk out with all the " +
                "RAVEBUX. All normal commands are disabled for both players in the game until the game ends.\n\n" +
                "Game initiated by " + rr_user1 + ".\n\nType .join to start game.\nType .help to display the list " +
                "of commands that are unique to this game.");
        }

        //Formats text in the code format for the game.
        static string Format(string text)
        {
            return "```\n" + text + "\n```";
        }

        //Help command that is unique to this game.
        static string Help()
        {
            return Format(".endgame: ends the game and returns bot back to normal functionality.\n\n" +
                ".pull: pull the trigger, risk dying.\n\n" +
                ".shoot: turn the gun on your opponent and try shooting them. if this is not " +
                "successful, overwatcher will shoot YOU!\n\n" +
                ".spin: spin the barrel, essentially resetting your chances of dying. skips your " +
                "turn, but it can only be used once.");
        }
        
        //Resets all values to their default.
        static void EndGame()
        {
            rr_isGameActive = false;
            rr_user1 = "";
            rr_user2 = "";
            rr_channel = "";
            rr_id1 = -1;
            rr_id2 = -1;
            rr_channelID = -1;
            bullet = -1;
            chamber = -1;
            currentPlayerID = -1;
            otherPlayerID = -1;
            hasUsedSpin1 = false;
            hasUsedSpin2 = false;
            currentPlayer = "";
            otherPlayer = "";
        }

        //Switches the name of the current player and flips the truth of the player's turn variable.
        static void EndTurn()
        {
            isTurnPlayer1 = !isTurnPlayer1;
            if (isTurnPlayer1 == true)
            {
                currentPlayer = rr_user1;
                otherPlayer = rr_user2;
                currentPlayerID = rr_id1;
                otherPlayerID = rr_id2;
            }
            else
            {
                currentPlayer = rr_user2;
                otherPlayer = rr_user1;
                currentPlayerID = rr_id2;
                otherPlayerID = rr_id1;
            }
        }

        //Gives all of the loser's points to the winner.
        static void DistributePoints(bool didFirstPlayerWin, MessageEventArgs e)
        {
            if (didFirstPlayerWin)
            {
                BotActions.AddPoints(BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), rr_id2),
                    rr_id1, e, Program.strPointsPath);
                BotActions.AddPoints(-BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), rr_id2),
                    rr_id2, e, Program.strPointsPath);
            }
            else
            {
                BotActions.AddPoints(BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), rr_id1),
                    rr_id2, e, Program.strPointsPath);
                BotActions.AddPoints(-BotActions.GetUserPoints(e, BotActions.GetFile(Program.strPointsPath), rr_id1),
                    rr_id1, e, Program.strPointsPath);
            }
        }

        //Returns true if the message that was sent was made by a user who joined the game.
        public static bool isUserMessage(MessageEventArgs e)
        {
            if (e.Message.User.Name == rr_user1 || e.Message.User.Name == rr_user2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //For use in the main program.
        public static bool IsValidMessage(MessageEventArgs e)
        {
            //If the user tries to start a game while another isn't active in the same channel, they have money, AND
            //they don't try to call another game in the same channel.
            if ((e.Message.Text.ToLower().StartsWith(".russianroulette") && 
                !BotActions.isChannelPlayingGame(Convert.ToInt64(e.Message.Channel.Id)) &&
                BotActions.hasSufficientFunds(0, e, Program.strPointsPath)) && 
                (!BotActions.isGameName(e) || e.Message.Text.ToLower().StartsWith(".russianroulette")))
            {
                return true;
            }
            //If the game is active, the message is from a person in the game, it's not a direct message,
            //the channel matches, AND they don't try to call another game in the same channel.
            else if ((rr_isGameActive == true && isUserMessage(e) &&
                !BotActions.isDirectMessage(e) && rr_channel == e.Message.Channel.Name) &&
                (!BotActions.isGameName(e) || e.Message.Text.ToLower().StartsWith(".russianroulette")) &&
                Convert.ToInt64(e.Message.Channel.Id) == rr_channelID)
            {
                return true;
            }
            //If the game is active, a second player has not joined yet, AND someone tries to join in the same channel.
            else if (rr_isGameActive == true && rr_id2 == -1 && e.Message.Text.StartsWith(".join") &&
                Convert.ToInt64(e.Message.Channel.Id) == rr_channelID)
            {
                return true;
            }
            return false;
        }

        //Return true if it's the user who entered the command's turn.
        static bool isUsersTurn(MessageEventArgs e)
        {
            if (
                ((isTurnPlayer1 == true && rr_id1 == e.Message.User.Discriminator) || (isTurnPlayer1 == false && rr_id2 == e.Message.User.Discriminator))
                && (rr_id1 != -1 && rr_id2 != -1)
                && (otherPlayer != "")
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
