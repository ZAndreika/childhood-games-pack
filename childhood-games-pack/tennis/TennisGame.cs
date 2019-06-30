﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace childhood_games_pack.tennis {
    public partial class TennisGame : Form {
        private MainMenuForm mainMenu;
        private Graphics table { get; }
        public UserRacket userRacket { get; }
        public Ball ball { get; }

        public TennisGame(MainMenuForm mainMenu) {
            InitializeComponent();
            this.mainMenu = mainMenu;

            table = TablePanel.CreateGraphics();

            userRacket = new UserRacket(this);
            ball = new Ball(this);

            TablePanel.Controls.Add(userRacket);
            TablePanel.Controls.Add(ball);
            userRacket.Show();
            ball.Show();
            
        }

        private void TennisMainForm_FormClosed(object sender, FormClosedEventArgs e) {
            mainMenu.Show();
        }

        private void TablePanel_Paint(object sender, PaintEventArgs e) {
            Pen whitePen = new Pen(Color.White, 3);
            Point leftPoint = new Point(0, TablePanel.Height / 2);
            Point rightPoint = new Point(TablePanel.Width, TablePanel.Height / 2);
            table.DrawLine(whitePen, leftPoint, rightPoint);
        }
    }

}