using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApi;

namespace RotMGRewardCollect
{
    public partial class FormMain : Form
    {
        private const int SKIP_FIRST_N_ACCOUNTS = 0;
        private const int COOLDOWN_EVERY_N_ACCOUNTS = 5;

        private static readonly Point WINDOW_OFFSET = new Point(1, 51);

        private static readonly Point LOGIN_BUTTON = new Point(760, 15);
        private static readonly Point EMAIL_FIELD = new Point(400, 250);
        private static readonly Point SIGN_IN_BUTTON = new Point(490, 400);
        private static readonly Point WIZARD_CHAR_CREATION = new Point(400, 140);

        private static readonly Point CONTINUE_BUTTON = new Point(490, 390);
        private static readonly Point SAVE_BUTTON = new Point(500, 415);
        private static readonly Point CONFIRM_BUTTON = new Point(490, 400);

        private static readonly Point PLAY_BUTTON = new Point(400, 555);
        private static readonly Point INTERACT_BUTTON = new Point(700, 575);
        private static readonly Point BACK_TO_HOME_BUTTON = new Point(700, 550);

        private static readonly Point FIRST_REWARD = new Point(155, 155);
        private const int REWARD_COLUMNS = 7;
        private const int REWARD_ROWS = 5;
        private const int NEXT_REWARD_OFFSET = 80;

        private const int SHORT_PAUSE = 250;
        private const int NORMAL_PAUSE = 2000;
        private const int LONG_PAUSE = 10000;

        public FormMain()
        {
            InitializeComponent();
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;

            IEnumerable<Account> accounts =
                File.ReadAllLines("accounts.js")
                .ToList()
                .Select(x => x.Trim(' ', '\t'))
                .Where(x => !x.StartsWith("//") && x.Length > 0)
                .SkipWhile(x => x.EndsWith("{"))
                .TakeWhile(x => !x.StartsWith("}"))
                .Skip(SKIP_FIRST_N_ACCOUNTS)
                .Select(x => new Account(x));

            Task.Factory.StartNew(() =>
            {
                // Give the user a few seconds to switch to the Flash Player window
                Thread.Sleep(5000);

                int accountsDone = 0;

                foreach (Account acc in accounts)
                {
                    // Log into the account
                    LogIn(acc);
                    // Collect the rewards
                    CollectRewards();

                    // Every N accounts
                    if (++accountsDone % COOLDOWN_EVERY_N_ACCOUNTS == 0)
                    {
                        // Wait N minutes to avoid breaking the IP limit
                        Thread.Sleep(600000);
                    }
                }

                buttonStart.BeginInvoke((Action)(() => buttonStart.Enabled = true));
            });
        }

        private void LogIn(Account acc)
        {
            // Click Log Out
            OffsetClick(LOGIN_BUTTON);
            Thread.Sleep(NORMAL_PAUSE);
            // Click Log In
            OffsetClick(LOGIN_BUTTON);
            Thread.Sleep(NORMAL_PAUSE);

            // Fill email
            OffsetClick(EMAIL_FIELD);
            TypeText(acc.Email);
            Thread.Sleep(SHORT_PAUSE);

            // Fill password
            Keyboard.Press(Keys.Tab);
            Thread.Sleep(SHORT_PAUSE);
            TypeText(acc.Password);
            Thread.Sleep(SHORT_PAUSE);

            // Click Sign In
            OffsetClick(SIGN_IN_BUTTON);
            Thread.Sleep(LONG_PAUSE);

            // Answer the security questions
            // This shouldn't be necessary when using a hacked client
            // From what I've seen, the CC doesn't show the security questions
            // For that reason I've commented out the code, but if you want to use
            // this tool with a vanilla client, you might want to uncomment it
            // I'm not very sure about the coordinates though
            // I don't recall testing this piece of code, so you might want to check the
            // real coordinates of all the buttons before running the code
            //OffsetClick(CONTINUE_BUTTON);
            //Thread.Sleep(NORMAL_PAUSE);
            //TypeText("answer");
            //Thread.Sleep(SHORT_PAUSE);
            //Keyboard.Press(Keys.Tab);
            //Thread.Sleep(SHORT_PAUSE);
            //TypeText("answer");
            //Thread.Sleep(SHORT_PAUSE);
            //Keyboard.Press(Keys.Tab);
            //Thread.Sleep(SHORT_PAUSE);
            //TypeText("answer");
            //Thread.Sleep(SHORT_PAUSE);
            //OffsetClick(SAVE_BUTTON);
            //Thread.Sleep(NORMAL_PAUSE);
            //OffsetClick(CONFIRM_BUTTON);
            //Thread.Sleep(NORMAL_PAUSE);

            // Get into the game
            OffsetClick(PLAY_BUTTON);
            Thread.Sleep(NORMAL_PAUSE);
            OffsetClick(WIZARD_CHAR_CREATION);
            Thread.Sleep(NORMAL_PAUSE);
            OffsetClick(PLAY_BUTTON);
            Thread.Sleep(LONG_PAUSE);
        }

