using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace Assets.Core.Models
{
    public interface IPlayerViewData
    {
        string Name { get; }
        IReadOnlyList<DieSet> ScoredDice { get; }
        int AmountOfScoredDice { get; }
        int Score { get; }
        IReadOnlyList<DieSet> ExiledDice { get; }
        int AmountOfExiledDice { get; }
        int RemainingDice { get; }
    }

    public class PlayerData : IPlayerViewData
    {
        public string Name { get; private set; }

        private List<DieSet> _scoredDice;
        public IReadOnlyList<DieSet> ScoredDice => _scoredDice.AsReadOnly();
        public int AmountOfScoredDice => _scoredDice.Sum(set => set.Count);
        public int Score => _scoredDice.Sum(set => set.Value * set.Count);

        private List<DieSet> _exiledDice;
        public IReadOnlyList<DieSet> ExiledDice => _exiledDice.AsReadOnly();
        public int AmountOfExiledDice => _scoredDice.Sum(set => set.Count);

        public int RemainingDice { get; private set; }

        public PlayerData(string name, int startingDiceAmount)
        {
            Name = name;
            RemainingDice = startingDiceAmount;
        }

        public void ScoreDice(int value, int amount)
        {
            RemainingDice=- amount;
            var scoredSet = _scoredDice.FirstOrDefault(set => set.Value == value);
            scoredSet.Count += amount;
            if (_scoredDice.Any(set => set.Value == value) == false)
            {
                scoredSet.Value = value;
                _scoredDice.Add(scoredSet);
            }
        }

        public bool ExileDie(int value)
        {
            var scoredSet = _scoredDice.FirstOrDefault(set => set.Value == value);
            if (scoredSet.Count < 1)
                return false;

            scoredSet.Count--;

            var exiledSet = _exiledDice.FirstOrDefault(set => set.Value == value);
            exiledSet.Count++;
            if (_exiledDice.Any(set => set.Value == value) == false)
            {
                exiledSet.Value = value;
                _exiledDice.Add(exiledSet);
            }

            return true;
        }
    }
}
