using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Mankalah
{
    /*
     * This class implements a new player named zs35, derived from the abstract class Player.
     */
    public class zs35Player : Player // class must be public
    {
        public zs35Player(Position pos, int maxTimePerMove) // constructor must match this signature
            : base(pos, "zs35Player", maxTimePerMove) // choose a string other than "MyPlayer"
        {
        }

        /*
         * This method calls the miniMax search to choose the next move.
         */
        public override int chooseMove(Board b)
        {
            moveResult bResult = new moveResult(0, 0);

            int d = 1;  // start with a depth of 1

            TimeSpan current_time = DateTime.Now.TimeOfDay;  // read the current system time and set up as the start time
            TimeSpan time_limit = TimeSpan.FromMilliseconds(getTimePerMove());  // get the time limit
            TimeSpan end_time = current_time.Add(time_limit);  // calculate the end time of the current move

            while (DateTime.Now.TimeOfDay < end_time)  // keep looping as long as the time value is less than the time limit
            {
                moveResult result = minimaxVal(b, d);  // do the minimax search for the current depth and get the result
                bResult = result;
                /*Console.WriteLine("current depth: " + d + " current move: " + bResult.getMove() + " current score: " + bResult.getResult());*/
                d++;  // increment the depth, keep looping until the time runs out
            }

            return bResult.getMove();  // return the best move
        }

        /*
         * This method implements a miniMax search to find the next move.
         */
        public moveResult minimaxVal(Board b, int d)  // d is the depth
        {
            int bestMove = 0;
            int bestVal = 0;

            if (b.gameOver() || d == 0)  // if the game is over or the depth reaches 0
            {
                return new moveResult(0, evaluate(b));
            }

            if (b.whoseMove() == Position.Top)  // if it's the top player's turn to move
            {
                bestVal = int.MinValue;
                for (int move = 7; move < 13; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b);  // duplicate board
                        b1.makeMove(move, false);  // make the move
                        int val = minimaxVal(b1, d - 1).getResult();  // find the result of this move
                        if (val > bestVal)  // if this is better than the current best value, store it as the best value
                        {
                            bestVal = val;
                            bestMove = move;
                        }
                    }
                }
            }

            if (b.whoseMove() == Position.Bottom)  // if it's the bottom player's turn to move
            {
                bestVal = int.MaxValue;
                for (int move = 0; move < 6; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b);  // duplicate board
                        b1.makeMove(move, false);  // make the move
                        int val = minimaxVal(b1, d - 1).getResult();  // find the result of this move
                        if (val < bestVal)  // if this is better than the current best value, store it as the best value
                        {
                            bestVal = val;
                            bestMove = move;
                        }
                    }
                }
            }

            return new moveResult(bestMove, bestVal);  // return the final result
        }

        /*
         * This method evaluates the performance this board.
         */
        public override int evaluate(Board b)
        {
            int stoneDif = b.stonesAt(13) - b.stonesAt(6);  // the difference of the stones both players get

            int remainDif = 0;  // the different of the remaining stones on both sides

            int go_again = 0;  // the potential go-again moves each side has

            for (int i = 0; i < 6; i++)  // the potential go-again moves of the bottom player
            {
                if (b.stonesAt(i) == (6 - i))  // if the stone in the cup == the distance between this cup and the end of the player's side
                {
                    go_again -= 1;  // this is a go-again move
                    remainDif -= b.stonesAt(i);  // this is a useful remaining stone, not likely to end in opponent's side
                }
                else if (b.stonesAt(i) < (6 - i))  // if the stone in the cup < the distance between this cup and the end of the player's side
                {
                    remainDif -= b.stonesAt(i);  // this is a useful remaining stone, not likely to end in opponent's side
                }
                else  // if the stone in the cup > the distance between this cup and the end of the player's side
                {
                    remainDif += b.stonesAt(i);  // this is not a useful remaining stone, likely to end in opponent's side
                }
            }

            for (int i = 7; i < 13; i++)  // the potential go-again moves of the top player
            {
                if (b.stonesAt(i) == (13 - i))  // if the stone in the cup == the distance between this cup and the end of the player's side
                {
                    go_again += 1;  // this is a go-again move
                    remainDif += b.stonesAt(i);  // this is a useful remaining stone, not likely to end in opponent's side
                }
                else if (b.stonesAt(i) < (13 - i))  // if the stone in the cup < the distance between this cup and the end of the player's side
                {
                    remainDif += b.stonesAt(i);  // this is a useful remaining stone, not likely to end in opponent's side
                }
                else  // if the stone in the cup < the distance between this cup and the end of the player's side
                {
                    remainDif -= b.stonesAt(i);  // this is not a useful remaining stone, likely to end in opponent's side
                }
            }

            int capture = 0; // the potential capture moves each side has, each move worths the same points as the number of stone it can capture

            for (int i = 0; i < 6; i++)  // the potential capture moves of the bottom player
            {
                if (b.stonesAt(i) == 0 && b.stonesAt(12 - i) != 0)  // if there exists an empty cup
                {
                    for (int j = 0; j < 6; j++)  // go through all other cups on the same side and check if there's a good number of stones
                    {
                        if (i != j)
                        {
                            if (b.stonesAt(j) == (i - j) || b.stonesAt(j) == 13 - (j - i))  // if the stone in a neighbour cup == the distance between the origin cup and the neighbour cup
                            { capture -= b.stonesAt(12 - i); }  // the point this capture worths == the number of stone this capture gets
                        }
                    }
                }
            }

            for (int i = 7; i < 13; i++)  // the potential capture moves of the top player
            {
                if (b.stonesAt(i) == 0 && b.stonesAt(12 - i) != 0)  // if there exists an empty cup
                {
                    for (int j = 7; j < 13; j++)  // go through all other cups on the same side and check if there's a good number of stones
                    {
                        if (i != j)
                        {
                            if (b.stonesAt(j) == (i - j) || b.stonesAt(j) == 13 - (j - i))  // if the stone in a neighbour cup == the distance between the origin cup and the neighbour cup
                            { capture += b.stonesAt(12 - i); }  // the point this capture worths == the number of stone this capture gets
                        }
                    }
                }
            }

            return 8*stoneDif + 11*go_again + 6*capture + 7*remainDif;  // calculate and return the final result with different factors for each term
        }

        /*
         * This method returns a message after winning.
         */
        public override String gloat() { return "I win! I'm the best Mankalah player in CS212!"; }
    }

    /*
     * This class implements a pair of integers to indicate the next move.
     */
    public class moveResult
    {
        public int move;
        public int result;

        /*
         * This constructor initiates the value of the next move and the result of the move.
         */
        public moveResult(int m, int r)
        {
            move = m;
            result = r;
        }
        public int getMove() { return move; }

        public int getResult() { return result; }
    }

}