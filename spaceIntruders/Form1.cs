/* Gurvir Uppal
 * Space Intruders Final Project
 * Mr.T
 * ICS3U
 * June 17, 2021
 */

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
using System.Media;

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

        int soundCounter;

        int score = 0;
        int lives = 3;

        bool aDown = false;
        bool dDown = false;

        string gameState = "waiting";

        SoundPlayer musicPlayer;
        SoundPlayer shootPlayer;
        SoundPlayer movingSound;
        SoundPlayer moving2Sound;

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
            gameTimer.Enabled = true;

            titleLabel.Text = "";
            subTitleLabel.Text = "";

            titleLabel.Visible = false;
            subTitleLabel.Visible = false;

            life1label.Visible = true;
            life2Label.Visible = true;
            life3Label.Visible = true;

            alienSpeed = 2;

            score = 0;
            lives = 3;

            scoreLabel.Text = $"Score {score}";

            alien.Clear();
            alienBullet.Clear();
            playerBullet.Clear();

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
                    if (gameState == "waiting" || gameState == "over" || gameState == "winner")
                    {
                        GameInitialize();
                    }
                    break;
                case Keys.Escape:
                    if (gameState == "waiting" || gameState == "over" || gameState == "winner")
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

            //stops it so the game end sound doesnt glitch and not play
            if (gameTimer.Enabled == true)
            {
                shootPlayer = new SoundPlayer(Properties.Resources.shoot);
                shootPlayer.Play();
            }
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

            //if bullet hits player 
            playerCollisionBullet();

            //if aliens reach the end
            aliensReachPlayer();

            //game over
            gameOver();

            //winner
            winner();

            //change sound
            speedSoundChange();

            soundCounter++;

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

                if (lives == 3)
                {
                    livesLabel.Text = $"lives";
                    life1label.Image = Properties.Resources.spaceship;
                    life2Label.Image = Properties.Resources.spaceship;
                    life3Label.Image = Properties.Resources.spaceship;
                }
                else if (lives == 2)
                {
                    livesLabel.Text = $"lives";
                    life3Label.Visible = false;
                }
                else if (lives == 1)
                {
                    livesLabel.Text = $"lives";
                    life1label.Image = Properties.Resources.spaceship;
                    life2Label.Visible = false;
                    life3Label.Visible = false;
                }
                else
                {
                    livesLabel.Text = $"lives";
                    life1label.Visible = false;
                    life2Label.Visible = false;
                    life3Label.Visible = false;
                }
            }
            else if (gameState == "over")
            {
                scoreLabel.Text = "";
                livesLabel.Text = "";

                life1label.Visible = false;
                life2Label.Visible = false;
                life3Label.Visible = false;

                titleLabel.Visible = true;
                subTitleLabel.Visible = true;

                titleLabel.Text = "GAME OVER";
                subTitleLabel.Text = $"Your final score was {score}";

                subTitleLabel.Text += "\nPress Space Barto Play Again or Escape to Exit";
            }
            else if (gameState == "winner")
            {
                scoreLabel.Text = "";
                livesLabel.Text = "";

                life1label.Visible = false;
                life2Label.Visible = false;
                life3Label.Visible = false;

                titleLabel.Visible = true;
                subTitleLabel.Visible = true;

                titleLabel.Text = "You Saved The World!!";
                subTitleLabel.Text = $"Your final score was {score}";

                subTitleLabel.Text += "\nPress Space Bar to Play Again or Escape to Exit";
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
                        musicPlayer = new SoundPlayer(Properties.Resources.invaderkilled);
                        musicPlayer.Play();
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
                    alienSpeed = 5;

                    movingSound = new SoundPlayer(Properties.Resources.fastinvader2);
                }
                else
                {
                    alienSpeed = -5;

                    movingSound = new SoundPlayer(Properties.Resources.fastinvader2);
                }
            }
            else if (alien.Count == 1)
            {
                if (alienSpeed > 0)
                {
                    alienSpeed = 20;

                    movingSound = new SoundPlayer(Properties.Resources.fastinvader4);
                }
                else
                {
                    alienSpeed = -20;

                    movingSound = new SoundPlayer(Properties.Resources.fastinvader4);
                }
            }
        }

        //spawn alien bullets
        public void spawnAlienBullet()
        {
            randValue = randGen.Next(0, 101);

            if (randValue < 6)
            {
                randValue = randGen.Next(0, alien.Count);   //pick a random alien

                int y = randGen.Next(alien[randValue].Y, alien[randValue].Y);  //get that random aliens X and put it into x
                int x = randGen.Next(alien[randValue].X, alien[randValue].X);  //get that random aliens Y and put it into y

                alienBullet.Add(new Rectangle(x, y + 20, 5, 5));               //spawn a bullet at x and y + 20

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

        //if alien bullet hits player
        public void playerCollisionBullet()
        {
            for (int i = 0; i < alienBullet.Count(); i++)
            {
                if (alienBullet[i].IntersectsWith(player))
                {
                    alienBullet.RemoveAt(i);
                    lives--;
                    musicPlayer = new SoundPlayer(Properties.Resources.explosion);
                    musicPlayer.Play();
                    break;
                }
            }
        }

        //alien reach player
        public void aliensReachPlayer()
        {
            for (int i = 0; i < alien.Count(); i++)
            {
                if (alien[i].Y > this.Height - 60)
                {
                    gameTimer.Enabled = false;
                    gameState = "over";
                    musicPlayer = new SoundPlayer(Properties.Resources.Game_Over_Arcade___Sound_Effect_HD);
                    musicPlayer.Play();
                }
            }
        }

        //speed sound change
        public void speedSoundChange()
        {
            if (soundCounter == 30 && alien.Count > 2)
            {
                movingSound = new SoundPlayer(Properties.Resources.fastinvader2);
                movingSound.Play();
                soundCounter = 0;
            }
            else if (soundCounter == 30 && alien.Count == 1)
            {
                movingSound.Stop();
                moving2Sound = new SoundPlayer(Properties.Resources.fastinvader4);
                moving2Sound.Play();
                soundCounter = 0;
            }
        }

        //game over
        public void gameOver()
        {
            if (lives == 0)
            {
                gameTimer.Enabled = false;
                gameState = "over";
                musicPlayer = new SoundPlayer(Properties.Resources.Game_Over_Arcade___Sound_Effect_HD);
                musicPlayer.Play();
            }
        }

        //winnneraaaad
        public void winner()
        {
            if (alien.Count == 0)
            {
                shootPlayer.Stop();
                gameTimer.Enabled = false;
                gameState = "winner";
                musicPlayer = new SoundPlayer(Properties.Resources.Success__win_sound_effect);
                musicPlayer.Play();
            }
        }

    }
}
