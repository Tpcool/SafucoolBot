using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace SafucoolBot
{
    class Blackjack
    {
        public static bool bj_isGameActive = false;
        public static long bj_channelID = -1;

        static Random rndm = new Random();
        static bool hasGameStarted = false;
        static bool pull2 = false;
        static bool hasAce = false;
        static string[] currentPlayers = new string[8]; //Stores the names of those who joined. Should be for display purposes ONLY.
        static int[] currentPlayersID = new int[8]; //Stores the IDs of those who joined (parallel to currentPlayers).
        static bool[] isPlayerFinished = new bool[8]; //If the player decided to stand, went bust, or reached 21, returns true.
        static int[] playerValues = new int[8]; //Stores the total number the player currently has.
        static int currentPlayerIndex = -1; //Stores the index of the current player's turn.
        static string[,] cards = new string[13, 4];
        

        public static void Execute(MessageEventArgs e)
        {
            if (e.Message.Text.ToLower().StartsWith(".blackjack") && !bj_isGameActive &&
                !BotActions.isChannelPlayingGame(Convert.ToInt64(e.Message.Channel.Id)))
            {
                SetPlayer(e);
                e.Channel.SendMessage(DisplayStartupMessage());
                bj_isGameActive = true;
                bj_channelID = Convert.ToInt64(e.Message.Channel.Id);
                return;
            }
            else if (e.Message.Text.ToLower().StartsWith(".blackjack") && bj_isGameActive)
            {
                e.Channel.SendMessage("A game has already been started by " + currentPlayers[0] + ".");
                return;
            }
            else if (e.Message.Text.ToLower().StartsWith(".blackjack") &&
                BotActions.isChannelPlayingGame(Convert.ToInt64(e.Message.Channel.Id)))
            {
                e.Channel.SendMessage("Sorry, but this game cannot be played while another one is already active " +
                    "in the same channel.");
                return;
            }

            if (e.Message.Text.ToLower().StartsWith(".join") && !hasGameStarted)
            {
                e.Channel.SendMessage(SetPlayer(e));
            }

            if (e.Message.Text.ToLower().StartsWith(".start") && !string.IsNullOrEmpty(currentPlayers[1]) && !hasGameStarted)
            {
                if (e.Message.Text.ToLower().StartsWith(".start2"))
                {
                    pull2 = true;
                }
                e.Channel.SendMessage(StartGame());
                return;
            }
            else if (e.Message.Text.ToLower().StartsWith(".start") && string.IsNullOrEmpty(currentPlayers[1]))
            {
                e.Channel.SendMessage(Format("You need at least 2 players to start the game."));
                return;
            }

            if (e.Message.Text.ToLower().StartsWith(".help"))
            {
                e.Channel.SendMessage(Help());
                return;
            }

            if (e.Message.Text.ToLower().StartsWith(".endgame") && !isPlayerFinished[GetPlayerIndex(e)])
            {
                EndGame();
                e.Channel.SendMessage(Format("Game ended by " + e.Message.User.Name + "."));
                return;
            }
            else if (e.Message.Text.ToLower().StartsWith(".endgame") && isPlayerFinished[GetPlayerIndex(e)])
            {
                e.Channel.SendMessage(Format("Sorry " + e.Message.User.Name + ", you can't ragequit!"));
                return;
            }

            if (e.Message.Text.ToLower().StartsWith(".getvalue") && hasGameStarted)
            {
                e.Channel.SendMessage(Format(currentPlayers[GetPlayerIndex(e)] + "'s current value is "
                    + playerValues[GetPlayerIndex(e)] + "."));
            }

            if (e.Message.Text.ToLower().StartsWith(".hit") && IsPlayersTurn(e) && !hasAce)
            {
                string card = GetCard();
                int value = GetCardValue(card);

                if (value == -1)
                {
                    e.Channel.SendMessage(Format("You got an " + card + "! Your current value is " + playerValues[currentPlayerIndex] + 
                        ", so decide what value the ace should be."));
                    hasAce = true;
                }
                else
                {
                    e.Channel.SendMessage(Format(SetPlayerFinish(card, value) + " " + GetNextTurn()));
                    TestIfGameIsOver(e);
                }
            }

            if (e.Message.Text.ToLower().StartsWith(".stand") && IsPlayersTurn(e) && !hasAce)
            {
                isPlayerFinished[currentPlayerIndex] = true;
                e.Channel.SendMessage(Format(currentPlayers[currentPlayerIndex] + " has decided to stand at " +
                    playerValues[currentPlayerIndex] + "." + " " + GetNextTurn()));
                TestIfGameIsOver(e);
            }

            if (e.Message.Text.ToLower().StartsWith(".11") && IsPlayersTurn(e) && hasAce)
            {
                e.Channel.SendMessage(Format(SetPlayerFinish("Ace", 11) + " " + GetNextTurn()));
                TestIfGameIsOver(e);
                hasAce = false;
            }
            else if (e.Message.Text.ToLower().StartsWith(".1") && IsPlayersTurn(e) && hasAce)
            {
                e.Channel.SendMessage(Format(SetPlayerFinish("Ace", 1) + " " + GetNextTurn()));
                TestIfGameIsOver(e);
                hasAce = false;
            }
        }

        //Used for the main program to determine if the message sent is meant for this game.
        public static bool IsValidMessage(MessageEventArgs e)
        {
            //For users entering normal commands while the game is active in the same channel.
            if (bj_isGameActive && e.Message.Text.StartsWith(".") &&
                Convert.ToInt64(e.Message.Channel.Id) == bj_channelID &&
                !BotActions.isDirectMessage(e))
            {
                for (int i = 0; i < currentPlayersID.Length; i++)
                {
                    if (currentPlayersID[i] == e.Message.User.Discriminator)
                    {
                        return true;
                    }
                }
            }
            //For users attempting to join the game in the same channel.
            if (e.Message.Text.ToLower().StartsWith(".join") && 
                Convert.ToInt64(e.Message.Channel.Id) == bj_channelID &&
                !BotActions.isDirectMessage(e))
            {
                return true;
            }
            //Attempting to start a game while another is active in the same chanel.
            else if (e.Message.Text.ToLower().StartsWith(".blackjack") &&
                BotActions.isChannelPlayingGame(Convert.ToInt64(e.Message.Channel.Id)) &&
                !BotActions.isDirectMessage(e))
            {
                return true;
            }
            return false;
        }

        //Sets all necessary variables to formally "begin" the game. Pulls 2 cards if set to.
        static string StartGame()
        {
            ResetArrays();
            do
            {
                currentPlayerIndex = rndm.Next(8);
            } while (string.IsNullOrEmpty(currentPlayers[currentPlayerIndex]));
            hasGameStarted = true;

            if (pull2)
            {
                int i = 0, c1 = 0, c2 = 0;
                string initialValues = "";
                do
                {
                    c1 = GetCardValue(GetCard());
                    c2 = GetCardValue(GetCard());
                    if (c1 == -1)
                    {
                        c1 = 1;
                    }
                    else if (c2 == -1)
                    {
                        c2 = 1;
                    }
                    playerValues[i] += c1 + c2;
                    initialValues += currentPlayers[i] + " starts with " + playerValues[i] + ".\n";
                    i++;
                } while (!string.IsNullOrEmpty(currentPlayers[i]));

                return Format(initialValues + "\nGame start! It is " + currentPlayers[currentPlayerIndex] + "'s turn first. " +
                    "Remember to use .help for a list of commands unique to this game.");
            }

            return Format("Game start! It is " + currentPlayers[currentPlayerIndex] + "'s turn first. " +
                "Remember to use .help for a list of commands unique to this game.");
        }

        //Inserts the player who joined into the current players array and returns a status message.
        static string SetPlayer(MessageEventArgs e)
        {
            for (int i = 0; i < currentPlayers.Length; i++)
            {
                if (string.IsNullOrEmpty(currentPlayers[i]))
                {
                    currentPlayers[i] = e.Message.User.Name;
                    currentPlayersID[i] = e.Message.User.Discriminator;
                    return Format(currentPlayers[i] + " has joined the game.");
                }
                else if (currentPlayersID[i] == e.Message.User.Discriminator)
                {
                    return Format("You're already in this game.");
                }
            }
            return Format("Sorry, but this game is full.");
        }

        //Sets the cards string to a standard, organized deck of 52 cards. Also resets the isPlayerFinished and
        //playerValues arrays.
        static void ResetArrays()
        {
            string[] cardNum = {"Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King"};
            string[] cardType = {"Hearts", "Spades", "Diamonds", "Clovers"};

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    cards[i, j] = cardNum[i] + " of " + cardType[j];
                }
            }

            for (int i = 0; i < isPlayerFinished.Length; i++)
            {
                isPlayerFinished[i] = false;
                playerValues[i] = 0;
            }
        }

        //Returns a random card out of the deck and "erases" it from the deck.
        static string GetCard()
        {
            string cardToReturn = "";
            int randomType, randomValue;
            do
            {
                randomValue = rndm.Next(13);
                randomType = rndm.Next(4);
                cardToReturn = cards[randomValue, randomType];
                cards[randomValue, randomType] = "";
            } while (string.IsNullOrEmpty(cardToReturn));
            return cardToReturn;
        }

        //Gets the numeric value of the specified card string.
        static int GetCardValue(string card)
        {
            if (card.StartsWith("Ace"))
            {
                return -1;
            }
            for (int i = 2; i < 10; i++)
            {
                if (card.StartsWith(i.ToString()))
                {
                    return i;
                }
            }
            return 10;
        }

        //Displays the message that starts up when the game is initiated.
        static string DisplayStartupMessage()
        {
            return Format("BLACKJACK\n\n2-8 players. The winner receives the amount of people playing times 25 " +
                "RAVEBUX. All normal commands are disabled every player in the game until the game ends.\n\n" +
                "Game initiated by " + currentPlayers[0] + ".\n\nType .join to join the game.\nType .start to start when " +
                "everybody is in.\nType .start2 to to start the game with two cards per player (usual Blackjack rules)." +
                "\nType .help to display the list of commands that are unique to this game.");
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
                ".getvalue: displays your current card value.\n\n" +
                ".hit: draw a card from the deck.\n\n" +
                ".stand: finalize your number.\n\n" +
                ".1/.11: use when you receive an ace. you decide what the value is.");
        }

        //If the current player's value is 21 or higher, the player is set to finished.
        //Also returns a status message and adds on the card's value to the user's total.
        static string SetPlayerFinish(string card, int value)
        {
            playerValues[currentPlayerIndex] += value;

            if (playerValues[currentPlayerIndex] > 21)
            {
                isPlayerFinished[currentPlayerIndex] = true;
                return "You got the " + card + ". You went bust! Your final value is " +
                    playerValues[currentPlayerIndex] + ".";
            }
            else if (playerValues[currentPlayerIndex] == 21)
            {
                isPlayerFinished[currentPlayerIndex] = true;
                return "You got the " + card + ". That's 21!";
            }
            else
            {
                return "Your card is the " + card + ". Your current value is " +
                    playerValues[currentPlayerIndex] + ".";
            }
        }

        //Returns a message about the results of the game and allocates points.
        static string FinalizeGame(MessageEventArgs e)
        {
            int highestValue = 0;
            int highestIndex = -1;

            for (int i = 0; i < playerValues.Length; i++)
            {
                if (playerValues[i] > highestValue && playerValues[i] <= 21)
                {
                    highestValue = playerValues[i];
                    highestIndex = i;
                }
                else if (playerValues[i] == highestValue)
                {
                    highestIndex = -1;
                }
            }

            int numPlayers = 0;
            for (int i = 0; i < currentPlayers.Length; i++)
            {
                if (string.IsNullOrEmpty(currentPlayers[i]))
                {
                    numPlayers = i;
                    break;
                }
            }

            if (highestIndex == -1)
            {
                return Format("Game over! There was a tie, so nobody gets RAVEBUX.");
            }
            else
            {
                BotActions.AddPoints(25 * numPlayers, currentPlayersID[highestIndex], e, Program.strPointsPath);
                return Format("Game over! " + currentPlayers[highestIndex] + " won and has received " +
                    25 * numPlayers + " RAVEBUX.");
            }
        }

        //If the game is over, finalize and end the game. If not, advance turns.
        static void TestIfGameIsOver(MessageEventArgs e)
        {
            if (IsGameOver())
            {
                e.Channel.SendMessage(FinalizeGame(e));
                EndGame();
                return;
            }
            else
            {
                AdvanceNextTurn();
            }
        }

        //If all values in the isPlayerFinished array are true, return true.
        static bool IsGameOver()
        {
            for (int i = 0; i < isPlayerFinished.Length; i++)
            {
                if (isPlayerFinished[i] == false && !string.IsNullOrEmpty(currentPlayers[i]))
                {
                    return false;
                }
            }
            return true;
        }

        //Moves it to the next player's turn, taking into account who is already finished.
        static void AdvanceNextTurn()
        {
            do
            {
                if (currentPlayerIndex == 7)
                {
                    currentPlayerIndex = 0;
                }
                else
                {
                    currentPlayerIndex++;
                }
            } while (string.IsNullOrEmpty(currentPlayers[currentPlayerIndex]) || isPlayerFinished[currentPlayerIndex]);
        }

        //Returns whose turn is going to be up next, but does not actually change the value.
        static string GetNextTurn()
        {
            if (IsGameOver())
            {
                return " ";
            }

            int temp = currentPlayerIndex;
            do
            {
                if (temp == 7)
                {
                    temp = 0;
                }
                else
                {
                    temp++;
                }
            } while (string.IsNullOrEmpty(currentPlayers[temp]) || isPlayerFinished[temp]);
            return "It is " + currentPlayers[temp] + "'s turn now.";
        }

        //Returns true if the player sending the message is up next.
        static bool IsPlayersTurn(MessageEventArgs e)
        {
            if (currentPlayerIndex != -1 && e.Message.User.Discriminator == currentPlayersID[currentPlayerIndex])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Returns the index of the player sending the message.
        static int GetPlayerIndex(MessageEventArgs e)
        {
            for (int i = 0; i < currentPlayersID.Length; i++)
            {
                if (currentPlayersID[i] == e.Message.User.Discriminator)
                {
                    return i;
                }
            }
            return -1;
        }

        //Resets all variables to their default.
        static void EndGame()
        {
            bj_isGameActive = false;
            bj_channelID = -1;
            hasGameStarted = false;
            pull2 = false;
            hasAce = false;
            Array.Clear(currentPlayers, 0, currentPlayers.Length);
            Array.Clear(currentPlayersID, 0, currentPlayersID.Length);
            ResetArrays();
            currentPlayerIndex = -1;
        }
    }
}
