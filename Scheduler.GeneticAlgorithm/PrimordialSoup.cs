using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;
using Scheduler.GeneticAlgorithm.Reproduction;
using Scheduler.GeneticAlgorithm.Rules;

namespace Scheduler.GeneticAlgorithm
{
    /// <summary>
    /// A basic genetic algorithm that uses Season objects as agents.
    /// </summary>
    public class PrimordialSoup
    {
        private readonly League league;
        private readonly List<IRule> rules;
        private readonly RuleBasedFitnessCalculator fitnessCalculator;
        private readonly SeasonFactory seasonFactory;
        private readonly int populationSize;
        private readonly SeasonComparerReversed seasonComparer = new SeasonComparerReversed();
        private readonly MatchupComparer matchupComparer = new MatchupComparer();

        private int totalFitness = 0;
        private Season mostFitSeason = null;

        /// <summary>
        /// The number of Seasons to promote to the next generation "as is".
        /// This helps ensure that extremely fit Seasons that are newly created
        /// do not disappear right away.
        /// </summary>
        private const int AutomaticGenerationHoppers = 50;

        private const double CrossOverPercent = 0.7;
        private const double MutationPercent = 0.0075; // 0.0025;

        private readonly List<ICrossoverAlgorithm> crossoverAlgorithms;
        private readonly List<IMutationAlgorithm> mutationAlgorithms;
        
        /// <summary>
        /// The Seasons that are currently competing in the algorithm.
        /// 
        /// Always sorted by fitness.
        /// </summary>
        public List<Season> CurrentPopulation { get; private set; }
        
        /// <summary>
        /// The number of generations that have elapsed since Initialize was called.
        /// </summary>
        public int CurrentGeneration { get; private set; }

        /// <summary>
        /// An immutable list of the rules that govern agent fitness within the algorithm.
        /// </summary>
        public ReadOnlyCollection<IRule> Rules 
        {
            get
            {
                return this.rules.AsReadOnly();
            }
        }

        /// <summary>
        /// Notification that a generation has completed.
        /// </summary>
        public event EventHandler GenerationComplete;

        public PrimordialSoup(League league, List<IRule> userRules, int populationSize)
        {
            if (league == null) throw new ArgumentNullException("league");
            if (userRules == null) throw new ArgumentNullException("userRules");
            if (populationSize <= AutomaticGenerationHoppers) throw new ArgumentOutOfRangeException("populationSize", populationSize, "Population size must be greater than " + AutomaticGenerationHoppers);
            if (populationSize % 2 > 0) throw new ArgumentOutOfRangeException("populationSize", populationSize, "Population size must be even.");

            this.league = league;
            this.populationSize = populationSize;

            // Add all default rules
            this.rules = new List<IRule> 
            {
                new ValidScheduleRule(),
                new GameslotAllocationRule(),
                new RepeatGameRule(),
                new GameSpacingRule(),
                new GameslotSpacingRule(),
            };
            this.rules.AddRange(userRules);

            // Add default reproduction strategies
            this.crossoverAlgorithms = new List<ICrossoverAlgorithm> 
            {
                new SinglePointCrossover(),
            };

            this.mutationAlgorithms = new List<IMutationAlgorithm> 
            {
                new GameShuffleMutation(),
                new MatchupShuffleMutation(this.league),
                new ScheduleShuffleMutation(),
            };

            this.fitnessCalculator = new RuleBasedFitnessCalculator(league, this.rules);
            this.seasonFactory = new SeasonFactory(league);
        }

        /// <summary>
        /// Prepare the genetic algorithm for a new run.
        /// </summary>
        public void Initialize()
        {
            this.CurrentPopulation = new List<Season>();
            this.CurrentGeneration = 0;
            this.totalFitness = 0;
            this.mostFitSeason = new Season { Fitness = int.MinValue };

            // Create initial population
            for (int i = 0; i < this.populationSize; i++)
            {
                var season = this.seasonFactory.GenerateRandom();
                this.Register(season);
            }
        }

        /// <summary>
        /// Run the algorithm for a specified number of generations.
        /// </summary>
        public void Run(int numberOfGenerations)
        {
            if (numberOfGenerations <= 0) throw new ArgumentOutOfRangeException("numberOfGenerations", numberOfGenerations, "Must specify a number greater than zero.");

            for (int i = 0; i < numberOfGenerations; i++)
            {
                this.RunGeneration();
            }
        }

