using System;
using System.Collections.Generic;

namespace ChessAI
{
    [Serializable]
    public class ChessBoard
    {
        public const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        private string _currentFen;
        private Dictionary<int, char> _boardDic = new Dictionary<int, char>();
        private int _whiteCost = 0;
        private int _blackCost = 0;
        private bool _isGameOver = false;
        private Stack<List<string>> _undoMoveStack = new Stack<List<string>>();
        private HashSet<int> _pawnNotMoved = new HashSet<int>();


        public ChessBoard()
        {
            _currentFen = StartFen;
            UpdateDicFromFen(_currentFen);
            UpdateCost();
            for (int i = 8; i < 16; i++)
            {
                _pawnNotMoved.Add(i);
            }

            for (int i = 48; i < 56; i++)
            {
                _pawnNotMoved.Add(i);
            }
        }

        public ChessBoard ReturnCopy()
        {
            ChessBoard cb = new ChessBoard();
            HashSet<int> pnm = new HashSet<int>();
            Stack<List<string>> ums = new Stack<List<string>>();
            var arr = _undoMoveStack.ToArray();

            foreach (var pos in _pawnNotMoved)
            {
                pnm.Add(pos);
            }

            for (var i = arr.Length - 1; i >= 0; --i)
            {
                ums.Push(arr[i]);
            }

            cb.SetCurrentFen(_currentFen);
            cb.SetPawnNotMoved(pnm);
            cb.SetUndoMovesStack(ums);
            cb.SetIsGameOver(_isGameOver);

            return cb;
        }

        private List<int> GetPawnLegalMoves(int pos, bool isInverted = false)
        {
            List<int> legalSquares = new List<int>();
            HashSet<int> ms = new HashSet<int>();

            int m1;
            int m2;
            int m3;
            int m4;

            if (IsBlackPiece(pos))
            {
                if (isInverted)
                {
                    m1 = pos + 7;
                    m2 = pos + 8;
                    m3 = pos + 9;
                    m4 = pos + 16;
                }
                else
                {
                    m1 = pos - 9;
                    m2 = pos - 8;
                    m3 = pos - 7;
                    m4 = pos - 16;
                }
            }
            else
            {
                if (!isInverted)
                {
                    m1 = pos + 7;
                    m2 = pos + 8;
                    m3 = pos + 9;
                    m4 = pos + 16;
                }
                else
                {
                    m1 = pos - 9;
                    m2 = pos - 8;
                    m3 = pos - 7;
                    m4 = pos - 16;
                }
            }


            ms.Add(m1);
            ms.Add(m2);
            ms.Add(m3);
            ms.Add(m4);

            if (pos % 8 == 0)
            {
                ms.Remove(m1);
            }
            else if ((m1 > 63 || m1 < 0) || IsEmptySquare(m1))
            {
                ms.Remove(m1);
            }
            else
            {
                if ((IsBlackPiece(pos) && IsBlackPiece(m1)) || IsWhitePiece(pos) && IsWhitePiece(m1))
                {
                    ms.Remove(m1);
                }
            }

            if ((m2 > 63 || m2 < 0) || !IsEmptySquare(m2))
            {
                ms.Remove(m2);
            }

            if ((pos + 1) % 8 == 0)
            {
                ms.Remove(m3);
            }
            else if ((m3 > 63 || m3 < 0) || IsEmptySquare(m3))
            {
                ms.Remove(m3);
            }
            else
            {
                if ((IsBlackPiece(pos) && IsBlackPiece(m3)) || IsWhitePiece(pos) && IsWhitePiece(m3))
                {
                    ms.Remove(m3);
                }
            }

            // if ((m4 > 63 || m4 < 0) || !_pawnNotMoved.Contains(pos) || !IsEmptySquare(m4) || !IsEmptySquare(m4 - 8))
            // {
            //     ms.Remove(m4);
            // }

            if (!_pawnNotMoved.Contains(pos) || (m4 > 63 || m4 < 0))
            {
                ms.Remove(m4);
            }
            else
            {
                if (pos > m4)
                {
                    if (!(IsEmptySquare(m4 + 8) && IsEmptySquare(m4)))
                    {
                        ms.Remove(m4);
                    }
                }
                else
                {
                    if (!(IsEmptySquare(m4 - 8) && IsEmptySquare(m4)))
                    {
                        ms.Remove(m4);
                    }
                }

            }

            foreach (var val in ms)
            {
                legalSquares.Add(val);
            }

            return legalSquares;
        }

