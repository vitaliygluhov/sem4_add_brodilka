namespace sem4_add_brodilka;
class Program
{
    static void Main(string[] args)
    {
        int xy(int XY)
        {
            while (XY > 255)
                XY -= 256;
            while (XY < 0)
                XY += 256;
            return XY;
        }
        //отрисовка карты
        void ScreenUpdata(int PlayerX, int PlayerY, int[,,] Map, int[,,] Screen, bool Force)
        {

            string Tile = "   ";
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 20; x++)
                {
                    int lx = xy(x + PlayerX - 10);
                    int ly = xy(y + PlayerY - 10);
                    if (Force || Screen[x, y, 0] != Map[lx, ly, 0] || Screen[x, y, 1] != Map[lx, ly, 1])
                    {
                        Screen[x, y, 0] = Map[lx, ly, 0];
                        Screen[x, y, 1] = Map[lx, ly, 1];
                        Console.SetCursorPosition(x * 3, y);
                        switch (Map[lx, ly, 0])
                        {
                            case 0: Console.BackgroundColor = ConsoleColor.DarkBlue; break;
                            case 1: Console.BackgroundColor = ConsoleColor.Green; break;
                            case 2: Console.BackgroundColor = ConsoleColor.DarkGray; break;

                        }
                        switch (Map[lx, ly, 1])
                        {
                            case 0: Tile = "   "; break;
                            case 1:
                                Tile = " w ";
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                break;
                            case 2:
                                Tile = "\\|/";
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                break;
                            case 10:
                                Tile = "***";
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case 20:
                                Tile = "[O]";
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.BackgroundColor = ConsoleColor.Red;
                                break;
                        }

                        if (PlayerX == lx && PlayerY == ly)
                        {
                            Tile = "]+[";
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        /*if (PlayerNextX == lx && PlayerNextY == ly && MoveState!=0)
                        {
                            str = " + ";
                            Console.ForegroundColor = ConsoleColor.Red;
                        }*/
                        //Console.SetCursorPosition(x * 3, y);
                        Console.Write(Tile);

                    }

                }
            }
            Console.ResetColor();
            Console.SetCursorPosition(0, 21);
        }
        void PrintInfoPanel(int PlayerX, int PlayerY, int TargetX, int TargetY, int PlayerSatiety)
        {
            int x = 65;
            int y = 1;
            string InfoStr = "";
            Console.SetCursorPosition(x, y);
            InfoStr = $"Позиция по X, Y: {PlayerX}, {PlayerY}            ";
            Console.WriteLine(InfoStr);
            Console.SetCursorPosition(x, y + 2);
            InfoStr = $"Цель X, Y: {TargetX}, {TargetY}            ";
            Console.WriteLine(InfoStr);
            Console.SetCursorPosition(x, y + 4);
            double Distance1 = Math.Round(Math.Sqrt(Math.Pow(TargetY - PlayerY + 256, 2) + Math.Pow(TargetX - PlayerX + 256, 2)), 2);
            double Distance2 = Math.Round(Math.Sqrt(Math.Pow(TargetY - PlayerY, 2) + Math.Pow(TargetX - PlayerX, 2)), 2);
            Console.WriteLine($"Расстояние до цели {Math.Min(Distance1, Distance2)}                     ");
            Console.SetCursorPosition(x, y + 6);
            InfoStr = $"Сытость: {PlayerSatiety}            ";
            Console.WriteLine(InfoStr);

        }
        int GetMoveDiff(int x, int y, int[,,] map)
        {
            // определяем очки движения по типу местностиж
            int ret = 0, gnd = map[x, y, 0], lnd = map[x, y, 1];
            switch (gnd)
            {
                case 0: ret = 3; break;
                case 1: ret = 1; break;
                case 2: ret = -1; break;
            }
            switch (lnd)
            {
                case 1: ret = 2; break;
                case 2: ret = 3; break;

            }
            return ret;
        }
        double AvNoise(int x, int y, double[,] Arr)
        {
            x = xy(x); y = xy(y);
            return ((Arr[x, y] * 0.3) + (Arr[(x / 2) * 2, (y / 2) * 2] * 1) + (Arr[(x / 4) * 4, (y / 4) * 4] * 1) + Arr[(x / 8) * 8, (y / 8) * 8] + Arr[(x / 16) * 16, (y / 16) * 16]) / 5;
        }
        void EndGame(bool Win)
        {
            string Msg = "";
            Console.ResetColor();
            Console.Clear();
            bool[,] Screen = new bool[20, 20];
            if (Win)
            {
                Msg = "Вы победили!";
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Msg = "\tВы проиграли!!! \r\n Ваш персонаж умер от голорда";
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.SetCursorPosition(5, 10);
            Console.Write(Msg);
            Console.SetCursorPosition(0, 21);
            Console.ResetColor();
            Console.WriteLine("");

        }








        Console.Clear();

        //string str = " ";
        double[,] PerlinNoise = new double[256, 256];
        int[,,] Map = new int[256, 256, 2];// карта в два слоя.
        int[,,] Screen = new int[20, 20, 2];// карта в поле экрана в два слоя.
        //int[,] Land = new int[256, 256];
        double[,] WhiteNoise = new double[256, 256];
        int PlayerX = 0;
        int PlayerY = 0;
        int PlayerSatiety = 200;
        int TargetX = 0;
        int TargetY = 0;
        // генерация карты
        Random Rnd = new Random();
        for (int y = 0; y < 256; y++)
            for (int x = 0; x < 256; x++)
            {
                WhiteNoise[x, y] = Rnd.NextDouble();
            }
        for (int y = 0; y < 256; y++)
            for (int x = 0; x < 256; x++)
            {
                double[,] arrtmp = new double[3, 3];
                for (int _y = 0; _y < 3; _y++)
                    for (int _x = 0; _x < 3; _x++)
                    {
                        arrtmp[_x, _y] = AvNoise(_x + xy(x - 1), _y + xy(y - 1), WhiteNoise);

                    }
                PerlinNoise[x, y] = ((arrtmp[1, 1] + (arrtmp[1, 0] * 0.1) + (arrtmp[0, 1] * 0.1) + (arrtmp[1, 2] * 0.1) + (arrtmp[2, 1] * 0.1) + (arrtmp[0, 0] * 0.147) + (arrtmp[0, 2] * 0.147) + (arrtmp[2, 2] * 0.147) + (arrtmp[2, 0] * 0.147)) / 2) * 1.1;
                // ЗАполнение игровой карты.
                if (PerlinNoise[x, y] < 0.4)
                    Map[x, y, 0] = 0; //вода
                else if (PerlinNoise[x, y] >= 0.4 && PerlinNoise[x, y] <= 0.72)
                    Map[x, y, 0] = 1; // равнина
                else if (PerlinNoise[x, y] > 0.72)
                    Map[x, y, 0] = 2; //камень

                if (PerlinNoise[x, y] > 0.55 && PerlinNoise[x, y] < 0.6)
                    Map[x, y, 1] = 1; //лесистость
                else if (PerlinNoise[x, y] >= 0.6 && PerlinNoise[x, y] < 0.69)
                    Map[x, y, 1] = 2; //лес
            }
        // размещение целей
        int Count = 0;
        while (Count < 256)
        {
            int ex = Rnd.Next(256);
            int ey = Rnd.Next(256);
            if (Map[ex, ey, 0] == 1 && Map[ex, ey, 1] == 0)
            {
                Map[ex, ey, 1] = 10;
                Count++;
            }
        }
        Count = 0;
        while (Count == 0)
        {
            int ex = Rnd.Next(256);
            int ey = Rnd.Next(256);
            if (Map[ex, ey, 0] == 1 && Map[ex, ey, 1] == 0)
            {
                Map[ex, ey, 1] = 20;
                TargetX = ex;
                TargetY = ey;
                Count++;
            }
        }
        Count = 0;
        while (Count == 0)
        {
            int ex = Rnd.Next(256);
            int ey = Rnd.Next(256);
            if (Map[ex, ey, 0] == 1 && Map[ex, ey, 1] == 0)
            {
                PlayerX = ex;
                PlayerY = ey;
                Count++;
            }
        }


        /*
                // ТЕСТОВАЯ ОТРИСОВКА

                void testprint()
                {


                    for (int y = -10; y < 10; y++)
                    {
                        for (int x = -10; x < 10; x++)
                        {
                            switch (Map[xy(x), xy(y), 1])
                            {
                                case 0: str = "   "; break;
                                case 1: str = "ш_ш"; break;
                                case 2: str = "Ш_Ш"; break;
                                case 10: str = "***"; break;
                                case 20: str = "[O]"; break;
                            }
                            str = $"{Convert.ToInt32((Math.Round(PerlinNoise[xy(x), xy(y)], 2)) * 100)}";


                            if (PerlinNoise[xy(x), xy(y)] < 0.4)
                                Console.BackgroundColor = ConsoleColor.DarkBlue;//вода
                            else if (PerlinNoise[xy(x), xy(y)] >= 0.4 && PerlinNoise[xy(x), xy(y)] <= 0.72)
                                Console.BackgroundColor = ConsoleColor.DarkGreen;// равнина
                            else if (PerlinNoise[xy(x), xy(y)] > 0.72)
                                Console.BackgroundColor = ConsoleColor.DarkGray;//камень
                            Console.SetCursorPosition((x + 10) * 3, (y) + 10);
                            Console.Write(str);
                            // сбрасываем в стандартный
                        }
                        Console.ResetColor();
                        Console.WriteLine();
                    }


                }
                void test()
                {
                    int _x = 2;
                    int _y = 2;
                    int x = 255;
                    int y = 255;
                    double[,] arrtmp = new double[3, 3];
                    arrtmp[_x, _y] = AvNoise(_x + xy(x - 1), _y + xy(y - 1), WhiteNoise);
                    Console.WriteLine(arrtmp[_x, _y]);
                    Console.WriteLine(AvNoise(_x + xy(x - 1), _y + xy(y - 1), WhiteNoise));
                    Console.WriteLine(WhiteNoise[x, y]);
                    for (int n_y = 0; n_y < 3; n_y++)
                    {
                        for (int n_x = 0; n_x < 3; n_x++)
                        {
                            arrtmp[n_x, n_y] = AvNoise(n_x + xy(x - 1), n_y + xy(y - 1), WhiteNoise);

                        }
                        Console.WriteLine($" {arrtmp[0, n_y]} {arrtmp[1, n_y]} {arrtmp[2, n_y]}");
                    }

                }
                //test();
                //testprint();
                */

        int TickCount = 0;
        bool t = true;
        int MoveCount = 0; //назначенные очки  движения. 
        int MoveState = 0; //
        int PlayerNextX = 0;
        int PlayerNextY = 0;
        
        ConsoleKeyInfo KeyInfo;

        while (t)
        {
            Thread.Sleep(10); //TPS
            TickCount++;




            if (TickCount % 20 == 0) // вычисление движения
            {
                if (MoveCount == -1)
                {
                    MoveState = 0;
                    MoveCount = 0;
                }
                if (MoveState != 0)
                {
                    MoveCount--;
                    PlayerSatiety--;
                    if (MoveCount == 0)
                    {
                        MoveState = 0;
                        MoveCount = 0;
                        PlayerX = PlayerNextX;
                        PlayerY = PlayerNextY;
                    }
                }
            }

            // отрисовка карты

            if (TickCount == 1){
                PrintInfoPanel(PlayerX, PlayerY, TargetX, TargetY, PlayerSatiety);
                ScreenUpdata(PlayerX, PlayerY, Map, Screen, true);
            }

            ScreenUpdata(PlayerX, PlayerY, Map, Screen, false);

            // Обработка игровых событий
            if (Map[PlayerNextX, PlayerNextY, 1] == 10)
            {
                PlayerSatiety += 20;
                Map[PlayerNextX, PlayerNextY, 1] = 0;
            }
            if (Map[PlayerNextX, PlayerNextY, 1] == 20)
            {
                EndGame(true);
                t = false;
            }
            if (PlayerSatiety == 0)
            {
                EndGame(false);
                t = false;
            }

            if (Console.KeyAvailable) // Обработка нажатия клавиш
            {
                KeyInfo = Console.ReadKey(true);

                if (MoveState == 0)
                {
                    if (KeyInfo.Key == ConsoleKey.DownArrow)
                    {
                        MoveState = 1;
                        PlayerNextX = xy(PlayerX);
                        PlayerNextY = xy(PlayerY + 1);
                        MoveCount = GetMoveDiff(PlayerNextX, PlayerNextY, Map);
                    }
                    if (KeyInfo.Key == ConsoleKey.RightArrow)
                    {
                        MoveState = 2;
                        PlayerNextX = xy(PlayerX + 1);
                        PlayerNextY = xy(PlayerY);
                        MoveCount = GetMoveDiff(PlayerNextX, PlayerNextY, Map);
                    }
                    if (KeyInfo.Key == ConsoleKey.UpArrow)
                    {
                        MoveState = 3;
                        PlayerNextX = xy(PlayerX);
                        PlayerNextY = xy(PlayerY - 1);
                        MoveCount = GetMoveDiff(PlayerNextX, PlayerNextY, Map);
                    }

                    if (KeyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        MoveState = 4;
                        PlayerNextX = xy(PlayerX - 1);
                        PlayerNextY = xy(PlayerY);
                        MoveCount = GetMoveDiff(PlayerNextX, PlayerNextY, Map);
                    }

                    PrintInfoPanel(PlayerX, PlayerY, TargetX, TargetY, PlayerSatiety);
                }

            }

            //===================
            //EndGame(true);
            //t = false;
            //====================
        }// while
    }//end
}
