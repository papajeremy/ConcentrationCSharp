using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;  //used to allow the Directory method to pull in the files names from a file
using System.Diagnostics;
using System.Threading; //for the Sleep thread to cause the program to pause

namespace ConcentrationGame
{
    public partial class frmConcentrationGame : Form
    {
        public frmConcentrationGame()
        {
            InitializeComponent();
        }


        //various instance variables 

        int m_firstCard = -1;
        int m_secondCard = -1;
        bool m_match;
        PictureBox m_picbox1;
        PictureBox m_picbox2;
        PictureBox m_cardBack;
        int m_pictureImage1;
        int m_pictureImage2;
        int m_seconds = 0;
        int m_minutes = 0;
        int m_score = 0;

        //sets path for the card back image
        string m_path_cardBack = "C:\\Users\\Jeremy\\Documents\\Spring_2012\\CS233C#\\Lab1\\ConcentrationGame\\OldRose.jpg";

        //creates card array
        string[] cards = Directory.GetFiles
            (Path.GetFileNameWithoutExtension
            (@"C:\\Users\\Jeremy\\Documents\\Spring_2012\\CS233C#\\Lab1\ConcentrationGame\\ConcentrationGame\\bin\\Debug\\Card_Images"));


        //shuffle the cards for later placement on the board in random order
        void Shuffle_Cards()
        {
            Random random = new Random();
            for (int j = 0; j <= 5; j++) //shuffling the shuffles
            {
                for (int h = 0; h <= 5; h++) //multiple shuffles
                {
                    for (int i = 0; i <= 39; i++)
                    {
                        int rnd = RandomNumber(ref random, 0, cards.Length);
                        string temp = cards[i];
                        cards[i] = cards[rnd];
                        cards[rnd] = temp;
                    }
                }
            }
        }

        //generates a unique random number based on the length of the array
        int RandomNumber(ref Random random, int min, int max)
        {

            return random.Next(min, max);
        }

        //shows the face of the card based on the which picture box is selected and the index from the cards array that corresponds to that picture box number -1
        void ShowCard(string[] cards, int image)
        {
            PictureBox card = (PictureBox)this.Controls["pictureBox" + (image)];
            card.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\" + cards[image - 1]);
        }

        //test for match between the 2 selected cards
        bool IsMatch(string[] cards, int index1, int index2)
        {
            if (cards[index1 - 1].Substring(12, 4) == cards[index2 - 1].Substring(12, 4))
                return true;
            else
                return false;
        }

        //similar to ShowCard method, except to re-show the back image of the card
        void ShowBack(string[] cards, int image)
        {
            PictureBox m_cardBack = (PictureBox)this.Controls["pictureBox" + (image)];
            m_cardBack.Image = Image.FromFile(m_path_cardBack);
        }

        //creates a new instance of the game to start over
        //referenced by F2, the New Game button, or New Game tool bar command
        //resets the game, reshuffles the cards, and resets the timer
        void NewGame()
        {
            this.Refresh();
            Shuffle_Cards();
            timer.Stop();
            lblTimer.ResetText();
            m_seconds = 0;
            m_minutes = 0;
            timer.Start();
            for (int j = 1; j <= 40; j++)
            {
                m_cardBack = (PictureBox)this.Controls["pictureBox" + (j)];
                m_cardBack.Image = Image.FromFile(m_path_cardBack);
                m_cardBack.Enabled = true;
            }
        }

        //sets a keybind to the F2 key to signal the form to reset the game
        void KeyEvent(object sender, KeyEventArgs e) //Keyup Event 
        {
            if (e.KeyCode == Keys.F2)
            {
                NewGame();
            }
        }


        //shuffles the card array when the form loads
        private void frmConcentrationGame_Load(object sender, EventArgs e)
        {
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(KeyEvent);
            KeyPreview = true;
            Shuffle_Cards();
            timer.Enabled = true;
            for (int j = 1; j <= 40; j++)
            {
                m_cardBack = (PictureBox)this.Controls["pictureBox" + (j)];
                m_cardBack.Image = Image.FromFile(m_path_cardBack);
            }
        }

        //displays a new picturebox picture from the shuffled card array
        //thanks to Clifton for looking at my code and guiding my corrections
        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (m_firstCard == -1)
            {
                m_picbox1 = (PictureBox)sender;
                m_pictureImage1 = Int32.Parse(m_picbox1.Name.Substring(10));
                ShowCard(cards, m_pictureImage1);
                m_firstCard = 1;
                m_picbox1.Enabled = false;
            }
            else
            {
                m_picbox2 = (PictureBox)sender;
                m_pictureImage2 = Int32.Parse(m_picbox2.Name.Substring(10));
                ShowCard(cards, m_pictureImage2);
                m_match = IsMatch(cards, m_pictureImage1, m_pictureImage2);
                m_secondCard = 1;
            }

            if (m_secondCard == 1 && m_match == true)
            {
                //code snippet borrowed from a classmate to make the program pause so the user can see the 2 cards
                //causes the program to pause for x milliseconds
                //Application.DoEvents();
                //Thread.Sleep(1000);
                m_firstCard = -1;
                m_secondCard = -1;
                m_picbox2.Enabled = false;
                m_score++;
            }
            else if (m_secondCard == 1 && m_match == false)
            {
                Application.DoEvents();
                Thread.Sleep(1000);
                ShowBack(cards, m_pictureImage1);
                ShowBack(cards, m_pictureImage2);
                m_firstCard = -1;
                m_secondCard = -1;
                m_picbox1.Enabled = true;
            }

            if (m_score == 20)
            {
                if (MessageBox.Show("You Win!\n Another Game?", "Congratulations", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    NewGame();
                else
                    this.Close();
            }
        }

        //simple timer event to show elapsed time of the game
        private void timer_Tick(object sender, EventArgs e)
        {
            m_seconds++;
            if (m_seconds == 60)
            {
                m_minutes++;
                m_seconds = 0;
            }
            lblTimer.Text = String.Format("{0:00}:{1:00}", m_minutes, m_seconds);
        }

        //new game toolbar command
        private void startGameF2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        //new game button click event
        private void btnNewGame_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        //exit game toolbar command, verifies if user really intended to close the program
        private void exitGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Quit Game?", "Exit Game", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                this.Close();
        }

        private void ExitGame_ButtonClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Quit Game?", "Exit Game", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                this.Close();
        }
    }
}