        private List<int> GetBishopLegalMoves(int pos, bool isInverted = false)
        {
            List<int> legalSquares = new List<int>();
            int temp = pos;

            while (true)
            {
                if (temp >= 56 || (temp + 1) % 8 == 0)
                {
                    break;
                }

                temp += 9;

                if (IsEmptySquare(temp))
                {
                    legalSquares.Add(temp);
                }
                else if ((IsBlackPiece(pos) && IsWhitePiece(temp)) || (IsBlackPiece(temp) && IsWhitePiece(pos)))
                {
                    legalSquares.Add(temp);
                    break;
                }
                else
                {
                    break;
                }
            }

            temp = pos;

            while (true)
            {
                if (temp >= 56 || temp % 8 == 0)
                {
                    break;
                }

                temp += 7;
                if (IsEmptySquare(temp))
                {
                    legalSquares.Add(temp);
                }
                else if ((IsBlackPiece(pos) && IsWhitePiece(temp)) || (IsBlackPiece(temp) && IsWhitePiece(pos)))
                {
                    legalSquares.Add(temp);
                    break;
                }
                else
                {
                    break;
                }
            }

            temp = pos;

            while (true)
            {
                if (temp <= 7 || temp % 8 == 0)
                {
                    break;
                }

                temp -= 9;
                if (IsEmptySquare(temp))
                {
                    legalSquares.Add(temp);
                }
                else if ((IsBlackPiece(pos) && IsWhitePiece(temp)) || (IsBlackPiece(temp) && IsWhitePiece(pos)))
                {
                    legalSquares.Add(temp);
                    break;
                }
                else
                {
                    break;
                }
            }

            temp = pos;

            while (true)
            {
                if (temp <= 7 || (temp + 1) % 8 == 0)
                {
                    break;
                }

                temp -= 7;
                if (IsEmptySquare(temp))
                {
                    legalSquares.Add(temp);
                }
                else if ((IsBlackPiece(pos) && IsWhitePiece(temp)) || (IsBlackPiece(temp) && IsWhitePiece(pos)))
                {
                    legalSquares.Add(temp);
                    break;
                }
                else
                {
                    break;
                }
            }

            return legalSquares;
        }

        private List<int> GetRookLegalMoves(int pos, bool isInverted = false)
        {
            List<int> legalSquares = new List<int>();

            int leftLimit = pos - (pos % 8);
            int rightLimit = leftLimit + 7;
            int downLimit = pos % 8;
            int upLimit = 8 * 7 + downLimit;


            for (int i = pos - 1; i >= leftLimit; --i)
            {
                if (IsEmptySquare(i))
                {
                    legalSquares.Add(i);
                }
                else if ((IsBlackPiece(pos) && IsWhitePiece(i)) || (IsBlackPiece(i) && IsWhitePiece(pos)))
                {
                    legalSquares.Add(i);
                    break;
                }
                else
                {
                    break;
                }
            }

            for (int i = pos + 1; i <= rightLimit; ++i)
            {
                if (IsEmptySquare(i))
                {
                    legalSquares.Add(i);
                }
                else if ((IsBlackPiece(pos) && IsWhitePiece(i)) || (IsBlackPiece(i) && IsWhitePiece(pos)))
                {
                    legalSquares.Add(i);
                    break;
                }
                else
                {
                    break;
                }
            }

            for (int i = pos - 8; i >= downLimit; i -= 8)
            {
                if (IsEmptySquare(i))
                {
                    legalSquares.Add(i);
                }
                else if ((IsBlackPiece(pos) && IsWhitePiece(i)) || (IsBlackPiece(i) && IsWhitePiece(pos)))
                {
                    legalSquares.Add(i);
                    break;
                }
                else
                {
                    break;
                }
            }

            for (int i = pos + 8; i <= upLimit; i += 8)
            {
                if (IsEmptySquare(i))
                {
                    legalSquares.Add(i);
                }
                else if ((IsBlackPiece(pos) && IsWhitePiece(i)) || (IsBlackPiece(i) && IsWhitePiece(pos)))
                {
                    legalSquares.Add(i);
                    break;
                }
                else
                {
                    break;
                }
            }

            return legalSquares;
        }

