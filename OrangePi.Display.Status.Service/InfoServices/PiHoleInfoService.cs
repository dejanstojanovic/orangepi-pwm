using Iot.Device.Graphics;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class PiHoleInfoService : IInfoService
    {
        private readonly IPiHoleService _piHoleService;
        public PiHoleInfoService(IPiHoleService piHoleService)
        {
            _piHoleService = piHoleService;
        }
        public string Label => "PIh";

        public async Task<BitmapImage> GetDisplay(int screenWidth, int screenHeight, string fontName, int fontSize)
        {
            return await this.GetDisplay(screenWidth, screenHeight, fontName, fontSize);
        }

        public async Task<StatusValue> GetValue()
        {
            double blockedPct = 0;
            string? blockedCountText = null;
            try
            {
                var piHoleSummary = await _piHoleService.GetSummary();
                blockedPct = Math.Round(piHoleSummary.AdsPercentageToday, 1);
                blockedCountText = $"{ piHoleSummary.AdsBlockedToday.ToString()} req";
            }
            catch { blockedPct = 0; throw; }
            return new StatusValue(
                valueText: $"{blockedPct}%",
                value: blockedPct,
                note: blockedCountText);
        }
    }
}
