using Gtk;

namespace FinalProject;

public static class Minimax
{
    private const int MinimaxLimit = 3;

    public static bool CanCheck(int x, int y, int dx, int dy)
    {
        // Can check the direction (line not outside the grid)
        if ((x + (dx * 4) <= 14 && x + (dx * 4) >= 0) && (y + (dy * 4) <= 14 && y + (dy * 4) >= 0) && x >= 0 && x <= 14 && y >= 0 && y <= 14)
            return true;
        return false;
    }

    private static int OneColor(Game g, int x, int y, int dx, int dy)
    {
        // Are there stones of one color in one line (without stones of the second player)
        int player = 0;
        for (int i = 0; i < 5; i++)
        {
            if (g.grid[x + i * dx, y + dy * i] != 0)
            {
                if (player == 0)
                {
                    player = g.grid[x + i * dx, y + dy * i];
                }
                else if (player != g.grid[x + i * dx, y + dy * i])
                {
                    return 0;
                }
            }
        }
        return player;
    }
    
    private static int Count(Game g, int x, int y, int dx, int dy)
    {
        // How many stones are in a line of 5 of one player
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            if (g.grid[x + i * dx, y + dy * i] > 0)
            {
                count++;
            }
        }

        return count;
    }
    
    private static bool PossibleMove(int x, int y)
    {
        // Move not outside the grid
        if (x >= 0 && x <= 14 && y >= 0 && y <= 14)
            return true;
        return false;
    }
    
    public static int Evaluate(Game g)
    {
        (int, int)[] checkDirections = { (-1, 0), (0, -1), (-1, -1), (-1, 1) };
        (int, int)[] winDirections = { (1, 0), (0, 1), (1, 1), (1, -1) };

        int player = 0;
        int computer = 0;

        for (int i = 0; i < g.sizeX; i++)
        {
            for (int j = 0; j < g.sizeY; j++)
            {
                if (g.grid[i, j] > 0)
                {
                    for (int q = 0; q < checkDirections.Length; q++)
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            if (CanCheck(i + checkDirections[q].Item1 * k, j + checkDirections[q].Item2 * k,
                                    winDirections[q].Item1, winDirections[q].Item2))
                            {
                                
                                int val = OneColor(g, i + checkDirections[q].Item1 * k, j + checkDirections[q].Item2 * k,
                                    winDirections[q].Item1, winDirections[q].Item2);

                                if ((PossibleMove(i - checkDirections[q].Item1, j - checkDirections[q].Item2) &&
                                     g.grid[i - checkDirections[q].Item1, j - checkDirections[q].Item2] != 3 - val) ||
                                    (PossibleMove(i + checkDirections[q].Item1 * 5, j + checkDirections[q].Item2 * 5) &&
                                     g.grid[i + checkDirections[q].Item1 * 5, j + checkDirections[q].Item2 * 5] !=
                                     3 - val))
                                {
                                    if (val > 0)
                                    {
                                        switch (Count(g, i + checkDirections[q].Item1 * k, j + checkDirections[q].Item2 * k,
                                                    winDirections[q].Item1, winDirections[q].Item2))
                                        {
                                            case 1:
                                                if (val == 1)
                                                    player += 1;
                                                else
                                                    computer += 1;
                                                break;
                                            case 2:
                                                if (val == 1)
                                                    player += 10;
                                                else
                                                    computer += 10;
                                                break;
                                            case 3:
                                                if (val == 1)
                                                    player += 100;
                                                else
                                                    computer += 100;
                                                break;
                                            case 4:
                                                if (val == 1)
                                                    player += 500;
                                                else
                                                    computer += 500;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
        }
        return (player - computer);
    }
    
    
    public static int Predict(Game g, out (int, int)? best, double alpha, double beta, int depth = 0)
    {
        // Minimax game evaluation
        best = null;

        if (g.winner == 1)
            return int.MaxValue;
        if (g.winner == 2)
            return int.MinValue;
        if (g.GridFull())
            return 0;

        if (depth >= MinimaxLimit)
        { 
            int res = Evaluate(g);
            return res;
        }
        

        bool maximizing = g.currentPlayer == 1;

        int v = maximizing ? int.MinValue : int.MaxValue;

        foreach (var move in g.PossibleMoves())
        {
            
            g.Move(move.Item1, move.Item2);
            int w = Predict(g, out (int, int)? dummy, alpha, beta, depth + 1);
            g.UnMove(move.Item1, move.Item2);
            if (maximizing && w > v)
            {
                v = w;
                best = move;

                // Alpha-beta pruining
                if (v > alpha) alpha = v;
                if (beta <= alpha)
                    break;
            }
            if (!maximizing && w < v)
            {
                v = w;
                best = move;

                // Alpha-beta pruining
                if (v < beta) beta = v;
                if (beta <= alpha)
                    break;
            }
        }

        return v;
    }
}
