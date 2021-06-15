using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace spaceIntruders
{
    public partial class Form1 : Form
    {
        Rectangle player = new Rectangle(280, 540, 40, 10);


        int moveTimer = 0;

        int alienSpeed = 1;

        int playerSpeed = 10;

        List<Rectangle> alien = new List<Rectangle>();
        List<int> alienSpeeds = new List<int>();

        int alienSize = 25;

        int score = 0;

        bool aDown = false;
        bool dDown = false;

        string gameState = "waiting";

        SolidBrush greenBrush = new SolidBrush(Color.LimeGreen);

        Image blue = Properties.Resources.alien2;

        Random randGen = new Random();
        int randValue = 0;

        public Form1()
        {
            InitializeComponent();
        }

        public void GameInitialize()
        {

            titleLabel.Text = "";
            subTitleLabel.Text = "";

            gameTimer.Enabled = true;
            gameState = "running";
            score = 0;
            alien.Clear();
            alienSpeeds.Clear();

            player.X = 280;
            player.Y = 540;

            //spawn alien
            spawnAlien();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    aDown = true;
                    break;
                case Keys.D:
                    dDown = true;
                    break;
                case Keys.Space:
                    if (gameState == "waiting" || gameState == "over")
                    {
                        GameInitialize();
                    }
                    break;
                case Keys.Escape:
                    if (gameState == "waiting" || gameState == "over")
                    {
                        Application.Exit();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    aDown = false;
                    break;
                case Keys.D:
                    dDown = false;
                    break;
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {

            //move player
            movePlayer();

            moveAlien();

            moveTimer++;

            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (gameState == "waiting")
            {
                livesLabel.Text = "";
                scoreLabel.Text = "";
                titleLabel.Text = "SPACE INTRUDERS";
                subTitleLabel.Text = "Press Space Bar to Start or Escape to Exit";
            }
            else if (gameState == "running")
            {
                e.Graphics.FillRectangle(greenBrush, player);

                for (int i = 0; i < alien.Count(); i++)
                {
                    //e.Graphics.FillRectangle(greenBrush, alien[i]);
                    e.Graphics.DrawImage(blue, alien[i]);
                }
            }
        }

        //Move player
        public void movePlayer()
        {
            if (aDown == true && player.X > 0)
            {
                player.X -= playerSpeed;
            }

            if (dDown == true && player.X < this.Width - player.Width)
            {
                player.X += playerSpeed;
            }
        }

        //spawn aliens
        public void spawnAlien()
        {
            int rows = 5;           // number of rows
            int numbAliens = 11;    //number of aliens in each row
            int gap = 20;           // gap in between each alien
            int x = 20;             //starting x
            int y = 50;             //starting y

            for (int i = 0; i < rows; i++)
            {
                for (int n = 0; n < numbAliens; n++)
                {
                    x += alienSize + gap;
                    alien.Add(new Rectangle(x, y, alienSize, alienSize));
                }

                y += alienSize + gap;
                x = 20;
            }
        }

        //move aliens
        public void moveAlien()
        {
            for (int i = 0; i < alien.Count(); i++)
            {
                int x = alien[i].X + alienSpeed;
                alien[i] = new Rectangle(x, alien[i].Y, alienSize, alienSize);
            }
        }




    }
}
