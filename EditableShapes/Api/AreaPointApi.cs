using EditableShapes.Models.Dto;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EditableShapes.Api
{
    public class AreaPointApi : ApiBase<AreaPointDto>, IDisposable
    {
        private const string API = "api/areaPoints";

        private bool _disposedValue;

        public async Task<AreaPointDto> CreateAsync(AreaPointDto model)
        {
            return await base.CreateAsync(API, model);
        }

        public async Task<IEnumerable<AreaPointDto>> ReadAsync(int areaId)
        {
            string api = API + $"?areaId={areaId}";

            return await base.ReadAsync(api);
        }

        public async Task<AreaPointDto> UpdateAsync(AreaPointDto model)
        {
            return await base.UpdateAsync(API, model);
        }

        public async Task<AreaPointDto> DeleteAsync(int id)
        {
            string api = API + $"?id={id}";

            return await base.DeleteAsync(api);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    base.Dispose(true);
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
