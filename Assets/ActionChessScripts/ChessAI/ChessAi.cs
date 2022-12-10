using System;
using System.Collections.Generic;

namespace ChessAI{

    public class ChessAi
    {
        private char _maximizingColor;
        // private ChessBoard _cb;
        //
        // public ChessAi(ChessBoard cb, char maximizingColor = 'w')
        // {
        //     this._cb = cb;
        //     this._maximizingColor = maximizingColor;
        // }

        public ChessAi(char maximizingColor = 'w')
        {
            this._maximizingColor = maximizingColor;
        }
        private int EvaluateHeuristic(ChessBoard cb)
        {
            if (_maximizingColor == 'w')
            {
                return cb.GetWhiteCost() - cb.GetBlackCost();
            }
            return cb.GetBlackCost() - cb.GetWhiteCost();
        }

        public int[] GetBlackNextMove(ChessBoard cb, int depth)
        {
            int[] bm = new int[3];
            return Minimax(cb, depth, int.MinValue, int.MaxValue, false,0);
        }
        
        public int[] GetWhiteNextMove(ChessBoard cb, int depth)
        {
            return Minimax(cb, depth, int.MinValue, int.MaxValue, true, 0);
        }
        public int[] Minimax(ChessBoard cb, int depth, int alpha, int beta, bool isMaximizing,int s)
        {
            if (depth == 0 || cb.GetIsGameOver())
            {
                int[] bm = new int[3];
                bm[2] = EvaluateHeuristic(cb);
                return bm;
            }
        
            var rand = new Random();
            
            if (isMaximizing)
            {
                var maxEval = int.MinValue;
                var pieces = _maximizingColor == 'w' ? cb.GetLegalMovesWhite() : cb.GetLegalMovesBlack();
                int[] bm = new int[3];
                HashSet<int> set = new HashSet<int>();
                List<int> lst = new List<int>();
                foreach (var val in pieces)
                {
                    lst.Add(val.Key);
                }
                
                foreach (var t in pieces)
                {
                    int index = rand.Next(lst.Count);
                    while (set.Contains(index))
                    {
                        index = rand.Next(lst.Count);
                    }
                    set.Add(index);
                    var curr = lst[index];
                    foreach (var des in pieces[curr])
                    {
                        var temp = cb.ReturnCopy();
                        temp.PlayerMove(curr, des);
                        // for (int i = 0; i < s; i++)
                        // {
                        //     Console.Write(" ");
                        // }
                        // Console.Write(piece.Key+"(w,"+temp.GetCurrentBoardDict()[des] + ")->" +des+"["+EvaluateHeuristic(temp)+"]\n");
                        var eval = Minimax(temp, depth - 1, alpha, beta, false, s+2)[2];
                        // maxEval = Math.Max(maxEval, eval);
                        if (eval > maxEval)
                        {
                            maxEval = eval;
                            bm[0] = curr;
                            bm[1] = des;
                            bm[2] = maxEval;
                        }
                        alpha = Math.Max(alpha, eval);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
                return bm;
            }
            else
            {
                var minEval = int.MaxValue;
                var pieces = _maximizingColor == 'w' ? cb.GetLegalMovesBlack() : cb.GetLegalMovesWhite();
                int[] bm = new int[3];
                HashSet<int> set = new HashSet<int>();
                List<int> lst = new List<int>();
                foreach (var val in pieces)
                {
                    lst.Add(val.Key);
                }
                
                foreach (var t in pieces)
                {
                    int index = rand.Next(lst.Count);
                    while (set.Contains(index))
                    {
                        index = rand.Next(lst.Count);
                    }
                    set.Add(index);
                    var curr = lst[index];
                    foreach (var des in pieces[curr])
                    {
                        var temp = cb.ReturnCopy();
                        temp.PlayerMove(curr, des);
                        // for (int i = 0; i < s; i++)
                        // {
                        //     Console.Write(" ");
                        // }
                        // Console.Write(piece.Key+"(b,"+temp.GetCurrentBoardDict()[des] + ")->" +des+"["+EvaluateHeuristic(temp)+"]\n");
                        var eval = Minimax(temp, depth - 1, alpha, beta, true,s+2)[2];
                        // minEval = Math.Min(minEval, eval);
                        if (eval < minEval)
                        {
                            minEval = eval;
                            bm[0] = curr;
                            bm[1] = des;
                            bm[2] = minEval;
                        }
                        beta = Math.Min(beta, eval);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
                return bm;
            }
            
        }
    }
}