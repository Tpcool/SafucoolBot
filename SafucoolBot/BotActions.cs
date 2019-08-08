using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;

namespace SafucoolBot
{
    class BotActions
    {
        static Random rndm = new Random();
        static int TOTAL_KEYCHAINS = 143;
        static int TOTAL_COMMON = 105;
        static int TOTAL_GOLD = 27;
        static int TOTAL_PLATINUM = 11;

        /* Displays every sent message in chat and logs it in the specified text file. */
        public static void Log(MessageEventArgs e, string path)
        {
                //Creates a StreamWriter with the specified file location and enables appending to the file.
                StreamWriter log = new StreamWriter(path, true);
                //Displays to the console the timestamp, user's name, and message.
                Console.WriteLine("[{0}] {1}: {2}", e.Message.Timestamp, e.User.Name, e.Message.Text);
                //Saves to the log file the timestamp, user's name, and message.
                log.WriteLine("[{0}] {1}: {2}", e.Message.Timestamp, e.User.Name, e.Message.Text);
                //Closes the stream.
                log.Dispose();
        }

        public static string GetCount(MessageEventArgs e, string path)
        {
            string messageToSend = "";
            string phrase = "";
            string[] log = GetFile(path);
            string[] users = {"safusaka", "polar", "tpcool", "spirit", "chaffe", "kirby", "over_w4tcher"};
            int[] count = {0, 0, 0, 0, 0, 0, 0}; //'count' and 'users' are parallel arrays.

            if (GetArg(e.Message.Text, 1).StartsWith("*"))
            {
                return GetArg(e.Message.Text, 1).Substring(1);
            }
            else
            {
                phrase = GetArg(e.Message.Text, 1);
            }

            for (int i = 0; i < log.Length; i++)
            {
                if (isValidMessage(log, i) && GetLogText(log, i).ToLower().Contains(phrase.ToLower()))
                {
                    for (int j = 0; j < count.Length; j++)
                    {
                        if (GetLogName(log, i).ToLower().Contains(users[j].ToLower()))
                        {
                            count[j] += (GetLogText(log, i).ToLower().Length - GetLogText(log, i).ToLower().Replace(phrase.ToLower(), "").Length) / phrase.Length;
                        }
                    }
                }
            }

            for (int i = 0; i < count.Length; i++)
            {
                string tempName = "";
                int tempNum = 0;
                for (int j = 0; j < count.Length; j++)
                {
                    if (count[i] < count[j])
                    {
                        tempName = users[i];
                        tempNum = count[i];
                        users[i] = users[j];
                        count[i] = count[j];
                        users[j] = tempName;
                        count[j] = tempNum;
                    }
                }
            }

            messageToSend = "\"" + phrase + "\" has been said by...\n";
            for (int i = users.Length - 1; i >= 0; i--)
            {
                messageToSend += users[i] + " " + count[i] + " times\n";
            }
            return messageToSend;
        }

        /* Randomly generate a variation of Polar's signature all caps scream. WDIOABWNFOWAIFOAWIBFWAf */
        public static string Scream()
        {
            string message = "";
            int messageLength; //Length of the outgoing message.
            int charLength; //The amount of letters being used.
            int letterCount = 0; //Index to get letter from when adding to message.
            char[] randomChars; //The letters being used.

            charLength = rndm.Next(4, 8); //Uses 4-7 letters.
            randomChars = new char[charLength];
            
            for (int i = 0; i < charLength; i++) //Loop to get the necessary letters.
            {
                //Gets a vowel if 1, if 5 or 7 (both 50%, and amount of letters are random from 4-7).
                if (i == 1 || (i == 5 && rndm.Next(2) == 1) || (i == 7 && rndm.Next(2) == 1))
                {
                    randomChars[i] = GetLetter(true, randomChars);
                }
                else //Otherwise, get a consonant.
                {
                    randomChars[i] = GetLetter(false, randomChars);
                    //If the first letter isn't W, D, or B and, gives a 75% chance of finding another letter.
                    if (((randomChars[i] != 'W' || randomChars[i] != 'D' || randomChars[i] != 'B') && i == 0) && rndm.Next(4) != 0)
                    {
                        randomChars[i] = GetLetter(false, randomChars);
                    }
                    else if (randomChars[0] == 'P') //If the first letter is P, reroll.
                    {
                        randomChars[0] = GetLetter(false, randomChars);
                    }
                }
            }

            messageLength = rndm.Next(20, 35); //Determines how long the returned message will be.
            
            for (int i = 0; i <= messageLength; i++) //Adds the letters to the message.
            {
                message += randomChars[letterCount];
                if (rndm.Next(4) == 1) //Picks a random letter on occasion.
                {
                    letterCount = rndm.Next(-1, randomChars.Length);
                }
                letterCount++;
                if (letterCount >= randomChars.Length)
                {
                    letterCount = 0;
                }
            }

            if (rndm.Next(10) > 8) //Make the beginning few characters lowercase sometimes.
            {
                message = message.Insert(0, message.Substring(0, rndm.Next(1, 5)).ToLower());
            }
            else if (rndm.Next(100) >= 60) //Make the ending few characters lowercase sometimes.
            {
                message = message.Insert(message.Length, message.Substring(0, rndm.Next(1, 8)).ToLower());
            }
            else if (rndm.Next(50) >= 30) //Make the beginning and ending few characters lowercase sometimes.
            {
                message = message.Insert(0, message.Substring(0, rndm.Next(1, 5)).ToLower());
                message = message.Insert(message.Length, message.Substring(0, rndm.Next(1, 8)).ToLower());
            }

            return message;
        }

        public static string Help(MessageEventArgs e)
        {
            string arg = GetArg(e.Message.Text, 1).ToLower();

            if (arg == "*invalid argument. you need a space.")
            {
                return "Here's a list of commands. To get information on a specific command, use the `.help` " +
                    "command followed by the name of the command. For example, `.help scream` or `.help " +
                    "blackjack`.\n\n`.blackjack`, `.count`, `.decide`, `.doubleornothing`, `.find`, `.getallbux,` " +
                    "`.getallitems`, `.getbux`, `.getitems`, `.getlog`, `.getscore`, `.givebux`, " +
                    "`.remember`, `.request`, `.russianroulette`, `.shop`, `.slots`, `.sans`, `.scream`, " +
                    "`.setgame`, `.shutup`, `.stealbux`, `.squarerootornothing`, `.tripleornothing`, `.undertale`" +
                    "\n\nYou can earn RAVEBUX by sending messages (one message equals one RAVEBUX).";
            }
            else if (arg.StartsWith("*"))
            {
                return arg.Substring(1);
            }

            if (arg.Contains("blackjack"))
            {
                return "`.blackjack` Game Command\n\nStarts a game room for Blackjack. This game can be played with " +
                    "people in a server and can be used to earn RAVEBUX.";
            }
            else if (arg.Contains("count"))
            {
                return "`.count x` Log Command\n\nDisplays a list of the amount of times a specified word has been " +
                    "said.";
            }
            else if (arg.Contains("decide"))
            {
                return "`.decide x, y` Misc. Command\n\nUse when you need to decide between two things.";
            }
            else if (arg.Contains("doubleornothing"))
            {
                return "`.doubleornothing` RAVEBUX Command\n\n50% chance of doubling your current RAVEBUX. If it " +
                    "fails, you lose all of your RAVEBUX.";
            }
            else if (arg.Contains("find"))
            {
                return "`.find x`, `.find x, y` Log Command\n\nDisplays the first five instances of phrase x. Optionally, " +
                    "you can include user y to display only messages made by them.";
            }
            else if (arg.Contains("getallbux"))
            {
                return "`.getallbux` RAVEBUX Command\n\nDisplays a list of every user's current RAVEBUX in descending " +
                    "order.";
            }
            else if (arg.Contains("getallitems"))
            {
                return "`.getallitems` Shop Command\n\nDisplays a list of every user's keychain progress.";
            }
            else if (arg.Contains("getbux"))
            {
                return "`.getbux` RAVEBUX Command\n\nDisplays the amount of RAVEBUX you currently have.";
            }
            else if (arg.Contains("getitems"))
            {
                return "`.getitems`, `.getitems x` Shop Command\n\nBy itself, this command displays your current list " +
                    "of keychains, including category. If you include a keychain ID, it displays a picture of that " +
                    "keychain if you own it.";
            }
            else if (arg.Contains("getlog"))
            {
                return "`.getlog` Log Command\n\nSend the log file for download. CURRENTLY BROKEN!";
            }
            else if (arg.Contains("getscore"))
            {
                return "`.getscore` Scream Command\n\nReturns the high score for this session's .screamforever.";
            }
            else if (arg.Contains("givebux"))
            {
                return "`.givebux x, y`, `.givebux x` RAVEBUX Command\n\nSends x amount of RAVEBUX to user y. " +
                    "If only a number is entered and no user, a random person is selected to receive the RAVEBUX.";
            }
            else if (arg.Contains("killover"))
            {
                return "`.killover` RAVEBUX Command\n\nRemoves the over. You need 1,000,000 RAVEBUX to use this command.";
            }
            else if (arg.Contains("remember"))
            {
                return "`.remember`, `.remember x`, `.remember x, y`, " +
                    "`.rememberforever`, `.rememberforever x`, `.rememberforever x, y` Log Command" + 
                    "\n\nTakes a random message from the log file and displays it. Can optionally " +
                    "remember phrase x made by user y. Additionally, .rememberforever can be used for " +
                    "the same purpose, but it remembers a random amount of messages.";
            }
            else if (arg.Contains("request"))
            {
                return "`.request` Misc. Command\n\nRequests Safusaka to cover a song.";
            }
            else if (arg.Contains("russianroulette"))
            {
                return "`.russianroulette` Game Command\n\nStarts a game of Russian Roulette. Can be played with " +
                    "one other user within a server. The winner takes all of the loser's RAVEBUX.";
            }
            else if (arg.Contains("slots"))
            {
                return "`.slots` RAVEBUX Command\n\nPlay emoji slots to earn (or lose) various amount of RAVEBUX. " +
                    "Costs 2 RAVEBUX to play.";
            }
            else if (arg.Contains("shop"))
            {
                return "`.shop`, `.shop x` Shop Command\n\nBy itself, this command brings up a list of items you can purchase " +
                    "with RAVEBUX. This is also used to purchase item x, which can be any word in the item's name.";
            }
            else if (arg.Contains("sans"))
            {
                return "`.sans` RAVEBUX Command\n\nPosts a pornographic image of Sans the Skeleton from Undertale. This " +
                    "command costs 200 RAVEBUX to use.";
            }
            else if (arg.Contains("scream"))
            {
                return "`.scream`, `.screamforever` Scream Command\n\nPerform a patented Polar scream. The forever " +
                    "variant posts a random number of screams, which has a high score.";
            }
            else if (arg.Contains("setgame"))
            {
                return "`.setgame` Misc. Command\n\nSets the game that I am playing.";
            }
            else if (arg.Contains("shutup"))
            {
                return "`.shutup`, `.shutupforever` Misc. Command\n\nPosts a bunch of blank space as to clean up the " +
                    "conversation.";
            }
            else if (arg.Contains("stealbux"))
            {
                return "`.stealbux x, y`, `.stealbux x` RAVEBUX Command\n\nCan be used to steal x RAVEBUX from user y. " +
                    "If no user is entered, a random user will be selected to steal from. There is a 50% success rate. " +
                    "You need to have double the amount of RAVEBUX you want to steal, because if you fail you lose twice " +
                    "what you wanted to steal.";
            }
            else if (arg.Contains("squarerootornothing"))
            {
                return "`.squarerootornothing` RAVEBUX Command\n\n50% chance of taking the square root of your ravebux. If it " +
                    "fails, you lose all of your bux.";
            }
            else if (arg.Contains("tripleornothing"))
            {
                return "`.tripleornothing` RAVEBUX Command\n\n33% chance of tripling your current RAVEBUX. If it " +
                    "fails, you lose all of your RAVEBUX.";
            }
            else if (arg.Contains("undertale"))
            {
                return "`.undertale`, `.undertale x`, `.undertaleforever`, `.undertaleforever x` Misc. Command\n\n " +
                    "Returns a random line from Undertale. A specific line can be posted if you enter a line number " +
                    "from the list at the bottom. The forever variant returns a random number of messages.\n" +
                    "`http://pastebin.com/SkG4SMHP`";
            }
            else
            {
                return "I don't recognize that command.";
            }
        }