        private void CollectRewards()
        {
            // Move to the portal area
            Keyboard.Down(Keys.W);
            Thread.Sleep(1000);
            Keyboard.Down(Keys.D);
            Thread.Sleep(4000);
            Keyboard.Up(Keys.W);
            Thread.Sleep(1000);
            Keyboard.Up(Keys.D);

            // Move to the portal itself
            Keyboard.Down(Keys.A);
            Thread.Sleep(500);
            Keyboard.Down(Keys.S);
            Thread.Sleep(500);
            Keyboard.Up(Keys.A);
            Keyboard.Up(Keys.S);

            // Use the portal
            Thread.Sleep(SHORT_PAUSE);
            OffsetClick(INTERACT_BUTTON);
            Thread.Sleep(LONG_PAUSE);

            // Move towards the NPC
            Keyboard.Down(Keys.W);
            Thread.Sleep(1500);
            Keyboard.Up(Keys.W);
            Keyboard.Down(Keys.A);
            Thread.Sleep(1000);
            Keyboard.Up(Keys.A);

            // Open the daily login reward menu
            Thread.Sleep(SHORT_PAUSE);
            OffsetClick(INTERACT_BUTTON);
            Thread.Sleep(NORMAL_PAUSE);

            // Collect all available the rewards
            for (int i = 0; i < REWARD_ROWS; i++)
            {
                int y = FIRST_REWARD.Y + (i * NEXT_REWARD_OFFSET);

                for (int j = 0; j < REWARD_COLUMNS; j++)
                {
                    int x = FIRST_REWARD.X + (j * NEXT_REWARD_OFFSET);

                    OffsetClick(x, y);
                    Thread.Sleep(SHORT_PAUSE);
                }
            }

            // Exit back to the character selection screen
            Keyboard.Press(Keys.O);
            Thread.Sleep(NORMAL_PAUSE);
            OffsetClick(BACK_TO_HOME_BUTTON);
            Thread.Sleep(LONG_PAUSE);
        }

        private void TypeText(string text)
        {
            // In spite of what it might seem like
            // this is actually the most efficient way to type
            // text when using the WinApi because it works
            // independently on your keyboard language
            // (this is a common problem with SendKeys)

            // Copy text to clipboard
            Invoke((Action)(() => Clipboard.SetText(text)));

            // Press Ctrl+V
            Keyboard.Down(Keys.LControlKey);
            Keyboard.Press(Keys.V);
            Keyboard.Up(Keys.LControlKey);
        }

        private void OffsetClick(int x, int y)
        {
            Mouse.LeftClick(WINDOW_OFFSET.X + x, WINDOW_OFFSET.Y + y);
        }

        private void OffsetClick(Point p) => OffsetClick(p.X, p.Y);

        private struct Account
        {
            public readonly string Email;
            public readonly string Password;

            public Account(string accountInfo)
            {
                string[] parts = string.Concat(accountInfo.Where(x => x != ' ' && x != '\'').TakeWhile(x => x != ',')).Split(':');

                if (parts.Length != 2)
                {
                    throw new Exception();
                }

                Email = parts[0];
                Password = parts[1];
            }
        }
    }
}