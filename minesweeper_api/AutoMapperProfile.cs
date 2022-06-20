using AutoMapper;
using minesweeper_api.Data.Models;
using minesweeper_api.Data.Models.DTOs;
using minesweeper_api.GameLogic;
using static minesweeper_api.GameLogic.Board;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Cell, CellDTO>()
            .ForMember(n => n.isBomb, opt => opt.MapFrom(t => t.IsBomb))
            .ForMember(n => n.isRevealed, opt => opt.MapFrom(t => t.IsRevealed))
            .ForMember(n => n.isFlaged, opt => opt.MapFrom(t => t.IsFlagged))
            .ForMember(n => n.bombCount, opt => opt.MapFrom(t => t.BombCount)).DisableCtorValidation();


        CreateMap<BoardState, BoardStateDTO>()
            .ForMember(n => n.grid, opt => opt.MapFrom(t => t.grid))        
            .ForMember(n => n.bombsLeft, opt => opt.MapFrom(t => t.BombsLeft))
            .ForMember(n => n.bombsGenerated, opt => opt.MapFrom(t => t.BombsGenerated)).DisableCtorValidation();



        CreateMap<Lobby, LobbyDTO>()
            .ForMember(r => r.userIdentities, opt => opt.MapFrom(t => t.UserIdentifiers));
    }
}
