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
        Rectangle player = new Rectangle(280, 540, 40, 40);

        List<Rectangle> alien = new List<Rectangle>();
        List<Rectangle> alienBullet = new List<Rectangle>();
        List<Rectangle> playerBullet = new List<Rectangle>();

        int bulletSpeed = -20;
        int alienBulletSpeed = 20;

        int alienSpeed = 2;

        int playerSpeed = 10;

        int alienSize = 25;

        int score = 0;

        bool aDown = false;
        bool dDown = false;

        string gameState = "waiting";

        SolidBrush greenBrush = new SolidBrush(Color.LimeGreen);
        SolidBrush redBrush = new SolidBrush(Color.Red);

        Image alienImage = Properties.Resources.alien2;
        Image spaceshipImage = Properties.Resources.spaceship;

        Random randGen = new Random();
        int randValue = 0;

        public Form1()
        {
            InitializeComponent();
        }

        public void GameInitialize()
        {
            gameState = "running";

            titleLabel.Text = "";
            subTitleLabel.Text = "";

            titleLabel.Visible = false;
            subTitleLabel.Visible = false;
            gameTimer.Enabled = true;

            score = 0;
            scoreLabel.Text = $"Score {score}";

            alien.Clear();

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
            //spawn bullet when mouse clicked
            playerBullet.Add(new Rectangle(player.X + 20, player.Y, 5, 5));
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //game boarder
            Rectangle boarder = new Rectangle(5, 5, this.Width - 10, this.Height - 10);

            //move player
            movePlayer();

            //move alien
            moveAlien();

            //alien collision with walls
            alienCollisionWalls();

            //move bullet
            moveBullet();

            //bullet collision with alien
            alienCollisionBullet();

            //alien speed change
            alienSpeedChange();

            //create alien bullets
            spawnAlienBullet();

            //move alien bullet
            moveAlienBullet();


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
                //e.Graphics.FillRectangle(greenBrush, player);
                e.Graphics.DrawImage(spaceshipImage, player);

                //draw alien image
                for (int i = 0; i < alien.Count(); i++)
                {
                    e.Graphics.DrawImage(alienImage, alien[i]);
                }

                //draw players bullets
                for (int i = 0; i < playerBullet.Count(); i++)
                {
                    e.Graphics.FillRectangle(redBrush, playerBullet[i]);

                }

                //draw aliens bullets
                for (int i = 0; i < alienBullet.Count(); i++)
                {
                    e.Graphics.FillRectangle(redBrush, alienBullet[i]);

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
            int rows = 4;           // number of rows
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

        //When alien collides with boarder
        public void alienCollisionWalls()
        {
            for (int i = 0; i < alien.Count(); i++)
            {
                if (alien[i].X <= 10)
                {
                    alienSpeed *= -1;

                    for (int j = 0; j < alien.Count(); j++)
                    {
                        int y = alien[j].Y + 15;
                        int x = alien[j].X + 4;
                        alien[j] = new Rectangle(x, y, alienSize, alienSize);
                    }
                }
                else if (alien[i].X >= this.Height - alienSize - 10)
                {
                    alienSpeed *= -1;

                    for (int j = 0; j < alien.Count(); j++)
                    {
                        int y = alien[j].Y + 15;
                        int x = alien[j].X - 4;
                        alien[j] = new Rectangle(x, y, alienSize, alienSize);
                    }
                }
            }
        }

        //move bullet
        public void moveBullet()
        {
            for (int i = 0; i < playerBullet.Count(); i++)
            {
                int y = playerBullet[i].Y + bulletSpeed;
                playerBullet[i] = new Rectangle(playerBullet[i].X, y, 5, 20);
            }
        }

        //alien collision with bullet
        public void alienCollisionBullet()
        {
            for (int i = 0; i < playerBullet.Count(); i++)
            {
                for (int j = 0; j < alien.Count(); j++)
                {
                    if (playerBullet[i].IntersectsWith(alien[j]))
                    {
                        playerBullet.RemoveAt(i);
                        alien.RemoveAt(j);
                        score += 50;
                        scoreLabel.Text = $"Score {score}";
                        break;
                    }
                }
            }
        }

        //alien speed change
        public void alienSpeedChange()
        {
            if (alien.Count == 40)
            {
                if (alienSpeed > 0)
                {
                    alienSpeed = 3;
                }
                else
                {
                    alienSpeed = -3;
                }
            }
            else if (alien.Count == 25)
            {
                if (alienSpeed > 0)
                {
                    alienSpeed = 4;
                }
                else
                {
                    alienSpeed = -4;
                }
            }
            else if (alien.Count == 1)
            {
                if (alienSpeed > 0)
                {
                    alienSpeed = 20;
                }
                else
                {
                    alienSpeed = -20;
                }
            }
        }

        //spawn alien bullets
        public void spawnAlienBullet()
        {
            randValue = randGen.Next(0, 101);

            if (randValue < 5)
            {
                for (int i = 0; i < alien.Count(); i++)
                {
                    alienBullet.Add(new Rectangle(alien[i].X, alien[i].Y + 20, 5, 5));
                    break;
                }
            }
        }

        //move alien bullets
        public void moveAlienBullet()
        {
            for (int i = 0; i < alienBullet.Count(); i++)
            {
                int y = alienBullet[i].Y + alienBulletSpeed;
                alienBullet[i] = new Rectangle(alienBullet[i].X, y, 5, 20);
            }
        }


    }
}
