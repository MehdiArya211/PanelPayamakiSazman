using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interface;
using Domain.Entities;
using DTO;
using DTO.User;
using Infrastructure.Data;

namespace BLL
{
    public interface IUnitsManager : IManager<User, ApplicationContext>
    {
        Task<List<UnitsDTO>> SearchUnitsAsync(UnitSearchRequest request, UserSessionDTO User);


    }
}