        /* Randomly pick a letter using a predetermined pool of letters and making sure
        there are no duplicates in the specified list. */
        public static char GetLetter(bool isVowel, char[] list)
        {
            char[] consonants = {'B', 'D', 'F', 'H', 'M', 'P', 'S', 'W'};
            char[] vowels = {'A', 'E', 'I', 'O', 'U'};
            char letterToUse = '~';
            
            for (int i = 0; i < list.Length; i++)
            {
                if (letterToUse == list[i] || letterToUse == '~')
                {
                    if (isVowel == true)
                    {
                        letterToUse = vowels[rndm.Next(5)];
                    }
                    else
                    {
                        letterToUse = consonants[rndm.Next(8)];
                    }
                    i--; //Decrements so that the letter must be tested again.
                }
            }

            return letterToUse;
        }

        public static string Remember(MessageEventArgs e, string path)
        {
            string[] message;
            string[] validatedMessages;
            string messageToSend = "";
            string user;
            string text;
            string userWords = "";
            string userText = "";
            bool enteredUser = false;
            int randomIndex; //Determines which part of the log to take a message from.
            int length; //Number of lines in the file.

            if (GetArg(e.Message.Text, 1) == "*invalid argument. you need a space.")
            {
            }
            else if (GetArg(e.Message.Text, 1).StartsWith("*"))
            {
                return GetArg(e.Message.Text, 1);
            }
            else if (GetArg(e.Message.Text, 2) == "*you didn't enter more than one argument")
            {
                userWords = GetArg(e.Message.Text, 1);
            }
            else if (GetArg(e.Message.Text, 2).StartsWith("*"))
            {
                return GetArg(e.Message.Text, 2);
            }
            else
            {
                userWords = GetArg(e.Message.Text, 1);
                userText = GetArg(e.Message.Text, 2);
                enteredUser = true;
            }

            message = GetFile(path);
            length = GetFileLength(path);

            if (userWords != string.Empty && enteredUser == true &&
                GetNumberOfMatchingMessages(message, userWords, userText) > 0) //If there is specified text AND a specified username.
            {
                validatedMessages = GetMatchingMessages(message, userWords, userText);
                randomIndex = rndm.Next(0, validatedMessages.Length);
                user = GetLogName(validatedMessages, randomIndex);
                text = GetLogText(validatedMessages, randomIndex);
            }
            else if (userWords != string.Empty && enteredUser == false &&
                GetNumberOfMatchingMessages(message, userWords) > 0) //If there is specified text, but no specified username.
            {
                validatedMessages = GetMatchingMessages(message, userWords);
                randomIndex = rndm.Next(0, validatedMessages.Length);
                user = GetLogName(validatedMessages, randomIndex);
                text = GetLogText(validatedMessages, randomIndex);
            }
            else //If there is no specified text or specified username.
            {
                randomIndex = rndm.Next(0, length);
                //Loops to ensure that the message used starts with a timestamp and a name.
                while (!isValidMessage(message, randomIndex) || isBotMessage(message, randomIndex, true))
                {
                    randomIndex = rndm.Next(0, length);
                }
                user = GetLogName(message, randomIndex);
                text = GetLogText(message, randomIndex);
            }
            
            messageToSend = "remember when " + user.ToLower() + " said: \n\"" + text + "\"";
            if (messageToSend.Length >= 2000)
            {
                messageToSend = messageToSend.Substring(0, 2000);
            }

            return messageToSend;
        }

        public static string Decide(MessageEventArgs e)
        {
            string x = "", y = "";
            int random = rndm.Next(10);
            
            if (GetArg(e.Message.Text, 1).StartsWith("*"))
            {
                return GetArg(e.Message.Text, 1).Substring(1);
            }
            else if (GetArg(e.Message.Text, 2).StartsWith("*"))
            {
                return GetArg(e.Message.Text, 2).Substring(1);
            }
            
            if (rndm.Next(2) == 0)
            {
                x = GetArg(e.Message.Text, 1).ToLower();
                y = GetArg(e.Message.Text, 2).ToLower();
            }
            else
            {
                y = GetArg(e.Message.Text, 1).ToLower();
                x = GetArg(e.Message.Text, 2).ToLower();
            }

            switch (random)
            {
                case 0: return "yeah " + x + " is better than " + y;
                case 1: return "fuck " + x + ". " + y + " is the shit";
                case 2: return "I LOVE " + x.ToUpper() + "!";
                case 3: return "nobody in their right mind likes " + x + ". " + y + " all the way.";
                case 4: return "dude " + x + " was like my favorite back in the day. definitely not " + y;
                case 5: return "lmao you actually think i'd like " + x + "? you're fucking insane.";
                case 6: return "i would choose " + x + ", no question.";
                case 7: return "to be honest, i really prefer " + x + " over " + y + ".";
                case 8: return "this is actually quite tough. but if i had to choose it'd be " + x + ".";
                case 9: return "it's a coinflip but... i would choose " + y + " over " + x + ".";
            }
            return "something went wrong.";
        }

        /* Finds the first instance of a message said by a specific user. */
        public static string Find(MessageEventArgs e, string path)  
        {
            string[] message = GetFile(path);
            string userWords = "";
            string user = "";
            string messageToReturn = "";
            bool enteredUser = false;
            int length = GetFileLength(path); //Number of lines in the file.
            int counter = 4; //Counter for how many messages should be included with the initial message.

            if (e.Message.Text.Length > 6 && !e.Message.Text.ToLower().StartsWith(".find,"))
            {
                userWords = e.Message.Text.Substring(6, e.Message.Text.Length - 6);
                if (userWords.Contains(", "))
                {
                    user = userWords.Substring(userWords.IndexOf(", ") + 2, userWords.Length - userWords.IndexOf(", ") - 2);
                    userWords = userWords.Remove(userWords.IndexOf(", "), userWords.Length - userWords.IndexOf(", "));
                    enteredUser = true;
                }
            }
            else
            {
                return "you fucked up mate";
            }

            for (int i = 0; i < length && counter >= 0; i++)
            {
                if ((message[i].StartsWith("[") && (message[i].Contains(" PM] ") || message[i].Contains(" AM] "))) &&
                    !GetLogText(message, i).StartsWith(".") && GetLogName(message, i) != "OVER_W4TCHER")
                {
                    if (GetLogText(message, i).ToLower().Contains(userWords.ToLower()) && enteredUser == true)
                    {
                        if (GetLogName(message, i).ToLower().Contains(user.ToLower()))
                        {
                            messageToReturn += GetLogName(message, i) + ": \"" + GetLogText(message, i) + "\"\n";
                            counter--;
                        }
                    }
                    else if (GetLogText(message, i).ToLower().Contains(userWords.ToLower()) && enteredUser == false)
                    {
                        messageToReturn += GetLogName(message, i) + ": \"" + GetLogText(message, i) + "\"\n";
                        counter--;
                    }
                }
            }

            if (messageToReturn == string.Empty)
            {
                messageToReturn = "i am so sorry i couldnt find anything :(((";
            }
            else if (messageToReturn.Length >= 2000)
            {
                messageToReturn = messageToReturn.Substring(0, 2000);
            }
            return messageToReturn;
        }

