using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.WebSocket;

namespace SafucoolBot
{
    class Program
    {
        static DiscordSocketClient bot = new DiscordSocketClient();
        static Random rndm = new Random();
        static string BOT_NAME = "OVER_W4TCHER";
        static string strLogPath = "..\\..\\..\\logs.txt";
        static string strMiscLogPath = "..\\..\\..\\misc_logs.txt";
        static string strSongPath = "..\\..\\..\\songs.txt";
        static string strUndertalePath = "..\\..\\..\\undertale.txt";
        static string strScorePath = "..\\..\\..\\score.txt";
        static string strSansPath = "..\\..\\..\\sans porn\\";
        static string strShopPath = "..\\..\\..\\shop.txt";
        public static string strPointsPath = "..\\..\\..\\points.txt";
        static int numHighScore = 0;
        static string strHighScore = "";

        static void Main(string[] args)
        {
            bot.Connected("bot@polese.us", "");

            bot.MessageReceived += Bot_MessageReceived;

            bot.Wait();
        }

        private static void Bot_MessageReceived(object sender, MessageEventArgs e)
        {
            BotActions.AddPoints(1, e, strPointsPath);
            if (BotActions.isSafucoolGroup(e))
            {
                BotActions.Log(e, strLogPath); //Logs every message in a text file.
            }
            else
            {
                BotActions.Log(e, strMiscLogPath); //Logs every message in a text file.
            }

            if (e.Message.Text.StartsWith(">") || BotActions.isKirbyMessage(e))
            {
                string message = BotActions.Kirby(e);
                if (message != string.Empty)
                {
                    e.Channel.SendMessage(message);
                }
                return;
            }

            if (RussianRoulette.IsValidMessage(e))
            {
                RussianRoulette.Execute(e);
                return;
            }
            else if (e.Message.Text.ToLower().StartsWith(".russianroulette") && 
                RussianRoulette.rr_isGameActive == true && !RussianRoulette.isUserMessage(e))
            {
                e.Channel.SendMessage("A game has already been started by " + RussianRoulette.rr_user1 + ".");
                return;
            }
            else if (e.Message.Text.ToLower().StartsWith(".russianroulette") &&
                BotActions.isChannelPlayingGame(Convert.ToInt64(e.Message.Channel.Id)))
            {
                e.Channel.SendMessage("Sorry, but you cannot play this while another game is taking place in the same channel.");
                return;
            }
            else if (e.Message.Text.ToLower().StartsWith(".russianroulette") &&
                !BotActions.hasSufficientFunds(0, e, strPointsPath))
            {
                e.Channel.SendMessage("Get out of debt first idiot??");
                return;
            }
            
            if (e.Message.Text.ToLower().StartsWith(".blackjack") || Blackjack.IsValidMessage(e))
            {
                Blackjack.Execute(e);
                return;
            }

            //Represents both scream commands, "screamforever" and "scream."
            if (e.Message.Text.ToLower().StartsWith(".screamforever"))
            {
                int randomNum = 0;
                int score = 0;
                do
                {
                    randomNum = rndm.Next(10);
                    e.Channel.SendMessage(BotActions.Scream());
                    score++;
                } while (randomNum < 7);

                if (score > BotActions.GetScreamScore(strScorePath))
                {
                    BotActions.SetScreamScore(strScorePath, score, e.Message.User.Name.ToLower());
                    e.Channel.SendMessage("HOLY %$#& A NEW ALL-TIME SCREAM HIGHSCORE HAS BEEN SET BY " +
                        e.Message.User.Name.ToUpper() + "!!! " + score + " screams!!!");
                    numHighScore = score;
                    strHighScore = e.Message.User.Name.ToLower();
                }
                else if (score > numHighScore)
                {
                    numHighScore = score;
                    strHighScore = e.Message.User.Name.ToLower();
                    e.Channel.SendMessage("congrats! " + strHighScore + 
                        " has this session's high score with " + numHighScore + " screams.");
                }
            }
            else if (e.Message.Text.ToLower().StartsWith(".scream"))
            {
                e.Channel.SendMessage(BotActions.Scream());
            }

            //Represents the getlog command.
            if (e.Message.Text.ToLower().StartsWith(".getlog") && BotActions.isSafucoolGroup(e))
            {
                using (var stream = File.Open(strLogPath, FileMode.Open))
                {
                    e.Channel.SendFile(strLogPath);
                }
            }
            else if (e.Message.Text.ToLower().StartsWith(".getlog"))
            {
                using (var stream = File.Open(strMiscLogPath, FileMode.Open))
                {
                    e.Channel.SendFile(strMiscLogPath);
                }
            }

            //Represents the getscore command.
            if (e.Message.Text.ToLower().StartsWith(".getscore"))
            {
                if (numHighScore > 0)
                {
                    e.Channel.SendMessage(strHighScore + " has this session's high score with " + numHighScore + " screams. All-time is " +
                        BotActions.GetScreamScore(strScorePath) + " screams by " + BotActions.GetScreamName(strScorePath) + ".");
                }
                else
                {
                    e.Channel.SendMessage("no high score for this session, but the all-time is " + BotActions.GetScreamScore(strScorePath) +
                        " screams by " + BotActions.GetScreamName(strScorePath));
                }
            }

            if (e.Message.Text.ToLower().StartsWith(".getbux"))
            {
                e.Channel.SendMessage(BotActions.GetPoints(e, strPointsPath));
            }

            if (e.Message.Text.ToLower().StartsWith(".getallbux"))
            {
                e.Channel.SendMessage(BotActions.GetAllPoints(e, strPointsPath));
            }

            if (e.Message.Text.ToLower().StartsWith(".givebux"))
            {
                e.Channel.SendMessage(BotActions.GivePoints(e, strPointsPath));
            }

            if (e.Message.Text.ToLower().StartsWith(".stealbux"))
            {
                e.Channel.SendMessage(BotActions.StealPoints(e, strPointsPath));
            }
            
            if (e.Message.Text.ToLower().StartsWith(".doubleornothing") && BotActions.hasSufficientFunds(0, e, strPointsPath))
            {
                e.Channel.SendMessage(BotActions.DoubleOrNothing(e, strPointsPath));
            }

            if (e.Message.Text.ToLower().StartsWith(".tripleornothing") && BotActions.hasSufficientFunds(0, e, strPointsPath))
            {
                e.Channel.SendMessage(BotActions.TripleOrNothing(e, strPointsPath));
            }

            if (e.Message.Text.ToLower().StartsWith(".squarerootornothing") && BotActions.hasSufficientFunds(0, e, strPointsPath))
            {
                e.Channel.SendMessage(BotActions.SquareRootOrNothing(e, strPointsPath));
            }

            if (e.Message.Text.ToLower().StartsWith(".slots") && BotActions.hasSufficientFunds(2, e, strPointsPath))
            {
                e.Channel.SendMessage(BotActions.Slot(e, strPointsPath));
            }
            else if (e.Message.Text.ToLower().StartsWith(".slots"))
            {
                e.Channel.SendMessage("you need 2 RAVEBUX to play slots.");
            }

            if (e.Message.Text.ToLower().StartsWith(".shop"))
            {
                string message = BotActions.Shop(e, strShopPath);
                if (message.Contains(".png"))
                {
                    e.Channel.SendMessage(message.Substring(message.IndexOf('~') + 1));
                    e.Channel.SendFile(message.Substring(0, message.IndexOf('~')));
                    return;
                }
                e.Channel.SendMessage(message);
                return;
            }

            if (e.Message.Text.ToLower().StartsWith(".getitems "))
            {
                string message = BotActions.GetItemsPicture(e, strShopPath);
                if (message.StartsWith("*"))
                {
                    e.Channel.SendMessage(message.Substring(1));
                }
                else
                {
                    e.Channel.SendFile(message);
                }
            }
            else if (e.Message.Text.ToLower().StartsWith(".getitems"))
            {
                e.Channel.SendMessage(BotActions.GetItems(e, strShopPath));
            }

            if (e.Message.Text.ToLower().StartsWith(".getallitems"))
            {
                e.Channel.SendMessage(BotActions.GetAllItems(e, strShopPath));
            }

            //Represents the help commands, "help" and "helpforever."
            if (e.Message.Text.ToLower().StartsWith(".helpforever"))
            {
                int randomNum = 0;
                do
                {
                    randomNum = rndm.Next(10);
                    e.Channel.SendMessage(BotActions.Help(e));
                } while (randomNum < 7);
            }
            else if (e.Message.Text.ToLower().StartsWith(".help"))
            {
                e.Channel.SendMessage(BotActions.Help(e));
            }

            //Represents the Undertale commands, including "undertale" and "undertaleforever."
            else if (e.Message.Text.ToLower().StartsWith(".undertaleforever"))
            {
                int randomNum = 0;
                do
                {
                    randomNum = rndm.Next(10);
                    e.Channel.SendMessage(BotActions.Undertale(e, strUndertalePath));
                } while (randomNum < 7);
            }
            else if (e.Message.Text.ToLower().StartsWith(".undertale"))
            {
                e.Channel.SendMessage(BotActions.Undertale(e, strUndertalePath));
            }

            //Represents the remember commands, including "rememberforever" and "remember."
            if (e.Message.Text.ToLower().StartsWith(".remember") && BotActions.Remember(e, strMiscLogPath).StartsWith("*"))
            {
                e.Channel.SendMessage(BotActions.Remember(e, strMiscLogPath).Substring(1));
            }
            else if (e.Message.Text.ToLower().StartsWith(".rememberforever") && BotActions.isSafucoolGroup(e))
            {
                int randomNum = 0;
                do
                {
                    randomNum = rndm.Next(10);
                    e.Channel.SendMessage(BotActions.Remember(e, strLogPath));
                } while (randomNum < 7);
            }
            else if (e.Message.Text.ToLower().StartsWith(".rememberforever"))
            {
                int randomNum = 0;
                do
                {
                    randomNum = rndm.Next(10);
                    e.Channel.SendMessage(BotActions.Remember(e, strMiscLogPath));
                } while (randomNum < 7);
            }
            else if (e.Message.Text.ToLower().StartsWith(".remember") && BotActions.isSafucoolGroup(e))
            {
                e.Channel.SendMessage(BotActions.Remember(e, strLogPath));
            }
            else if (e.Message.Text.ToLower().StartsWith(".remember"))
            {
                e.Channel.SendMessage(BotActions.Remember(e, strMiscLogPath));
            }

            //Represents the find command.
            if (e.Message.Text.ToLower().StartsWith(".find") && BotActions.isSafucoolGroup(e))
            {
                e.Channel.SendMessage(BotActions.Find(e, strLogPath));
            }
            else if (e.Message.Text.ToLower().StartsWith(".find"))
            {
                e.Channel.SendMessage(BotActions.Find(e, strMiscLogPath));
            }

            //Represents the request command.
            if (e.Message.Text.ToLower().StartsWith(".request"))
            {
                e.Channel.SendMessage(BotActions.Request(e, strSongPath));
            }

            if (e.Message.Text.ToLower().StartsWith(".decide"))
            {
                e.Channel.SendMessage(BotActions.Decide(e));
            }

            //Represents the shut up command.
            if (e.Message.Text.ToLower().StartsWith(".shutupforever"))
            {
                int randomNum = 0;
                do
                {
                    randomNum = rndm.Next(10);
                    e.Channel.SendMessage(BotActions.ShutUpCommand());
                } while (randomNum < 7);
            }
            else if (e.Message.Text.ToLower().StartsWith(".shutup"))
            {
                e.Channel.SendMessage(BotActions.ShutUpCommand());
            }

            //Represents the sans command.
            if (e.Message.Text.ToLower().StartsWith(".sans") && BotActions.hasSufficientFunds(200, e, strPointsPath))
            {
                e.Channel.SendFile(strSansPath + rndm.Next(100) + ".png");
                BotActions.AddPoints(-200, e, strPointsPath);
                return;
            }
            else if (e.Message.Text.ToLower().StartsWith(".sans"))
            {
                e.Channel.SendMessage("you need 200 RAVEBUX to post sans porn.");
                return;
            }

            /*Represents the killover command.
            if (e.Message.Text.ToLower().StartsWith(".killover") && BotActions.hasSufficientFunds(1000000, e, strPointsPath))
            {
                //BotActions.AddPoints(-1000000, e, strPointsPath);
                //175369931309842433
                //e.Server.Ban();
                return;
            }
            else if (e.Message.Text.ToLower().StartsWith(".killover"))
            {
                e.Channel.SendMessage("you need ONE MILLION RAVEBUX for the over.");
                return;
            }*/

            //Represents the count command.
            if (e.Message.Text.ToLower().StartsWith(".count") && BotActions.isSafucoolGroup(e))
            {
                e.Channel.SendMessage(BotActions.GetCount(e, strLogPath));
            }
            else if (e.Message.Text.ToLower().StartsWith(".count"))
            {
                e.Channel.SendMessage(BotActions.GetCount(e, strMiscLogPath));
            }

            //Represents the setgame command.
            if (e.Message.Text.ToLower().StartsWith(".setgame"))
            {
                try
                {
                    bot.SetGame(e.Message.Text.Substring(9, e.Message.Text.Length - 9));
                }
                catch
                {
                    e.Channel.SendMessage("bad game name idiot");
                }
            }

            //Bot responds to certain phrases.
            if (e.Message.Text.ToLower().Contains("shut up") && e.Message.Text.ToLower().Contains("bot") && e.Message.User.Name != BOT_NAME)
            {
                string message = BotActions.ShutUpBot(e);
                if (message != string.Empty)
                {
                    e.Channel.SendMessage(message);
                }
            }
            else if (e.Message.Text.ToLower().Contains("shut up") && e.Message.User.Name != BOT_NAME)
            {
                string message = BotActions.ShutUp(e);
                if (message != string.Empty)
                {
                    e.Channel.SendMessage(message);
                }
            }
            else if (e.Message.Text == "." && e.Message.User.Name != BOT_NAME)
            {
                if (rndm.Next(4) == 0)
                {
                    e.Channel.SendMessage(".");
                }
            }
            else if (e.Message.Text.Contains("😂") && e.Message.User.Name != BOT_NAME)
            {
                string message = BotActions.Joy(e);
                if (message != string.Empty)
                {
                    e.Channel.SendMessage(message);
                }
            }
            else if (e.Message.Text == "😈" || e.Message.Text.ToLower() == "the over" && e.Message.User.Name != BOT_NAME)
            {
                string message = BotActions.Over(e);
                if (message != string.Empty)
                {
                    e.Channel.SendMessage(message);
                }
            }

            //Bot responds to certain people.
            if (e.Message.User.Name.ToLower().Contains("polar"))
            {
                if (e.Message.Text.ToUpper().Contains("AAAAAAA"))
                {
                    string message = BotActions.AScream();
                    if (message != string.Empty)
                    {
                        e.Channel.SendMessage(message);
                    }
                }
            }
            else if (e.Message.User.Name.ToLower().Contains("polar"))
            {
                if (e.Message.Text.ToLower().Contains(" ale "))
                {
                    string message = BotActions.Ale();
                    if (message != string.Empty)
                    {
                        e.Channel.SendMessage(message);
                    }
                }
                if (e.Message.Text.ToLower().Contains("polar?") || e.Message.Text.ToLower().Contains(" polar "))
                {
                    string message = BotActions.MentionsPolar();
                    if (message != string.Empty)
                    {
                        e.Channel.SendMessage(message);
                    }
                }
                else if (e.Message.Text.ToLower().Contains(" burger "))
                {
                    string message = BotActions.Burger();
                    if (message != string.Empty)
                    {
                        e.Channel.SendMessage(message);
                    }
                }
            }
            
        }
    }
}