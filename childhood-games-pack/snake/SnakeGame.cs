﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace childhood_games_pack.snake {

    public enum SNAKE_DIRECTION {STOP = 0, LEFT = 1, RIGHT = 2, UP = 3, DOWN = 4 };

    public partial class SnakeGame : Form 
    {
        MainMenuForm mainMenu;

        Point foodPoint;
        List<Point> snakeBlocks;

        Random random = new Random();
        Graphics snakePanelCanvas;

        int BLOCK_SIZE  = 20;
        int initialSnakeSize = 3;
        bool startGame;

        int startSpeed;
        int score;
        SNAKE_DIRECTION snakeDirection;

        public SnakeGame(MainMenuForm mainMenu)
        {
            InitializeComponent();

            this.mainMenu = mainMenu;

            snakePanel.Hide();
            scoreLabel.Hide();
            resLabel.Hide();
            pauseGameButton.Hide();
            endGameButton.Hide();

            this.DoubleBuffered = true;
        }

        void StartGameButton_Click(object sender, EventArgs e)
        {
            startGameButton.Hide();

            snakePanel.Show();
            resLabel.Text = "0";
            resLabel.Show();
            score = 0;
            scoreLabel.Show();
            pauseGameButton.Show();
            endGameButton.Show();

            startSpeed = 400;
            snakeDirection = (int)SNAKE_DIRECTION.STOP;

            snakePanelCanvas = snakePanel.CreateGraphics();
            snakePanel.Invalidate();

            Point startBlock = CreateStartSnakeBlocks();
            Rectangle snake;
            snakeBlocks = new List<Point>();

            for (int i = 0; i < initialSnakeSize; i++) 
            {
                startBlock.X = startBlock.X - BLOCK_SIZE;
                snakeBlocks.Add(startBlock);//храним координаты блоков

                snake = new Rectangle(startBlock, new Size(BLOCK_SIZE, BLOCK_SIZE));
                if (i == 0)
                {
                    snakePanelCanvas.FillEllipse(Brushes.DarkCyan, snake);
                    snakePanelCanvas.DrawEllipse(Pens.DarkCyan, snake);
                }
                else
                {
                    snakePanelCanvas.DrawEllipse(Pens.DarkCyan, snake);
                }
            }

            CreateFood();
            startGame = true; 

            snakePanel.Focus();

            snakeBackgroundWorker.RunWorkerAsync();
        }

        void SnakeGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            startGame = false;
            snakeBackgroundWorker.CancelAsync();
            mainMenu.Show();
        }

        bool СheckStartSnake(Point head) 
        {
            if (head.X - 2 * BLOCK_SIZE < snakePanel.Left ||
                head.X + BLOCK_SIZE > snakePanel.Right ||
                head.Y + BLOCK_SIZE > snakePanel.Bottom ||
                head.Y < snakePanel.Top)
            {
                return false;
            }
            return true;
        }

        bool CheckBorderAndFood(Point block)
        {
            if (block.X + 3 * BLOCK_SIZE < snakePanel.Left ||
                block.X + 3 * BLOCK_SIZE > snakePanel.Right ||
                block.Y + BLOCK_SIZE > snakePanel.Bottom ||
                block.Y + BLOCK_SIZE < snakePanel.Top ||
                snakeBlocks.Contains(block))
            {

                EndGame();

                return false;
            }

            if (block == foodPoint)
            {
                score += 10;
                this.InvokeIfNeeded(() => resLabel.Text = score.ToString());

                snakeBlocks.Add(snakeBlocks[snakeBlocks.Count - 1]);
                CreateFood();

                switch (snakeBlocks.Count)
                {
                    case 5: {
                        startSpeed = 300;
                        break;
                    }

                    case 10: {
                        startSpeed = 200;
                        break;
                    }

                    case 15: {
                        startSpeed = 100;
                        break;
                    }
                }
            }
            return true;
        }

        void EndGame()
        {
            startGame = false;
            snakeDirection = (int)SNAKE_DIRECTION.STOP;

            MessageBox.Show("Game over!");

            snakeBackgroundWorker.CancelAsync();

            startGameButton.Enabled = true;
            this.InvokeIfNeeded(() => startGameButton.Show());
        }

        Point CreateStartSnakeBlocks()
        {
            Point block = new Point(random.Next() % (snakePanel.Width - BLOCK_SIZE), random.Next() % (snakePanel.Height - BLOCK_SIZE));
            while (!СheckStartSnake(block) || block.X % BLOCK_SIZE != 0 || block.Y % BLOCK_SIZE != 0)
            {
                block.X = random.Next() % snakePanel.Width;
                block.Y = random.Next() % snakePanel.Height;
            }

            return block;
        }

        void CreateFood()
        {
            foodPoint = new Point(random.Next() % (snakePanel.Width - BLOCK_SIZE), random.Next() % (snakePanel.Height - BLOCK_SIZE));

            while (snakeBlocks.Contains(foodPoint) || foodPoint.X % BLOCK_SIZE != 0 || foodPoint.Y % BLOCK_SIZE != 0)
            {
                foodPoint.X = random.Next() % (snakePanel.Width - BLOCK_SIZE);
                foodPoint.Y = random.Next() % (snakePanel.Height - BLOCK_SIZE);
            }

            Rectangle food = new Rectangle(foodPoint, new Size(BLOCK_SIZE, BLOCK_SIZE));
            snakePanel.Invalidate(food);
        }

        void SnakePanel_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < snakeBlocks.Count; i++)
            {
                Rectangle snake = new Rectangle(snakeBlocks[i], new Size(BLOCK_SIZE, BLOCK_SIZE));

                if (i == 0)
                {
                    snakePanelCanvas.FillEllipse(Brushes.DarkCyan, snake);
                    snakePanelCanvas.DrawEllipse(Pens.DarkCyan, snake);
                }
                else
                {
                    snakePanelCanvas.DrawEllipse(Pens.DarkCyan, snake);
                }
            }

            Rectangle food = new Rectangle(foodPoint, new Size(BLOCK_SIZE, BLOCK_SIZE));
            snakePanelCanvas.FillEllipse(Brushes.Coral, food);
            snakePanelCanvas.DrawEllipse(Pens.Coral, food);
        }

        void SnakePanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left:
                { 
                    e.IsInputKey = true;
                    if (snakeDirection == SNAKE_DIRECTION.RIGHT || 
                        snakeDirection == SNAKE_DIRECTION.STOP)
                    {
                        break;
                    }

                    snakeDirection = SNAKE_DIRECTION.LEFT;
                    break;
                }

                case Keys.D:
                case Keys.Right:
                {
                    e.IsInputKey = true;
                    if (snakeDirection == SNAKE_DIRECTION.LEFT)
                    {
                        break;
                    }

                    snakeDirection = SNAKE_DIRECTION.RIGHT;
                    break;
                }

                case Keys.S:
                case Keys.Down: 
                { 
                    e.IsInputKey = true;
                    if (snakeDirection == SNAKE_DIRECTION.UP)
                    {
                        break;
                    }

                    snakeDirection = SNAKE_DIRECTION.DOWN;
                    break;
                }

                case Keys.W:
                case Keys.Up: 
                {
                    e.IsInputKey = true;
                    if (snakeDirection == SNAKE_DIRECTION.DOWN)
                    {
                        break;
                    }

                    snakeDirection = SNAKE_DIRECTION.UP;
                    break;
                }

                default: {
                    break;
                }
            }
        }

        void ReDrawSnake(Point head)
        {
            List<Point> helpList = new List<Point>();

            if (head == snakeBlocks[1])
            {
                return;
            }

            Point topLeft = snakeBlocks[0];
            Point bottonRight = snakeBlocks[0];
            for (int i = 1; i < snakeBlocks.Count; i++)
            {
                if (snakeBlocks[i].X < topLeft.X)
                {
                    topLeft.X = snakeBlocks[i].X;
                }
                if (snakeBlocks[i].Y < topLeft.Y)
                {
                    topLeft.Y = snakeBlocks[i].Y;
                }
                if (snakeBlocks[i].X + BLOCK_SIZE > bottonRight.X)
                {
                    bottonRight.X = snakeBlocks[i].X + BLOCK_SIZE;
                }
                if (snakeBlocks[i].Y + BLOCK_SIZE > bottonRight.Y)
                {
                    bottonRight.Y = snakeBlocks[i].Y + BLOCK_SIZE;
                }
            }

            helpList.Add(head);
            for (int i = 1; i < snakeBlocks.Count; i++)
            {
                helpList.Add(snakeBlocks[i - 1]);
            }
            
            snakeBlocks = helpList;
            
            Rectangle snakeBlock = new Rectangle(topLeft, new Size(bottonRight.X - topLeft.X + BLOCK_SIZE, bottonRight.Y - topLeft.Y + BLOCK_SIZE));
            snakePanel.Invalidate(snakeBlock);
        }

        void SnakeBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Point head;
            while (true)
            {
                if (snakeBackgroundWorker.CancellationPending)
                {
                    return;
                }

                while (startGame)
                {
                    switch (snakeDirection) {
                        case SNAKE_DIRECTION.LEFT: {
                            head = new Point(snakeBlocks[0].X - BLOCK_SIZE, snakeBlocks[0].Y);
                            if (!CheckBorderAndFood(head))
                            {
                                continue;
                            }
                            ReDrawSnake(head);

                            break;
                        }

                        case SNAKE_DIRECTION.RIGHT: 
                        { 
                            head = new Point(snakeBlocks[0].X + BLOCK_SIZE, snakeBlocks[0].Y);
                            if (!CheckBorderAndFood(head))
                            {
                                continue;
                            }
                            ReDrawSnake(head);

                            break;
                        }

                        case SNAKE_DIRECTION.UP: 
                        { 
                            head = new Point(snakeBlocks[0].X, snakeBlocks[0].Y - BLOCK_SIZE);
                            if (!CheckBorderAndFood(head))
                            {
                                continue;
                            }
                            ReDrawSnake(head);

                            break;
                        }

                        case SNAKE_DIRECTION.DOWN:
                        { 
                            head = new Point(snakeBlocks[0].X, snakeBlocks[0].Y + BLOCK_SIZE);
                            if (!CheckBorderAndFood(head))
                            {
                                continue;
                            }
                            ReDrawSnake(head);

                            break;
                        }
                    }

                    Thread.Sleep(startSpeed);
                }
            }
        }

        void EndGameButton_Click(object sender, EventArgs e)
        {
            EndGame();
        }

        void PauseGameButton_Click(object sender, EventArgs e)
        {
            if (pauseGameButton.Text == "Pause game") 
            {
                startGame = false;
                pauseGameButton.Text = "Continue game";
            }
            else if (pauseGameButton.Text == "Continue game")
            {
                startGame = true;
                snakePanel.Focus();
                pauseGameButton.Text = "Pause game";
            }
        }
    }

    public static class ControlExtentions
    {
        public static void InvokeIfNeeded(this Control control, Action doit)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(doit);
            }
            else
            {
                doit();
            }
        }
    }
}