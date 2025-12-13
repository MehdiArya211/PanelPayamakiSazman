using DTO.Base;
using DTO.Project.RouteDefinition;

namespace BLL.Project.RouteDefinition
{
    public interface IRouteDefinitionManager
    {
        /// <summary>
        /// دریافت لیست Route Definitions
        /// </summary>
        List<RouteDefinitionListDTO> GetAll();

        /// <summary>
        /// ویرایش Route Definition
        /// </summary>
        BaseResult Update(string routeKey, RouteDefinitionUpdateDTO model);

        /// <summary>
        /// حذف Route Definition
        /// </summary>
        BaseResult Delete(string routeKey);
    }
}
