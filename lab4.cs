﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace lab4
{
    // Interface 
    public interface IEntity
    {
        int Id { get; set; }
    }

    // Player entity
    public class GameAccount : IEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int CurrentRating { get; set; }
        public List<GameHistory> GamesHistory { get; set; } = new List<GameHistory>();
    }

    // Game entity
    public class Game : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // Game history entity
    public class GameHistory : IEntity
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int GameId { get; set; }
        public bool Won { get; set; }
        public int RatingChange { get; set; }
    }

    // DbContext (database)
    public class GameDbContext
    {
        public List<GameAccount> GameAccounts { get; set; } = new List<GameAccount>();
        public List<Game> Games { get; set; } = new List<Game>();
        public List<GameHistory> GameHistories { get; set; } = new List<GameHistory>();
    }

    // Repository interface for GameAccount
    public interface IGameAccountRepository
    {
        void Create(GameAccount entity);
        GameAccount GetById(int id);
        IEnumerable<GameAccount> GetAll();
        void Update(GameAccount entity);
        void Delete(int id);
    }

    // Repository interface for Game
    public interface IGameRepository
    {
        void Create(Game entity);
        Game GetById(int id);
        IEnumerable<Game> GetAll();
        void Update(Game entity);
        void Delete(int id);
    }

    // Repository interface for GameHistory
    public interface IGameHistoryRepository
    {
        void Create(GameHistory entity);
        GameHistory GetById(int id);
        IEnumerable<GameHistory> GetAll();
        void Update(GameHistory entity);
        void Delete(int id);
    }

    //service interface 
    public interface IDataService
    {
        void CreateAccount(string userName, int initialRating);
        IEnumerable<GameAccount> GetAllAccounts();
        void SimulateGame(int playerId, int gameId, bool won, int ratingChange);
        IEnumerable<GameHistory> GetPlayerGames(int playerId);
        IEnumerable<GameHistory> GetAllGames();
    }

    // repositories
    public class GameAccountRepository : IGameAccountRepository
    {
        private readonly GameDbContext _dbContext;

        public GameAccountRepository(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(GameAccount entity)
        {
            _dbContext.GameAccounts.Add(entity);
        }

        public GameAccount GetById(int id)
        {
            return _dbContext.GameAccounts.FirstOrDefault(account => account.Id == id);
        }

        public IEnumerable<GameAccount> GetAll()
        {
            return _dbContext.GameAccounts.ToList();
        }

        public void Update(GameAccount entity)
        {

        }

        public void Delete(int id)
        {
            var account = GetById(id);
            if (account != null)
            {
                _dbContext.GameAccounts.Remove(account);
            }
        }
    }

    public class GameRepository : IGameRepository
    {
        private readonly GameDbContext _dbContext;

        public GameRepository(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(Game entity)
        {
            _dbContext.Games.Add(entity);
        }

        public Game GetById(int id)
        {
            return _dbContext.Games.FirstOrDefault(game => game.Id == id);
        }

        public IEnumerable<Game> GetAll()
        {
            return _dbContext.Games.ToList();
        }

        public void Update(Game entity)
        {

        }

        public void Delete(int id)
        {
            var game = GetById(id);
            if (game != null)
            {
                _dbContext.Games.Remove(game);
            }
        }
    }

    public class GameHistoryRepository : IGameHistoryRepository
    {
        private readonly GameDbContext _dbContext;

        public GameHistoryRepository(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(GameHistory entity)
        {
            _dbContext.GameHistories.Add(entity);
        }

        public GameHistory GetById(int id)
        {
            return _dbContext.GameHistories.FirstOrDefault(history => history.Id == id);
        }

        public IEnumerable<GameHistory> GetAll()
        {
            return _dbContext.GameHistories.ToList();
        }

        public void Update(GameHistory entity)
        {

        }

        public void Delete(int id)
        {
            var history = GetById(id);
            if (history != null)
            {
                _dbContext.GameHistories.Remove(history);
            }
        }
    }

    // data service
    public class DataService : IDataService
    {
        private readonly IGameAccountRepository _accountRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGameHistoryRepository _historyRepository;

        public DataService(
            IGameAccountRepository accountRepository,
            IGameRepository gameRepository,
            IGameHistoryRepository historyRepository)
        {
            _accountRepository = accountRepository;
            _gameRepository = gameRepository;
            _historyRepository = historyRepository;
        }

        public void CreateAccount(string userName, int initialRating)
        {
            var account = new GameAccount { UserName = userName, CurrentRating = initialRating };
            _accountRepository.Create(account);
        }

        public IEnumerable<GameAccount> GetAllAccounts()
        {
            return _accountRepository.GetAll();
        }

        public void SimulateGame(int playerId, int gameId, bool won, int ratingChange)
        {
            var history = new GameHistory
            {
                PlayerId = playerId,
                GameId = gameId,
                Won = won,
                RatingChange = ratingChange
            };

            _historyRepository.Create(history);

            var player = _accountRepository.GetById(playerId);
            if (player != null)
            {
                player.CurrentRating += ratingChange;
                _accountRepository.Update(player);
            }
        }

        public IEnumerable<GameHistory> GetPlayerGames(int playerId)
        {
            return _historyRepository.GetAll().Where(history => history.PlayerId == playerId);
        }

        public IEnumerable<GameHistory> GetAllGames()
        {
            return _historyRepository.GetAll();
        }
    }

    // Command interface
    public interface ICommandExecutor
    {
        void ExecuteCommand();
        void DisplayCommandOptions();
    }

    //display players 
    public class DisplayPlayersCommand : ICommandExecutor
    {
        private readonly GameDbContext _dbContext;

        public DisplayPlayersCommand(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ExecuteCommand()
        {
            foreach (var player in _dbContext.GameAccounts)
            {
                Console.WriteLine($"Player: {player.UserName}, Rating: {player.CurrentRating}");
            }
        }

        public void DisplayCommandOptions()
        {
            Console.WriteLine("1. Display Players");// !
        }
    }

    // add a new player
    public class AddPlayerCommand : ICommandExecutor
    {
        private readonly GameDbContext _dbContext;

        public AddPlayerCommand(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ExecuteCommand()
        {
            Console.Write("Enter player name: ");
            string playerName = Console.ReadLine();

            Console.Write("Enter initial rating: ");
            int initialRating = int.Parse(Console.ReadLine());

            var newPlayer = new GameAccount { UserName = playerName, CurrentRating = initialRating };
            _dbContext.GameAccounts.Add(newPlayer);

            Console.WriteLine($"Player {playerName} added successfully.");
        }

        public void DisplayCommandOptions()
        {
            Console.WriteLine("2. Add New Player");
        }
    }

    // interact with the game
    public class GameCommand : ICommandExecutor
    {
        private readonly IDataService _dataService;

        public GameCommand(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void ExecuteCommand()
        {
            Console.WriteLine("Choose a game command:");
            Console.WriteLine("3. Simulate Game");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 3:
                       
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }
        }

        public void DisplayCommandOptions()
        {
            Console.WriteLine("3. Game Commands");
        }
    }

    // User interface
    public class UserInterface
    {
        private readonly List<ICommandExecutor> _commands;

        public UserInterface(List<ICommandExecutor> commands)
        {
            _commands = commands;
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Choose a command:");
                foreach (var command in _commands)
                {
                    command.DisplayCommandOptions();
                }

                int choice;
                if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= _commands.Count)
                {
                    _commands[choice - 1].ExecuteCommand(); //!
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }
    }

    // Program entry point
    public class Program
    {
        public static void Main(string[] args)
        {
            var dbContext = new GameDbContext();
            var accountRepository = new GameAccountRepository(dbContext);
            var gameRepository = new GameRepository(dbContext);
            var historyRepository = new GameHistoryRepository(dbContext);

            var dataService = new DataService(accountRepository, gameRepository, historyRepository);
            var displayPlayersCommand = new DisplayPlayersCommand(dbContext);
            var addPlayerCommand = new AddPlayerCommand(dbContext);
            var gameCommand = new GameCommand(dataService);

            var commands = new List<ICommandExecutor> { displayPlayersCommand, addPlayerCommand, gameCommand };
            var userInterface = new UserInterface(commands);

            // Create players
            dataService.CreateAccount("Player1", 1000);
            dataService.CreateAccount("Player2", 1200);

            userInterface.Run();
        }
    }
}