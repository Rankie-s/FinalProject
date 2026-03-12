using System.Globalization;
using System.Reflection.Metadata;

namespace Final;

public static class SpecialCards
{
    public static readonly Dictionary<(Suit? suit, CardNum cardNum), Action<Player, Player?, Player?, int>> specialCardsList = new()
        {
            {(Suit.Spade, CardNum.Ace), SpadeAceEffect},
            {(Suit.Club, CardNum.Ace), ClubAceEffect},
            {(Suit.Heart, CardNum.Ace), HeartAceEffect},
            {(Suit.Diamond, CardNum.Ace), DiamondAceEffect},
            //{(null, CardNum.BlackJoker), BlackJokerEffect},
            //{(null, CardNum.RedJoker), RedJokerEffect},
        };
    // check if played cards have joker. if true, apply joker effect
    public static bool CheckBlackJoker((Suit? suit, CardNum cardNum)[] card) => card.Any(c => c.cardNum == CardNum.BlackJoker);
    public static bool CheckRedJoker((Suit? suit, CardNum cardNum)[] card) => card.Any(c => c.cardNum == CardNum.RedJoker);
    public static void CheckSpecialCard((Suit? suit, CardNum cardNum) card, Player user, Player? winner, Player? loser, int diff)
    {
        // if (specialCardsList.Any(n => n.Key == card)) {Action action1 = specialCardsList[card]; action1();};
        // for some reason, I think check and apply should be divided
        if (specialCardsList.TryGetValue(card, out Action<Player, Player?, Player?, int>? action)) ApplySpecialEffect(action, user, winner, loser, diff);
    }
    static void ApplySpecialEffect(Action<Player, Player?, Player?, int> action, Player user, Player? winner, Player? loser, int diff)
    {
        action(user, winner, loser, diff);
    }
    static void SpadeAceEffect(Player user, Player? winner, Player? loser, int diff)
    {
        if (winner is null || loser is null) return;
        if (user == winner) loser.TakeDamage(diff + 13); // if the special card user wins, take extra 13 damage
        else loser.TakeDamage(diff); // if not, take normal damage
    }
    static void ClubAceEffect(Player user, Player? winner, Player? loser, int diff)
    {
        if (winner is null || loser is null) return;
        loser.TakeActualDamage(diff); // deal actual damage first.
        // use a Club Ace and wins, gain shield equal to the damage for the next round
        if (user == winner) user.restoredShield = diff;
    }
    static void HeartAceEffect(Player user, Player? winner, Player? loser, int diff)
    {
        // only the user can recover HP
        int userHeartScore = Program.game.CalculateScore(user, Suit.Heart);
        user.Heal(userHeartScore);
        
        // don't need it anymore. but I want to keep it because it is a brillant idea.
        /*int userHeartScore = Program.game.CalculateScore(user, Suit.Heart);
        int opponentHeartScore = 0;
        if (winner is not null && loser is not null)
        {
            opponentHeartScore = Program.game.CalculateScore((user == winner ? loser : winner), Suit.Heart);
        }
        // because outside the code two players will recover HP based on regular check,
        // this code should:
        // if the user is the winner: and add opponentHeartScore to winner.
        // if the user is the loser: sub diff to winner, and recover the userHeartScore to loser.
        // if it is draw: recover the userHeartScore to user.
        if (diff == 0)
        {
            user.Heal(userHeartScore);
        }
        else if (user == winner)
        {
            user.Heal(opponentHeartScore);
        }
        else if (user == loser)
        {
            winner!.TakeActualDamage(diff);
            user.Heal(userHeartScore);
        }*/
    }
    static void DiamondAceEffect(Player user, Player? winner, Player? loser, int diff)
    {
        if (winner is null || loser is null) return;
        winner.GetShield(diff); // get normal shield first
        // if the user of diamond ACE wins, keep the shield until it is broken.
        if (user == winner) user.canKeepShield = true;
    }
}