using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;

namespace GameOfLife
{
    //Дефолтные правила:
    //3 живые клетки == новая жизнь
    //2 или 3 живые соседки, продолжает жить
    // < 2 || > 3, клетка умирает
    internal partial class CellAutomat
    {
        public CellAutomat(int width, int height)
        {
            Width = width;
            Height = height;
            Bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            ProcentGeneralCell = 30;
            procentProducerCell = 15;
            procentDestroyerCell = 20;
            procentGooderCell = 10;
        }

        public void StartGame()
        {
            StartRandomGenerate(); //Генерируем рандомное игровое поле
        }

        private void StartRandomGenerate()
        {
            ClearColor();
            GenerateRandomColor();
        }

        public unsafe partial bool IsContinue();
        //private unsafe partial bool Eval(byte* prevRowY, byte* curRowY, byte* nextRowY, int curX, int curY = -1);
        private unsafe partial void GenerateRandomColor();
        private unsafe partial void ClearColor();
    }
}
