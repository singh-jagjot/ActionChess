// using System;
// using System.Collections.Generic;

// namespace ChessAI
// {
//     class Driver
//     {
//         static void Main(string[] args)
//         {
//             // string fen = "8/5k2/3p4/1p1Pp2p/pP2Pp1P/P4P1K/8/8 b - - 99 50";
//             // string fen = "rnbqkb1r/pppp1ppp/8/8/4p3/2N5/PPPPPPPP/R1B1KnNR b - - 0 0";

//             // ChessBoard cb = new ChessBoard();
//             // cb.setCurrentFen(fen);
//             // cb.displayCurrentBoard();
//             // cb.updateCost();
//             // cb.displayCurrentBoardDebug(cb.getCurrentBoardDict(), 4);
//             // Console.WriteLine(cb.getWhiteCost() +" " + cb.getBlackCost());
//             // cb.updateDicFromFEN(ChessBoard.startFEN);
//             // int[] lst = {43,33,35,36,39,24,25,28,29,31,16,21};
//             // for(int i = 8; i<=15;++i){
//             // foreach (var i in lst)
//             // {
//             //     var l = cb.getPawnLegalMoves(i);
//             //     Console.Write(i+": ");
//             //     foreach (var item in l)
//             //     {
//             //         Console.Write(item+" ");                    
//             //     }
//             //     Console.WriteLine("S:"+l.Count);
//             // }
            
//             // Dictionary<int, char> d;
//             // for (int i = 53; i < 54; i++)
//             // {
//             //     d = new Dictionary<int, char>();
//             //     for (int j = 0; j < 64; j++)
//             //     {
//             //         if (i == j)
//             //         {
//             //             d.Add(i, 'p');
//             //         }
//             //         else
//             //         {
//             //             d.Add(j, ' ');
//             //         }
//             //     }
//             //
//             //     // cb.updateFenFromDic(d);
//             //     cb.setBoardDict(d);
//             //     Console.WriteLine(cb.getCurrentFEN());
//             //     // cb.displayCurrentBoard();
//             //     var l = cb.getLegalMoves(i, false);
//             //     cb.displayCurrentBoardDebug(d, i, false);
//             //     Console.Write("LegalMoves: ");
//             //     foreach (var item in l)
//             //     {
//             //         Console.Write(item + " ");
//             //     }
//             //
//             //     // foreach (var item in cb.getBoardDict())
//             //     // {
//             //     //     Console.Write(item);
//             //     // }
//             //     Console.WriteLine();
//             // }

//             // ChessBoard cb = new ChessBoard();
//             // Console.WriteLine(cb.GetWhiteCost() +" " + cb.GetBlackCost());
//             //
//             // cb.DisplayCurrentBoard();
//             // cb.DisplayCurrentBoardDebug(54);
//             // cb.PlayerMove(11, 19);
//             // cb.DisplayCurrentBoard();
//             // cb.PlayerMove(2, 11);
//             // cb.DisplayCurrentBoard();
//             // cb.PlayerMove(11,25);
//             // cb.DisplayCurrentBoard();
//             // cb.PlayerMoveUndo();
//             // cb.DisplayCurrentBoard();
//             // cb.PlayerMoveUndo();
//             // cb.DisplayCurrentBoard();
//             // cb.PlayerMoveUndo();
//             // cb.DisplayCurrentBoard();
//             // cb.DisplayCurrentBoardDebug(10);
//             // cb.DisplayCurrentBoardDebug(19);
//             // cb.DisplayCurrentBoardDebug(51);

//             ChessBoard cb = new ChessBoard();
//             ChessAi ai = new ChessAi();
//             cb.SetCurrentFen("rnbqkb1r/7p/4p3/p1p2p2/1p2n3/2N5/1P1P2p1/2B1K3 b - - 0 0");
//             // cb.DisplayCurrentBoard();
//             // cb.DisplayCurrentBoardDebug(56);
//             // foreach (var val in cb.GetLegalMovesWhite())
//             // {
//             //     Console.Write(val.Key + "["+cb.GetCurrentBoardDict()[val.Key]+"]:");
//             //     foreach (var pos in val.Value)
//             //     {
//             //         Console.Write(pos + ",");
//             //     }
//             //     Console.WriteLine();
//             // }

//             string inp = "";
//             var count = 0;
//             while (true)
//             {
//                 cb.DisplayCurrentBoard();
//                 inp = Console.ReadLine();
//                 if (inp == "Q" || cb.GetIsGameOver())
//                 {
//                     break;
//                 }
//                 Console.WriteLine("Truns: " + ++count);
//                 var m = inp.Split(" ");
//                 cb.PlayerMove(int.Parse(m[0]), int.Parse(m[1]));
                
//                 var cm = ai.GetBlackNextMove(cb, 5);
//                 cb.PlayerMove(cm[0], cm[1]);
//                 for (int i = 0; i < 3; i++)
//                 {
//                     Console.Write(cm[i] + " ");
//                 }
//                 Console.WriteLine();
//             }

//             // var rand = new Random();
//             // foreach (var p in cb.GetLegalMovesBlack())
//             // {
//             //     Console.Write(p.Key + " ");
//             //     Console.WriteLine("Five random integers between 0 and 100:");
//             //     for (int ctr = 0; ctr <= 4; ctr++)
//             //         Console.Write(rand.Next(101)+" ");
//             //     Console.WriteLine();
//             //
//             // }
            
//         }
//     }
// }