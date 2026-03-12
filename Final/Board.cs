namespace Final;

public static class Board
{
    public static void RenderStatus(Player p1, Player p2)
    {
        Console.WriteLine("========================================");
        Console.WriteLine($"[Player 1] HP: {p1.HP}/50 | Shield: {p1.shield}");
        Console.WriteLine($"[Player 2] HP: {p2.HP}/50 | Shield: {p2.shield}");
        Console.WriteLine("========================================");
    }

    // render each card
    static void RenderCard((CardNum num, Suit? suit) card, bool hasCursor, bool isSelected)
    {
        string cursor = hasCursor ? ">" : " ";
        string checkbox = isSelected ? "[X]" : "[ ]";
        string suit = SwitchSuitIcon(card.suit);
        string num = card.num switch{
            CardNum.Ace => "A", CardNum.Two => "2", CardNum.Three => "3", CardNum.Four => "4", CardNum.Five => "5", CardNum.Six => "6",
            CardNum.Seven => "7", CardNum.Eight => "8", CardNum.Nine => "9", CardNum.Ten => "10", CardNum.Jack => "J", CardNum.Queen => "Q",
            CardNum.King => "K", CardNum.BlackJoker => "Black Joker", CardNum.RedJoker => "Red Joker", _ => ""
            };
        Console.WriteLine($"{cursor} {checkbox} {suit} {num}");
    }

    static string SwitchSuitIcon(Suit? suit)
    {
        string result = suit switch{Suit.Spade => "♠", Suit.Club => "♣", Suit.Heart => "♥", Suit.Diamond => "♦", _ => ""};
        return result;
    }

    public static void PressEnterToContinue()
    {
        Console.WriteLine("Press ENTER to continue...");
        while(Console.ReadKey(true).Key is not ConsoleKey.Enter) continue;
    }

    // UI of selecting cards
    public static int[] SelectCards(Player currentPlayer, Player p1, Player p2, bool doNeedRenderOpponentPlayedCards)
    {
        var hand = currentPlayer.hand;

        List<int> CardsSelected = new();
        int cursorIndex = 0;
        string playerName = currentPlayer == p1 ? "Player 1" : "Player2";

        while (true) // one player selects cards and play
        {
            Console.Clear();
            Console.WriteLine("=============================================================");
            Console.WriteLine("Effect of suit");
            Console.WriteLine("♠ : Deal damage to the opponent.");
            Console.WriteLine("♣ : Deal damage that ignores shields.");
            Console.WriteLine("♥ : Restore health in the end of this round (cannot exceed 50 HP).");
            Console.WriteLine("♦ : Gain a temporary shield at the beginning of the round.");
            Console.WriteLine("=============================================================");
            RenderStatus(p1, p2); // it's really tedious to render status each time after .Clear. Is there any better way?
            Console.WriteLine($" {playerName}'s Turn ");
            Console.WriteLine("Use ↑/↓ arrows to move, [SPACE] to select, [ENTER] to confirm. You need to play 4 cards.");
            Console.WriteLine($"Selected Cards: {CardsSelected.Count}/4");
            Console.WriteLine("=============================================================");
            if(doNeedRenderOpponentPlayedCards)RenderOpponentPlayedCards(currentPlayer == p1 ? p2 : p1); // render opponent's played cards in second player's turn.

            // render each cards.
            for (int i = 0; i < hand.Count; i++)
            {
                //string cursor = i == cursorIndex ? ">" : " ";
                //string checkbox = CardsSelected.Contains(i) ? "[X]" : "[ ]";
                bool hasCursor = i == cursorIndex ? true : false;
                bool isSelected = CardsSelected.Contains(i) ? true : false;
                RenderCard(hand[i], hasCursor, isSelected);
            }

            var keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (cursorIndex > 0) cursorIndex--;
                    break;
                case ConsoleKey.DownArrow:
                    if (cursorIndex < hand.Count - 1) cursorIndex++;
                    break;
                case ConsoleKey.Spacebar:
                    if (CardsSelected.Contains(cursorIndex)) // if has been selected, remove it
                    {
                        CardsSelected.Remove(cursorIndex);
                    }
                    else if (CardsSelected.Count < 4)
                    {
                        CardsSelected.Add(cursorIndex);
                    }
                    break;
                case ConsoleKey.Enter:
                    if (CardsSelected.Count == 4)
                    {
                        return CardsSelected.ToArray(); // return an array instead of List
                    }
                    break;
            }
        }
    }

    public static void RenderOpponentPlayedCards(Player player)
    {
        if (player.playCards is null) return;
        Console.WriteLine(" Opponent's Played cards ");
        
        // add num of same suit and render it
        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            var cardsOfSuit = player.playCards.Where(c => c.suit == suit).ToArray();

            if (cardsOfSuit.Length > 0)
            {
                int total = cardsOfSuit.Sum(c => (int)c.num);
                string icon = SwitchSuitIcon(suit);
                Console.WriteLine($"  {icon}  Total: {total}");
            }
        }
        Console.WriteLine("================================");
    }
}