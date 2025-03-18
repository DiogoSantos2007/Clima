using Clima.Components.Pages.Dialogs;
using Clima.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;

namespace Clima.Components.Pages
{
    public partial class ClimaPage
    {
        [Inject]
        IJSRuntime JS { get; set; }
        private List<Tb_registos> registos = new List<Tb_registos>();
        private Tb_registos? latestRecord;
        private bool loading = true;
        private string searchString = "";
        private DateRange dateRange = new DateRange(DateTime.Now.AddDays(-7), DateTime.Now);
        private int tempRiskFilter = -1;
        private int humidRiskFilter = -1;
        private int soilHumidRiskFilter = -1; // Added soil humidity risk filter
        private Statistics? statistics;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            loading = true;
            try
            {
                // Get all data within date range and applying filters
                var query = DbContext.Tb_Registos.AsQueryable();

                if (dateRange.Start.HasValue)
                    query = query.Where(r => r.data_registo >= dateRange.Start.Value);

                if (dateRange.End.HasValue)
                    query = query.Where(r => r.data_registo <= dateRange.End.Value.AddDays(1));

                if (tempRiskFilter != -1)
                    query = query.Where(r => r.risco_temperatura == tempRiskFilter);

                if (humidRiskFilter != -1)
                    query = query.Where(r => r.risco_humidade == humidRiskFilter);

                if (soilHumidRiskFilter != -1) // Added soil humidity risk filter
                    query = query.Where(r => r.risco_humidade_solo == soilHumidRiskFilter);

                registos = await query.OrderByDescending(r => r.data_registo).ToListAsync();

                // Get latest record
                latestRecord = await DbContext.Tb_Registos.OrderByDescending(r => r.data_registo).FirstOrDefaultAsync();

                // Calculate statistics
                var allRecords = await DbContext.Tb_Registos.ToListAsync();
                if (allRecords.Any())
                {
                    statistics = new Statistics
                    {
                        TotalRecords = allRecords.Count,
                        AvgTemperature = allRecords.Average(r => r.temperatura),
                        MaxTemperature = allRecords.Max(r => r.temperatura),
                        MinTemperature = allRecords.Min(r => r.temperatura),
                        AvgHumidity = allRecords.Average(r => r.humidade),
                        MaxHumidity = allRecords.Max(r => r.humidade),
                        MinHumidity = allRecords.Min(r => r.humidade),
                        AvgSoilHumidity = allRecords.Average(r => r.humidade_solo), // Added soil humidity stats
                        MaxSoilHumidity = allRecords.Max(r => r.humidade_solo),     // Added soil humidity stats
                        MinSoilHumidity = allRecords.Min(r => r.humidade_solo),     // Added soil humidity stats
                        DateRange = $"{allRecords.Min(r => r.data_registo):d} - {allRecords.Max(r => r.data_registo):d}"
                    };
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading data: {ex.Message}", Severity.Error);
            }
            finally
            {
                loading = false;
            }
        }

        private async Task ApplyFilters()
        {
            await LoadData();
        }

