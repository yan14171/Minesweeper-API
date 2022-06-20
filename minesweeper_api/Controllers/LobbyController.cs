using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using minesweeper_api.Data.Interfaces;
using minesweeper_api.Data.Models;
using minesweeper_api.Data.Models.DTOs;
using minesweeper_api.Filters;
using minesweeper_api.GameLogic;
using minesweeper_api.Hubs;

namespace minesweeper_api.Controllers
{
    [AuthExceptionFilter]
    [ApiController]
    [Route("[controller]")]
    public class LobbyController: ControllerBase
    {
        private readonly ILobbyManipulator _lobbyManipulator;
        private readonly IBoardManipulator _boardManipulator;
        private readonly IMapper _mapper;
        private readonly ILogger<LobbyController> _logger;

        public LobbyController(ILobbyManipulator lobbyManipulator,
                               IBoardManipulator boardManipulator,
                               IMapper mapper, 
                               ILogger<LobbyController> logger)
        {
            _lobbyManipulator = lobbyManipulator;
            _boardManipulator = boardManipulator;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<LobbyDTO> PostLobby([FromBody] Lobby lobby)
        {
            try
            {
                _logger.LogInformation("Posting Lobby");
                var board = new Board()
                { LobbyId = lobby.Id };

                _boardManipulator.Add(board);
                return _mapper.Map<LobbyDTO>(_lobbyManipulator.Add(lobby));
            }
            catch (Exception e)
            {
                throw new BadHttpRequestException(e.Message);
            }
        }

        [HttpGet]
        public async Task<IEnumerable<LobbyDTO>> GetLobbies()
        {
            _logger.LogInformation("Querieng lobbies");
            return _mapper.Map<IEnumerable<LobbyDTO>>(_lobbyManipulator.GetAll());
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPatch]
        public async Task<LobbyDTO> JoinLobby([FromBody]int id) 
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type.Contains("email"));
            if (userEmail is null)
                throw new UnauthorizedAccessException("Current user is not valid. Try and login again");

            try
            {
                var lobby = _lobbyManipulator.GetById(id);
                if (lobby.UserIdentifiers.Count > 1)
                    throw new BadHttpRequestException("Lobby specified is already full");
                if (lobby.UserIdentifiers.Contains(userEmail.Value))
                     throw new BadHttpRequestException("Current user is already in this lobby");

                _lobbyManipulator.Remove(lobby);
                lobby.UserIdentifiers.Add(userEmail.Value);
                _lobbyManipulator.Add(lobby);
                
                return _mapper.Map<LobbyDTO>(lobby);
            }
            catch (Exception ex)
            {
                throw new BadHttpRequestException("Error while joining lobby", ex);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        public async Task<LobbyDTO> DisconnectFromLobby([FromBody]int id) 
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type.Contains("email"));
            if (userEmail is null)
                throw new UnauthorizedAccessException("Current user is not valid. Try and login again");
            try
            {
                var lobby = _lobbyManipulator.Disconnect(id, userEmail.Value);
                return _mapper.Map<LobbyDTO>(lobby);
                
            }
            catch (Exception ex)
            {
                throw new BadHttpRequestException("Error while disconnecting from lobby", ex);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("close")]
        public async Task<LobbyDTO> CloseLobby([FromBody]int id)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type.Contains("email"));
            if (userEmail is null)
                throw new UnauthorizedAccessException("Current user is not valid. Try and login again");
            try
            {
                var lobby = _lobbyManipulator.Close(id, userEmail.Value);
                return _mapper.Map<LobbyDTO>(lobby);

            }
            catch (Exception ex)
            {
                throw new BadHttpRequestException("Error while closing lobby", ex);
            }
        }
    }

}