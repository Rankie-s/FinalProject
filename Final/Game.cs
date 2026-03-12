namespace Final;

public class Game
{
    public string CompareHandsAndApplyEffects(Player p1, Player p2)
    {
        string result = "";
        foreach (Suit suit in Enum.GetValues<Suit>()) // this is the same as in from, I can't use Suit but have to use Enum.GetValues<Suit>() for some reason I don't understand
        {
            // select the same suit and add them for each player
            // including special effect of JOKER
            int p1Score = CalculateScore(p1, suit);
            int p2Score = CalculateScore(p2, suit);

            // note: if draw, winner will be p2 and loser will be p1
            Player? winner = null; Player? loser = null; int winnerNum = 0;
            (winner, loser, winnerNum) = (p1Score - p2Score) switch
            {
                0 => (null, null, 0),
                < 0 => (p1, p2, 1),
                > 0 => (p2, p1, 2)
            };
            int diff = Math.Abs(p1Score - p2Score);

            // check if special card is used
            Player? specialCardUser = null;
            if (p1.playCards.Any(c => c.suit == suit && c.num == CardNum.Ace)) specialCardUser = p1;
            if (p2.playCards.Any(c => c.suit == suit && c.num == CardNum.Ace)) specialCardUser = p2;

            // if has special card, check if special effect should be applied first
            if (specialCardUser is not null)
            {
                int user = specialCardUser == p1 ? 1 : 2;
                if (suit == Suit.Spade && specialCardUser == winner) 
                {
                    result += $"♠ Player {user} played ♠ ACE and win the compare! Deal extra 13 damage! Total damage is {diff + 13}! \n";
                    SpecialCards.CheckSpecialCard((suit, CardNum.Ace), specialCardUser, winner, loser, diff);
                    continue; // if the special effect is applied, skip the normal compare.
                }
                else if (suit == Suit.Club && specialCardUser == winner) 
                {
                    result += $"♣ Player {user} played ♣ ACE and win the compare! Deal {diff} damage and turn it into shield the next turn! \n";
                    SpecialCards.CheckSpecialCard((suit, CardNum.Ace), specialCardUser, winner, loser, diff);
                    continue;
                }
                else if (suit == Suit.Heart)
                {
                    int heal = p1 == specialCardUser ? p1Score : p2Score;
                    result += $"♥ Player {user} played ♥ ACE! Negate opponent and heal! Heal {heal} HP! \n";
                    SpecialCards.CheckSpecialCard((suit, CardNum.Ace), specialCardUser, winner, loser, diff);
                    continue;
                }
                else if (suit == Suit.Diamond && specialCardUser == winner)
                {
                    result += $"♦ Player {user} played ♦ ACE and win the compare! The shield will last until broken! Restore {diff} shield! \n";
                    SpecialCards.CheckSpecialCard((suit, CardNum.Ace), specialCardUser, winner, loser, diff);
                    continue;
                }
            }

            // if not has special card or not meet the requirement, apply normal compare
            if (winner is null || loser is null) continue; // no winner, skip this compare
            switch(suit)
            {
                case Suit.Spade: 
                    SpadeEffect(winner, loser, diff);
                    result += $"♠ Player {winnerNum} wins ♠ by {diff} \n";
                    break;
                case Suit.Club: 
                    ClubEffect(winner, loser, diff);
                    result += $"♣ Player {winnerNum} wins ♣ by {diff} \n"; 
                    break;
                case Suit.Heart: 
                    HeartEffect(winner, loser, diff);
                    result += $"♥ Player {winnerNum} wins ♥ by {diff} \n"; 
                    break;
                case Suit.Diamond: 
                    DiamondEffect(winner, loser, diff);
                    result += $"♦ Player {winnerNum} wins ♦ by {diff} \n"; 
                    break;
            }
        }
        return result;
    }

    public int CalculateScore(Player player, Suit suit)
    {
        // basic calculate: add the same suit
        int score = player.playCards.Where(c => c.suit == suit).Sum(c => (int)c.num);

        // special effect of JOKER: add 13 to two suits
        bool hasBlackJoker = player.playCards.Any(c => c.num == CardNum.BlackJoker);
        bool hasRedJoker = player.playCards.Any(c => c.num == CardNum.RedJoker);

        // apply black joker effect to Spade and Club
        if (hasBlackJoker && (suit == Suit.Spade || suit == Suit.Club))
        {
            score += 13; 
        }
        // apply red joker effect to Heart and Diamond
        else if (hasRedJoker && (suit == Suit.Heart || suit == Suit.Diamond))
        {
            score += 13;
        }
        return score;
    }
    
    // Spade loser will take damage
    // Action<Player, Player, int> SpadeEffect = (winner, loser, diff) => {if(diff - loser.shield > 0) loser.TakeDamage(diff - loser.shield);};
    void SpadeEffect(Player winner, Player loser, int diff)
    {
        loser.TakeDamage(diff);
    }
    // Club loser will take actual damage (ignore shield)
    void ClubEffect(Player winner, Player loser, int diff)
    {
        loser.TakeActualDamage(diff);
    }
    // Heart winner will recover HP
    void HeartEffect(Player winner, Player loser, int diff)
    {
        winner.Heal(diff);
    }
    // Diamond winner will get shield
    void DiamondEffect(Player winner, Player loser, int diff)
    {
        winner.GetShield(diff);
    }

    public void GameOver(Player p1, Player p2)
    {
        // game over
        Console.Clear();
        Console.WriteLine("=== GAME OVER ===");
        if (p1.isDead || p2.isDead) // end the game by opponent's HP is lower than 0
        {
            if (p1.isDead && p2.isDead) Console.WriteLine("It's a Draw!");
            else if (p1.isDead) Console.WriteLine("Player 2 Wins!");
            else if (p2.isDead) Console.WriteLine("Player 1 Wins!");
        }
        else // end the game by empty deck
        {
            if (p1.HP == p2.HP) Console.WriteLine("It's a Draw!");
            else if (p1.HP > p2.HP) Console.WriteLine("Player 1 Wins!");
            else if (p1.HP < p2.HP) Console.WriteLine("Player 2 Wins!");
        }
    }
}