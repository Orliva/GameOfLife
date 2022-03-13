using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    //TODO:Добавить клетки которые производнят другие клетки (Столовка в общаге)
    //TODO:Добавить клетки, которые убивают другие клетки (универ)
    //TODO:Божественное проведение! (Клетки сами создаются, если вокруг только пустые клетки! (возможно какой то фигурой))
    internal partial class CellAutomat
    {
        public int Width { get; }
        public int Height { get; }
        public Bitmap Bitmap { get; }
        private const int pixelSize = 4; // 32 bits per pixel
        private readonly Color frontColor = Color.Green;
        private readonly Color backColor = Color.FromArgb(192, 185, 221);
        private readonly Color producerColor = Color.Red;
        private readonly Color badColor = Color.DeepPink;

        private readonly Random rnd = new Random();
        private int gen = 0;
        private int genHelper = 1;

        private int procentGeneralCell;
        public int ProcentGeneralCell
        {
            get
            {
                return procentGeneralCell;
            }
            set
            {
                if (value >= 0 && value <= 100)
                    procentGeneralCell = value;
            }
        }
        private int procentProducerCell;
        public int ProcentProducerCell
        {
            get
            {
                return procentProducerCell;
            }
            set
            {
                if (value >= 0 && value <= 100)
                    procentProducerCell = value;
            }
        }

        private int procentDestroyerCell;
        public int ProcentDestroyerCell
        {
            get
            {
                return procentDestroyerCell;
            }
            set
            {
                if (value >= 0 && value <= 100)
                    procentDestroyerCell = value;
            }
        }

        private int procentGooderCell;
        public int ProcentGooderCell
        {
            get
            {
                return procentGooderCell;
            }
            set
            {
                if (value >= 0 && value <= 100)
                    procentGooderCell = value;
            }
        }


        private BitmapData? sourceData = null!;
        //TODO:Косяк с границей
        //отдельно проверять x=0, x==max, y=0, y=max
        public unsafe partial bool IsContinue()
        {
            int counter = 0;

            try
            {
                sourceData = Bitmap.LockBits(
                  new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                  ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                for (int y = 1; y < Bitmap.Height - 1; ++y)
                {
                    byte* prevRow = (byte*)sourceData.Scan0 + ((y - 1) * sourceData.Stride);
                    byte* curRow = (byte*)sourceData.Scan0 + (y * sourceData.Stride);
                    byte* nextRow = (byte*)sourceData.Scan0 + ((y + 1) * sourceData.Stride);

                    for (int x = 1; x < Bitmap.Width - 1; ++x)
                    {
                        if (Eval(prevRow, curRow, nextRow, x, y))
                            counter++;
                    }
                }

                if (Configs.Complication5)
                {
                    for (int y = 1; y < Bitmap.Height - 1; ++y)
                    {
                        byte* prevRow = (byte*)sourceData.Scan0 + ((y - 1) * sourceData.Stride);
                        byte* curRow = (byte*)sourceData.Scan0 + (y * sourceData.Stride);
                        byte* nextRow = (byte*)sourceData.Scan0 + ((y + 1) * sourceData.Stride);

                        for (int x = 1; x < Bitmap.Width - 1; ++x)
                        {
                            if (IsBadCell(curRow, x))
                                BadBoom(prevRow, curRow, nextRow, x);
                        }
                    }
                }
            }
            finally
            {
                if (sourceData != null)
                    Bitmap.UnlockBits(sourceData);
                sourceData = null;
            }

            if (genHelper == counter)
                gen++;
            else
                gen = 0;

            genHelper = counter;

            if (gen > 10)
            {
                gen = 0;
                return false;
            }

            if (counter > 0)
                return true;
            return false;
        }

        private unsafe bool Eval(byte* prevRowY, byte* curRowY, byte* nextRowY, int curX, int curY)
        {
            bool isLife = IsLife(curRowY, curX); //Состояние клетки: живое, неживое
            int counter = isLife ? -1 : 0; //Если клетка живая, то мы не будет учитывать ее саму
                                           //при подсчете живых соседей
            //int counter = -1; //Считать ли саму клетку своей соседкой??
            //Кол-во живых клеток рядом
            for (int x = curX - 1; x <= curX + 1; x++)
            {
                if (IsLife(prevRowY, x))
                    counter++;

                if (IsLife(curRowY, x))
                    counter++;

                if (IsLife(nextRowY, x))
                    counter++;
            }


            if (!isLife) //Если клетка неживая, то попробуй оживить
            {
                if (counter == 0)
                {
                    //  x   x
                    //    x
                    //  x   x
                    if (Configs.Complication4 == true)
                    {
                        if (rnd.Next(0, 100) < procentGooderCell)
                        {
                            CreateCell(prevRowY, curX - 1);
                            CreateCell(curRowY, curX);
                            CreateCell(nextRowY, curX + 1);
                            CreateCell(prevRowY, curX + 1);
                            CreateCell(nextRowY, curX - 1);
                        }
                    }
                }
                else if (counter == 3)
                {
                    SetCell(curRowY, curX, counter);
                    return true;
                }
            }
            //Если клетка живая, смотрим нужно ли ее умертвить
            else if (counter < 2)
            {
                if (Configs.Complication1 == false)
                {
                    curRowY[curX * pixelSize + 0] = backColor.B;
                    curRowY[curX * pixelSize + 1] = backColor.G;
                    curRowY[curX * pixelSize + 2] = backColor.R;
                    curRowY[curX * pixelSize + 3] = backColor.A;
                    return true;
                }
            }
            else if (counter > 3)
            {
                if (Configs.Complication2 == false) //Должны умереть от перенаселения
                {
                    if (Configs.Complication3 == true)
                        if (counter >= 5)
                            SetCell(curRowY, curX, counter, GetEmptyCell(prevRowY, curRowY, nextRowY, curX, 8 - counter));

                    curRowY[curX * pixelSize + 0] = backColor.B;
                    curRowY[curX * pixelSize + 1] = backColor.G;
                    curRowY[curX * pixelSize + 2] = backColor.R;
                    curRowY[curX * pixelSize + 3] = backColor.A;
                    return true;
                }
                else //НЕ должны умирать от "перенаселения"
                {
                    if (Configs.Complication3 == true)
                        if (counter >= 5)
                        {
                            SetCell(curRowY, curX, counter, GetEmptyCell(prevRowY, curRowY, nextRowY, curX, 8 - counter));
                            return true;
                        }
                }
            }
            return false;
        }

        private unsafe bool IsBadCell(byte* curRowY, int curX)
        {
            if ((curRowY[curX * pixelSize + 0] == badColor.B) &&
                (curRowY[curX * pixelSize + 1] == badColor.G) &&
                (curRowY[curX * pixelSize + 2] == badColor.R) &&
                (curRowY[curX * pixelSize + 3] == badColor.A))
            {
                return true;
            }
            return false;
        }

        private unsafe bool IsLife(byte* curRowY, int curX)
        {
            if ((curRowY[curX * pixelSize + 0] == frontColor.B || curRowY[curX * pixelSize + 0] == producerColor.B) &&
                (curRowY[curX * pixelSize + 1] == frontColor.G || curRowY[curX * pixelSize + 1] == producerColor.G) &&
                (curRowY[curX * pixelSize + 2] == frontColor.R || curRowY[curX * pixelSize + 2] == producerColor.R) &&
                (curRowY[curX * pixelSize + 3] == frontColor.A || curRowY[curX * pixelSize + 3] == producerColor.A))
            {
                return true;
            }
            return false;
        }

        private unsafe void SetCell(byte* curRowY, int curX, int countLifeCell, 
            byte*[]? emptyCells = null, byte* prevRowY = null, byte* nextRowY = null)
        {
            if (countLifeCell == 3)
                CreateCell(curRowY, curX);
            else if (countLifeCell >= 5)
            {
                if (Configs.Complication3 == false) //Должны произвести 2 штуки новых если counter >= 5
                    return;
                else                                //Должны произвести 2 штуки новых если counter >= 5
                {
                    if (emptyCells == null)
                        throw new ArgumentException($"{nameof(emptyCells)} is null!", nameof(emptyCells));

                    int len = GetLength(emptyCells);
                    if (len == 0)
                        return;
                    byte*[] newLifeCell = new byte*[2];
                    int index = -100;
                    int tmpIndex;
                    int genTmp = 0;

                    for (int i = 0; i < 2; i++)
                    {
                        while ((tmpIndex = rnd.Next(0, (len - 1))) == index)
                        {
                            if (genTmp > 8)
                                break;
                            genTmp++;
                        }
                        index = tmpIndex;
                        newLifeCell[i] = emptyCells[index];

                        CreateCell(newLifeCell[i], 0);
                    }
                }
            }
        }

        private unsafe byte*[] GetEmptyCell(byte* prevRowY, byte* curRowY, byte* nextRowY, int curX, int countDeadCell)
        {
            byte*[] emptes = new byte*[countDeadCell];
            int count = 0;

            for (int x = curX - 1; x <= curX + 1; x++)
            {
                if (!IsLife(prevRowY, x))
                    emptes[count++] = prevRowY + (x * pixelSize + 0);

                if (!IsLife(curRowY, x))
                    emptes[count++] = curRowY + (x * pixelSize + 0);

                if (!IsLife(nextRowY, x))
                    emptes[count++] = nextRowY + (x * pixelSize + 0);
            }

            return emptes;
        }

        private unsafe int GetLength(byte*[] arr)
        {
            int len = 0;
            foreach(var i in arr)
                len++;
            return len;
        }

        private unsafe void EvalBadBoom(byte* prevRowY, byte* curRowY, byte* nextRowY, int curX, int curY)
        {
            if (curY > 1 && curY < Bitmap.Height - 2)
                BadBoom((byte*)sourceData!.Scan0 + ((curY - 2) * sourceData.Stride),
                    prevRowY, curRowY, curX - 1);
            else
                BadBoom(prevRowY, curRowY, nextRowY, curX);
        }

        private unsafe void BadBoom(byte* prevRowY, byte* curRowY, byte* nextRowY, int curX)
        {
            for (int x = curX - 1; x <= curX + 1; x++)
            {
                prevRowY[x * pixelSize + 0] = backColor.B;
                prevRowY[x * pixelSize + 1] = backColor.G;
                prevRowY[x * pixelSize + 2] = backColor.R;
                prevRowY[x * pixelSize + 3] = backColor.A;

                curRowY[x * pixelSize + 0] = backColor.B;
                curRowY[x * pixelSize + 1] = backColor.G;
                curRowY[x * pixelSize + 2] = backColor.R;
                curRowY[x * pixelSize + 3] = backColor.A;

                nextRowY[x * pixelSize + 0] = backColor.B;
                nextRowY[x * pixelSize + 1] = backColor.G;
                nextRowY[x * pixelSize + 2] = backColor.R;
                nextRowY[x * pixelSize + 3] = backColor.A;
            }
        }

        private unsafe bool IsBadCellCreated(byte* curRowY, int curX)
        {
            CreateCell(curRowY, curX);
            
            if (Configs.Complication4)
                return IsBadCell(curRowY, curX);

            return false;
        }

        private unsafe void CreateCell(byte* curRowY, int curX)
        {
            //НЕ существуют "производителей" и "плохих" клеток
            if (!Configs.Complication3 && !Configs.Complication5) 
            {
                curRowY[curX * pixelSize + 0] = frontColor.B;
                curRowY[curX * pixelSize + 1] = frontColor.G;
                curRowY[curX * pixelSize + 2] = frontColor.R;
                curRowY[curX * pixelSize + 3] = frontColor.A;
            }
            else if (Configs.Complication3 && Configs.Complication5) //Существуют "производители" и "плохие" клетки
            {
                //TODO: написать проверку на то, что проценты производителей и разрушителей в сумме не превышали 100
                int tmp = rnd.Next(-1, 100);
                if (tmp < procentProducerCell)
                {
                    curRowY[curX * pixelSize + 0] = producerColor.B;
                    curRowY[curX * pixelSize + 1] = producerColor.G;
                    curRowY[curX * pixelSize + 2] = producerColor.R;
                    curRowY[curX * pixelSize + 3] = producerColor.A;
                }
                else if (tmp > procentProducerCell && tmp < (procentProducerCell + procentDestroyerCell))
                {
                    curRowY[curX * pixelSize + 0] = badColor.B;
                    curRowY[curX * pixelSize + 1] = badColor.G;
                    curRowY[curX * pixelSize + 2] = badColor.R;
                    curRowY[curX * pixelSize + 3] = badColor.A;
                }
                else
                {
                    curRowY[curX * pixelSize + 0] = frontColor.B;
                    curRowY[curX * pixelSize + 1] = frontColor.G;
                    curRowY[curX * pixelSize + 2] = frontColor.R;
                    curRowY[curX * pixelSize + 3] = frontColor.A;
                }
            }
            else if (Configs.Complication3)
            {
                int tmp = rnd.Next(-1, 100);

                if (tmp < procentProducerCell)
                {
                    curRowY[curX * pixelSize + 0] = producerColor.B;
                    curRowY[curX * pixelSize + 1] = producerColor.G;
                    curRowY[curX * pixelSize + 2] = producerColor.R;
                    curRowY[curX * pixelSize + 3] = producerColor.A;
                }
                else
                {
                    curRowY[curX * pixelSize + 0] = frontColor.B;
                    curRowY[curX * pixelSize + 1] = frontColor.G;
                    curRowY[curX * pixelSize + 2] = frontColor.R;
                    curRowY[curX * pixelSize + 3] = frontColor.A;
                }
            }
            else if (Configs.Complication5)
            {
                int tmp = rnd.Next(-1, 100);

                if (tmp < procentDestroyerCell)
                {
                    curRowY[curX * pixelSize + 0] = badColor.B;
                    curRowY[curX * pixelSize + 1] = badColor.G;
                    curRowY[curX * pixelSize + 2] = badColor.R;
                    curRowY[curX * pixelSize + 3] = badColor.A;
                }
                else
                {
                    curRowY[curX * pixelSize + 0] = frontColor.B;
                    curRowY[curX * pixelSize + 1] = frontColor.G;
                    curRowY[curX * pixelSize + 2] = frontColor.R;
                    curRowY[curX * pixelSize + 3] = frontColor.A;
                }
            }
        }

        private unsafe partial void GenerateRandomColor()
        {
            BitmapData sourceData = null!;
            bool rndFlag;

            try
            {
                sourceData = Bitmap.LockBits(
                  new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                  ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                for (int y = 0; y < Bitmap.Height; ++y)
                {
                    byte* sourceRow = (byte*)sourceData.Scan0 + (y * sourceData.Stride);

                    for (int x = 0; x < Bitmap.Width; ++x)
                    {
                        rndFlag = rnd.Next(-1, 100) < ProcentGeneralCell;// [0..100]
                        if (rndFlag)
                        {
                            if (Configs.Complication3)
                            {
                                rndFlag = rnd.Next(-1, 100) < procentProducerCell;// [0..100]
                                if (rndFlag)
                                {
                                    sourceRow[x * pixelSize + 0] = producerColor.B;
                                    sourceRow[x * pixelSize + 1] = producerColor.G;
                                    sourceRow[x * pixelSize + 2] = producerColor.R;
                                    sourceRow[x * pixelSize + 3] = producerColor.A;
                                }
                                else
                                {
                                    sourceRow[x * pixelSize + 0] = frontColor.B;
                                    sourceRow[x * pixelSize + 1] = frontColor.G;
                                    sourceRow[x * pixelSize + 2] = frontColor.R;
                                    sourceRow[x * pixelSize + 3] = frontColor.A;
                                }
                            }
                            else if (Configs.Complication5)
                            {
                                rndFlag = rnd.Next(-1, 100) < procentProducerCell;// [0..100]
                                if (rndFlag)
                                {
                                    sourceRow[x * pixelSize + 0] = producerColor.B;
                                    sourceRow[x * pixelSize + 1] = producerColor.G;
                                    sourceRow[x * pixelSize + 2] = producerColor.R;
                                    sourceRow[x * pixelSize + 3] = producerColor.A;
                                }
                                else
                                {
                                    sourceRow[x * pixelSize + 0] = frontColor.B;
                                    sourceRow[x * pixelSize + 1] = frontColor.G;
                                    sourceRow[x * pixelSize + 2] = frontColor.R;
                                    sourceRow[x * pixelSize + 3] = frontColor.A;
                                }
                            }
                            else
                            {
                                sourceRow[x * pixelSize + 0] = frontColor.B;
                                sourceRow[x * pixelSize + 1] = frontColor.G;
                                sourceRow[x * pixelSize + 2] = frontColor.R;
                                sourceRow[x * pixelSize + 3] = frontColor.A;
                            }
                        }
                        else
                        {
                            sourceRow[x * pixelSize + 0] = backColor.B;
                            sourceRow[x * pixelSize + 1] = backColor.G;
                            sourceRow[x * pixelSize + 2] = backColor.R;
                            sourceRow[x * pixelSize + 3] = backColor.A;
                        }

                    }
                }
            }
            finally
            {
                if (sourceData != null)
                    Bitmap.UnlockBits(sourceData);
            }
        }

        private unsafe partial void ClearColor()
        {
            BitmapData sourceData = null!;

            try
            {
                sourceData = Bitmap.LockBits(
                  new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                  ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                for (int y = 0; y < Bitmap.Height; ++y)
                {
                    byte* sourceRow = (byte*)sourceData.Scan0 + (y * sourceData.Stride);

                    for (int x = 0; x < Bitmap.Width; ++x)
                    {
                        sourceRow[x * pixelSize + 0] = Color.White.B;
                        sourceRow[x * pixelSize + 1] = Color.White.G;
                        sourceRow[x * pixelSize + 2] = Color.White.R;
                        sourceRow[x * pixelSize + 3] = Color.White.A;
                    }
                }
            }
            finally
            {
                if (sourceData != null)
                    Bitmap.UnlockBits(sourceData);
            }
        }
    }
}
