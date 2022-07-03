namespace FinalProject;


public class Game
{
    // Properties
    public int sizeX { get; private set; }
    public int sizeY { get; private set; }
    public int[,] grid { get; private set; }
    public int winner { get; private set; }
    public (int, int, int, int) winnerData { get; private set; }
    public int winSize { get; private set; }
    public int currentPlayer { get; private set; }
    public bool newGame { get; set; }
    private readonly (int, int)[] winDirections = { (1, 0), (0, 1), (1, 1), (1, -1) };
    private readonly (int, int)[] checkDirections = { (-1, 0), (0, -1), (-1, -1), (-1, 1) };
    private int moveCounter;
    

    // Constructor
    public Game()
    {
        currentPlayer = 1;
        newGame = false;
        winSize = 5;
        sizeX = 15;
        sizeY = 15;
        grid = new int[sizeX, sizeY];
        winner = 0;
        moveCounter = 0;
    }

    // Methods
    public bool GridFull()
    {
        if (moveCounter < sizeX * sizeY)
            return false;
        return true;
    }
    public bool CanMove(int x, int y)
    {
        if (x >= 0 && x < sizeX && y >= 0 && y < sizeY && winner == 0 && !GridFull() && grid[x, y] == 0)
            return true;
        return false;
    }
    public (int, int)[] startingCoordinates(int x, int y)
    {
        // Get the starting coordinates of the line in all directions
        (int, int)[] resArr = { (x, y), (x, y), (x, y), (x, y) };
        for (int i = 0; i < resArr.Length; i++)
        {
            for (int j = 1; j < winSize; j++)
            {
                if (x + checkDirections[i].Item1 * j >= 0 && x + checkDirections[i].Item1 * j < sizeX &&
                    y + checkDirections[i].Item2 * j >= 0 && y + checkDirections[i].Item2 * j < sizeY &&
                    grid[x + checkDirections[i].Item1 * j, y + checkDirections[i].Item2 * j] == grid[x, y])
                {
                    resArr[i] = (x + checkDirections[i].Item1 * j, y + checkDirections[i].Item2 * j);
                }
                    
                else
                    break;
            }
        }

        return resArr;
    }
    private bool CheckWinner(int x, int y)
    {
        (int, int)[] checkCoords = startingCoordinates(x, y);
        int counter;
        
        // Check right

        if (checkCoords[0].Item1 <= sizeX - 5)
        {
            counter = 0;
            for (int j = 0; j < 5; j++)
            {
                if (grid[checkCoords[0].Item1 + winDirections[0].Item1 * j, checkCoords[0].Item2 + winDirections[0].Item2 * j] == grid[x, y])
                    counter++;
                else
                    break;
            }

            if (counter == 5)
            {
                winnerData = (checkCoords[0].Item1, checkCoords[0].Item2, winDirections[0].Item1, winDirections[0].Item2);
                newGame = true;
                return true;
            }
        }
        
        // Check down

        if (checkCoords[1].Item2 <= sizeY - 5)
        {
            counter = 0;
            for (int j = 0; j < 5; j++)
            {
                if (grid[checkCoords[1].Item1 + winDirections[1].Item1 * j, checkCoords[1].Item2 + winDirections[1].Item2 * j] == grid[x, y])
                    counter++;
                else
                    break;
            }

            if (counter == 5)
            {
                winnerData = (checkCoords[1].Item1, checkCoords[1].Item2, winDirections[1].Item1, winDirections[1].Item2);
                newGame = true;
                return true;
            }
        }
        
        // Check left-down diagonal
        
        if (checkCoords[2].Item1 <= sizeX - 5 && checkCoords[2].Item2 <= sizeY - 5)
        {
            counter = 0;
            for (int j = 0; j < 5; j++)
            {
                if (grid[checkCoords[2].Item1 + winDirections[2].Item1 * j, checkCoords[2].Item2 + winDirections[2].Item2 * j] == grid[x, y])
                    counter++;
                else
                    break;
            }

            if (counter == 5)
            {
                winnerData = (checkCoords[2].Item1, checkCoords[2].Item2, winDirections[2].Item1, winDirections[2].Item2);
                newGame = true;
                return true;
            }
        }
        
        // Check right-down diagonal
        
        if (checkCoords[3].Item1 <= sizeX - 5 && checkCoords[3].Item2 >= 4)
        {
            counter = 0;
            for (int j = 0; j < 5; j++)
            {
                if (grid[checkCoords[3].Item1 + winDirections[3].Item1 * j, checkCoords[3].Item2 + winDirections[3].Item2 * j] == grid[x, y])
                    counter++;
                else
                    break;
            }

            if (counter == 5)
            {
                winnerData = (checkCoords[3].Item1, checkCoords[3].Item2, winDirections[3].Item1, winDirections[3].Item2);
                return true;
            }
        }
        

        return false;
    }

    public void Move(int x, int y)
    {
        grid[x, y] = currentPlayer;
        if (CheckWinner(x, y)) 
            winner = currentPlayer;
        currentPlayer = (3 - currentPlayer);
        moveCounter++;
    }
    
    private static Random rng = new Random();  

    public static List<(int, int)> Shuffle(List<(int, int)> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            (int, int) value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }

        return list;
    }

    private List<(int, int)> Neighbours(int x, int y)
    {   
        // All neighbours of the current point
        List<(int, int)> result = new List<(int, int)>();
        (int, int)[] around = { (-1, 0), (-1, -1), (-1, 1), (1, -1), (1, 1), (1, 0), (0, 1), (0, -1)};
        foreach (var dir in around)
        {
            if (x + dir.Item1 >= 0 && x + dir.Item1 < sizeX && y + dir.Item2 >= 0 && y + dir.Item2 < sizeY)
            {
                result.Add((x + dir.Item1, y + dir.Item2));
            }
        }

        return result;
    }

    public List<(int, int)> PossibleMoves()
    {
        List<(int, int)> result = new List<(int, int)>();
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++) 
            {
                // Check if the neighbors are non-zero
                if (grid[i, j] > 0)
                {
                    foreach (var elem in Neighbours(i, j))
                    {
                        if (grid[elem.Item1, elem.Item2] == 0 && !result.Contains((elem.Item1, elem.Item2)))
                        {
                            result.Add((elem.Item1, elem.Item2));
                        }
                    }
                }
            }
        }

        if (result.Count == 0)
            result.Add((7, 7));
        
        return Shuffle(result);
    }
    

    public void UnMove(int x, int y)
    {
        newGame = false;
        winner = 0;
        moveCounter--;
        grid[x, y] = 0;
        currentPlayer = (3 - currentPlayer);
    }

}