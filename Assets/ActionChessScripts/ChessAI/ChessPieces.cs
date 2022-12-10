namespace ChessAI
{
    public static class ChessPieces
    {
        private const int None = 0;
        private const int Pawn = 1;
        private const int Knight = 3;
        private const int Bishop = 3;
        private const int Rook = 5;
        private const int Queen = 9;
        private const int King = 100;

        public static int GetCost(char piece)
        {
            if (piece == 'p' || piece == 'P')
            {
                return ChessPieces.Pawn;
            }
            else if (piece == 'n' || piece == 'N')
            {
                return ChessPieces.Knight;
            }
            else if (piece == 'b' || piece == 'B')
            {
                return ChessPieces.Bishop;
            }
            else if (piece == 'q' || piece == 'Q')
            {
                return ChessPieces.Queen;
            }
            else if (piece == 'k' || piece == 'K')
            {
                return ChessPieces.King;
            }
            else if (piece == 'r' || piece == 'R')
            {
                return ChessPieces.Rook;
            }
            else
            {
                return ChessPieces.None;
            }
        }

        public static bool IsPawn(char piece)
        {
            if (piece == 'p' || piece == 'P')
            {
                return true;
            }

            return false;
        }

        public static bool IsKnight(char piece)
        {
            if (piece == 'n' || piece == 'N')
            {
                return true;
            }

            return false;
        }

        public static bool IsBishop(char piece)
        {
            if (piece == 'b' || piece == 'B')
            {
                return true;
            }

            return false;
        }

        public static bool IsRook(char piece)
        {
            if (piece == 'r' || piece == 'R')
            {
                return true;
            }

            return false;
        }

        public static bool IsQueen(char piece)
        {
            if (piece == 'q' || piece == 'Q')
            {
                return true;
            }

            return false;
        }

        public static bool IsKing(char piece)
        {
            if (piece == 'k' || piece == 'K')
            {
                return true;
            }

            return false;
        }

    }
}