        private List<int> GetKnightLegalMoves(int pos, bool isInverted = false)
        {
            List<int> legalSquares = new List<int>();
            int[] mv = { -17, 17, -15, 15, -10, 10, -6, 6 };

            HashSet<int> ms = new HashSet<int>();
            foreach (var val in mv)
            {
                ms.Add(pos + val);
            }

            if (pos % 8 == 0)
            {
                ms.Remove(pos + 15);
                ms.Remove(pos + 6);
                ms.Remove(pos - 10);
                ms.Remove(pos - 17);
            }

            if ((pos + 1) % 8 == 0)
            {
                ms.Remove(pos + 17);
                ms.Remove(pos + 10);
                ms.Remove(pos - 6);
                ms.Remove(pos - 15);
            }

            if (pos < 8)
            {
                ms.Remove(pos - 10);
                ms.Remove(pos - 17);
                ms.Remove(pos - 15);
                ms.Remove(pos - 6);
            }

            if (pos > 55)
            {
                ms.Remove(pos + 6);
                ms.Remove(pos + 15);
                ms.Remove(pos + 17);
                ms.Remove(pos + 10);
            }

            if (pos > 47 && pos < 56)
            {
                ms.Remove(pos + 15);
                ms.Remove(pos + 17);
            }

            if ((pos - 1) % 8 == 0)
            {
                ms.Remove(pos + 6);
                ms.Remove(pos - 10);
            }

            if (pos > 7 && pos < 16)
            {
                ms.Remove(pos - 17);
                ms.Remove(pos - 15);
            }

            if ((pos + 2) % 8 == 0)
            {
                ms.Remove(pos + 10);
                ms.Remove(pos - 6);
            }

            foreach (var val in ms)
            {
                if (IsEmptySquare(val) || (IsBlackPiece(pos) && IsWhitePiece(val)) ||
                    (IsBlackPiece(val) && IsWhitePiece(pos)))
                {
                    legalSquares.Add(val);
                }
            }

            return legalSquares;
        }

        private List<int> GetQueenLegalMoves(int pos, bool isInverted = false)
        {
            List<int> legalSquares = GetBishopLegalMoves(pos, isInverted);

            foreach (var val in GetRookLegalMoves(pos, isInverted))
            {
                legalSquares.Add(val);
            }

            return legalSquares;
        }

        private List<int> GetKingLegalMoves(int pos, bool isInverted = false)
        {
            List<int> legalSquares = new List<int>();
            HashSet<int> temp = new HashSet<int>();
            int h1 = pos + 1;
            int h2 = pos - 1;
            int v1 = pos + 8;
            int v2 = pos - 8;
            int df1 = pos + 7;
            int df2 = pos - 7;
            int ds1 = pos + 9;
            int ds2 = pos - 9;

            temp.Add(h1);
            temp.Add(h2);
            temp.Add(v1);
            temp.Add(v2);
            temp.Add(df1);
            temp.Add(df2);
            temp.Add(ds1);
            temp.Add(ds2);

            if (pos % 8 == 0)
            {
                temp.Remove(h2);
                temp.Remove(df1);
                temp.Remove(ds2);
            }

            if ((pos + 1) % 8 == 0)
            {
                temp.Remove(h1);
                temp.Remove(df2);
                temp.Remove(ds1);
            }

            if (pos < 8)
            {
                temp.Remove(v2);
                temp.Remove(df2);
                temp.Remove(ds2);
            }

            if (pos > 55)
            {
                temp.Remove(v1);
                temp.Remove(df1);
                temp.Remove(ds1);
            }

            foreach (var val in temp)
            {
                if (IsEmptySquare(val) || (IsBlackPiece(pos) && IsWhitePiece(val)) ||
                    (IsBlackPiece(val) && IsWhitePiece(pos)))
                {
                    legalSquares.Add(val);
                }
            }

            return legalSquares;
        }

        public List<int> GetLegalMoves(int pos, bool isInverted = false)
        {
            char piece = _boardDic[pos];
            if (ChessPieces.IsPawn(piece))
            {
                return GetPawnLegalMoves(pos, isInverted);
            }
            else if (ChessPieces.IsKnight(piece))
            {
                return GetKnightLegalMoves(pos, isInverted);
            }
            else if (ChessPieces.IsBishop(piece))
            {
                return GetBishopLegalMoves(pos, isInverted);
            }
            else if (ChessPieces.IsQueen(piece))
            {
                return GetQueenLegalMoves(pos, isInverted);
            }
            else if (ChessPieces.IsRook(piece))
            {
                return GetRookLegalMoves(pos, isInverted);
            }
            else
            {
                return GetKingLegalMoves(pos, isInverted);
            }
        }