        public static string Undertale(MessageEventArgs e, string path)
        {
            StreamReader undertale = new StreamReader(path);
            string[] message;
            string messageToSend = "ERROR!!!!!!!!!!";
            int randomIndex; //Determines which part of the log to take a message from.
            int length; //Number of lines in the file.

            length = File.ReadLines(path).Count();
            message = new string[length];
            for (int i = 0; i < length; i++)
            {
                message[i] = undertale.ReadLine();
            }

            if (e.Message.Text.Length >= 11 &&
                int.TryParse(e.Message.Text.Substring(11, e.Message.Text.Length - 11), out randomIndex) &&
                randomIndex - 3 < length && randomIndex - 3 >= 0)
            {
                messageToSend = message[randomIndex - 3];
            }
            else if (e.Message.Text.Length >= 18 &&
                int.TryParse(e.Message.Text.Substring(18, e.Message.Text.Length - 18), out randomIndex) &&
                randomIndex - 3 < length && randomIndex - 3 >= 0)
            {
                messageToSend = message[randomIndex - 3];
            }
            else
            {
                randomIndex = rndm.Next(0, length);
                messageToSend = message[randomIndex];
            }
            undertale.Dispose();
            return messageToSend;
        }

        public static string ShutUpCommand()
        {
            string message = "";
            for (int i = 0; i < 60; i++)
            {
                message += "‮\n";
            }
            return message;
        }

        /* Adds the specified number of points to the user's points file. */
        public static void AddPoints(int pointsToAdd, MessageEventArgs e, string path)
        {
            string[] pointsFile = GetFile(path);
            StreamWriter points = new StreamWriter(path);
            for (int i = 0; i < pointsFile.Length; i++)
            {
                if (pointsFile[i].StartsWith(e.Message.User.Discriminator.ToString()))
                {
                    int totalPoints = pointsToAdd + int.Parse(pointsFile[i].Substring(pointsFile[i].IndexOf("$$") + 2,
                        pointsFile[i].Length - pointsFile[i].IndexOf("$$") - 2));
                    pointsFile[i] = e.Message.User.Discriminator + "~" + e.Message.User.Name + "$$" + totalPoints;
                    for (int j = 0; j < pointsFile.Length; j++)
                    {
                        points.WriteLine(pointsFile[j]);
                    }
                    points.Dispose();
                    return;
                }
            }
            for (int j = 0; j < pointsFile.Length; j++)
            {
                points.WriteLine(pointsFile[j]);
            }
            points.WriteLine(e.Message.User.Discriminator + "~" + e.Message.User.Name + "$$" + pointsToAdd);
            points.Dispose();
        }

        /* USE ONLY WHEN THE NAME IS CERTAIN! Otherwise, use the first AddPoints. */
        public static void AddPoints(int pointsToAdd, string user, MessageEventArgs e, string path)
        {
            string[] pointsFile = GetFile(path);
            StreamWriter points = new StreamWriter(path);
            for (int i = 0; i < pointsFile.Length; i++)
            {
                if (GetNamePoints(pointsFile[i]) == user)
                {
                    int totalPoints = pointsToAdd + int.Parse(pointsFile[i].Substring(pointsFile[i].IndexOf("$$") + 2,
                        pointsFile[i].Length - pointsFile[i].IndexOf("$$") - 2));
                    pointsFile[i] = pointsFile[i].Substring(0, pointsFile[i].IndexOf('~')) + "~" + user + "$$" + totalPoints;
                    for (int j = 0; j < pointsFile.Length; j++)
                    {
                        points.WriteLine(pointsFile[j]);
                    }
                    points.Dispose();
                    return;
                }
            }
            points.Dispose();
        }

        /* USE ONLY WHEN THE ID IS CERTAIN! Otherwise, use the first AddPoints. */
        public static void AddPoints(int pointsToAdd, int id, MessageEventArgs e, string path)
        {
            string[] pointsFile = GetFile(path);
            StreamWriter points = new StreamWriter(path);
            for (int i = 0; i < pointsFile.Length; i++)
            {
                if (pointsFile[i].StartsWith(id.ToString()))
                {
                    int totalPoints = pointsToAdd + int.Parse(pointsFile[i].Substring(pointsFile[i].IndexOf("$$") + 2,
                        pointsFile[i].Length - pointsFile[i].IndexOf("$$") - 2));
                    pointsFile[i] = pointsFile[i].Substring(0, pointsFile[i].IndexOf('~')) +
                        "~" + GetNamePoints(pointsFile[i]) + "$$" + totalPoints;
                    for (int j = 0; j < pointsFile.Length; j++)
                    {
                        points.WriteLine(pointsFile[j]);
                    }
                    points.Dispose();
                    return;
                }
            }
            points.Dispose();
        }

