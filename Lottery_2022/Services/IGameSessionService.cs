using Lottery_2022.Models;

namespace Lottery_2022.Services
{
    public interface IGameSessionService
    {
        void ValidateSession(GameSessionViewModel model);
    }
}
