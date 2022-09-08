﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeslaSolarCharger.Server.Contracts;
using TeslaSolarCharger.Shared.Dtos.ChargingCost;

namespace TeslaSolarCharger.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChargingCostController : ControllerBase
    {
        private readonly IChargingCostService _chargingCostService;

        public ChargingCostController(IChargingCostService chargingCostService)
        {
            _chargingCostService = chargingCostService;
        }

        [HttpGet]
        public Task<DtoChargeSummary> GetChargeSummary(int carId)
        {
            return _chargingCostService.GetChargeSummary(carId);
        }

        [HttpGet]
        public Task<List<DtoChargePrice>> GetChargePrices()
        {
            return _chargingCostService.GetChargePrices();
        }

        [HttpPost]
        public Task UpdateChargePrice([FromBody] DtoChargePrice chargePrice)
        {
            return _chargingCostService.UpdateChargePrice(chargePrice);
        }
    }
}