        private void RunGeneration()
        {
            // Start by incrementing the generation counter
            this.CurrentGeneration++;

            // Take a copy of the previous generation, and then blank the list
            var parentGeneration = new List<Season>(this.CurrentPopulation);
            int parentGenerationFitness = this.totalFitness;

            // Reset the generation data
            this.CurrentPopulation.Clear();
            this.totalFitness = 0;
            this.mostFitSeason = new Season { Fitness = int.MinValue };

            // Automatically promote the most fit Seasons to the next generation
            // without changing them
            for (int i = 0; i < AutomaticGenerationHoppers; i++)
            {
                this.Register(parentGeneration[i].DeepCopy());
            }

            // Select pairs of Seasons to populate the new generation.
            // We might select duplicates, but that's alright.
            for (int i = AutomaticGenerationHoppers / 2; i < (this.populationSize) / 2; i++)
            {
                // Use Roulette Wheel selection to determine the two parents.  
                var parent1 = RouletteSelect(parentGeneration, parentGenerationFitness);
                var parent2 = RouletteSelect(parentGeneration, parentGenerationFitness);

                // Crossover
                List<Season> children; ;
                if (RandomProvider.NextDouble() <= CrossOverPercent)
                {
                    int crossIndex = RandomProvider.Next(this.crossoverAlgorithms.Count);
                    children = this.crossoverAlgorithms[crossIndex].Breed(parent1, parent2);
                }
                else
                {
                    children = new List<Season> { parent1.DeepCopy(), parent2.DeepCopy() };
                }

                // Mutation (separate chance for each child)
                foreach (var child in children)
                {
                    foreach (var alg in this.mutationAlgorithms) // Could experience multiple types of mutation
                    {
                        if (RandomProvider.NextDouble() <= MutationPercent)
                        {
                            alg.Mutate(child);
                        }
                    }
                }

                // Add the children to the new generation
                children.ForEach(c => this.Register(c));
            }

            // notify
            this.OnGenerationFinished(this.CurrentGeneration, this.mostFitSeason);
        }

        private void OnGenerationFinished(int generationNumber, Season mostFitSeason)
        {
            if (GenerationComplete != null)
            {
                GenerationComplete(this, new GenerationEventArgs(generationNumber, mostFitSeason));
            }
        }

        /// <summary>
        /// Adds a new season to the list of Seasons, ensuring that the list is sorted.
        /// 
        /// Also keeps track of the most fit Season that has been seen, and the total fitness
        /// across all Seasons in the algorithm.
        /// </summary>
        private void Register(Season season)
        {
            // Fix up all of the weeks in the season so that the game slots are sorted by time.
            // (random mutation might have muddled them all up)
            season.Weeks.ForEach(w => w.Games.Sort(this.matchupComparer));

            // Figure out the fitness of this season, and apply it to the running total
            season.Fitness = this.fitnessCalculator.Calculate(season);
            this.totalFitness += season.Fitness;
            if (season.Fitness > this.mostFitSeason.Fitness) this.mostFitSeason = season;

            /// Insert the item in the proper location to keep the entire list sorted.
            int index = this.CurrentPopulation.BinarySearch(season, this.seasonComparer);
            if (index < 0)
            {
                this.CurrentPopulation.Insert(~index, season);
            }
            else
            {
                this.CurrentPopulation.Insert(index, season);
            }
        }

        /// <summary>
        /// Select a Season from the list of all Seasons in the current generation of the algorithm.
        /// Each Season has as many votes as it has fitness to be selected, which gives the more fit
        /// Seasons a better chance.
        /// </summary>
        private static Season RouletteSelect(List<Season> seasons, int totalFitness)
        {
            int i = RandomProvider.Next(totalFitness);

            int runningTotal = 0;
            foreach (var season in seasons)
            {
                runningTotal += season.Fitness;
                if (i < runningTotal)
                    return season;
            }

            return seasons.Last(); // Should never happen
        }
        
        /// <summary>
        /// Compare Seasons based on Fitness.
        /// 
        /// We want bigger numbers to appear at the top of the list.
        /// </summary>
        private class SeasonComparerReversed : IComparer<Season>
        {
            public int Compare(Season x, Season y)
            {
                if (x == null)
                {
                    if (y == null) // If x is null and y is null, they're equal.  
                        return 0;
                    else // If x is null and y is not null, y is greater.                       
                        return 1;
                }
                else
                {
                    if (y == null) // If x is not null and y is null, x is greater.
                        return -1;
                    else // Both are non-null, so compare the fitness values
                        return x.Fitness.CompareTo(y.Fitness) * -1;
                }
            }
        }

        /// <summary>
        /// Compare Matchup based on game slot time.
        /// </summary>
        private class MatchupComparer : IComparer<Matchup>
        {
            public int Compare(Matchup x, Matchup y)
            {
                if (x == null)
                {
                    if (y == null) // If x is null and y is null, they're equal.  
                        return 0;
                    else // If x is null and y is not null, y is greater.                       
                        return 1;
                }
                else
                {
                    if (y == null) // If x is not null and y is null, x is greater.
                        return -1;
                    else // Both are non-null, so compare the time values
                        return x.Slot.StartTime.CompareTo(y.Slot.StartTime);
                }
            }
        }
    }
}
