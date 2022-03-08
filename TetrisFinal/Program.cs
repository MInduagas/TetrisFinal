int xpos, ypos;
int lines;
bool cleared = false;
Start();

void Start()
{
    Console.ReadLine();
    //gameloop
    bool loop = true;
    while (loop == true)
    {
        Console.Clear();
        Random rand = new Random();
        int boardStart = 0;
        int xboard = (3 * 12) + 2;
        int yboard = (3 * 10) + 2;
        string[,] board = new string[xboard, yboard];
        int currPiece = rand.Next(1, 8);
        lines = 0;
        bool game = true;
        while (game == true)
        {
            int nextpiece = rand.Next(1, 8);
            string[,] piece = Pieces(currPiece);
            string[,] nextPiece = Pieces(nextpiece);
            piece = gamelogic(xboard, yboard, board, piece, boardStart, nextPiece, currPiece);
            board = SaveBoardStatus(xpos, ypos, piece, board, xboard, yboard, currPiece);
            board = clearLine(board, xboard, yboard);
            updateBoard(board, xboard, yboard);
            currPiece = nextpiece;
            game = checkEndgame(board, xboard, yboard);
        }
        Console.SetCursorPosition(52, 3);
        Console.Write("Game Over, Your score was = " + lines);
        Console.ReadLine();
        Console.SetCursorPosition(52, 4);
        Console.Write("Type E to exit or C to continue: ");
        string asde = Convert.ToString(Console.ReadLine().ToUpper());
        if (asde == "E")
            loop = false;
    }
}

string[,] gamelogic(int xboard, int yboard, string[,] board, string[,] piece, int boardStart, string[,] nextPiece, int piecenum)
{
    bool run = true;
    xpos = 1;
    ypos = 13 + boardStart;
    int timepassed = 1;
    while (run == true)
    {
        Console.CursorVisible = false;
        ConsoleKeyInfo cki = new ConsoleKeyInfo();
        do
        {
            GameBoard(xboard, yboard, boardStart);
            while (Console.KeyAvailable == false)
            {
                PrintPiece(xpos, ypos, piece, piecenum);
                PrintPiece(3, 38, nextPiece, 8);
                run = CheckCollision(xpos, ypos, xboard, yboard, board, piece, boardStart);
                if (run == false)
                    break;
                Thread.Sleep(50);
                clearPiece(xpos, ypos, piece);
                xpos = Fall(timepassed, xpos);
                timepassed++;
            }
            if (run == false)
                break;
            cki = Console.ReadKey(true);
            ypos = MoveSide(ypos, cki, yboard, piece, boardStart);
            xpos = MoveDown(xpos, ypos, cki, xboard, yboard, piece, board, boardStart);
            piece = Rotation(cki, piece);

        } while (cki.Key != ConsoleKey.RightArrow || cki.Key != ConsoleKey.LeftArrow || cki.Key != ConsoleKey.DownArrow || cki.Key != ConsoleKey.UpArrow || cki.Key != ConsoleKey.C);
        if (run == false)
            break;
    }
    clearPiece(3, 38, nextPiece);
    return piece;
}

string[,] printSquare()
{
    string[,] box = new string[2, 3];
    for (int i = 0; i < 2; i++)
        for (int j = 0; j < 3; j++)
        {
            if (i == 0 && j == 0)
                box[i, j] = "╔";
            if (i == 0 && j == 2)
                box[i, j] = "╗";
            if (i == 1 && j == 0)
                box[i, j] = "╚";
            if (i == 1 && j == 2)
                box[i, j] = "╝";
            if (i == 0 && j == 1 || i == 1 && j == 1)
                box[i, j] = "═";
            //═ ╝ ╚ ╗ ╔
        }
    return box;
}

int Fall(int timepassed, int xpos)
{
    if (timepassed % 10 == 0)
        xpos += 2;
    return xpos;
}

int MoveSide(int ypos, ConsoleKeyInfo cki, int yboard, string[,] piece, int boardstart)
{
    int[] pos = new int[4];
    int c = 0;
    for (int i = 0; i < 4; i++)
    {
        int len = 0;
        for (int j = 0; j < 4; j++)
        {
            if (piece[i, j] == "x")
            {
                pos[c] = len;
                c++;
            }
            len += 3;
        }
    }
    Array.Sort(pos);
    switch (cki.Key)
    {
        case ConsoleKey.RightArrow:
            if (ypos + pos[3] >= yboard - 6 + boardstart)
                return ypos;
            ypos += 3;
            return ypos;
        case ConsoleKey.LeftArrow:
            if (ypos + pos[0] <= boardstart + 2)
                return ypos;
            ypos -= 3;
            return ypos;
        default:
            break;
    }
    return ypos;
}

int MoveDown(int xpos, int ypos, ConsoleKeyInfo cki, int xboard, int yboard, string[,] piece, string[,] board, int gameboard)
{
    bool hit = CheckCollision(xpos, ypos, xboard, yboard, board, piece, gameboard);
    switch (cki.Key)
    {
        case ConsoleKey.DownArrow:
            if (hit == false)
                return xpos;
            xpos += 2;
            return xpos;
        default:
            break;
    }
    return xpos;
}

