namespace Final;

public class Game
{
    public string CompareHandsAndApplyEffects(Player p1, Player p2)
    {
        // TODO: complete the joker num
        // each suit should be trigger separately
        (int clubNum, int winner)club = (0, 0); (int spadeNum, int winner)spade = (0, 0);
        (int diamondNum, int winner)diamond = (0, 0); (int heartNum, int winner)heart = (0, 0);
        foreach (Suit suit in Enum.GetValues<Suit>()) // this is the same as in from, I can't use Suit but have to use Enum.GetValues<Suit>() for some reason I don't understand
        {
            // select the same suit and add them for each player
            (CardNum num, Suit? suit)[] p1Card = p1.playCards.Where(c => c.suit == suit).ToArray();
            int p1Score = p1Card.Sum(n => (int)n.num + 1);
            (CardNum num, Suit? suit)[] p2Card = p2.playCards.Where(c => c.suit == suit).ToArray();
            int p2Score = p2Card.Sum(n => (int)n.num + 1);

            // if tied, nothing happends.
            if (p1Score == p2Score) continue;

            // decide who is the winner for the effect applied later.
            Player winner = p1Score > p2Score ? p1 : p2;
            Player loser  = p1Score > p2Score ? p2 : p1;
            int diff = Math.Abs(p1Score - p2Score);
            Action<Player, Player, int>? action = null;
            switch(suit)
            {
                case Suit.Spade: action = SpadeEffect; spade.spadeNum = diff; spade.winner = winner == p1 ? 1 : 2; break;
                case Suit.Club: action = ClubEffect; club.clubNum = diff; club.winner = winner == p1 ? 1 : 2; break;
                case Suit.Heart: action = HeartEffect; heart.heartNum = diff; heart.winner = winner == p1 ? 1 : 2; break;
                case Suit.Diamond: action = DiamondEffect; diamond.diamondNum = diff; diamond.winner = winner == p1 ? 1 : 2; break;
            }
            if(action is not null) action(winner, loser, diff);
        }
        string result = "";
        // a stupid way but fine
        if(spade.spadeNum != 0)      result += $"♠ Player {spade.winner} winns by {spade.spadeNum} \n";
        if(club.clubNum != 0)        result += $"♣ Player {club.winner} winns by {club.clubNum} \n";
        if(heart.heartNum != 0)      result += $"♥ Player {heart.winner} winns by {heart.heartNum} \n";
        if(diamond.diamondNum != 0)  result += $"♦ Player {diamond.winner} winns by {diamond.diamondNum} \n";
        return result;
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
}