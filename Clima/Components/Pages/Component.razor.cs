using Clima.Data;
using Clima.Models;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace Clima.Components.Pages
{
    public partial class Component(AppDbContext appDbContext)
    {
        DataGridFilterMode _filterMode = DataGridFilterMode.Simple;
        DataGridFilterCaseSensitivity _caseSensitivity = DataGridFilterCaseSensitivity.Default;

        private IEnumerable<Tb_registos> registos = new List<Tb_registos>();
        private bool _sortNameByLength;
        private SortMode _sortMode = SortMode.Multiple;

        protected override async Task OnInitializedAsync()
        {
            registos = await appDbContext.Tb_Registos.ToListAsync();
        }
    }
}