        public Dictionary<int, List<int>> GetLegalMovesBlack(bool isInverted = false)
        {
            Dictionary<int, List<int>> d = new Dictionary<int, List<int>>();
            foreach (var pair in _boardDic)
            {
                if (IsBlackPiece(pair.Key))
                {
                    var lms = GetLegalMoves(pair.Key, isInverted);
                    if (lms.Count > 0)
                    {
                        d.Add(pair.Key, lms);
                    }
                }
            }

            return d;
        }

        public Dictionary<int, List<int>> GetLegalMovesWhite(bool isInverted = false)
        {
            Dictionary<int, List<int>> d = new Dictionary<int, List<int>>();
            foreach (var pair in _boardDic)
            {
                if (IsWhitePiece(pair.Key))
                {
                    var lms = GetLegalMoves(pair.Key, isInverted);
                    if (lms.Count > 0)
                    {
                        d.Add(pair.Key, lms);
                    }
                }
            }

            return d;
        }

        public bool PlayerMove(int curr, int des, bool isInverted = false)
        {
            var lms = GetLegalMoves(curr, isInverted);
            bool isLegal = false;
            foreach (var v in lms)
            {
                if (v == des)
                {
                    isLegal = true;
                    List<string> undoMove = new List<string>();
                    undoMove.Add(curr.ToString());
                    undoMove.Add(_boardDic[curr].ToString());
                    undoMove.Add(des.ToString());
                    undoMove.Add(_boardDic[des].ToString());

                    _undoMoveStack.Push(undoMove);

                    if (ChessPieces.IsPawn(_boardDic[curr]))
                    {
                        _pawnNotMoved.Remove(curr);
                    }

                    if (ChessPieces.IsKing(_boardDic[des]))
                    {
                        _isGameOver = true;
                    }

                    if (ChessPieces.IsPawn(_boardDic[curr]))
                    {
                        if (des < 8 || des > 55)
                        {
                            _boardDic[curr] = IsBlackPiece(curr) ? 'q' : 'Q';
                        }
                    }
                    _boardDic[des] = _boardDic[curr];
                    _boardDic[curr] = ' ';
                    UpdateFenFromDic(_boardDic);
                    UpdateCost();
                }
            }

            return isLegal;
        }

        // public int[] PlayerMoveTest(int curr, int des)
        // {
        //     Dictionary<int, char> d = new Dictionary<int, char>();
        //     int black = 0;
        //     int white = 0;
        //     foreach (var piece in _boardDic)
        //     {
        //         if (IsWhitePiece(piece.Key))
        //         {
        //             white += ChessPieces.GetCost(piece.Value);
        //         }
        //         else
        //         {
        //             black += ChessPieces.GetCost(piece.Value);
        //         }
        //     }
        //     int[] a = new[] {white, black};
        //     
        //     return a;
        // }
        public void PlayerMoveUndo()
        {
            if (_undoMoveStack.Count > 0)
            {
                var undoMove = _undoMoveStack.Pop();
                _boardDic[int.Parse(undoMove[0])] = char.Parse(undoMove[1]);
                _boardDic[int.Parse(undoMove[2])] = char.Parse(undoMove[3]);
                UpdateFenFromDic(_boardDic);
            }
        }

        public void UpdateDicFromFen(string fen)
        {
            Dictionary<int, char> dic = new Dictionary<int, char>();
            int inisq = 56;
            int temp = 0;
            foreach (var p in fen.Split(' ')[0])
            {
                if (Char.IsLetter(p))
                {
                    dic.Add(inisq + temp, p);
                    temp++;
                }
                else if (Char.IsDigit(p))
                {
                    for (int i = 0; i < Int32.Parse(p.ToString()); i++)
                    {
                        dic.Add(inisq + temp, ' ');
                        temp++;
                    }
                }

                if (temp == 8)
                {
                    inisq -= temp;
                    temp = 0;
                }
            }

            _boardDic = dic;
            // foreach (var item in boardDic)
            // {
            //     Console.WriteLine(item);
            // }
        }

        public void UpdateFenFromDic(Dictionary<int, char> boardDic)
        {
            int inisq = 56;
            int inc = 0;
            int temp = 0;
            string fen = "";

            while (true)
            {
                if (boardDic[inisq + temp] == ' ')
                {
                    temp += 1;
                    inc += 1;
                }
                else
                {
                    if (inc != 0)
                    {
                        fen += inc.ToString() + boardDic[inisq + temp].ToString();
                    }
                    else
                    {
                        fen += boardDic[inisq + temp].ToString();
                    }

                    inc = 0;
                    temp += 1;
                }

                if (temp == 8)
                {
                    if (inc != 0)
                    {
                        fen += inc.ToString() + "/";
                    }
                    else
                    {
                        fen += "/";
                    }

                    inisq -= 8;
                    temp = 0;
                    inc = 0;
                }

                if (inisq < 0)
                {
                    fen = fen.Substring(0, fen.Length - 1);
                    break;
                }
            }

            fen += " w - - 0 0";
            _currentFen = fen;
        }

