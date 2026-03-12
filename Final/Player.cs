namespace Final;

public class Player
{
    public List<(Suit? suit, CardNum num)> deck {get; private set;} // cards in each player's deck
    public List<(Suit? suit, CardNum num)> hand {get; private set;} // cards at hand
    public List<(Suit? suit, CardNum num)> playCards; // cards chosen to play in this round
    public bool isDead {get; private set;} = false;
    public int HP {get; private set;} = 50;
    public int shield {get; private set;} = 0;
    public bool canKeepShield {get; set;} = false; // keep shield ever after
    public int restoredShield {get; set;} = 0; // shield restored for the next round
    int maxHand = 7;
    int drawNum; // draw the card at this pos in deck

    public Player() // init player's deck, hand and some other things
    {
        deck = new
        (
            from num in Enum.GetValues<CardNum>()
            from Suit? suit in Enum.GetValues<Suit>()
            where num < CardNum.BlackJoker // don't select two jokers here because they don't have suit
            select (suit, num)//.ToList() don't need it because new() already calls it
        );
        deck.Add((null, CardNum.BlackJoker));
        deck.Add((null, CardNum.RedJoker));

        hand = new();
        playCards = new();
        drawNum = 0;
    }

    public void Shuffle() // suffle player's deck
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
        drawNum = 0;
        /*
        This is my origional idea, but it proves to be wrong later. 
        This is wrong because it creates (deck.Count)^(deck.Count) ways, while the num of suffle should be (deck.Count)!

        for(int i = 0; i < deck.Count; i++)
        {
            int j = Random.Shared.Next(deck.Count);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
        */
    }
    public void DrawHand() // draw hand to maxHand(7)
    {
        while (hand.Count < maxHand)
        {
            if (drawNum > deck.Count) return;
            hand.Add(deck[drawNum]);
            drawNum++;
        }
    }
    public void PlayHand(int[]? playNum)
    {
        if (playNum is null) return;
        playCards.Clear(); // clear the playlist before playing new cards
        
        // restore cards played first.
        var playedCards = (from i in playNum select hand[i]).ToArray();
        playCards.AddRange(playedCards);
        
        // remove them from hand
        foreach (var card in playedCards) hand.Remove(card);
    }
    public virtual int[]? ChooseCards() {return null;}
    public void TakeDamage(int dmg)
    {
        int actualDmg = dmg - shield;
        if (actualDmg > 0) // dmg greater than shield, break the shield.
        {
            HP -= actualDmg;
            shield = 0;
            canKeepShield = false;
        }
        else shield -= dmg;
        if (HP <= 0) isDead = true;
    }
    public void TakeActualDamage(int dmg)
    {
        HP -= dmg;
        if (HP <= 0) isDead = true;
    }
    public void Heal(int heal)
    {
        if (HP + heal > 50) HP = 50;
        else HP += heal;
    }
    public void ClearShield()
    {
        shield = 0;
    }
    public void GetShield(int get)
    {
        shield += get;
    }
}

public class HumanPlayer : Player
{
}

public class ComputerPlayer : Player
{
    public override int[] ChooseCards()
    {
        // choose 4 cards randomly (for now)
        int[] number = [0, 1, 2, 3, 4, 5, 6];
        for (int i = number.Length - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (number[i], number[j]) = (number[j], number[i]);
        }
        return [number[0], number[1], number[2], number[3]];
    }
}

public enum CardNum
{
     Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, BlackJoker, RedJoker
}

public enum Suit
{
    Diamond, Spade, Club, Heart
}