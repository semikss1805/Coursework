﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Shape
    {
        public int x;
        public int y;
        public int[,] matrix;
        public int[,] nextMatrix;
        public int sizeMatrix;
        public int sizeNextMatrix;
        Random r = new Random();
        public int[,] tetr1 = new int[4, 4]
        {
            {0,1,0,0},
            {0,1,0,0},
            {0,1,0,0},
            {0,1,0,0},
        };
        public int[,] tetr2 = new int[3, 3]
        {
            {2,0,0},
            {2,2,0},
            {0,2,0},
        };
        public int[,] tetr3 = new int[3, 3]
        {
            {0,3,0},
            {0,3,3},
            {0,3,0},
        };
        public int[,] tetr4 = new int[3, 3]
        {
            {4,0,0},
            {4,0,0},
            {4,4,0},
        };
        public int[,] tetr5 = new int[3, 3]
        {
            {0,5,0},
            {0,5,0},
            {5,5,0},
        };
        public int[,] tetr6 = new int[2, 2]
        {
            {6,6},
            {6,6},
        };
        public int[,] tetr7 = new int[3, 3]
        {
            {0,7,0},
            {7,7,0},
            {7,0,0},
        };

        public Shape(int _x, int _y)
        {
            x = _x;
            y = _y;
            r = new Random(GetHashCode() + (int)DateTime.Now.Ticks);
            matrix = GenerateMatrix();
            sizeMatrix = (int)Math.Sqrt(matrix.Length);
            nextMatrix = GenerateMatrix();       
            sizeNextMatrix = (int)Math.Sqrt(nextMatrix.Length);
        }
        public void ResetShape(int _x, int _y)
        {
            x = _x;
            y = _y;
            r = new Random(GetHashCode() + (int)DateTime.Now.Ticks);
            matrix = nextMatrix;
            sizeMatrix = (int)Math.Sqrt(matrix.Length);
            nextMatrix = GenerateMatrix();
            sizeNextMatrix = (int)Math.Sqrt(nextMatrix.Length);
        }
        public int[,] GenerateMatrix()
        {
            int[,] _matrix = tetr1;
                switch (r.Next(1, 8))
                {
                    case 1:
                        _matrix = tetr1;
                        break;
                    case 2:
                        _matrix = tetr2;
                        break;
                    case 3:
                        _matrix = tetr3;
                        break;
                    case 4:
                        _matrix = tetr4;
                        break;
                    case 5:
                        _matrix = tetr5;
                        break;
                    case 6:
                        _matrix = tetr6;
                        break;
                    case 7:
                        _matrix = tetr7;
                        break;
            }
            return _matrix;
        }

        public void RotateShape()
        {
            int[,] tempMatrix = new int[sizeMatrix, sizeMatrix];
            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    tempMatrix[i, j] = matrix[j, (sizeMatrix - 1) - i];
                }
            }
            matrix = tempMatrix;
            int offset1 = (8 - (x + sizeMatrix));
            if (offset1 < 0)
            {
                for (int i = 0; i < Math.Abs(offset1); i++)
                    MoveLeft();
            }

            if (x < 0)
            {
                for (int i = 0; i < Math.Abs(x) + 1; i++)
                    MoveRight();
            }

        }
        public void MoveDown()
        {
            y++;
        }
        public void MoveRight()
        {

            x++;
        }
        public void MoveLeft()
        {
            x--;
        }
    }
}