        public void UpdateCost()
        {
            foreach (var piece in GetCurrentBoard())
            {
                if (Char.IsLower(piece))
                {
                    _blackCost += ChessPieces.GetCost(piece);
                }
                else
                {
                    _whiteCost += ChessPieces.GetCost(piece);
                }
            }
        }

        public int GetWhiteCost()
        {
            return _whiteCost;
        }

        public int GetBlackCost()
        {
            return _blackCost;
        }

        public string GetCurrentFen()
        {
            return _currentFen;
        }

        public void DisplayCurrentBoard()
        {
            Console.Write("|");
            foreach (var p in GetCurrentBoard())
            {
                if (p == '/')
                {
                    Console.Write("\n|");
                }
                else if (Char.IsDigit(p))
                {
                    for (int i = 0; i < Int32.Parse(p.ToString()); i++)
                    {
                        Console.Write(" |");
                    }
                }
                else
                {
                    Console.Write(p + "|");
                }
            }

            Console.WriteLine("\n");
        }

        public void DisplayCurrentBoardDebug(int pos, bool isInverted = false)
        {
            var lms = GetLegalMoves(pos, isInverted);
            int inisq = 56;
            int temp = 0;
            bool flag = false;
            Console.Write("|");
            int flag2 = 0;
            while (true)
            {
                var idx = inisq + temp;
                var ch = _boardDic[idx];
                foreach (var i in lms)
                {
                    if (i == idx)
                    {
                        flag = true;
                    }
                }

                if (flag && ch != ' ')
                {
                    Console.Write("T|");
                }
                else if (flag)
                {
                    Console.Write("X|");
                }
                else
                {
                    Console.Write(ch + "|");
                }

                flag = false;
                temp += 1;
                if (temp == 8)
                {
                    flag2 += 1;
                    inisq -= temp;
                    temp = 0;
                    if (flag2 == 8)
                    {
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("\n|");
                    }
                }

                if (inisq < 0)
                {
                    break;
                }
            }

            Console.WriteLine();
        }

        public string GetCurrentBoard()
        {
            return _currentFen.Split(' ')[0];
        }

        public string GetCurrentPlayerToPlay()
        {
            return _currentFen.Split(' ')[1];
        }

        public string GetCurrentCastlingState()
        {
            return _currentFen.Split(' ')[2];
        }

        public void SetCurrentFen(string fen)
        {
            _currentFen = fen;
            UpdateDicFromFen(_currentFen);
        }

        public bool IsBlackPiece(int pos)
        {
            return Char.IsLetter(_boardDic[pos]) && Char.IsLower(_boardDic[pos]);
        }

        public bool IsWhitePiece(int pos)
        {
            return Char.IsLetter(_boardDic[pos]) && Char.IsUpper(_boardDic[pos]); ;
        }

        public bool IsEmptySquare(int pos)
        {
            return _boardDic[pos] == ' ';
        }

        public void SetBoardDict(Dictionary<int, char> dict)
        {
            _boardDic = dict;
            UpdateFenFromDic(_boardDic);
        }

        public Dictionary<int, char> GetCurrentBoardDict()
        {
            return _boardDic;
        }

        public bool GetIsGameOver()
        {
            return _isGameOver;
        }

        public void SetIsGameOver(bool igo)
        {
            this._isGameOver = igo;
        }

        public void SetUndoMovesStack(Stack<List<string>> s)
        {
            this._undoMoveStack = s;
        }

        public Stack<List<string>> GetUndoMovesStack()
        {
            return this._undoMoveStack;
        }

        public void SetPawnNotMoved(HashSet<int> p)
        {
            this._pawnNotMoved = p;
        }

        public HashSet<int> GetPawnNotMoved()
        {
            return this._pawnNotMoved;
        }

        public bool IsCapture(int curr, int des)
        {
            bool b = true;
            if (IsEmptySquare(des))
            {
                b = false;
            }
            else
            {
                if ((IsBlackPiece(curr) && IsBlackPiece(des)) || IsWhitePiece(curr) && IsWhitePiece(des))
                {
                    b = false;
                }
            }
            return b;
        }
    }
}