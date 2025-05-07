using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace SignalRHubPrintQ.Hubs
{
    public class MonitorHub : Hub
    {
        public static ConcurrentDictionary<string, string> PrinterStatuses = new();
        public static ConcurrentDictionary<string, Dictionary<string, string>> AllPrinters = new();
        public static ConcurrentDictionary<string, string> ConnectedClients = new();
        public override async Task OnConnectedAsync()
        {
            ConnectedClients.TryAdd(Context.ConnectionId, "Connected");
            await Clients.All.SendAsync("ClientsUpdated", ConnectedClients.Keys.ToList());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedClients.TryRemove(Context.ConnectionId, out _);
            await Clients.All.SendAsync("ClientsUpdated", ConnectedClients.Keys.ToList());
            await base.OnDisconnectedAsync(exception);
        }

        // เพิ่ม endpoint สำหรับให้ client ขอ list ได้
        public Task<List<string>> GetConnectedClients()
        {
            return Task.FromResult(ConnectedClients.Keys.ToList());
        }
        public async Task SendPrinterStatus(string branch, string status)
        {
            PrinterStatuses[branch] = status;
            await Clients.All.SendAsync("PrinterStatus", branch, status);
        }

        public async Task RequestPrinterStatus(string branch)
        {
            Console.WriteLine($"🔍 HQ ขอข้อมูลสถานะจาก: {branch}");
            await Clients.All.SendAsync("RequestPrinterStatus", branch);
        }

        public async Task SendAllPrinters(string branch, Dictionary<string, string> printers)
        {
            AllPrinters[branch] = printers;
            await Clients.All.SendAsync("AllPrinterStatus", branch, printers);
        }

        public async Task RequestAllPrinters(string branch)
        {
            Console.WriteLine($"🔍 HQ ขอข้อมูลเครื่องพิมพ์ทั้งหมดจาก: {branch}");
            await Clients.All.SendAsync("RequestAllPrinters", branch);
        }
    }
}