        private async Task DeleteRecord(Tb_registos record)
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", $"Tens a certeza que queres eliminar o registo #{record.ID_registo}? Esta ação é irreversível.");
            parameters.Add("ButtonText", "Apagar");
            parameters.Add("Color", Color.Error);

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmar Eliminição", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                try
                {
                    DbContext.Tb_Registos.Remove(record);
                    await DbContext.SaveChangesAsync();

                    Snackbar.Add($"O registo #{record.ID_registo} foi apagado", Severity.Success);
                    await LoadData();
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Error deleting record: {ex.Message}", Severity.Error);
                }
            }
        }

        private async Task ViewDetails(Tb_registos record)
        {
            var parameters = new DialogParameters();
            parameters.Add("Record", record);

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
            await DialogService.ShowAsync<RecordDetailDialog>("Detalhes do Registo", parameters, options);
        }

        private Color GetTemperatureColor(int riskLevel) => riskLevel switch
        {
            0 => Color.Success,
            1 => Color.Info,
            2 => Color.Warning,
            3 => Color.Error,
            _ => Color.Default
        };

        private Color GetHumidityColor(int riskLevel) => riskLevel switch
        {
            0 => Color.Success,
            1 => Color.Info,
            2 => Color.Warning,
            3 => Color.Error,
            _ => Color.Default
        };

        private Color GetSoilHumidityColor(int riskLevel) => riskLevel switch // Added soil humidity color method
        {
            0 => Color.Success,
            1 => Color.Info,
            2 => Color.Warning,
            3 => Color.Error,
            _ => Color.Default
        };

        private async void DownloadExcel()
        {
            try
            {
                byte[] fileBytes = await ExportToExcel(registos);

                // Convert to Base64 for JavaScript handling
                string base64Data = Convert.ToBase64String(fileBytes);

                // Use JS interop to trigger download
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                await JS.InvokeVoidAsync("downloadFileFromStream", $"Export_{DateTime.Now}", contentType, base64Data);

                // Show success message
                Snackbar.Add($"Excel file 'Export_{DateTime.Now}' downloaded successfully", Severity.Success);
            }
            catch (Exception ex)
            {
                // Handle and display any errors
                Snackbar.Add($"Export failed: {ex.Message}", Severity.Error);
                Console.Error.WriteLine($"Excel export error: {ex}");
            }
        }

        public async Task<byte[]> ExportToExcel(List<Tb_registos> data)
        {
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Climate Data");

                // Add headers with formatting
                worksheet.Cell(1, 1).Value = "Record ID";
                worksheet.Cell(1, 2).Value = "Temperature (°C)";
                worksheet.Cell(1, 3).Value = "Humidity (%)";
                worksheet.Cell(1, 4).Value = "Temperature Risk Level";
                worksheet.Cell(1, 5).Value = "Humidity Risk Level";
                worksheet.Cell(1, 6).Value = "Soil Humidity (%)"; // Added soil humidity header
                worksheet.Cell(1, 7).Value = "Soil Humidity Risk Level"; // Added soil humidity risk header
                worksheet.Cell(1, 8).Value = "Date & Time"; // Adjusted column index

                // Style the header row
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                headerRow.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                // Add data rows
                int row = 2;
                foreach (var record in data)
                {
                    worksheet.Cell(row, 1).Value = record.ID_registo;
                    worksheet.Cell(row, 2).Value = record.temperatura;
                    worksheet.Cell(row, 3).Value = record.humidade;
                    worksheet.Cell(row, 4).Value = record.risco_temperatura;
                    worksheet.Cell(row, 5).Value = record.risco_humidade;
                    worksheet.Cell(row, 6).Value = record.humidade_solo; // Added soil humidity value
                    worksheet.Cell(row, 7).Value = record.risco_humidade_solo; // Added soil humidity risk value
                    worksheet.Cell(row, 8).Value = record.data_registo; // Adjusted column index

                    // Format temperature cells based on risk level
                    if (record.risco_temperatura >= 2)
                    {
                        worksheet.Cell(row, 2).Style.Font.FontColor = ClosedXML.Excel.XLColor.Red;
                    }

                    // Format humidity cells based on risk level
                    if (record.risco_humidade >= 2)
                    {
                        worksheet.Cell(row, 3).Style.Font.FontColor = ClosedXML.Excel.XLColor.Blue;
                    }

                    // Format soil humidity cells based on risk level
                    if (record.risco_humidade_solo >= 2)
                    {
                        worksheet.Cell(row, 6).Style.Font.FontColor = ClosedXML.Excel.XLColor.DarkGreen;
                    }

                    row++;
                }

                // Auto-size columns
                worksheet.Columns().AdjustToContents();

                // Add a summary section
                row += 2;
                worksheet.Cell(row, 1).Value = "Summary";
                worksheet.Cell(row, 1).Style.Font.Bold = true;

                row++;
                worksheet.Cell(row, 1).Value = "Total Records:";
                worksheet.Cell(row, 2).Value = data.Count;

                row++;
                worksheet.Cell(row, 1).Value = "Average Temperature:";
                worksheet.Cell(row, 2).Value = data.Count > 0 ? data.Average(r => r.temperatura) : 0;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "0.00 °C";

                row++;
                worksheet.Cell(row, 1).Value = "Average Humidity:";
                worksheet.Cell(row, 2).Value = data.Count > 0 ? data.Average(r => r.humidade) : 0;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "0.00 %";

                row++;
                worksheet.Cell(row, 1).Value = "Average Soil Humidity:"; // Added soil humidity summary
                worksheet.Cell(row, 2).Value = data.Count > 0 ? data.Average(r => r.humidade_solo) : 0;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "0.00 %";

                row++;
                worksheet.Cell(row, 1).Value = "Date Range:";
                if (data.Count > 0)
                {
                    var minDate = data.Min(r => r.data_registo);
                    var maxDate = data.Max(r => r.data_registo);
                    worksheet.Cell(row, 2).Value = $"{minDate:yyyy-MM-dd} to {maxDate:yyyy-MM-dd}";
                }
                else
                {
                    worksheet.Cell(row, 2).Value = "No data";
                }

                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        public class Statistics
        {
            public int TotalRecords { get; set; }
            public double AvgTemperature { get; set; }
            public double MaxTemperature { get; set; }
            public double MinTemperature { get; set; }
            public double AvgHumidity { get; set; }
            public double MaxHumidity { get; set; }
            public double MinHumidity { get; set; }
            public double AvgSoilHumidity { get; set; } // Added soil humidity stats
            public double MaxSoilHumidity { get; set; } // Added soil humidity stats
            public double MinSoilHumidity { get; set; } // Added soil humidity stats
            public string DateRange { get; set; } = string.Empty;
        }
    }
}