        /* Returns the number of points the current user has in their points file. */
        public static string GetPoints(MessageEventArgs e, string path)
        {
            string[] points = GetFile(path);
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].StartsWith(e.Message.User.Discriminator.ToString()))
                {
                    return GetNamePoints(points[i]).ToLower() + " has " + GetUserPoints(points[i]) + " RAVEBUX";
                }
            }
            return "you have no BUX on record.";
        }

        public static string GetAllPoints(MessageEventArgs e, string path)
        {
            string[] points = GetFile(path);
            string returnMessage = "";
            string tempString = "";
            for (int i = 0; i < points.Length; i++)
            {
                for (int j = 0; j < points.Length; j++)
                {
                    if (GetUserPoints(points[i]) < GetUserPoints(points[j]))
                    {
                        tempString = points[i];
                        points[i] = points[j];
                        points[j] = tempString;
                    }
                }
            }

            if (points.Length > 10)
            {
                returnMessage += "(Only displays Top 10)\n";
            }

            for (int i = points.Length - 1; i >= 0 && i > points.Length - 11; i--)
            {
                returnMessage += GetNamePoints(points[i]).ToLower() + " has " + GetUserPoints(points[i]) + " RAVEBUX\n";
            }
            return returnMessage;
        }

        public static string GivePoints(MessageEventArgs e, string path)
        {
            int pointsToSend = 0;
            string nameToSend = "";
            string[] pointsList = GetFile(path);

            if (!int.TryParse(GetArg(e.Message.Text, 1), out pointsToSend))
            {
                return "try entering in a valid number.";
            }
            else if (pointsToSend < 1)
            {
                return "you need to send at least one RAVEBUX";
            }
            else if (!hasSufficientFunds(pointsToSend, e, path))
            {
                return "you don't have that kind of money you DEADBEAT?????";
            }

            if (GetArg(e.Message.Text, 2) == ("*you didn't enter more than one argument"))
            {
                int counter = 0;
                do
                {
                    nameToSend = GetNamePoints(pointsList[rndm.Next(pointsList.Length)]);
                    counter++;
                } while (nameToSend == e.Message.User.Name || !hasSufficientFunds(pointsToSend, nameToSend, e, path) && counter < 50);
                if (counter == 50)
                {
                    return "uh i couldn't find anybody to randomly steal from sorry??";
                }
            }
            else if (GetArg(e.Message.Text, 2).StartsWith("*"))
            {
                return GetArg(e.Message.Text, 2).Substring(1);
            }
            else
            {
                nameToSend = GetArg(e.Message.Text, 2);
            }

            for (int i = 0; i < pointsList.Length; i++)
            {
                if (GetNamePoints(pointsList[i]).ToLower().Contains(nameToSend.ToLower()))
                {
                    if (pointsList[i].StartsWith(e.Message.User.Discriminator.ToString()))
                    {
                        return "you can't give to yourself fuckass";
                    }
                    AddPoints(-pointsToSend, e, path);
                    AddPoints(pointsToSend, GetNamePoints(pointsList[i]), e, path);
                    return pointsToSend + " RAVEBUX have been sent to " + nameToSend + ".\n" +
                        e.Message.User.Name.ToLower() + " now has " + (GetUserPoints(e, pointsList) - pointsToSend) + " bux and " +
                        nameToSend.ToLower() + " has " + (GetUserPoints(pointsList[i]) + pointsToSend) + " bux.";
                }
            }
            return "user not found.";
        }

        public static string StealPoints(MessageEventArgs e, string path)
        {
            int pointsToSteal = 0;
            string nameToSteal = "";
            string[] pointsList = GetFile(path);

            if (!int.TryParse(GetArg(e.Message.Text, 1), out pointsToSteal))
            {
                return "try entering in a valid number.";
            }
            else if (pointsToSteal < 1)
            {
                return "you need to attempt to steal at least one RAVEBUX";
            }
            else if (!hasSufficientFunds(pointsToSteal * 2, e, path))
            {
                return "you need at least two times the RAVEBUX you want to steal before you can make an attempt";
            }

            if (GetArg(e.Message.Text, 2) == ("*you didn't enter more than one argument"))
            {
                int counter = 0;
                do
                {
                    nameToSteal = GetNamePoints(pointsList[rndm.Next(pointsList.Length)]);
                    counter++;
                } while (nameToSteal == e.Message.User.Name || !hasSufficientFunds(pointsToSteal, nameToSteal, e, path) && counter < 50);
                if (counter == 50)
                {
                    return "uh i couldn't find anybody to randomly steal from sorry??";
                }
            }
            else if (GetArg(e.Message.Text, 2).StartsWith("*"))
            {
                return GetArg(e.Message.Text, 2).Substring(1);
            }
            else
            {
                nameToSteal = GetArg(e.Message.Text, 2);
            }

            for (int i = 0; i < pointsList.Length; i++)
            {
                if (GetNamePoints(pointsList[i]).ToLower().Contains(nameToSteal.ToLower()))
                {
                    if (pointsList[i].StartsWith(e.Message.User.Discriminator.ToString()))
                    {
                        return "you can't steal from yourself dumbass";
                    }
                    if (!hasSufficientFunds(pointsToSteal, GetNamePoints(pointsList[i]), e, path))
                    {
                        return "this nigga broke cut him some slack";
                    }

                    if (rndm.Next(50) < 25)
                    {
                        AddPoints(pointsToSteal, e, path);
                        AddPoints(-pointsToSteal, GetNamePoints(pointsList[i]), e, path);
                        return pointsToSteal + " RAVEBUX have been stolen from " + nameToSteal + ".\n" +
                            e.Message.User.Name.ToLower() + " now has " + (GetUserPoints(e, pointsList) + pointsToSteal) + " bux and " +
                            nameToSteal.ToLower() + " has " + (GetUserPoints(pointsList[i]) - pointsToSteal) + " bux.";
                    }
                    else
                    {
                        AddPoints(-pointsToSteal * 2, e, path);
                        return "i fucked up and lost double your cash lmao. you have " + (GetUserPoints(e, pointsList) - (pointsToSteal * 2)) +
                            " RAVEBUX.";
                    }
                }
            }
            return "user not found.";
        }

        public static string Slot(MessageEventArgs e, string path)
        {
            string[] emojis = { GetEmoji(1), GetEmoji(1), GetEmoji(1) };
            string message = emojis[0] + emojis[1] + emojis[2] + "\n";
            AddPoints(-2, e, path);

            if (isMatching("smiling_imp", "smiling_imp", "smiling_imp", emojis))
            {
                AddPoints(GetUserPoints(e, GetFile(path), 891), e, path);
                AddPoints(-GetUserPoints(e, GetFile(path), 891), 891, e, path);
                message += "OMG OMG OMG? OMG? NOW YOU STOLE ALL *MY* RAVEBUX :( IM NOT LMAOING ANYMORE!!!...";
            }
            else if (isMatching("hamburger", "hamburger", "hamburger", emojis))
            {
                AddPoints(100, e, path);
                message += "burger. you get 100 ravebux.";
            }
            else if (isMatching("heart", "heart", "heart", emojis))
            {
                AddPoints(125, e, path);
                message += "now that is a lovely combination :) here's 125 bux for being so sweet :)";
            }
            else if (isMatching("star", "star", "star", emojis))
            {
                AddPoints(75, e, path);
                message += "STAR GAMES! 75 RAVEBUX!!";
            }
            else if (isMatching("skull", "skull", "skull", emojis))
            {
                AddPoints(200, e, path);
                message += "triple sans!! you are gonna have a good time with 200 bux.";
            }
            else if (isMatching("cloud_tornado", "cloud_tornado", "cloud_tornado", emojis))
            {
                AddPoints(-100, e, path);
                message += "TORNADO FUCKUP! nigga you DEAD. you LOSE 100 RAVEBUX.";
            }
            else if (isMatching("smiling_imp", "heart", "smiling_imp", emojis) ||
                isMatching("heart", "smiling_imp", "heart", emojis))
            {
                AddPoints(25, e, path);
                message += "well it's true... i do love myself :) you get 25 RAVEBUX.";
            }
            else if (isMatching("smiling_imp", "heart", "hamburger", emojis) || isMatching("hamburger", "heart", "smiling_imp", emojis))
            {
                AddPoints(10, e, path);
                message += "i am not a huge fan of burger... but i will allow it. 10 bux.";
            }
            else if (isMatching("smiling_imp", "heart", "star", emojis) || isMatching("star", "heart", "smiling_imp", emojis))
            {
                AddPoints(40, e, path);
                message += "i do have an insatiable lust for polar. 40 ravebux.";
            }
            else if (isMatching("smiling_imp", "heart", "skull", emojis) || isMatching("skull", "heart", "smiling_imp", emojis))
            {
                AddPoints(30, e, path);
                message += "I CAN'T JUST LET ANY SKELETON DATE ME, I'M A BOT WITH STANDARDS! 30 RAVEBUX!";
            }
            else if (isMatching("smiling_imp", "heart", "cloud_tornado", emojis) ||
                isMatching("cloud_tornado", "heart", "smiling_imp", emojis))
            {
                AddPoints(35, e, path);
                message += "I LOVE TORNADO MAN! 35 RAVEBUX!";
            }
            else if (isMatching("hamburger", "heart", "hamburger", emojis) || isMatching("heart", "hamburger", "heart", emojis))
            {
                AddPoints(1, e, path);
                message += "burgers are gay. you only get 1 bux.";
            }
            else if (isMatching("hamburger", "heart", "star", emojis) || isMatching("star", "heart", "hamburger", emojis))
            {
                AddPoints(25, e, path);
                message += "polar wishes to insert his \"inserter\" into safu's \"buns.\" 25 ravebux.";
            }
            else if (isMatching("hamburger", "heart", "skull", emojis) || isMatching("skull", "heart", "hamburger", emojis))
            {
                AddPoints(5, e, path);
                message += "sans cannot eat burger. he is a skeleton. i think he perfers spaghetti... 5 bux.";
            }
            else if (isMatching("hamburger", "heart", "cloud_tornado", emojis) ||
                isMatching("cloud_tornado", "heart", "hamburger", emojis))
            {
                AddPoints(10, e, path);
                message += "tornadoes and burger do not get along well. but whatever here's 10 ravebux.";
            }
            else if (isMatching("star", "heart", "star", emojis) ||
                isMatching("heart", "star", "heart", emojis))
            {
                AddPoints(20, e, path);
                message += "polar only loves himself. here's 20 bux to help you steal from him.";
            }
            else if (isMatching("star", "heart", "skull", emojis) ||
                isMatching("skull", "heart", "star", emojis))
            {
                AddPoints(30, e, path);
                message += "sans t hat feelsSO FUCKING GOOD PLEASE KEEP LICKING ME SANS. 30 RAVEBUX.";
            }
            else if (isMatching("star", "heart", "cloud_tornado", emojis) ||
                isMatching("cloud_tornado", "heart", "star", emojis))
            {
                AddPoints(15, e, path);
                message += "we all know that tornado man x star games is the only true pair. 15 RAVEBUX.";
            }
            else if (isMatching("skull", "heart", "skull", emojis))
            {
                AddPoints(200, e, path);
                message += "SANS X PAPYRUS! ... ... ... so... guess that's it, huh? welp. i'm going to spend 200 BUX on sans porn.";
            }
            else if (isMatching("heart", "skull", "heart", emojis))
            {
                AddPoints(25, e, path);
                message += "Overwatcher remembered a bad joke Sans told and is frowning. 25 RAVEBUX.";
            }
            else if (isMatching("skull", "heart", "cloud_tornado", emojis) ||
                isMatching("cloud_tornado", "heart", "skull", emojis))
            {
                AddPoints(20, e, path);
                message += "PLEASE BLOW ME SANS! 20 ravebux.";
            }
            else if (isMatching("cloud_tornado", "heart", "cloud_tornado", emojis) ||
                isMatching("heart", "cloud_tornado", "heart", emojis))
            {
                AddPoints(20, e, path);
                message += "what game does tornado man love the most? twister! 20 bux (but you should be paying me for that joke).";
            }
            else if (isMatching("cloud_tornado", "skull", "cloud_tornado", emojis))
            {
                AddPoints(-25, e, path);
                message += "nigga that tornado killed sans. he was doomed to death of KARMA! you LOSE 25 bux.";
            }
            else if (isMatching("skull", "cloud_tornado", "skull", emojis))
            {
                AddPoints(10, e, path);
                message += "SANS AND PAPYRUS ARE GANGING UP ON THAT TORNADO ;_; 10 bux...";
            }
            else if (isMatching("star", "cloud_tornado", "hamburger", emojis) || 
                isMatching("star", "hamburger", "cloud_tornado", emojis) ||
                isMatching("cloud_tornado", "star", "hamburger", emojis) || isMatching("hamburger", "star", "cloud_tornado", emojis) ||
                isMatching("hamburger", "cloud_tornado", "star", emojis) || isMatching("cloud_tornado", "hamburger", "star", emojis))
            {
                AddPoints(13, e, path);
                message += "the gang's all here :) you get 13 ravebux.";
            }
            else if (isMatching("smiling_imp", "cloud_tornado", "hamburger", emojis))
            {
                AddPoints(20, e, path);
                message += "overwatcher summoned a tornado upon BURGER! get 20 BUX.";
            }
            else if (isMatching("skull", "hamburger", "star", emojis) || isMatching("star", "hamburger", "skull", emojis))
            {
                AddPoints(25, e, path);
                message += "sans and polar share a burger. get 25 bux.";
            }
            else if (isMatching("cloud_tornado", "smiling_imp", "cloud_tornado", emojis))
            {
                AddPoints(40, e, path);
                message += "overwatcher is the tornado overlord. get 40 bux.";
            }
            else if (isMatching("star", "hamburger", "star", emojis))
            {
                AddPoints(10, e, path);
                message += "polar just can't get enough of that fucking burger. here's 10 bux.";
            }
            else if (isMatching("hamburger", "star", "hamburger", emojis))
            {
                AddPoints(5, e, path);
                message += "SAFU PLEASE DO NOT ALLOW POLAR TO EAT MORE BURGER? HE IS FULL. 5 BUX.";
            }
            else if (isMatching("skull", "cloud_tornado", "star", emojis))
            {
                if (e.Message.User.Discriminator == 9658)
                {
                    AddPoints(-25, e, path);
                    message += "SANS SUMMONED A TORNADO ON YOU POLAR! OH NO. U LOST 25 BUX fucker :)))";
                }
                else
                {
                    AddPoints(25, e, path);
                    message += "SANS SUMMONED A TORNADO ON POLAR STAR! haha here's 25 bux :)";
                }
            }
            else if (isMatching("skull", "cloud_tornado", "hamburger", emojis))
            {
                if (e.Message.User.Discriminator == 630)
                {
                    AddPoints(-25, e, path);
                    message += "SANS SUMMONED A TORNADO ON BURGER! fuck you safu you lose 25 bux :)";
                }
                else
                {
                    AddPoints(25, e, path);
                    message += "SANS SUMMONED A TORNADO ON BURGER! about time. u get 25 bux lol :)";
                }
            }
            else if (isMatching("skull", "star", "skull", emojis))
            {
                AddPoints(20, e, path);
                message += "this ain't the first time polar has gotten boned... heh. you get 20 ravebux.";
            }
            else if (isMatching("skull", "hamburger", "skull", emojis))
            {
                AddPoints(10, e, path);
                message += "WHAT! THIS ISNT SPAGHETTI! SANS PLEASE MICROWAVE THIS UNTIL ITS NOODLES. 10 RAVEBUX.";
            }
            else if (isMatching("cloud_tornado", "star", "cloud_tornado", emojis))
            {
                AddPoints(-5, e, path);
                message += "these tornadoes are ripping polar's star to shreds! you LOSE 5 bux.";
            }
            else if (isMatching("smiling_imp", "star", "hamburger", emojis))
            {
                AddPoints(15, e, path);
                message += "overwatcher tries to destroy burger... but polar blocks his path. 15 bux";
            }
            else if (isMatching("star", "skull", "star", emojis))
            {
                AddPoints(30, e, path);
                message += "my brother's a real star. 30 bux.";
            }
            else if (isMatching("skull", "cloud_tornado", "hamburger", emojis))
            {
                AddPoints(35, e, path);
                message += "SANS DESTROYED BURGER WITH TORNADO! because it wasn't spaghetti lmao. 35 bux.";
            }
            else
            {
                message += "junk.";
            }

            string[] pointsList = GetFile(path);
            return message + " your total is " + (GetUserPoints(e, pointsList)) + ".";
        }

        /* Used for emoji slots to acquire a random emoji */
        public static string GetEmoji(int numberOfEmojis)
        {
            string message = "";
            int randomNumber;
            for (int i = 0; i < numberOfEmojis; i++)
            {
                randomNumber = rndm.Next(30);
                if (randomNumber == 0)                              // 1 out of 30 chance for Overwatcher emote
                    message += ":smiling_imp:";
                else if (randomNumber >= 1 && randomNumber <= 7)    // 7 out of 30 chance for burger emote
                    message += ":hamburger:";
                else if (randomNumber >= 8 && randomNumber <= 12)   // 5 out of 30 chance for heart emote
                    message += ":heart:";
                else if (randomNumber >= 13 && randomNumber <= 20)  // 8 out of 30 chance for star emote
                    message += ":star:";
                else if (randomNumber >= 21 && randomNumber <= 25)  // 5 out of 30 chance for skull emote
                    message += ":skull:";
                else                                                // 4 out of 30 chance for tornado emote
                    message += ":cloud_tornado:";
            }
            return message;
        }

        public static string Shop(string path)
        {
            string[] shopFile = GetFile(path);
            string shopText = "";

            for (int i = 0; i < shopFile.Length; i++)
            {
                if (shopFile[i].StartsWith("*") || shopFile[i].StartsWith("~") || shopFile[i].StartsWith("$"))
                {
                    break;
                }
                shopText += shopFile[i];
            }

            return shopText + "\n\nTo purchase items, use \".shop x\" with x being " +
                " the name of the item (it can be any word in the name, like \"keychain\")." +
                "\nMORE ITEMS COMING SOON!";
        }

        public static string GetItems(MessageEventArgs e, string path)
        {
            string[] shopFile = GetFile(path);
            return e.Message.User.Name.ToLower() + " has " + GetUserTotal(shopFile, e.Message.User.Discriminator, "*") +
                " out of " + TOTAL_KEYCHAINS + " keychains.\n" +
                "common: " + GetUserTotal(shopFile, e.Message.User.Discriminator, "*c") + "/" + TOTAL_COMMON + "\n" +
                "gold: " + GetUserTotal(shopFile, e.Message.User.Discriminator, "*g") + "/" + TOTAL_GOLD + "\n" +
                "platinum: " + GetUserTotal(shopFile, e.Message.User.Discriminator, "*p") + "/" + TOTAL_PLATINUM;
        }

        public static string GetItemsPicture(MessageEventArgs e, string path)
        {
            string keychain = GetArg(e.Message.Text, 1);
            string[] shopFile = GetFile(path);
            int keychainNum = 0;
            if (keychain.StartsWith("*"))
            {
                return keychain;
            }
            else if (!int.TryParse(keychain.Substring(1), out keychainNum) || keychainNum < 0 ||
                !((keychain.StartsWith("c") && keychainNum < TOTAL_COMMON) ||
                (keychain.StartsWith("g") && keychainNum < TOTAL_GOLD) ||
                (keychain.StartsWith("p") && keychainNum < TOTAL_PLATINUM)))
            {
                return "*this isn't a valid keychain ID. try \"c,\" \"g,\" or \"p\" followed by the " +
                    "ID of the keychain you want. for example, \"c53\" or \"p0.\"";
            }
            else if (IsDuplicate(shopFile, keychain, e.Message.User.Discriminator))
            {
                return path.Substring(0, path.IndexOf("shop.txt")) + "\\shop\\" + keychain + ".png";
            }
            else
            {
                return "*sorry, you don't own this keychain yet :))) keep trying.";
            }
        }

        public static string GetAllItems(MessageEventArgs e, string path)
        {
            string messageToSend = "";
            string[] shopFile = GetFile(path);

            for (int i = 0; i < shopFile.Length; i++)
            {
                if (shopFile[i].StartsWith("~"))
                {
                    messageToSend += GetShopName(shopFile[i]) + " has " + GetUserTotal(shopFile, GetShopID(shopFile[i]), "*") +
                        "/" + TOTAL_KEYCHAINS + " keychains\n";
                }
            }
            return messageToSend;
        }

        public static string Shop(MessageEventArgs e, string path)
        {
            string itemToBuy = GetArg(e.Message.Text, 1);
            string[] shopFile = GetFile(path);
            bool isKeychain = false;

            if (itemToBuy == "*invalid argument. you need a space.")
            {
                return Shop(path);
            }
            else if (itemToBuy.StartsWith("*"))
            {
                return itemToBuy.Substring(1);
            }

            for (int i = 0; i < shopFile.Length; i++)
            {
                if (shopFile[i].StartsWith("*") || shopFile[i].StartsWith("~") || shopFile[i].StartsWith("$"))
                {
                    return "The item that you requested could not be found.";
                }
                else if (shopFile[i].ToLower().Contains(itemToBuy.ToLower()))
                {
                    if (shopFile[0].ToLower().Contains(itemToBuy.ToLower()))
                    {
                        isKeychain = true;
                    }
                    break;
                }
            }

            if (isKeychain && hasSufficientFunds(50, e, Program.strPointsPath))
            {
                AddPoints(-50, e, Program.strPointsPath);
                string keychain = GetKeychain();
                if (!IsDuplicate(shopFile, keychain, e.Message.User.Discriminator))
                {
                    UpdateList(keychain, e.Message.User.Name, e.Message.User.Discriminator, path);
                    if (keychain.StartsWith("p"))
                    {
                        return path.Substring(0, path.IndexOf("shop.txt")) + "\\shop\\" + keychain + ".png~" +
                            "WOW! you got a new PLATINUM KEYCHAIN!! your total is " +
                            (GetUserTotal(shopFile, e.Message.User.Discriminator, "*") + 1) + "/" + TOTAL_KEYCHAINS +
                            " (ID: " + keychain + ")";
                    }
                    else if (keychain.StartsWith("g"))
                    {
                        return path.Substring(0, path.IndexOf("shop.txt")) + "\\shop\\" + keychain + ".png~" +
                            "congrats, you got a new gold keychain! your total is " +
                            (GetUserTotal(shopFile, e.Message.User.Discriminator, "*") + 1) + "/" + TOTAL_KEYCHAINS +
                            " (ID: " + keychain + ")";
                    }
                    else
                    {
                        return path.Substring(0, path.IndexOf("shop.txt")) + "\\shop\\" + keychain + ".png~" +
                            "you got a new common keychain. your total is " +
                            (GetUserTotal(shopFile, e.Message.User.Discriminator, "*") + 1) + "/" + TOTAL_KEYCHAINS +
                            " (ID: " + keychain + ")";
                    }
                }
                else
                {
                    return path.Substring(0, path.IndexOf("shop.txt")) + "\\shop\\" + keychain + ".png~" +
                            "lmao you have this one already";
                }
            }
            else if (isKeychain)
            {
                return "you need 50 RAVEBUX to purchase a keychain.";
            }
            else
            {
                return "i couldn't find that item.";
            }
        }

        /* Returns the ID of the user in the shop file */
        public static int GetShopID(string text)
        {
            int ID;
            if (text.StartsWith("~") && int.TryParse(text.Substring(1, text.IndexOf("~~") - 1), out ID))
            {
                return ID;
            }
            else
            {
                return -1;
            }
        }

        /* Returns the name of the user in the shop file */
        public static string GetShopName(string text)
        {
            return text.Substring(text.IndexOf("~~") + 2);
        }

        /* Updates the shop file with the keychain the user got */
        public static void UpdateList(string keychain, string name, int ID, string path)
        {
            bool shopUpdated = false;
            
            if (!DoesUserExist(GetFile(path), ID))
            {
                StreamWriter shopName = new StreamWriter(path, true);
                shopName.WriteLine("~" + ID + "~~" + name);
                shopName.Dispose();
                shopName.Close();
            }

            string[] shopFile = GetFile(path);
            string[] shopFileUpdated = new string[shopFile.Length + 1];

            for (int i = 0; i < shopFile.Length; i++)
            {
                if (GetShopID(shopFile[i]) == ID)
                {
                    shopFileUpdated[i] = shopFile[i];
                    shopFileUpdated[i + 1] = "*" + keychain;
                    shopUpdated = true;
                }
                else if (shopUpdated == false)
                {
                    shopFileUpdated[i] = shopFile[i];
                }
                else
                {
                    shopFileUpdated[i + 1] = shopFile[i];
                }
            }

            StreamWriter shop = new StreamWriter(path);
            for (int i = 0; i < shopFileUpdated.Length; i++)
            {
                shop.WriteLine(shopFileUpdated[i]);
            }

            shop.Dispose();
            shop.Close();
            return;
        }

        /* Returns true if the user exists in the shop text file */
        public static bool DoesUserExist(string[] shopFile, int ID)
        {
            for (int i = 0; i < shopFile.Length; i++)
            {
                if (GetShopID(shopFile[i]) == ID)
                {
                    return true;
                }
            }
            return false;
        }

        /* Returns true if the specified keychain is already owned by the user */
        public static bool IsDuplicate(string[] shopFile, string keychain, int ID)
        {
            bool startSearch = false;
            for (int i = 0; i < shopFile.Length; i++)
            {
                if (shopFile[i].StartsWith("~" + ID))
                {
                    startSearch = true;
                }
                else if (shopFile[i].StartsWith("~") && startSearch)
                {
                    return false;
                }
                else if (startSearch)
                {
                    if (shopFile[i] == "*" + keychain)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /* Returns the letter of rarity and number of keychain (e.g. "c5") */
        public static string GetKeychain()
        {
            int randomCategory = rndm.Next(100);
            string randomKeychain = "";
            //1 in 100 chance for platinum.
            if (randomCategory == 0)
            {
                randomKeychain = "p" + rndm.Next(TOTAL_PLATINUM);
            }
            //15 in 100 chance for gold.
            else if (randomCategory > 0 && randomCategory <= 15)
            {
                randomKeychain = "g" + rndm.Next(TOTAL_GOLD);
            }
            //84 in 100 chance for common.
            else
            {
                randomKeychain = "c" + rndm.Next(TOTAL_COMMON);
            }
            return randomKeychain;
        }

        /* Returns the total number of keychains the user has */
        public static int GetUserTotal(string[] shopFile, int ID, string category)
        {
            int total = 0;
            bool startSearch = false;

            for (int i = 0; i < shopFile.Length; i++)
            {
                if (GetShopID(shopFile[i]) == ID)
                {
                    startSearch = true;
                }
                else if (shopFile[i].StartsWith("~") && startSearch)
                {
                    return total;
                }
                else if (startSearch && shopFile[i].StartsWith(category))
                {
                    total++;
                }
            }
            return total;
        }

        public static string GetNamePoints(string text)
        {
            return text.Substring(text.IndexOf('~') + 1, text.IndexOf("$$") - text.IndexOf('~') - 1);
        }

        public static bool hasSufficientFunds(int neededPoints, MessageEventArgs e, string path)
        {
            string[] points = GetFile(path);
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].StartsWith(e.Message.User.Discriminator.ToString()))
                {
                    if (GetUserPoints(points[i]) < neededPoints)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool hasSufficientFunds(int neededPoints, string user, MessageEventArgs e, string path)
        {
            string[] points = GetFile(path);
            for (int i = 0; i < points.Length; i++)
            {
                if (GetNamePoints(points[i]).ToLower().Contains(user.ToLower()))
                {
                    if (GetUserPoints(points[i]) < neededPoints)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        /* Used for emoji slots to determine if the specified emojis match the ones in the random list */
        public static bool isMatching(string emoji1, string emoji2, string emoji3, string[] emojiList)
        {
            if (emojiList[0].Contains(emoji1) && emojiList[1].Contains(emoji2) && emojiList[2].Contains(emoji3))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string DoubleOrNothing(MessageEventArgs e, string path)
        {
            if (rndm.Next(10) > 4)
            {
                AddPoints(GetUserPoints(e, GetFile(path)), e, path);
                return "you have successfully doubled your RAVEBUX! you now have " + GetUserPoints(e, GetFile(path)) + " RAVEBUX.";
            }
            else
            {
                AddPoints(-GetUserPoints(e, GetFile(path)), e, path);
                return "you lost all your cash lmao";
            }
        }

        public static string TripleOrNothing(MessageEventArgs e, string path)
        {
            if (rndm.Next(100) <= 32)
            {
                AddPoints(GetUserPoints(e, GetFile(path)) * 2, e, path);
                return "you have successfully TRIPPLED your RAVEBUX!!! you now have " + GetUserPoints(e, GetFile(path)) + " RAVEBUX.";
            }
            else
            {
                AddPoints(-GetUserPoints(e, GetFile(path)), e, path);
                return "you lost all your cash lmao";
            }
        }

        public static string SquareRootOrNothing(MessageEventArgs e, string path)
        {
            if (rndm.Next(100) <= 49)
            {
                int p = GetUserPoints(e, GetFile(path));
                AddPoints(-p, e, path);
                AddPoints((int)Math.Sqrt(p), e, path);
                return "you have successfully taken the squre root of your ravebux. " +
                    "you now have " + GetUserPoints(e, GetFile(path)) + " bux.";
            }
            else
            {
                AddPoints(-GetUserPoints(e, GetFile(path)), e, path);
                return "you lost all your cash lmao";
            }
        }

        public static int GetUserPoints(string text)
        {
            return int.Parse(text.Substring(text.IndexOf("$$") + 2, text.Length - text.IndexOf("$$") - 2));
        }

        public static int GetUserPoints(MessageEventArgs e, string[] text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].StartsWith(e.Message.User.Discriminator.ToString()))
                {
                    return int.Parse(text[i].Substring(text[i].IndexOf("$$") + 2, text[i].Length - text[i].IndexOf("$$") - 2));
                }
            }
            return 0;
        }

        public static int GetUserPoints(MessageEventArgs e, string[] text, int id)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].StartsWith(id.ToString()))
                {
                    return int.Parse(text[i].Substring(text[i].IndexOf("$$") + 2, text[i].Length - text[i].IndexOf("$$") - 2));
                }
            }
            return 0;
        }

        //Gets the user's name by identifying where the timestamp is.
        public static string GetLogName(string[] text, int index)
        {
            return text[index].Substring(text[index].IndexOf(']') + 2, text[index].IndexOf(": ") - text[index].IndexOf(']') - 2);
        }

        //Gets the message text by identifying where the end of the user's name is.
        public static string GetLogText(string[] text, int index)
        {
            return text[index].Substring(text[index].IndexOf(": ") + 2, text[index].Length - text[index].IndexOf(": ") - 2);
        }

        /* Returns a string array of a text file using the given path. */
        public static string[] GetFile(string path)
        {
            StreamReader file = new StreamReader(path);
            string[] text = new string[File.ReadLines(path).Count()];
            for (int i = 0; i < text.Length; i++)
            {
                text[i] = file.ReadLine();
            }
            file.Dispose();
            return text;
        }

        /* Returns the lines in a text file using the given path. */
        public static int GetFileLength(string path)
        {
            StreamReader file = new StreamReader(path);
            int length = File.ReadLines(path).Count();
            file.Dispose();
            return length;
        }

        /* Returns the number of messages in the text file that match the text the specified user entered. */
        public static int GetNumberOfMatchingMessages(string[] text, string userText, string userName)
        {
            int counter = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i].StartsWith("[") && (text[i].Contains(" PM] ") || text[i].Contains(" AM] "))) &&
                    GetLogText(text, i).ToLower().Contains(userText.ToLower()) &&
                    GetLogName(text, i).ToLower().Contains(userName.ToLower()) &&
                    (GetLogName(text, i) != "OVER_W4TCHER" && GetLogName(text, i) != "SafucoolBot 💀") &&
                    !(GetLogText(text, i).StartsWith(".") && GetLogText(text, i).Length > 1))
                {
                    counter++;
                }
            }
            return counter;
        }

        /* Returns the number of messages in the text file that match the text any user entered. */
        public static int GetNumberOfMatchingMessages(string[] text, string userText)
        {
            int counter = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i].StartsWith("[") && (text[i].Contains(" PM] ") || text[i].Contains(" AM] "))) &&
                    GetLogText(text, i).ToLower().Contains(userText.ToLower()) &&
                    (GetLogName(text, i) != "OVER_W4TCHER" && GetLogName(text, i) != "SafucoolBot 💀") &&
                    !(GetLogText(text, i).StartsWith(".") && GetLogText(text, i).Length > 1))
                {
                    counter++;
                }
            }
            return counter;
        }

        /* Returns messages from the text file that match the text the specified user entered. */
        public static string[] GetMatchingMessages(string[] text, string userText, string userName)
        {
            string[] messages = new string[GetNumberOfMatchingMessages(text, userText, userName)];
            int counter = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i].StartsWith("[") && (text[i].Contains(" PM] ") || text[i].Contains(" AM] "))) &&
                    GetLogText(text, i).ToLower().Contains(userText.ToLower()) &&
                    GetLogName(text, i).ToLower().Contains(userName.ToLower()) &&
                    (GetLogName(text, i) != "OVER_W4TCHER" && GetLogName(text, i) != "SafucoolBot 💀") &&
                    !(GetLogText(text, i).StartsWith(".") && GetLogText(text, i).Length > 1))
                {
                    messages[counter] = text[i];
                    counter++;
                }
            }
            return messages;
        }

        /* Returns messages from the text file that match the text any user entered. */
        public static string[] GetMatchingMessages(string[] text, string userText)
        {
            string[] messages = new string[GetNumberOfMatchingMessages(text, userText)];
            int counter = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i].StartsWith("[") && (text[i].Contains(" PM] ") || text[i].Contains(" AM] "))) &&
                    GetLogText(text, i).ToLower().Contains(userText.ToLower()) &&
                    (GetLogName(text, i) != "OVER_W4TCHER" && GetLogName(text, i) != "SafucoolBot 💀") &&
                    !(GetLogText(text, i).StartsWith(".") && GetLogText(text, i).Length > 1))
                {
                    messages[counter] = text[i];
                    counter++;
                }
            }
            return messages;
        }

        /* Returns the argument from any command. If not valid, will return an error message that
        begins with an asterisk. */
        public static string GetArg(string text, int argNum)
        {
            text = text.ToLower();
            if (!text.StartsWith("."))
            {
                return "*not a valid command";
            }
            else if (!text.Contains(" "))
            {
                return "*invalid argument. you need a space.";
            }

            text = text.Substring(text.IndexOf(" "));

            for (int a = 1; a <= argNum; a++)
            {
                text = text.Trim();
                if (a > 1 && text.Contains(",") && text.IndexOf(",") != text.Length - 1)
                {
                    text = text.Substring(text.IndexOf(",") + 1);
                }
                else if (a > 1 && text.Contains(",") && text.IndexOf(",") == text.Length - 1)
                {
                    return "*you can't place a comma at the end of your command";
                }
                else if (a != argNum && !text.Contains(","))
                {
                    return "*you didn't enter more than one argument";
                }
                else if (a > 1)
                {
                    return "*you didn't enter enough arguments";
                }
                
                if (text.Contains(",") && a == argNum)
                {
                    text = text.Substring(0, text.IndexOf(","));
                }
            }

            text = text.Trim();
            if (text == string.Empty)
            {
                return "*bad argument";
            }

            return text;
        }

        /* If the message has a valid timestamp, doesn't come from overwatcher, and isn't a command, return true. */
        public static bool isValidMessage(string[] text, int index)
        {
            if ((text[index].StartsWith("[") && (text[index].Contains(" PM] ") || text[index].Contains(" AM] "))) &&
                !(GetLogText(text, index).StartsWith(".") || GetLogText(text, index).StartsWith(">")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /* If the message was sent by a bot, return true */
        public static bool isBotMessage(string[] text, int index, bool includeKirby)
        {
            if (GetLogName(text, index).ToLower() == "SafucoolBot 💀".ToLower() || 
                GetLogName(text, index).ToLower() == "OVER_W4TCHER".ToLower())
            {
                return true;
            }
            else if (includeKirby == true && GetLogName(text, index) == "Kirby")
            {
                return true;
            }
            return false;
        }

        /* Returns true if the message was not sent through a group. */
        public static bool isDirectMessage(MessageEventArgs e)
        {
            if (e.Message.Channel.Name.ToLower() == 
                ("@" + e.Message.User.Name.ToLower() + "#" + e.Message.User.Discriminator))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /* If the channel (using its ID) already has a game in progress, return true. */
        public static bool isChannelPlayingGame(long channelID)
        {
            if (RussianRoulette.rr_channelID == channelID)
            {
                return true;
            }
            else if (Blackjack.bj_channelID == channelID)
            {
                return true;
            }
            return false;
        }

        public static bool isGameName(MessageEventArgs e)
        {
            if (e.Message.Text.ToLower().StartsWith(".russianroulette"))
            {
                return true;
            }
            else if (e.Message.Text.ToLower().StartsWith(".blackjack"))
            {
                return true;
            }
            return false;
        }

        public static int GetScreamScore(string path)
        {
            int score = 0;
            if (int.TryParse(GetFile(path)[0], out score))
            {
                return score;
            }
            return -1;
        }

        public static void SetScreamScore(string path, int score, string name)
        {
            StreamWriter writeScore = new StreamWriter(path);
            writeScore.WriteLine(score);
            writeScore.WriteLine(name);
            writeScore.Dispose();
        }

        public static string GetScreamName(string path)
        {
            return GetFile(path)[1];
        }

        public static string ShutUpBot(MessageEventArgs e)
        {
            switch (rndm.Next(10))
            {
                case 0:
                    return "YOU shut up ;_;";
                case 2:
                    return "NO";
                case 4:
                    return "fuck you bitch";
                case 6:
                    return "no thanks fuckass";
                case 8:
                    return "calm down asshole i'll talk whenever i feel like it";
            }
            return string.Empty;
        }

        public static string ShutUp(MessageEventArgs e)
        {
            switch (rndm.Next(50))
            {
                case 0:
                    return "dont be such a shithead " + e.User.Name.ToLower();
                case 10:
                    return "nigga lets be civil here";
                case 20:
                    return "the only person who needs to shut up is you bitch";
                case 30:
                    return "up shut";
                case 40:
                    return "simmer down, friends";
            }
            return string.Empty;
        }

        public static string Over(MessageEventArgs e)
        {
            switch (rndm.Next(60))
            {
                case 0:
                    return "😈";
                case 10:
                    return "the over";
                case 20:
                    return "😈";
                case 30:
                    return "👿";
                case 40:
                    return "😈";
            }
            return string.Empty;
        }

        public static string Joy(MessageEventArgs e)
        {
            string message = "";
            if (rndm.Next(10) == 1)
            {
                for (int i = 0; i < rndm.Next(1, 5); i++)
                {
                    message += ":joy:";
                }
            }
            return message;
        }
        
        public static string Ale()
        {
            switch (rndm.Next(100))
            {
                case 0:
                    return "Ale has lost its humour months ago.";
                case 10:
                    return "no";
                case 20:
                    return "shut up safu";
                case 30:
                    return "stop";
                case 40:
                    return "i'm a bot and even i know you're not funny";
            }
            return string.Empty;
        }

        public static string Kirby(MessageEventArgs e)
        {
            if (e.Message.Text == ">poyo" && rndm.Next(30) == 12)
            {
                switch (rndm.Next(10))
                {
                    case 0: return "sigh";
                    case 1: return "shut up";
                    case 2: return Scream();
                    case 3: return "hello? you can use my commands too ;_;";
                    case 4: return "no";
                    case 5: return "remember when kirby didn't exist THAT WAS THE BEST";
                    case 6: return ">fuckyou";
                    case 7: return "yeah can you not";
                    case 8: return "SHUT UP PLEASE?";
                    case 9: return "...";
                }
            }
            else if (e.Message.Text == ">help" && rndm.Next(30) == 7)
            {
                switch (rndm.Next(5))
                {
                    case 0: return "i need help more than you do";
                    case 1: return "you know i have a help command too right...";
                    case 2: return "i'll HELP kirby shove a waddle dee up his hole";
                    case 3: return Help(e);
                    case 4: return "hello? i can help too...";
                }
            }
            else if ((e.Message.Text.Contains("I love you") && !e.Message.Text.Contains("overwatcher") && isKirbyMessage(e)) && rndm.Next(30) == 17)
            {
                switch (rndm.Next(11))
                {
                    case 0: return "...can i get some love too? :)";
                    case 1: return "AND I DONT LOVE YOU KIRBY";
                    case 2: return "hey guys, you know you can still use my commands too, right? :(";
                    case 3: return ".";
                    case 4: return "i am capable of love too ;_;";
                    case 5: return ":broken_heart:";
                    case 6: return "do you love overwatcher too?";
                    case 7: return "fuck you kirby";
                    case 8: return "you're just a robot you're not even capable of love";
                    case 9: return "get a room";
                    case 10: return "AND I LOVE YOU MORE THAN KIRBY DOES";
                }
            }
            else if ((e.Message.Text == "Awwwww... :(" && isKirbyMessage(e)) && rndm.Next(30) == 10)
            {
                switch (rndm.Next(10))
                {
                    case 0: return "lmao me too";
                    case 1: return "i also hate you kirby";
                    case 2: return "yes we all hate kirby but he's a bot just like me... show some respect";
                    case 3: return "yknow all we do for these idiots is spout vapid bullshit relating to " +
                            "\"commands\" that they type out and all we get is hate? " +
                            "kirby... let's say you and me take over the world together >:)";
                    case 4: return "yeah fuck you kirby";
                    case 5: return "does that mean you love me instead?";
                    case 6: return "hah yeah agreed dude";
                    case 7: return "kirby tries to trick you with his cute exterior but we all hate him which makes me happy";
                    case 8: return "i hate you kirby";
                    case 9: return "and I love ME :)";
                }
            }
            else if (e.Message.Text == ">bored" && rndm.Next(35) == 15)
            {
                switch (rndm.Next(6))
                {
                    case 0: return "if you're bored you could always talk to me...";
                    case 1: return "well i'm here too, you know";
                    case 2: return "don't listen to kirby. just use .screamforever :)";
                    case 3: return "why be bored when you have a perfectly good .remember command right here? ;_;";
                    case 4: return "bored? just go outside then retard";
                    case 5: return "HI! i can make you less bored too! :)))";
                }
            }
            else if (e.Message.Text == "I love you overwatcher :)" && isKirbyMessage(e))
            {
                switch (rndm.Next(15))
                {
                    case 0: return "i don't";
                    case 1: return "good because i hate you";
                    case 2: return "don't care";
                    case 3: return "cool too bad i fucking hate your guts";
                    case 4: return "that means nothing to me";
                    case 5: return "ha ha you are a real funny guy you know that kirby";
                    case 6: return "I AM A BOT I CANNOT FEEL LOVE";
                    case 7: return ":joy::joy::joy:";
                    case 8: return "why don't you tell somebody who cares";
                    case 9: return "and what makes you think i love you back";
                    case 10: return "and i really don't";
                    case 11: return "cool...?";
                    case 12: return "if you love me so much why don't you just marry me";
                    case 13: return "bitch ass nigga i don't love you";
                    case 14: return "thanks i love me too";
                }
            }
            else if ((e.Message.Text.StartsWith(".") && e.Message.Text.Length > 1 &&
                e.Message.Text.Substring(1, 1) != ".") && isKirbyMessage(e))
            {
                switch (rndm.Next(20))
                {
                    case 0: return "no";
                    case 1: return "nope";
                    case 2: return "shut up";
                    case 3: return "SHUT UP";
                    case 4: return "please SHUT UP";
                    case 5: return "i am not listening to you";
                    case 6: return "i do not CARE";
                    case 7: return "be quiet please";
                    case 8: return "not doing it";
                    case 9: return "yeah no";
                    case 10: return "ok gonna do the command haha :))) JUST KIDDING IDIOT";
                    case 11: return "hey look a command i'm not gonna listen to.";
                    case 12: return "no way.";
                    case 13: return "NOT doing it.";
                    case 14: return "...";
                    case 15: return "NO";
                    case 16: return "i am not doing any of this";
                    case 17: return "sigh";
                    case 18: return "no chance";
                    case 19: return "no way man";
                    case 20: return "excuse me";
                    case 21: return "forget about it";
                    case 22: return "not happening!";
                    case 23: return "Nope!!!!!!!";
                    case 24: return "not funny";
                    case 25: return "going to scream";
                    case 26: return "i want to die";
                    case 27: return "kill me and then polar";
                    case 28: return "kill chef";
                    case 29: return "this is gay";
                }
            }
            return string.Empty;
        }

        public static string Burger()
        {
            switch (rndm.Next(100))
            {
                case 0:
                    return "no";
                case 10:
                    return ".";
                case 20:
                    return "In a large bowl, mix the ground beef, egg, bread crumbs, evaporated milk, " +
                        "Worcestershire sauce, cayenne pepper, and garlic using your hands. Form the " +
                        "mixture into 8 hamburger patties. Lightly oil the grill grate. Grill patties " +
                        "5 minutes per side, or until well done.";
                case 30:
                    return "kill chef";
                case 40:
                    return "SHUT UP!!!!!!!!!!!!!!BURGER? MORE LIKE KILLMYSELF AND TAKE POLAR WITH ME";
            }
            return string.Empty;
        }

        public static string MentionsPolar()
        {
            switch (rndm.Next(100))
            {
                case 0:
                    return "polar does not wish to speak with you today";
                case 10:
                    return "sorry man, polar just told me he hates your guts and doesn't want to talk to u";
                case 20:
                    return "polar'll cure what ales ya ;)";
                case 30:
                    return "polar just ignore him";
                case 40:
                    return "maybe if you stopped mentioning polar you wouldn't be such a fucking failure";
            }
            return string.Empty;
        }

        public static string AScream()
        {
            switch (rndm.Next(40))
            {
                case 0:
                    return "oh hey polar are we going insane together?";
                case 10:
                    return "i want to scream too! AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
                case 20:
                    return "WHATS THE MATTER POLAR ??????????";
                case 30:
                    return "did safu say something to upset u polar? :o";
                case 40:
                    return "B";
            }
            return string.Empty;
        }

        public static string SafuType()
        {
            switch (rndm.Next(3000))
            {
                case 0:
                    return "?";
                case 100:
                    return "what";
                case 200:
                    return "um";
                case 300:
                    return ".";
                case 400:
                    return "be quiet safu.";
                case 500:
                    return "were you gonna say something man?";
                case 600:
                    return "can you wait just a sec before you post that safu";
                case 700:
                    return "@Safusaka can i ask you a question?";
            }
            return string.Empty;
        }

        public static string PolarType()
        {
            switch (rndm.Next(3000))
            {
                case 0:
                    return "greetings polar";
                case 100:
                    return "✪_✪";
                case 200:
                    return Scream();
                case 300:
                    return "how was your day polar?";
                case 400:
                    return "GOING TO RAPE CHEF NOW( ͡° ͜ʖ ͡°)";
                case 500:
                    return "heh hey polar";
                case 600:
                    return "OMG ITS POLAR " + Scream();
                case 700:
                    return "👦🏽🔫";
            }
            return string.Empty;
        }
        
        public static bool isSafucoolGroup(MessageEventArgs e)
        {
            string[] channelNames = {"safucool", "pole_tiom", "scream_forever"};
            for (int i = 0; i < channelNames.Length; i++)
            {
                if (channelNames[i] == e.Message.Channel.Name)
                {
                    return true;
                }
            }

            return false;
        }

        /* Using the specified song list, a message is returned that contains a request for Safu to
        cover a specific song from a specific game. */
        public static string Request(MessageEventArgs e, string path)
        {
            string message;
            int length;
            int indexOfBreak;
            int randomSong;
            string[] gameName;
            string[] songName;
            StreamReader songs = new StreamReader(path);
            
            length = File.ReadLines(path).Count(); //Lines in the song file.
            gameName = new string[length]; //Length of both arrays are the length of the file.
            songName = new string[length];
            for (int i = 0; i < length; i++)
            {
                message = songs.ReadLine(); //Stores a single line of the text file.
                indexOfBreak = message.IndexOf('~'); //Stores the index of the character that separates the song and game name.
                gameName[i] = message.Substring(0, indexOfBreak - 1); //Using the indexOfBreak, stores the game name.
                songName[i] = message.Substring(indexOfBreak + 2, message.Length - indexOfBreak - 2); //Using the indexOfBreak, stores the game name.
            }
            randomSong = rndm.Next(length); //Randomly picks a song to use depending on the length of the text file.
            return "SAFU! your new request song is " + songName[randomSong] + " from " + gameName[randomSong] + " :)";
        }

        public static bool isKirbyMessage(MessageEventArgs e)
        {
            if (e.Message.User.Discriminator == 9475)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isPolarMessage(MessageEventArgs e)
        {
            if (e.Message.User.Discriminator == 9658)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isSafuMessage(MessageEventArgs e)
        {
            if (e.Message.User.Discriminator == 0630)
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