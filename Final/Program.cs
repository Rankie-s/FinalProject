namespace Final;

class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;
        Console.Clear();
        Console.WriteLine("Welcome to this Game!");
        Console.WriteLine("Select game mode: ");
        Console.WriteLine("1. PvP ");
        Console.WriteLine("2. PvE (Coming Soon)");

        Action? action = null;
        while(action is null)
        {
            action = Console.ReadKey(true).Key switch
            {
                ConsoleKey.D1 or ConsoleKey.NumPad1 => RunPvP,
                //ConsoleKey.D2 or ConsoleKey.NumPad2 => RunPvE, no pve mode yet!
                _ => null
            };
        }
        action();
    }

    static void RunPvP()
    {
        // init players and game
        Player p1 = new Player();
        Player p2 = new Player();
        Game game = new Game();

        // shuffle players' decks and draw hands
        p1.Shuffle();
        p2.Shuffle();
        p1.DrawHand();
        p2.DrawHand();

        int round = 1;
        bool p1GoesFirst = true;

        // win condition: one player HP <= 0
        while (!p1.isDead && !p2.isDead)
        {
            Player goesFirst = p1GoesFirst ? p1 : p2;
            Player goesSecond = p1GoesFirst? p2 : p1;
            string first = p1GoesFirst ? "Player 1" : "Player 2";
            string second = p1GoesFirst ? "Player 2" : "Player 1";
            
            Console.Clear();
            Console.WriteLine($"ROUND {round}, {first} plays cards first.");
            
            // clear shield before each round
            // note: shield now applies for THIS turn instead of the next turn
            p1.ClearShield();
            p2.ClearShield();

            // first player's turn
            bool doNeedRenderOpponentPlayedCards = false;
            int[] firstSelection = Board.SelectCards(goesFirst, p1, p2, doNeedRenderOpponentPlayedCards);
            goesFirst.PlayHand(firstSelection); 
            
            Console.Clear();
            Console.WriteLine($"{first} has chosen their cards. Pass the device to {second}.");
            Board.PressEnterToContinue();

            // second player's turn
            Console.Clear();
            doNeedRenderOpponentPlayedCards = true;
            int[] secondSelection = Board.SelectCards(goesSecond, p1, p2, doNeedRenderOpponentPlayedCards);
            goesSecond.PlayHand(secondSelection); 

            // end of each round, need to compare
            Console.Clear();
            Console.WriteLine($"=== ROUND {round} RESULTS ===");
            // compare played cards and render the effect
            string result = game.CompareHandsAndApplyEffects(p1, p2);
            Console.WriteLine(result);
            // update new player status
            Board.RenderStatus(p1, p2);
            
            // if have a winner stop the loop
            if (p1.isDead || p2.isDead) break;

            // draw hand to 7
            p1.DrawHand();
            p2.DrawHand();

            Board.PressEnterToContinue();
            round++;
            p1GoesFirst = !p1GoesFirst; // switch who goes first in the next round
        }

        // game over
        // TODO: add the gameover when the deck is empty
        Console.Clear();
        Console.WriteLine("=== GAME OVER ===");
        if (p1.isDead && p2.isDead) Console.WriteLine("It's a Draw!");
        else if (p1.isDead) Console.WriteLine("Player 2 Wins!");
        else if (p2.isDead) Console.WriteLine("Player 1 Wins!");
    }

    static void RunPvE()
    {
        Console.WriteLine("coming soon...");
    }
}