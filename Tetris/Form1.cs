﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Form1 : Form
    {
        Shape currentShape;
        int size;
        int[,] map = new int[16, 8];
        int lineRemoved;
        int score;
        public Form1()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            size = 25;

            score = 0;
            lineRemoved = 0;
            label1.Text = "Score: " + score;
            label2.Text = "Lines : " + lineRemoved;

            currentShape = new Shape(3, 0);

            this.KeyUp += new KeyEventHandler(KeyFunc);

            timer1.Interval = 400;
            timer1.Tick += new EventHandler(Update);

            Invalidate();
        }

        private void KeyFunc(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    Update(this, new EventArgs());
                    break;
                case Keys.Up:
                    if (!IsOverlay())
                    {
                        ResetArea();
                        currentShape.RotateShape();
                        Merge();
                        Invalidate();
                    }
                    break;
                case Keys.Left:
                    if (!CollideHor(-1))
                    {
                        ResetArea();
                        currentShape.MoveLeft();
                        Merge();
                        Invalidate();
                    }
                    break;
                case Keys.Right:
                    if (!CollideHor(+1))
                    {
                        ResetArea();
                        currentShape.MoveRight();
                        Merge();
                        Invalidate();
                    }
                    break;
            }
        }

        private void Update(object sender, EventArgs e)
        {
            ResetArea();
            if (!Collide())
            {
                currentShape.MoveDown();
            }
            else
            {
                Merge();
                SliceMap();
                timer1.Interval = 400;
                currentShape = new Shape(3, 0);

                if (Collide())
                {
                    score = 0;
                    lineRemoved = 0;
                    label1.Text = "Score: " + score;
                    label2.Text = "Lines : " + lineRemoved;
                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            map[i, j] = 0;
                        }
                    }
                }
            }
            Merge();
            Invalidate();
        }
        public void SliceMap()
        {
            int count = 0;
            int curRemovedLines = 0;
            for (int i = 0; i < 16; i++)
            {
                count = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (map[i, j] != 0)
                        count++;
                }
                if (count == 8)
                {
                    curRemovedLines++;
                    for (int k = i; k >= 1; k--)
                    {
                        for (int o = 0; o < 8; o++)
                        {
                            map[k, o] = map[k - 1, o];
                        }
                    }
                }
            }
            for (int i = 0; i < curRemovedLines; i++)
            {
                score += (10 * (i + 1));
            }
            lineRemoved += curRemovedLines;
            label1.Text = "Score: " + score;
            label2.Text = "Lines : " + lineRemoved;
        }
        public bool IsOverlay()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (j >= 0 && j <= 7)
                    {
                        if (i >= 0 && i <= 15)
                        {
                            if (map[i, j] != 0 && currentShape.matrix[i - currentShape.y, j - currentShape.x] == 0)
                            {
                                return true;
                            }
                        }
                        if (i == 16)
                        {
                            return true;
                        }
                    }
                    if (j == 8)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void Merge()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        map[i, j] = currentShape.matrix[i - currentShape.y, j - currentShape.x];
                }
            }
        }

        public bool Collide()
        {
            for (int i = currentShape.y + currentShape.sizeMatrix - 1; i >= currentShape.y; i--)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (i + 1 == 16)
                            return true;
                        if (map[i + 1, j] != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool CollideHor(int dir)
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (j + 1 * dir > 7 || j + 1 * dir < 0)
                            return true;
                        if (map[i, j + 1 * dir] != 0)
                        {
                            if (j - currentShape.x + 1 * dir >= currentShape.sizeMatrix || j - currentShape.x + 1 * dir < 0)
                            {
                                return true;
                            }
                            if (currentShape.matrix[i - currentShape.y, j - currentShape.x + 1 * dir] == 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public void ResetArea()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (i >= 0 && j >= 0 && i < 16 && j < 8)
                    {
                        if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        {
                            map[i, j] = 0;
                        }
                    }
                }
            }
        }
        public void DrawMap(Graphics e)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (map[i, j] == 1)
                    {
                        e.FillRectangle(Brushes.DeepSkyBlue, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.LightBlue, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.DarkOrange, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.BlanchedAlmond, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.LightGreen, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 6)
                    {
                        e.FillRectangle(Brushes.Violet, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                    if (map[i, j] == 7)
                    {
                        e.FillRectangle(Brushes.FloralWhite, new Rectangle(50 + j * size + 3, 50 + i * size + 3, size - 3, size - 3));
                    }
                }
            }
        }

        public void DrawGrid(Graphics g)
        {

            for (int i = 0; i <= 16; i++)
            {
                g.DrawLine(Pens.Black, new Point(50, 50 + i * size), new Point(50 + 8 * size, 50 + i * size));
            }
            for (int i = 0; i <= 8; i++)
            {
                g.DrawLine(Pens.Black, new Point(50 + i * size, 50), new Point(50 + i * size, 50 + 16 * size));
            }
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    g.FillRectangle(Brushes.Gray, new Rectangle(50 + j * size + 1, 50 + i * size + 1, size - 1, size - 1));
                }
            }
        }
        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
            DrawMap(e.Graphics);
        }
    }
}