string[,] Rotation(ConsoleKeyInfo cki, string[,] piece)
{
    switch (cki.Key)
    {
        case ConsoleKey.UpArrow:
            string[,] RotatedPiece = new string[4, 4];
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    RotatedPiece[i, j] = piece[4 - j - 1, i];
                }
            }
            return RotatedPiece;
        default:
            break;
    }
    return piece;
}

bool CheckCollision(int xpos, int ypos, int xboard, int yboard, string[,] BoardStatus, string[,] piece, int gameboard)
{
    if (xpos >= xboard - 5)
        return false;
    string[,] fakeBoard = new string[xboard + gameboard + 10, yboard + gameboard + 10];
    int hei = 0;
    for (int i = 0; i < 4; i += 1)
    {
        int len = 0;
        for (int j = 0; j < 4; j += 1)
        {
            if (piece[i, j] == "x")
            {
                fakeBoard[xpos - 1 + hei, ypos - 1 + len] = "Q";
                Console.SetCursorPosition(0 + j, 51 + i);
                Console.Write(fakeBoard[i, j]);
                if (xpos + hei >= xboard - 3)
                {
                    return false;
                }
            }
            len += 3;
        }
        hei += 2;
    }

    for (int i = 0; i < xboard; i += 2)
        for (int j = gameboard; j < yboard + gameboard; j += 3)
            if (fakeBoard[i, j] == "Q" && BoardStatus[i + 2, j] == "o")
                return false;

    return true;
}

string[,] Pieces(int piece)
{
    string[,] arr = new string[4, 4];

    switch (piece)
    {
        case 1: // |__
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (i == 1 && j == 0)
                        arr[i, j] = "x";
                    else if (i == 2 && j != 3)
                        arr[i, j] = "x";
                    else arr[i, j] = "" +
                            "\0";
            break;
        case 2: // __|
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (i == 1 && j == 2)
                        arr[i, j] = "x";
                    else if (i == 2 && j != 3)
                        arr[i, j] = "x";
                    else arr[i, j] = "\0";
            break;
        case 3: // ____
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (i == 2)
                        arr[i, j] = "x";
                    else arr[i, j] = "\0";
            break;
        case 4: // _|_
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (i == 1 && j == 1)
                        arr[i, j] = "x";
                    else if (i == 2 && j != 3)
                        arr[i, j] = "x";
                    else arr[i, j] = "\0";
            break;
        case 5: // Z
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (i == 1 && j == 0 || i == 1 && j == 1)
                        arr[i, j] = "x";
                    else if (i == 2 && j == 1 || i == 2 && j == 2)
                        arr[i, j] = "x";
                    else arr[i, j] = "\0";
            break;
        case 6: // Other z
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (i == 1 && j == 2 || i == 1 && j == 1)
                        arr[i, j] = "x";
                    else if (i == 2 && j == 1 || i == 2 && j == 0)
                        arr[i, j] = "x";
                    else arr[i, j] = "\0";
            break;
        case 7: // square
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (i == 1 && j == 1 || i == 1 && j == 2)
                        arr[i, j] = "x";
                    else if (i == 2 && j == 1 || i == 2 && j == 2)
                        arr[i, j] = "x";
                    else arr[i, j] = "\0";
            break;
        default:
            break;
    }
    return arr;
}

void PrintPiece(int x, int y, string[,] Piece, int pieceNum)
{
    if (pieceNum == 0)
        Console.ForegroundColor = ConsoleColor.Gray;
    if (pieceNum == 1)
        Console.ForegroundColor = ConsoleColor.Blue;
    if (pieceNum == 2)
        Console.ForegroundColor = ConsoleColor.DarkYellow;
    if (pieceNum == 3)
        Console.ForegroundColor = ConsoleColor.Cyan;
    if (pieceNum == 4)
        Console.ForegroundColor = ConsoleColor.Magenta;
    if (pieceNum == 5)
        Console.ForegroundColor = ConsoleColor.Red;
    if (pieceNum == 6)
        Console.ForegroundColor = ConsoleColor.Green;
    if (pieceNum == 7)
        Console.ForegroundColor = ConsoleColor.Yellow;

    string[,] box = printSquare();
    int hei = 0;
    for (int i = 0; i < 4; i++)
    {
        int len = 0;
        for (int j = 0; j < 4; j++)
        {
            if (Piece[i, j] == "x")
                for (int k = 0; k < 2; k++)
                    for (int l = 0; l < 3; l++)
                    {
                        Console.SetCursorPosition(l + y + len, k + x + hei);
                        Console.Write(box[k, l]);
                    }
            len += 3;
        }
        hei += 2;
    }
    Console.ForegroundColor = ConsoleColor.Gray;
}

