using System.Security.Cryptography.X509Certificates;

namespace Final;

public class Program
{
    public static Game game = new Game();
    public static int round = 0;
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
        while (action is null)
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

        // shuffle players' decks and draw hands
        p1.Shuffle();
        p2.Shuffle();
        p1.DrawHand();
        p2.DrawHand();
        bool p1GoesFirst = true;

        // win condition: one player HP <= 0
        while (!p1.isDead && !p2.isDead)
        {
            Player goesFirst = p1GoesFirst ? p1 : p2;
            Player goesSecond = p1GoesFirst? p2 : p1;
            string first = p1GoesFirst ? "Player 1" : "Player 2";
            string second = p1GoesFirst ? "Player 2" : "Player 1";
            
            Console.Clear();
            
            // clear shield before each round
            // note: shield now applies for THIS turn instead of the next turn
            // but if the player doesn't need to clear shield (effect of diamond ACE), don't clear shield.
            if (!p1.canKeepShield) p1.ClearShield();
            if (!p2.canKeepShield) p2.ClearShield();

            // (special effect of Club Ace: get shield restored the last round)
            if (p1.restoredShield != 0) { p1.GetShield(p1.restoredShield); p1.restoredShield = 0;}
            if (p2.restoredShield != 0) { p2.GetShield(p2.restoredShield); p2.restoredShield = 0;}
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

            // end of each round, compare played cards and render the effect
            Board.RenderRoundResult(round, p1, p2);
            string result = Program.game.CompareHandsAndApplyEffects(p1, p2);
            Console.WriteLine(result);
            // show new player status
            Board.RenderStatus(p1, p2);

            // if have a winner or the deck is empty stop the loop
            if (p1.isDead || p2.isDead || p1.deck.Count <= 0 || p2.deck.Count <= 0) break;

            // draw hand to 7
            p1.DrawHand();
            p2.DrawHand();

            Board.PressEnterToContinue();
            round++;
            p1GoesFirst = !p1GoesFirst; // switch who goes first in the next round
        }

        // run gameover logic when game is over
        game.GameOver(p1,  p2);
    }

    static void RunPvE()
    {
        Console.WriteLine("coming soon...");
    }
}