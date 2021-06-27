using EditableShapes.Models.Dto;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace EditableShapes.Api
{
    public class AreaApi : ApiBase<AreaDto>
    {
        private const string API = "api/areas";

        public async Task<AreaDto> CreateAsync(AreaDto model)
        {
            return await base.CreateAsync(API, model);
        }

        public async Task<IEnumerable<AreaDto>> ReadAsync()
        {
            return await base.ReadAsync(API);
        }

        public async Task<AreaDto> UpdateAsync(AreaDto model)
        {
            return await base.UpdateAsync(API, model);
        }

        public async Task<AreaDto> DeleteAsync(int id)
        {
            string api = API + $"?id={id}";

            return await base.DeleteAsync(api);
        }
    }
}