void clearPiece(int x, int y, string[,] Piece)
{
    int hei = 0;
    for (int i = 0; i < 4; i++)
    {
        int len = 0;
        for (int j = 0; j < 4; j++)
        {
            if (Piece[i, j] == "x")
                for (int k = 0; k < 2; k++)
                    for (int l = 0; l < 3; l++)
                    {
                        Console.SetCursorPosition(l + y + len, k + x + hei);
                        Console.Write(" ");
                    }
            len += 3;
        }
        hei += 2;
    }
}

string[,] SaveBoardStatus(int xpos, int ypos, string[,] Piece, string[,] Board, int xboard, int yboard, int piecenum)
{
    int hei = 0;
    for (int i = 0; i < 4; i++)
    {
        int len = 0;
        for (int j = 0; j < 4; j++)
        {
            if (Piece[i, j] == "x")
            {
                Board[xpos + hei - 1, ypos + len - 1] = "o";
                Board[xpos + hei - 1, ypos + len] = "" + piecenum;
            }
            len += 3;
        }
        hei += 2;
    }
    for (int i = 1; i < xboard - 2; i++)
        for (int j = 0; j < yboard - 2; j++)
        {
            if (i % 2 == 0 && j % 3 == 0 && Board[i, j] == null)
                Board[i, j] = "x";
        }
    return Board;
}

void GameBoard(int xboard, int yboard, int boardStart)
{
    string[,] Board = new string[xboard, yboard];
    for (int i = 0; i < xboard; i++)
        for (int j = 0; j < yboard; j++)
        {
            if (i == 0 && j == 0)
            {
                Board[i, j] = "╔";
                continue;
            }
            if (i == xboard - 1 && j == 0)
            {
                Board[i, j] = "╚";
                continue;
            }
            if (i == xboard - 1 && j == yboard - 1)
            {
                Board[i, j] = "╝";
                continue;
            }
            if (i == 0 && j == yboard - 1)
            {
                Board[i, j] = "╗";
                continue;
            }
            if (i == 0 || i == xboard - 1)
            {
                Board[i, j] = "═";
                continue;
            }
            if (j == 0 || j == yboard - 1)
            {
                Board[i, j] = "║";
                continue;
            }
        }
    for (int i = 0; i < xboard; i++)
        for (int j = 0; j < yboard; j++)
        {
            Console.SetCursorPosition(j + boardStart, i);
            Console.Write(Board[i, j]);
        }
}

void updateBoard(string[,] board, int xboard, int yboard)
{
    if (cleared == true)
        for (int i = 0; i < xboard; i++)
            for (int j = 0; j < yboard; j++)
            {
                Console.SetCursorPosition(j, i);
                Console.Write(" ");
            }
    string[,] box = printSquare();
    for (int i = 0; i < xboard; i++)
        for (int j = 0; j < yboard; j++)
            if (board[i, j] == "o")
            {
                int pieceNum = Convert.ToInt32(board[i, j + 1]);
                if (pieceNum == 0)
                    Console.ForegroundColor = ConsoleColor.Gray;
                if (pieceNum == 1)
                    Console.ForegroundColor = ConsoleColor.Blue;
                if (pieceNum == 2)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                if (pieceNum == 3)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                if (pieceNum == 4)
                    Console.ForegroundColor = ConsoleColor.Magenta;
                if (pieceNum == 5)
                    Console.ForegroundColor = ConsoleColor.Red;
                if (pieceNum == 6)
                    Console.ForegroundColor = ConsoleColor.Green;
                if (pieceNum == 7)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                for (int k = 0; k < 2; k++)
                    for (int l = 0; l < 3; l++)
                    {
                        Console.SetCursorPosition(j + l + 1, i + k + 1);
                        Console.Write(box[k, l]);
                        cleared = false;
                    }
                Console.ForegroundColor = ConsoleColor.Gray;
            }
}

string[,] clearLine(string[,] board, int xboard, int yboard)
{
    int[] lineC = new int[4];
    int cl = 0;
    for (int i = 2; i < xboard - 2; i += 2)
    {
        int pil = 0;
        for (int j = 0; j < yboard - 3; j += 3)
        {
            if (board[i, j] == "o")
                pil++;
            else
                pil = 0;

            if (pil == 10)
            {
                lineC[cl] = i;
                cl++;
            }
        }
    }
    Array.Sort(lineC);
    cl = 3;
    string[,] CopyBoard = new string[xboard, yboard];
    int ctr = 0;
    for (int i = xboard - 2; i > 2; i -= 2)
    {
        if (cl != -1 && lineC[cl] == i && lineC[cl] != 0)
        {
            ctr += 2;
            cl--;
            lines++;
            cleared = true;
            continue;
        }
        for (int j = 0; j < yboard; j++)
        {
            CopyBoard[i + ctr, j] = board[i, j];
        }
    }
    return CopyBoard;
}

bool checkEndgame(string[,] board, int xboard, int yboard)
{
    for (int i = 0; i < xboard; i += 2)
        for (int j = 0; j < yboard; j += 3)
            if (board[i, j] == "o" && i <= 4)
                return false;

    return true;
}